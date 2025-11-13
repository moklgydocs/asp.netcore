namespace MokAbp.Application
{
    /// <summary>
    /// 应用程序启动初始化接口
    /// </summary>
    public interface IApplicationInitialization
    {
        void OnApplicationInitialization(ApplicationInitializationContext context);
    }
}
