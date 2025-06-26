using AuthServer.Models;
using System.Security.Claims;

namespace AuthServer.Services
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
        Task<IList<Claim>> GetUserClaimsAsync(ApplicationUser user);

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<(bool Success, string[] Errors)> CreateUserAsync(ApplicationUser user, string password);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        Task<(bool Success, string[] Errors)> UpdateUserAsync(ApplicationUser user);

        /// <summary>
        /// 删除用户
        /// </summary>
        Task<bool> DeleteUserAsync(string userId);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        Task<(List<ApplicationUser> Users, int Total)> GetUsersAsync(int page = 1, int pageSize = 10, string? search = null);

        /// <summary>
        /// 检查用户是否有特定权限
        /// </summary>
        Task<bool> HasPermissionAsync(ApplicationUser user, string permission);
    }
}