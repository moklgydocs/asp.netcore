using IdentityServer.Models;

namespace IdentityServer.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        Task<ApplicationUser?> GetUserByIdAsync(string userId);

        /// <summary>
        /// 验证用户密码
        /// </summary>
        Task<bool> ValidatePasswordAsync(ApplicationUser user, string password);

        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        Task UpdateLastLoginAsync(ApplicationUser user);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);

        /// <summary>
        /// 获取用户声明
        /// </summary>
        Task<IList<System.Security.Claims.Claim>> GetUserClaimsAsync(ApplicationUser user);
    }
}