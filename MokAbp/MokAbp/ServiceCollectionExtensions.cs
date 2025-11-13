using Microsoft.Extensions.DependencyInjection;
using MokAbp.Application;
using System;

namespace MokAbp
{
    /// <summary>
    /// IServiceCollection扩展方法
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加MokAbp应用程序
        /// </summary>
        public static MokAbpApplication AddMokAbpApplication<TStartupModule>(this IServiceCollection services)
            where TStartupModule : class
        {
            return MokAbpApplicationFactory.Create<TStartupModule>(services);
        }

        /// <summary>
        /// 添加MokAbp应用程序
        /// </summary>
        public static MokAbpApplication AddMokAbpApplication(this IServiceCollection services, Type startupModuleType)
        {
            return MokAbpApplicationFactory.Create(startupModuleType, services);
        }
    }
}
