using DependencyInject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services)
       where TImplementation : TService
        {
            services.Add(ServiceDescriptor.Singleton<TService, TImplementation>());
            return services;
        }

        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService instance)
        {
            services.Add(ServiceDescriptor.Singleton(typeof(TService), instance));
            return services;
        }

        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            services.Add(ServiceDescriptor.Singleton<TService>(sp => factory(sp)));
            return services;
        }

        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            services.Add(ServiceDescriptor.Scoped<TService, TImplementation>());
            return services;
        }

        public static IServiceCollection AddScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            services.Add(ServiceDescriptor.Scoped<TService>(sp => factory(sp)));
            return services;
        }

        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : TService
        {
            services.Add(ServiceDescriptor.Transient<TService, TImplementation>());
            return services;
        }

        public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
        {
            services.Add(ServiceDescriptor.Transient<TService>(sp => factory(sp)));
            return services;
        }
        // 构建容器
        public static IServiceProvider BuildServiceProvider(this IServiceCollection services)
        {
            return new DIContainer(services);
        }

    }
}
