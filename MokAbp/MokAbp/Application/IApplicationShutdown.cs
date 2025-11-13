namespace MokAbp.Application
{
    /// <summary>
    /// 应用程序关闭接口
    /// </summary>
    public interface IApplicationShutdown
    {
        void OnApplicationShutdown(ApplicationShutdownContext context);
    }
}
