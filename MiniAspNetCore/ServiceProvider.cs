using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomAspNetCore
{
    /// <summary>
    /// 服务提供者接口 - 依赖注入容器的核心
    /// </summary>
    public interface IServiceProvider
    {
        T GetService<T>();
        object GetService(Type serviceType);
        T GetRequiredService<T>();
        object GetRequiredService(Type serviceType);
        IServiceScope CreateScope();
    }

    /// <summary>
    /// 服务提供者实现 - ASP.NET Core DI容器的核心实现
    /// 职责：管理对象的创建、生命周期和依赖关系解析
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services;
        private readonly ConcurrentDictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, object> _scopedInstances = new();
        private readonly object _lock = new object();

        // 修复：接受IServiceCollection接口，内部转换为具体类型
        public ServiceProvider(IServiceCollection services)
        {
            if (services is ServiceCollection serviceCollection)
            {
                _services = serviceCollection.GetServices();
            }
            else
            {
                throw new ArgumentException("ServiceCollection must be of type ServiceCollection", nameof(services));
            }

            // 注册自身作为IServiceProvider
            if (!_services.ContainsKey(typeof(IServiceProvider)))
            {
                _services[typeof(IServiceProvider)] = new ServiceDescriptor(
                    typeof(IServiceProvider),
                    this,
                    ServiceLifetime.Singleton);
            }
        }

        /// <summary>
        /// 内部构造函数：直接接受服务字典（用于作用域）
        /// </summary>
        internal ServiceProvider(Dictionary<Type, ServiceDescriptor> services)
        {
            _services = new Dictionary<Type, ServiceDescriptor>(services);

            // 注册自身作为IServiceProvider
            _services[typeof(IServiceProvider)] = new ServiceDescriptor(
                typeof(IServiceProvider),
                this,
                ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 获取指定类型的服务实例（可选）
        /// </summary>
        public T GetService<T>()
        {
            var result = GetService(typeof(T));
            return result != null ? (T)result : default(T);
        }

        /// <summary>
        /// 获取指定类型的服务实例（可选）
        /// </summary>
        public object GetService(Type serviceType)
        {
            try
            {
                return GetRequiredService(serviceType);
            }
            catch (Exception ex)
            {
                // 记录调试信息
                Console.WriteLine($"[调试] 无法解析服务 {serviceType.Name}: {ex.Message}");
                return null; // GetService返回null表示服务未注册
            }
        }

        /// <summary>
        /// 获取指定类型的服务实例（必需）
        /// </summary>
        public T GetRequiredService<T>()
        {
            return (T)GetRequiredService(typeof(T));
        }

        /// <summary>
        /// 获取指定类型的服务实例（必需）
        /// 核心原理：根据服务的生命周期决定如何创建和管理实例
        /// </summary>
        public object GetRequiredService(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                // 提供更详细的错误信息
                var registeredServices = string.Join(", ", _services.Keys.Select(t => t.Name));
                throw new InvalidOperationException(
                    $"服务 '{serviceType.Name}' 未注册到依赖注入容器。\n" +
                    $"已注册的服务: {registeredServices}\n" +
                    $"请确保在 builder.Services 中注册了该服务。");
            }

            // 根据生命周期返回不同的实例
            return descriptor.Lifetime switch
            {
                ServiceLifetime.Singleton => GetSingleton(descriptor),
                ServiceLifetime.Transient => CreateInstance(descriptor),
                ServiceLifetime.Scoped => GetScoped(descriptor),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// 获取单例服务 - 整个应用程序生命周期内只创建一次
        /// 使用双重检查锁定模式确保线程安全
        /// </summary>
        private object GetSingleton(ServiceDescriptor descriptor)
        {
            return _singletonInstances.GetOrAdd(descriptor.ServiceType, _ =>
            {
                lock (_lock)
                {
                    if (_singletonInstances.TryGetValue(descriptor.ServiceType, out var existingInstance))
                    {
                        return existingInstance;
                    }

                    return descriptor.Instance ?? CreateInstance(descriptor);
                }
            });
        }

        /// <summary>
        /// 获取作用域服务 - 在同一个请求范围内是同一个实例
        /// </summary>
        private object GetScoped(ServiceDescriptor descriptor)
        {
            lock (_lock)
            {
                if (!_scopedInstances.TryGetValue(descriptor.ServiceType, out var instance))
                {
                    instance = CreateInstance(descriptor);
                    _scopedInstances[descriptor.ServiceType] = instance;
                }
                return instance;
            }
        }

        /// <summary>
        /// 创建服务实例 - 支持工厂方法和反射创建
        /// </summary>
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            try
            {
                // 如果有预创建的实例，直接返回
                if (descriptor.Instance != null)
                    return descriptor.Instance;

                // 如果有工厂方法，使用工厂方法创建
                if (descriptor.Factory != null)
                    return descriptor.Factory(this);

                // 使用反射创建实例并解析构造函数依赖
                return CreateInstanceByReflection(descriptor.ImplementationType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"创建服务 '{descriptor.ServiceType.Name}' 的实例时发生错误: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 通过反射创建实例并解析构造函数依赖
        /// </summary>
        private object CreateInstanceByReflection(Type implementationType)
        {
            var constructors = implementationType.GetConstructors();

            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"类型 '{implementationType.Name}' 没有公共构造函数");
            }

            // 选择参数最多的构造函数（ASP.NET Core的策略）
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];

            // 递归解析构造函数依赖
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;

                try
                {
                    args[i] = GetRequiredService(parameterType);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(
                        $"无法解析类型 '{implementationType.Name}' 构造函数中的参数 '{parameterType.Name}'。\n" +
                        $"请确保该类型已注册到依赖注入容器。", ex);
                }
            }

            try
            {
                return Activator.CreateInstance(implementationType, args);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"创建类型 '{implementationType.Name}' 的实例时发生错误", ex);
            }
        }

        /// <summary>
        /// 创建服务作用域 - 用于控制Scoped服务的生命周期
        /// </summary>
        public IServiceScope CreateScope()
        {
            // 创建新的作用域，复制当前服务注册但清空Scoped实例
            return new ServiceScope(new ServiceProvider(_services));
        }
    }

    /// <summary>
    /// 服务作用域接口
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }

    /// <summary>
    /// 服务作用域实现 - 管理Scoped服务的生命周期
    /// </summary>
    public class ServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; }
        private bool _disposed = false;

        public ServiceScope(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // 清理Scoped服务实例
                if (ServiceProvider is ServiceProvider sp)
                {
                    // 在实际实现中，这里会遍历并Dispose所有实现了IDisposable的Scoped服务
                    Console.WriteLine("[作用域] ServiceScope disposed - Scoped services cleaned up");
                }

                _disposed = true;
            }
        }
    }
}