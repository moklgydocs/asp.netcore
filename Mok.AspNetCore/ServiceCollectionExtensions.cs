using Mok.AspNetCore;
using Mok.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static T? GetServices<T>(this IServiceCollection services)
        {
            return services
                .GetSingletonInstance<IApplication>()
                .ServiceProvider
                .GetService<T>();
        }

        public static object? GetServices(this IServiceCollection services, Type type)
        {
            return services
                .GetSingletonInstance<IApplication>()
                .ServiceProvider
                .GetService(type);

        }
        public static T GetRequiredService<T>(this IServiceCollection services) where T : notnull
        {
            return services
                .GetSingletonInstance<IApplication>()
                .ServiceProvider
                .GetRequiredService<T>();
        }
        public static object GetRequiredService(this IServiceCollection services, Type type)
        {
            return services
                .GetSingletonInstance<IApplication>()
                .ServiceProvider
                .GetRequiredService(type);
        }
        public static Lazy<T> GetRequiredServiceLazy<T>(this IServiceCollection services) where T : notnull
        {
            return new Lazy<T>(services.GetRequiredService<T>, true);
        }
        internal static Lazy<T?> GetServiceLazy<T>(this IServiceCollection services)
        {
            return new Lazy<T?>(services.GetService<T>, true);
        }

        public static T? GetService<T>(this IServiceCollection services)
        { 
            return services
                .GetSingletonInstance<IApplication>()
                .ServiceProvider
                .GetService<T>();
        }

        internal static T GetSingletonInstance<T>(this IServiceCollection services)
        {
            var service = services.GetSingletonInstanceOrNull<T>();
            if (service == null)
            {
                throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            }
            return service;
        }

        internal static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T?)services
                  .FirstOrDefault(d => d.ServiceType == typeof(T))
                  ?.NormalizedImplementationInstance();
        }


    }
}
