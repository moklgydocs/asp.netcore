using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IMokModule
    {
        // (可选) 在主要 ConfigureServices 执行前配置服务
        void PreConfigureServices(ServiceConfigurationContext context);

        // 配置此模块的服务
        void ConfigureServices(ServiceConfigurationContext context);

        // (可选) 在主要 ConfigureServices 执行后配置服务
        void PostConfigureServices(ServiceConfigurationContext context);

        // (可选) 应用程序初始化前
        void OnPreApplicationInitialization(ApplicationInitializationContext context);

        // 应用程序初始化
        void OnApplicationInitialization(ApplicationInitializationContext context);

        // (可选) 应用程序初始化后
        void OnPostApplicationInitialization(ApplicationInitializationContext context);

        // 应用程序关闭
        void OnApplicationShutdown(ApplicationShutdownContext context);

        // --- 异步版本 (推荐) ---
        Task PreConfigureServicesAsync(ServiceConfigurationContext context);
        Task ConfigureServicesAsync(ServiceConfigurationContext context);
        Task PostConfigureServicesAsync(ServiceConfigurationContext context);
        Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context);
        Task OnApplicationInitializationAsync(ApplicationInitializationContext context);
        Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context);
        Task OnApplicationShutdownAsync(ApplicationShutdownContext context);
    }
}
