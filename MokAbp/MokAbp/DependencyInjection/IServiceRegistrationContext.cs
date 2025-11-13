using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MokAbp.DependencyInjection
{
    /// <summary>
    /// 服务注册上下文接口
    /// </summary>
    public interface IServiceRegistrationContext
    {
        IServiceCollection Services { get; }
        Assembly[] Assemblies { get; }
    }
}
