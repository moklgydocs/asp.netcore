using DependencyInject.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    public class DIContainer : IServiceProvider, IServiceScopeFactory, IDisposable
    {
        private readonly Dictionary<Type, List<ServiceDescriptor>> _serviceDescriptors;
        private readonly ConcurrentDictionary<ServiceCacheKey, object> _singletonInstances;
        private readonly ConcurrentDictionary<ServiceCacheKey, object> _scopedInstances;
        private readonly DIContainer _rootContainer;
        private bool _disposed;

        // 用于服务实例缓存的键
        private readonly struct ServiceCacheKey : IEquatable<ServiceCacheKey>
        {
            public readonly Type Type;
            public readonly int? ImplementationIndex;

            public ServiceCacheKey(Type type, int? implementationIndex = null)
            {
                Type = type;
                ImplementationIndex = implementationIndex;
            }

            public bool Equals(ServiceCacheKey other)
                => Type == other.Type && ImplementationIndex == other.ImplementationIndex;

            public override bool Equals(object obj)
                => obj is ServiceCacheKey key && Equals(key);

            public override int GetHashCode()
                => HashCode.Combine(Type, ImplementationIndex);
        }

        // 根容器构造函数，用于初始创建
        public DIContainer(IServiceCollection services)
        {
            _rootContainer = this;
            _serviceDescriptors = new Dictionary<Type, List<ServiceDescriptor>>();
            _singletonInstances = new ConcurrentDictionary<ServiceCacheKey, object>();
            _scopedInstances = new ConcurrentDictionary<ServiceCacheKey, object>();

            // 处理服务注册
            foreach (var descriptor in services)
            {
                if (!_serviceDescriptors.TryGetValue(descriptor.ServiceType, out var descriptors))
                {
                    descriptors = new List<ServiceDescriptor>();
                    _serviceDescriptors[descriptor.ServiceType] = descriptors;
                }
                descriptors.Add(descriptor);
            }

            // 注册容器自身
            RegisterContainerServices();
        }

        // 用于创建子作用域的构造函数
        private DIContainer(DIContainer rootContainer)
        {
            _rootContainer = rootContainer;
            _serviceDescriptors = rootContainer._serviceDescriptors;
            _singletonInstances = rootContainer._singletonInstances;
            _scopedInstances = new ConcurrentDictionary<ServiceCacheKey, object>();
        }

        // 注册容器自身提供的服务
        private void RegisterContainerServices()
        {
            // 注册自身为服务提供者
            if (!_serviceDescriptors.ContainsKey(typeof(IServiceProvider)))
            {
                _serviceDescriptors[typeof(IServiceProvider)] = new List<ServiceDescriptor>
                {
                    ServiceDescriptor.Singleton(typeof(IServiceProvider), this)
                };
            }

            // 注册作用域工厂
            if (!_serviceDescriptors.ContainsKey(typeof(IServiceScopeFactory)))
            {
                _serviceDescriptors[typeof(IServiceScopeFactory)] = new List<ServiceDescriptor>
                {
                    ServiceDescriptor.Singleton(typeof(IServiceScopeFactory), this)
                };
            }
        }

        // 实现IServiceProvider接口
        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DIContainer));
            }

            // 处理服务集合请求
            if (IsServiceCollection(serviceType, out var elementType))
            {
                return ResolveServices(elementType);
            }

            // 获取服务注册信息
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptors) || descriptors.Count == 0)
            {
                return null; // 未注册的服务返回null
            }

            // 获取默认实现（最后注册的服务）
            var descriptor = descriptors.Last();
            return ResolveService(descriptor);
        }

        // 检查是否是集合类型
        private bool IsServiceCollection(Type serviceType, out Type elementType)
        {
            elementType = null;

            if (serviceType.IsGenericType)
            {
                var genericType = serviceType.GetGenericTypeDefinition();
                if (genericType == typeof(IEnumerable<>) ||
                    genericType == typeof(ICollection<>) ||
                    genericType == typeof(IList<>))
                {
                    elementType = serviceType.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }

        // 解析服务集合
        private object ResolveServices(Type elementType)
        {
            if (!_serviceDescriptors.TryGetValue(elementType, out var descriptors))
            {
                // 返回空集合
                var listType = typeof(List<>).MakeGenericType(elementType);
                return Activator.CreateInstance(listType);
            }

            var instances = new List<object>();
            for (int i = 0; i < descriptors.Count; i++)
            {
                var instance = ResolveService(descriptors[i], i);
                if (instance != null)
                {
                    instances.Add(instance);
                }
            }

            // 转换为请求的集合类型
            var array = Array.CreateInstance(elementType, instances.Count);
            for (int i = 0; i < instances.Count; i++)
            {
                array.SetValue(instances[i], i);
            }

            return array;
        }

        // 解析单个服务
        private object ResolveService(ServiceDescriptor descriptor, int? index = null)
        {
            var cacheKey = new ServiceCacheKey(descriptor.ServiceType, index);

            switch (descriptor.ServiceLifetime)
            {
                case ServiceLifetime.Singleton:
                    return _rootContainer._singletonInstances.GetOrAdd(cacheKey, _ => CreateInstance(descriptor));

                case ServiceLifetime.Scoped:
                    return _scopedInstances.GetOrAdd(cacheKey, _ => CreateInstance(descriptor));

                case ServiceLifetime.Transient:
                    return CreateInstance(descriptor);

                default:
                    throw new NotSupportedException($"不支持的生命周期类型: {descriptor.ServiceLifetime}");
            }
        }

        // 创建服务实例
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            // 如果已有实例，直接返回
            if (descriptor.Instance != null)
            {
                return descriptor.Instance;
            }

            // 如果有工厂方法，使用工厂创建
            if (descriptor.Factory != null)
            {
                return descriptor.Factory(this);
            }

            // 通过类型创建实例
            return ActivateInstance(descriptor.ImplementationType);
        }

        // 激活类型实例
        private object ActivateInstance(Type type)
        {
            // 获取所有构造函数
            var constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                // 没有公共构造函数，尝试使用默认构造函数
                return Activator.CreateInstance(type);
            }

            // 选择参数最多的构造函数（通常是主构造函数）
            var constructor = constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            // 解析构造函数参数
            var parameters = constructor.GetParameters();
            var parameterInstances = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameterInstances[i] = GetService(parameter.ParameterType);

                // 如果参数不是可选的，但无法解析，抛出异常
                if (parameterInstances[i] == null && !parameter.IsOptional)
                {
                    throw new InvalidOperationException(
                        $"无法解析类型 {type.FullName} 的构造函数参数 {parameter.Name} (类型: {parameter.ParameterType.FullName})");
                }
            }

            // 创建实例
            var instance = constructor.Invoke(parameterInstances);

            // 属性注入
            InjectProperties(instance);

            return instance;
        }

        // 属性注入
        private void InjectProperties(object instance)
        {
            var type = instance.GetType();

            // 查找标记了 [Inject] 特性的属性
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var injectAttribute = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute != null)
                {
                    var propertyValue = GetService(property.PropertyType);
                    if (propertyValue != null)
                    {
                        property.SetValue(instance, propertyValue);
                    }
                }
            }
        }

        // 实现IServiceScopeFactory接口
        public IServiceScope CreateScope()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DIContainer));
            }

            return new ServiceScope(new DIContainer(_rootContainer));
        }

        // 实现IDisposable接口
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // 释放作用域服务
            foreach (var service in _scopedInstances.Values)
            {
                (service as IDisposable)?.Dispose();
            }

            // 只有根容器才释放单例服务
            if (this == _rootContainer)
            {
                foreach (var service in _singletonInstances.Values)
                {
                    (service as IDisposable)?.Dispose();
                }
            }

            // 清空缓存
            _scopedInstances.Clear();
        }
         
    }
}
