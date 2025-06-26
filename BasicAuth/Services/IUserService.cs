using BasicAuth.Models;

namespace BasicAuth.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 验证用户凭据
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功返回用户信息，失败返回null</returns>
        Task<User?> ValidateUserAsync(string username, string password);

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户信息</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// 将用户实体转换为用户信息模型
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>用户信息模型</returns>
        UserInfo ToUserInfo(User user);
    }
}