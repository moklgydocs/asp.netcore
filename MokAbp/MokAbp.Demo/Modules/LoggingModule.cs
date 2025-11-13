using MokAbp.Application;
using MokAbp.Modularity;

namespace MokAbp.Demo.Modules
{
    /// <summary>
    /// 日志模块
    /// </summary>
    [DependsOn(typeof(CoreModule))]
    public class LoggingModule : MokAbpModule, IApplicationInitialization, IApplicationShutdown
    {
        public override void ConfigureServices(ModuleContext context)
        {
            Console.WriteLine("LoggingModule: ConfigureServices");
            // 日志相关的服务会通过约定自动注册
        }

        public void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Console.WriteLine("LoggingModule: 日志系统已初始化");
        }

        public void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            Console.WriteLine("LoggingModule: 日志系统正在关闭");
        }
    }
}
