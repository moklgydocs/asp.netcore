namespace MokAbp.Modularity
{
    /// <summary>
    /// MokAbp模块接口
    /// </summary>
    public interface IMokAbpModule
    {
        /// <summary>
        /// 配置服务前
        /// </summary>
        void PreConfigureServices(ModuleContext context);

        /// <summary>
        /// 配置服务
        /// </summary>
        void ConfigureServices(ModuleContext context);

        /// <summary>
        /// 配置服务后
        /// </summary>
        void PostConfigureServices(ModuleContext context);
    }
}
