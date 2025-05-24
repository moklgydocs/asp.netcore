using DependencyInject.Core;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace MiddleWare
{
    public class CustomServiceProviderFactory : IServiceProviderFactory<DependencyInject.Core.IServiceCollection>
    {
        public DependencyInject.Core.IServiceCollection CreateBuilder(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            return services.Adapt<DependencyInject.Core.IServiceCollection>();
        }

        public System.IServiceProvider CreateServiceProvider(DependencyInject.Core.IServiceCollection containerBuilder)
        {
            var builder = containerBuilder.Adapt<DependencyInject.Core.IServiceCollection>();
            var provider = new DIContainer(builder);
            return provider.Adapt<System.IServiceProvider>();
        }
    }
}
