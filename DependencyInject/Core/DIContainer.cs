using DependencyInject.Enums; // 引入服务生命周期枚举
using System;
using System.Collections.Concurrent; // 用于线程安全的字典
using System.Collections.Generic;
using System.Linq;
using System.Reflection; // 用于反射
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 依赖注入容器，负责服务的注册、解析、作用域管理和生命周期管理。
    /// 实现IServiceProvider、IServiceScopeFactory、IDisposable接口。
    /// </summary>
    public class DIContainer : IServiceProvider, IServiceScopeFactory, IDisposable
    {
        // 服务描述符字典，key为服务类型，value为该类型的所有注册描述符（支持多实现）
        private readonly Dictionary<Type, List<ServiceDescriptor>> _serviceDescriptors;
        // 单例服务实例缓存，线程安全
        private readonly ConcurrentDictionary<ServiceCacheKey, object> _singletonInstances;
        // 作用域服务实例缓存，线程安全
        private readonly ConcurrentDictionary<ServiceCacheKey, object> _scopedInstances;
        // 根容器引用，用于作用域容器访问单例缓存
        private readonly DIContainer _rootContainer;
        // 容器是否已释放
        private bool _disposed;

        /// <summary>
        /// 服务缓存键，用于唯一标识服务实例（类型+实现索引）
        /// </summary>
        private readonly struct ServiceCacheKey : IEquatable<ServiceCacheKey>
        {
            /// <summary>
            /// 服务类型
            /// </summary>
            public readonly Type Type;
            /// <summary>
            /// 实现索引（同类型多实现时区分）
            /// </summary>
            public readonly int? ImplementationIndex;

            /// <summary>
            /// 构造函数，初始化服务类型和实现索引
            /// </summary>
            /// <param name="type">服务类型</param>
            /// <param name="implementationIndex">实现索引</param>
            public ServiceCacheKey(Type type, int? implementationIndex = null)
            {
                Type = type;
                ImplementationIndex = implementationIndex;
            }

            /// <summary>
            /// 判断两个ServiceCacheKey是否相等
            /// </summary>
            public bool Equals(ServiceCacheKey other)
                => Type == other.Type && ImplementationIndex == other.ImplementationIndex;

            /// <summary>
            /// 重写Equals方法
            /// </summary>
            public override bool Equals(object obj)
                => obj is ServiceCacheKey key && Equals(key);

            /// <summary>
            /// 重写GetHashCode，保证字典查找正确
            /// </summary>
            public override int GetHashCode()
                => HashCode.Combine(Type, ImplementationIndex);
        }

        /// <summary>
        /// 根容器构造函数，初始化服务描述符、缓存字典，并注册自身服务
        /// </summary>
        /// <param name="services">服务集合</param>
        public DIContainer(IServiceCollection services)
        {
            _rootContainer = this; // 根容器指向自身
            _serviceDescriptors = new Dictionary<Type, List<ServiceDescriptor>>(); // 初始化服务描述符字典
            _singletonInstances = new ConcurrentDictionary<ServiceCacheKey, object>(); // 初始化单例缓存
            _scopedInstances = new ConcurrentDictionary<ServiceCacheKey, object>(); // 初始化作用域缓存

            // 遍历服务集合，将每个服务描述符按类型分组存储
            foreach (var descriptor in services)
            {
                if (!_serviceDescriptors.TryGetValue(descriptor.ServiceType, out var descriptors))
                {
                    descriptors = new List<ServiceDescriptor>();
                    _serviceDescriptors[descriptor.ServiceType] = descriptors;
                }
                descriptors.Add(descriptor);
            }

            // 注册容器自身（IServiceProvider、IServiceScopeFactory）
            RegisterContainerServices();
        }

        /// <summary>
        /// 子作用域构造函数，复用根容器的服务描述符和单例缓存，独立作用域缓存
        /// </summary>
        /// <param name="rootContainer">根容器</param>
        private DIContainer(DIContainer rootContainer)
        {
            _rootContainer = rootContainer; // 作用域容器持有根容器引用
            _serviceDescriptors = rootContainer._serviceDescriptors; // 复用服务描述符
            _singletonInstances = rootContainer._singletonInstances; // 复用单例缓存
            _scopedInstances = new ConcurrentDictionary<ServiceCacheKey, object>(); // 独立作用域缓存
        }

        /// <summary>
        /// 注册容器自身为服务（IServiceProvider、IServiceScopeFactory）
        /// </summary>
        private void RegisterContainerServices()
        {
            // 注册IServiceProvider接口
            if (!_serviceDescriptors.ContainsKey(typeof(IServiceProvider)))
            {
                _serviceDescriptors[typeof(IServiceProvider)] = new List<ServiceDescriptor>
                {
                    ServiceDescriptor.Singleton(typeof(IServiceProvider), this)
                };
            }

            // 注册IServiceScopeFactory接口
            if (!_serviceDescriptors.ContainsKey(typeof(IServiceScopeFactory)))
            {
                _serviceDescriptors[typeof(IServiceScopeFactory)] = new List<ServiceDescriptor>
                {
                    ServiceDescriptor.Singleton(typeof(IServiceScopeFactory), this)
                };
            }
        }

        /// <summary>
        /// 实现IServiceProvider接口，解析指定类型的服务
        /// </summary>
        /// <param name="serviceType">要解析的服务类型</param>
        /// <returns>服务实例或null</returns>
        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DIContainer));
            }

            // 判断是否请求集合类型（如IEnumerable<T>）
            if (IsServiceCollection(serviceType, out var elementType))
            {
                return ResolveServices(elementType);
            }

            // 查找服务描述符
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptors) || descriptors.Count == 0)
            {
                return null; // 未注册返回null
            }

            // 默认返回最后注册的实现
            var descriptor = descriptors.Last();
            return ResolveService(descriptor);
        }

        /// <summary>
        /// 判断类型是否为集合类型（IEnumerable&lt;T&gt;/ICollection&lt;T&gt;/IList&lt;T&gt;）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="elementType">集合元素类型</param>
        /// <returns>是否为集合类型</returns>
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

        /// <summary>
        /// 解析服务集合（如IEnumerable&lt;T&gt;），返回所有实现的实例数组
        /// </summary>
        /// <param name="elementType">集合元素类型</param>
        /// <returns>元素类型实例数组</returns>
        private object ResolveServices(Type elementType)
        {
            // 查找所有该类型的服务描述符
            if (!_serviceDescriptors.TryGetValue(elementType, out var descriptors))
            {
                // 未注册，返回空集合
                var listType = typeof(List<>).MakeGenericType(elementType);
                return Activator.CreateInstance(listType);
            }

            var instances = new List<object>();
            // 遍历所有实现，依次解析
            for (int i = 0; i < descriptors.Count; i++)
            {
                var instance = ResolveService(descriptors[i], i);
                if (instance != null)
                {
                    instances.Add(instance);
                }
            }

            // 转为目标类型的数组
            var array = Array.CreateInstance(elementType, instances.Count);
            for (int i = 0; i < instances.Count; i++)
            {
                array.SetValue(instances[i], i);
            }

            return array;
        }

        /// <summary>
        /// 解析单个服务实例，根据生命周期缓存或创建
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <param name="index">实现索引（多实现时区分）</param>
        /// <returns>服务实例</returns>
        private object ResolveService(ServiceDescriptor descriptor, int? index = null)
        {
            // 构造缓存键
            var cacheKey = new ServiceCacheKey(descriptor.ServiceType, index);

            switch (descriptor.ServiceLifetime)
            {
                case ServiceLifetime.Singleton:
                    // 单例：全局唯一，存储在根容器
                    return _rootContainer._singletonInstances.GetOrAdd(cacheKey, _ => CreateInstance(descriptor));
                case ServiceLifetime.Scoped:
                    // 作用域：每个作用域唯一
                    return _scopedInstances.GetOrAdd(cacheKey, _ => CreateInstance(descriptor));
                case ServiceLifetime.Transient:
                    // 瞬时：每次都新建
                    return CreateInstance(descriptor);
                default:
                    throw new NotSupportedException($"不支持的生命周期类型: {descriptor.ServiceLifetime}");
            }
        }

        /// <summary>
        /// 创建服务实例（支持实例、工厂、类型三种方式）
        /// </summary>
        /// <param name="descriptor">服务描述符</param>
        /// <returns>服务实例</returns>
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            // 已有实例直接返回
            if (descriptor.Instance != null)
            {
                return descriptor.Instance;
            }

            // 有工厂方法则调用工厂
            if (descriptor.Factory != null)
            {
                return descriptor.Factory(this);
            }

            // 通过类型反射创建实例
            return ActivateInstance(descriptor.ImplementationType);
        }

        /// <summary>
        /// 通过反射激活类型实例，自动注入构造函数参数和属性
        /// </summary>
        /// <param name="type">要实例化的类型</param>
        /// <returns>实例对象</returns>
        private object ActivateInstance(Type type)
        {
            // 获取所有公共构造函数
            var constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                // 没有公共构造函数，尝试默认构造
                return Activator.CreateInstance(type);
            }

            // 选择参数最多的构造函数（优先主构造函数）
            var constructor = constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            // 获取构造函数参数
            var parameters = constructor.GetParameters();
            var parameterInstances = new object[parameters.Length];

            // 依次解析每个参数
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                parameterInstances[i] = GetService(parameter.ParameterType);

                // 必选参数无法解析时抛异常
                if (parameterInstances[i] == null && !parameter.IsOptional)
                {
                    throw new InvalidOperationException(
                        $"无法解析类型 {type.FullName} 的构造函数参数 {parameter.Name} (类型: {parameter.ParameterType.FullName})");
                }
            }

            // 反射调用构造函数创建实例
            var instance = constructor.Invoke(parameterInstances);

            // 属性注入
            InjectProperties(instance);

            return instance;
        }

        /// <summary>
        /// 属性注入：为带[Inject]特性的属性赋值
        /// </summary>
        /// <param name="instance">实例对象</param>
        private void InjectProperties(object instance)
        {
            var type = instance.GetType();

            // 查找所有公开实例属性
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // 检查是否有[Inject]特性
                var injectAttribute = property.GetCustomAttribute<InjectAttribute>();
                if (injectAttribute != null)
                {
                    // 解析属性类型的服务
                    var propertyValue = GetService(property.PropertyType);
                    if (propertyValue != null)
                    {
                        property.SetValue(instance, propertyValue);
                    }
                }
            }
        }

        /// <summary>
        /// 实现IServiceScopeFactory接口，创建新的服务作用域
        /// </summary>
        /// <returns>新的IServiceScope实例</returns>
        public IServiceScope CreateScope()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DIContainer));
            }

            // 创建新的作用域容器并包装为ServiceScope
            return new ServiceScope(new DIContainer(_rootContainer));
        }

        /// <summary>
        /// 实现IDisposable接口，释放容器及其服务实例
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // 释放所有作用域服务
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

            // 清空作用域缓存
            _scopedInstances.Clear();
        }

    }
}
