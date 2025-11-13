using Microsoft.Extensions.DependencyInjection;
using System;

namespace MokAbp.Application
{
    /// <summary>
    /// MokAbp应用程序工厂
    /// </summary>
    public static class MokAbpApplicationFactory
    {
        /// <summary>
        /// 创建MokAbp应用程序
        /// </summary>
        public static MokAbpApplication Create<TStartupModule>(IServiceCollection services)
            where TStartupModule : class
        {
            return Create(typeof(TStartupModule), services);
        }

        /// <summary>
        /// 创建MokAbp应用程序
        /// </summary>
        public static MokAbpApplication Create(Type startupModuleType, IServiceCollection services)
        {
            return new MokAbpApplication(startupModuleType, services);
        }

        /// <summary>
        /// 创建MokAbp应用程序并构建ServiceProvider
        /// </summary>
        public static MokAbpApplication CreateAndBuild<TStartupModule>(IServiceCollection services)
            where TStartupModule : class
        {
            var application = Create<TStartupModule>(services);
            var serviceProvider = services.BuildServiceProvider();
            application.Initialize(serviceProvider);
            return application;
        }
    }
}
