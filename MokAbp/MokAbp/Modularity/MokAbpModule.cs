namespace MokAbp.Modularity
{
    /// <summary>
    /// MokAbp模块基类
    /// </summary>
    public abstract class MokAbpModule : IMokAbpModule
    {
        public virtual void PreConfigureServices(ModuleContext context)
        {
            // 默认空实现
        }

        public virtual void ConfigureServices(ModuleContext context)
        {
            // 默认空实现
        }

        public virtual void PostConfigureServices(ModuleContext context)
        {
            // 默认空实现
        }
    }
}
