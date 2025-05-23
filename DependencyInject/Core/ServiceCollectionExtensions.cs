using DependencyInject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// IServiceCollection 扩展方法，提供常用的服务注册方式（单例、作用域、多例），
    /// 便于开发者以简洁的方式将服务添加到依赖注入容器中。
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册单例服务，服务类型与实现类型分离。
        /// 在应用程序生命周期内只创建一个实现实例。
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">服务实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            // 通过ServiceDescriptor注册单例服务
            services.Add(ServiceDescriptor.Singleton<TService, TImplementation>());
            return services;
        }

        /// <summary>
        /// 注册单例服务，直接指定实例。
        /// 该实例会被容器直接复用，不会再创建新实例。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="instance">服务实例</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService instance)
        {
            // 通过ServiceDescriptor注册单例实例
            services.Add(ServiceDescriptor.Singleton(typeof(TService), instance));
            return services;
        }

        /// <summary>
        /// 注册单例服务，使用工厂方法创建实例。
        /// 工厂方法只会被调用一次，生成的实例会被复用。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="factory">实例工厂方法，参数为IServiceProvider</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            // 通过ServiceDescriptor注册单例工厂
            services.Add(ServiceDescriptor.Singleton<TService>(sp => factory(sp)));
            return services;
        }

        /// <summary>
        /// 注册作用域服务，服务类型与实现类型分离。
        /// 每个作用域（如每次Web请求）创建一个新实例。
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">服务实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            // 通过ServiceDescriptor注册作用域服务
            services.Add(ServiceDescriptor.Scoped<TService, TImplementation>());
            return services;
        }

        /// <summary>
        /// 注册作用域服务，使用工厂方法创建实例。
        /// 每个作用域内工厂方法只会被调用一次。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="factory">实例工厂方法，参数为IServiceProvider</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            // 通过ServiceDescriptor注册作用域工厂
            services.Add(ServiceDescriptor.Scoped<TService>(sp => factory(sp)));
            return services;
        }

        /// <summary>
        /// 注册瞬时（多例）服务，服务类型与实现类型分离。
        /// 每次请求服务时都会创建新实例。
        /// </summary>
        /// <typeparam name="TService">服务接口类型</typeparam>
        /// <typeparam name="TImplementation">服务实现类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            // 通过ServiceDescriptor注册瞬时服务
            services.Add(ServiceDescriptor.Transient<TService, TImplementation>());
            return services;
        }

        /// <summary>
        /// 注册瞬时（多例）服务，使用工厂方法创建实例。
        /// 每次请求服务时都会调用工厂方法创建新实例。
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="factory">实例工厂方法，参数为IServiceProvider</param>
        /// <returns>服务集合本身，便于链式调用</returns>
        public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            // 通过ServiceDescriptor注册瞬时工厂
            services.Add(ServiceDescriptor.Transient<TService>(sp => factory(sp)));
            return services;
        }
        /// <summary>
        /// 构建依赖注入容器，生成IServiceProvider用于解析服务。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>IServiceProvider实例</returns>
        public static IServiceProvider BuildServiceProvider(this IServiceCollection services)
        {
            // 通过DIContainer实现IServiceProvider
            return new DIContainer(services);
        }

    }
}
