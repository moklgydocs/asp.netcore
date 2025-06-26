using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityServer.Models;

namespace IdentityServer.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userManager.FindByNameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据用户名 {Username} 获取用户时发生错误", username);
                return null;
            }
        }

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据邮箱 {Email} 获取用户时发生错误", email);
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根据ID {UserId} 获取用户时发生错误", userId);
                return null;
            }
        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        public async Task<bool> ValidatePasswordAsync(ApplicationUser user, string password)
        {
            try
            {
                return await _userManager.CheckPasswordAsync(user, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证用户 {Username} 密码时发生错误", user.UserName);
                return false;
            }
        }

        /// <summary>
        /// 更新用户最后登录时间
        /// </summary>
        public async Task UpdateLastLoginAsync(ApplicationUser user)
        {
            try
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("更新用户 {Username} 最后登录时间", user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新用户 {Username} 最后登录时间时发生错误", user.UserName);
            }
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            try
            {
                return await _userManager.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户 {Username} 角色时发生错误", user.UserName);
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取用户声明
        /// </summary>
        public async Task<IList<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            try
            {
                var claims = new List<Claim>();

                // 添加基本声明
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
                claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));

                // 添加扩展声明
                if (!string.IsNullOrEmpty(user.DisplayName))
                    claims.Add(new Claim("display_name", user.DisplayName));

                if (!string.IsNullOrEmpty(user.FirstName))
                    claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));

                if (!string.IsNullOrEmpty(user.LastName))
                    claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

                if (!string.IsNullOrEmpty(user.Department))
                    claims.Add(new Claim("department", user.Department));

                claims.Add(new Claim("age", user.Age.ToString()));

                // 添加角色声明
                var roles = await GetUserRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // 添加权限声明
                var permissions = user.GetPermissions();
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }

                // 添加Identity存储的声明
                var identityClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(identityClaims);

                return claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户 {Username} 声明时发生错误", user.UserName);
                return new List<Claim>();
            }
        }
    }
}