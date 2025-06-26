namespace WebClient.Services
{
    /// <summary>
    /// API服务接口
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// 获取天气预报
        /// </summary>
        Task<string> GetWeatherForecastAsync();

        /// <summary>
        /// 获取用户列表
        /// </summary>
        Task<string> GetUsersAsync();

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<string> CreateUserAsync(object user);

        /// <summary>
        /// 获取当前用户的令牌信息
        /// </summary>
        Task<string> GetTokenInfoAsync();
    }
}