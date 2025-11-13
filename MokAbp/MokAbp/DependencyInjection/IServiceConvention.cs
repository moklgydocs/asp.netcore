namespace MokAbp.DependencyInjection
{
    /// <summary>
    /// 服务注册约定接口
    /// </summary>
    public interface IServiceConvention
    {
        void RegisterServices(IServiceRegistrationContext context);
    }
}
