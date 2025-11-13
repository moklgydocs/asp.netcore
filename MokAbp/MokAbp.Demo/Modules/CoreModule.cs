using MokAbp.Application;
using MokAbp.Modularity;

namespace MokAbp.Demo.Modules
{
    /// <summary>
    /// 核心模块 - 提供基础功能
    /// </summary>
    public class CoreModule : MokAbpModule, IApplicationInitialization
    {
        public override void PreConfigureServices(ModuleContext context)
        {
            Console.WriteLine("CoreModule: PreConfigureServices");
        }

        public override void ConfigureServices(ModuleContext context)
        {
            Console.WriteLine("CoreModule: ConfigureServices");
            // 这里可以手动注册一些核心服务
        }

        public override void PostConfigureServices(ModuleContext context)
        {
            Console.WriteLine("CoreModule: PostConfigureServices");
        }

        public void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Console.WriteLine("CoreModule: 应用程序初始化");
        }
    }
}
