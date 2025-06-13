using System;
using System.Collections.Generic;

namespace CustomAspNetCore
{
    /// <summary>
    /// Web应用程序构建器 - 模拟ASP.NET Core的WebApplicationBuilder
    /// 核心职责：配置服务、中间件和应用程序设置
    /// </summary>
    public class WebApplicationBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }

        public WebApplicationBuilder()
        {
            Services = new ServiceCollection();
            Configuration = new Configuration();

            // 添加框架核心服务 - 使用实例注册
            Services.AddSingleton<IConfiguration>(Configuration);
        }

        /// <summary>
        /// 构建Web应用程序
        /// </summary>
        public WebApplication Build()
        {
            // 修复：确保传入的是ServiceCollection类型
            var serviceProvider = new ServiceProvider(Services);
            return new WebApplication(serviceProvider, Configuration);
        }
    }

    /// <summary>
    /// 服务集合接口 - 依赖注入容器的服务注册
    /// </summary>
    public interface IServiceCollection
    {
        void AddSingleton<T>(T instance);
        void AddSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface;
        void AddSingleton<T>(Func<IServiceProvider, T> factory);
        void AddTransient<T>() where T : class;
        void AddTransient<T>(Func<IServiceProvider, T> factory);
        void AddScoped<T>() where T : class;
        void AddScoped<T>(Func<IServiceProvider, T> factory);
    }

    /// <summary>
    /// 服务集合实现 - 管理服务的生命周期和注册
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();

        /// <summary>
        /// 注册单例服务实例
        /// </summary>
        public void AddSingleton<T>(T instance)
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), instance, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 注册单例服务类型映射
        /// </summary>
        public void AddSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            _services[typeof(TInterface)] = new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 注册单例服务工厂方法
        /// </summary>
        public void AddSingleton<T>(Func<IServiceProvider, T> factory)
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), provider => factory(provider), ServiceLifetime.Singleton);
        }

        /// <summary>
        /// 注册瞬态服务
        /// </summary>
        public void AddTransient<T>() where T : class
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), typeof(T), ServiceLifetime.Transient);
        }

        /// <summary>
        /// 注册瞬态服务工厂方法
        /// </summary>
        public void AddTransient<T>(Func<IServiceProvider, T> factory)
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), provider => factory(provider), ServiceLifetime.Transient);
        }

        /// <summary>
        /// 注册作用域服务
        /// </summary>
        public void AddScoped<T>() where T : class
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), typeof(T), ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 注册作用域服务工厂方法
        /// </summary>
        public void AddScoped<T>(Func<IServiceProvider, T> factory)
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), provider => factory(provider), ServiceLifetime.Scoped);
        }

        /// <summary>
        /// 获取服务注册字典（内部使用）
        /// </summary>
        internal Dictionary<Type, ServiceDescriptor> GetServices() => _services;
    }

    /// <summary>
    /// 服务描述符 - 描述服务的类型、实现和生命周期
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public object Instance { get; }
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// 工厂方法（通过委托自定义实例创建逻辑）
        /// </summary>
        public Func<IServiceProvider, object> Factory { get; }

        /// <summary>
        /// 构造函数：基于类型映射的服务描述符
        /// </summary>
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// 构造函数：基于实例的服务描述符
        /// </summary>
        public ServiceDescriptor(Type serviceType, object instance, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Instance = instance;
            Lifetime = lifetime;
        }

        /// <summary>
        /// 构造函数：基于工厂方法的服务描述符
        /// </summary>
        public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Factory = factory;
            Lifetime = lifetime;
        }
    }

    /// <summary>
    /// 服务生命周期枚举
    /// </summary>
    public enum ServiceLifetime
    {
        Singleton,  // 单例：整个应用程序生命周期内只创建一次
        Transient,  // 瞬态：每次请求都创建新实例
        Scoped      // 作用域：在同一个请求范围内是同一个实例
    }
}