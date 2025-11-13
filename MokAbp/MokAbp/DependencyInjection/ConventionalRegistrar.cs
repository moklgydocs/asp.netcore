using Microsoft.Extensions.DependencyInjection;
using MokAbp.DependencyInjection.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace MokAbp.DependencyInjection
{
    /// <summary>
    /// 约定注册器，负责按照约定自动注册服务
    /// </summary>
    public class ConventionalRegistrar : IServiceConvention
    {
        public void RegisterServices(IServiceRegistrationContext context)
        {
            foreach (var assembly in context.Assemblies)
            {
                RegisterAssembly(context.Services, assembly);
            }
        }

        private void RegisterAssembly(IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType);

            foreach (var type in types)
            {
                var dependencyAttribute = type.GetCustomAttribute<DependencyAttribute>();
                if (dependencyAttribute != null)
                {
                    RegisterType(services, type, dependencyAttribute);
                }
            }
        }

        private void RegisterType(IServiceCollection services, Type implementationType, DependencyAttribute attribute)
        {
            var serviceTypes = GetServiceTypes(implementationType);
            var lifetime = ConvertLifetime(attribute.Lifetime);

            foreach (var serviceType in serviceTypes)
            {
                var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);

                if (attribute.ReplaceServices)
                {
                    // Remove existing registrations and add new one
                    var existing = services.FirstOrDefault(d => d.ServiceType == serviceType);
                    if (existing != null)
                    {
                        services.Remove(existing);
                    }
                    services.Add(descriptor);
                }
                else if (attribute.TryRegister)
                {
                    if (!services.Any(d => d.ServiceType == serviceType))
                    {
                        services.Add(descriptor);
                    }
                }
                else
                {
                    services.Add(descriptor);
                }
            }
        }

        private Type[] GetServiceTypes(Type implementationType)
        {
            var exposeAttribute = implementationType.GetCustomAttribute<ExposeServicesAttribute>();
            
            if (exposeAttribute != null)
            {
                var types = new System.Collections.Generic.List<Type>(exposeAttribute.ServiceTypes);
                
                if (exposeAttribute.IncludeDefaults)
                {
                    types.Add(implementationType);
                }
                
                if (exposeAttribute.IncludeInterfaces)
                {
                    types.AddRange(implementationType.GetInterfaces());
                }
                
                return types.Distinct().ToArray();
            }

            // 默认注册实现类本身和所有接口
            var defaultTypes = new System.Collections.Generic.List<Type> { implementationType };
            defaultTypes.AddRange(implementationType.GetInterfaces());
            
            return defaultTypes.ToArray();
        }

        private Microsoft.Extensions.DependencyInjection.ServiceLifetime ConvertLifetime(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,
                ServiceLifetime.Scoped => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped,
                ServiceLifetime.Transient => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient,
                _ => Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient
            };
        }
    }
}
