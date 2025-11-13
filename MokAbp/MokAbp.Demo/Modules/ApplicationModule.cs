using MokAbp.Application;
using MokAbp.Modularity;

namespace MokAbp.Demo.Modules
{
    /// <summary>
    /// 应用程序模块 - 依赖于日志模块
    /// </summary>
    [DependsOn(typeof(LoggingModule))]
    public class ApplicationModule : MokAbpModule, IApplicationInitialization, IApplicationShutdown
    {
        public override void ConfigureServices(ModuleContext context)
        {
            Console.WriteLine("ApplicationModule: ConfigureServices");
            // 应用相关的服务会通过约定自动注册
        }

        public void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Console.WriteLine("ApplicationModule: 应用程序模块已初始化");
        }

        public void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Console.WriteLine("ApplicationModule: 应用程序模块正在关闭");
        }
    }
}
