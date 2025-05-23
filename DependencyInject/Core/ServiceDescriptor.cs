using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInject.Enums;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务描述符（ServiceDescriptor）
    /// <para>用于描述服务的注册信息，包括服务类型、实现类型、实例、工厂方法及生命周期。</para>
    /// <para>依赖注入容器通过ServiceDescriptor来管理服务的创建和生命周期。</para>
    /// <para>支持三种注册方式：</para>
    /// <para>1. 类型到类型的映射（接口到实现类）</para>
    /// <para>2. 直接实例（单例）</para>
    /// <para>3. 工厂方法（自定义实例创建逻辑）</para>
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// 服务类型（通常为接口或抽象类）
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// 实现类型（具体的实现类，类型映射注册时使用）
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// 服务实例（直接实例注册时使用，始终为单例）
        /// </summary>
        public object Instance { get; internal set; }

        /// <summary>
        /// 工厂方法（通过委托自定义实例创建逻辑）
        /// </summary>
        public Func<IServiceProvider, object> Factory { get; }

        /// <summary>
        /// 服务生命周期（单例、作用域、瞬时）
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }

        /// <summary>
        /// 构造函数：基于类型映射的服务描述符
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceLifetime">生命周期</param>
        private ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ServiceLifetime = serviceLifetime;
        }

        /// <summary>
        /// 构造函数：基于实例的服务描述符（始终为单例）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务实例</param>
        private ServiceDescriptor(Type serviceType, object instance)
        {
            ServiceType = serviceType;
            Instance = instance;
            ServiceLifetime = ServiceLifetime.Singleton; // 实例注册总是单例
        }

        /// <summary>
        /// 构造函数：基于工厂方法的服务描述符
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="factory">工厂方法</param>
        /// <param name="serviceLifetime">生命周期</param>
        private ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            Factory = factory;
            ServiceLifetime = serviceLifetime;
        }

        /// <summary>
        /// 工厂方法：创建基于类型映射的服务描述符
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceLifetime">生命周期</param>
        /// <returns>服务描述符实例</returns>
        /// <exception cref="ArgumentNullException">参数为null时抛出</exception>
        public static ServiceDescriptor Describe(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            return new ServiceDescriptor(serviceType, implementationType, serviceLifetime);
        }

        /// <summary>
        /// 创建单例服务描述符（类型映射）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Singleton(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 创建单例服务描述符（直接实例）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务实例</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Singleton(Type serviceType, object instance)
        {
            return new ServiceDescriptor(serviceType, instance);
        }

        /// <summary>
        /// 创建单例服务描述符（泛型，直接实例）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务实例</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Singleton<TService>(object instance)
        {
            return Singleton(typeof(TService), instance);
        }

        /// <summary>
        /// 创建单例服务描述符（泛型，类型映射）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Singleton<TService, TImplementation>()
        {
            return Singleton(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// 创建单例服务描述符（泛型，工厂方法）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="factory">工厂方法</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Singleton<TService>(Func<IServiceProvider, object> factory)
        {
            // 注意：此处生命周期应为Singleton，若为Scoped请根据实际需求调整
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 创建作用域服务描述符（类型映射）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Scoped(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 创建作用域服务描述符（泛型，类型映射）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Scoped<TService, TImplementation>()
            where TImplementation : TService
        {
            return Scoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// 创建作用域服务描述符（泛型，工厂方法）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="factory">工厂方法</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Scoped<TService>(Func<IServiceProvider, object> factory)
        {
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 创建瞬时（多例）服务描述符（类型映射）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementationType">实现类型</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Transient(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// 创建瞬时（多例）服务描述符（泛型，类型映射）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TImplementation : TService
        {
            return Transient(typeof(TService), typeof(TImplementation));
        }

        // 预留：泛型无参瞬时注册
        //public static ServiceDescriptor Transient<TService>()
        //{
        //    return Transient(typeof(TService));
        //}

        /// <summary>
        /// 创建瞬时（多例）服务描述符（泛型，工厂方法）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="factory">工厂方法</param>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Transient<TService>(Func<IServiceProvider, object> factory)
        {
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// 创建瞬时（多例）服务描述符（泛型，无实现类型或工厂方法，主要用于标记）
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns>服务描述符实例</returns>
        public static ServiceDescriptor Transient<TService>()
        {
            return new ServiceDescriptor(typeof(TService), ServiceLifetime.Transient);
        }

        /// <summary>
        /// 构造函数：仅指定服务类型和生命周期（无实现类型/工厂/实例，主要用于标记或扩展）
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceLifetime">生命周期</param>
        private ServiceDescriptor(Type serviceType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
        }
    }
}
