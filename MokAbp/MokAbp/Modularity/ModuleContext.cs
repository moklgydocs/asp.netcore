using Microsoft.Extensions.DependencyInjection;

namespace MokAbp.Modularity
{
    /// <summary>
    /// 模块上下文
    /// </summary>
    public class ModuleContext
    {
        public IServiceCollection Services { get; }

        public ModuleContext(IServiceCollection services)
        {
            Services = services;
        }
    }
}
