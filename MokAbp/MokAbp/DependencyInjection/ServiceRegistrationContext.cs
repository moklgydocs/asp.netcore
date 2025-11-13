using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MokAbp.DependencyInjection
{
    /// <summary>
    /// 服务注册上下文实现
    /// </summary>
    public class ServiceRegistrationContext : IServiceRegistrationContext
    {
        public IServiceCollection Services { get; }
        public Assembly[] Assemblies { get; }

        public ServiceRegistrationContext(IServiceCollection services, Assembly[] assemblies)
        {
            Services = services;
            Assemblies = assemblies;
        }
    }
}
