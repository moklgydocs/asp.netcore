using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInject.Enums;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务描述 
    /// <para>描述一个服务注册，包含服务类型、如何创建服务及其生命周期</para>
    /// <para>ServiceDescriptor是依赖注入系统的核心数据结构，它包含了如何创建和管理服务实例的所有信息：</para>
    /// <para>三种注册方式 </para>
    /// <para>   - 类型到类型的映射：最常见的注册方式，服务接口映射到具体实现类</para>
    /// <para>   - 直接实例：提前创建的实例，总是作为单例</para>
    /// <para>   - 工厂方法：允许自定义复杂的实例创建逻辑</para>
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public object Instance { get; internal set; }

        public Func<IServiceProvider, object> Factory { get; }

        /// <summary>
        /// 定义服务的生命周期，影响实例的创建和缓存策略
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }


        /// <summary>
        /// 基于实现类型的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="serviceLifetime"></param>
        private ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;

            ImplementationType = implementationType;

            ServiceLifetime = serviceLifetime;
        }

        /// <summary>
        /// 基于实例的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        private ServiceDescriptor(Type serviceType, object instance)
        {
            ServiceType = serviceType;
            Instance = instance;
            ServiceLifetime = ServiceLifetime.Singleton; // 实例总是单例
        }
        /// <summary>
        /// 基于工厂方法的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="factory"></param>
        /// <param name="serviceLifetime"></param>
        private ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            Factory = factory;
            ServiceLifetime = serviceLifetime;
        }


        /// <summary>
        /// 工厂方法： 创建基于类型的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// 创建一个单例服务-实现描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public static ServiceDescriptor Singleton(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Singleton);
        }
        /// <summary>
        /// 创建一个单例服务，基于服务类型实例的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ServiceDescriptor Singleton(Type serviceType, object instance)
        {
            return new ServiceDescriptor(serviceType, instance);
        }

        /// <summary>
        /// 创建一个单例服务，基于实例描述符
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ServiceDescriptor Singleton<TService>(object instance)
        {
            return Singleton(typeof(TService), instance);
        }

        /// <summary>
        /// 创建一个单例服务，基于实例描述符
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ServiceDescriptor Singleton<TService, TImplementation>()
        {
            return Singleton(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// 创建一个单例服务，基于工厂方法的描述符
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static ServiceDescriptor Singleton<TService>(Func<IServiceProvider, object> factory)
        {
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Scoped);
        }


        public static ServiceDescriptor Scoped(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Scoped);
        }
        public static ServiceDescriptor Scoped<TService, TImplementation>()
            where TImplementation : TService
        {
            return Scoped(typeof(TService), typeof(TImplementation));
        }
        public static ServiceDescriptor Scoped<TService>(Func<IServiceProvider, object> factory)
        {
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Scoped);
        }
        /// <summary>
        /// 创建一个多例服务，基于工厂方法的描述符
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public static ServiceDescriptor Transient(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Transient);
        }

        public static ServiceDescriptor Transient<TService, TImplementation>()
            where TImplementation : TService
        {
            return Transient(typeof(TService), typeof(TImplementation));
        }

        //public static ServiceDescriptor Transient<TService>()
        //{
        //    return Transient(typeof(TService));
        //}
        public static ServiceDescriptor Transient<TService>(Func<IServiceProvider, object> factory)
        {
            return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Transient);
        }
    }
}
