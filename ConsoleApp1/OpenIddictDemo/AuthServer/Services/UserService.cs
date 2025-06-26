using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AuthServer.Data;
using AuthServer.Models;

namespace AuthServer.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
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
                return await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
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
                return await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
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
                user.UpdateLastLogin();
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("更新用户 {Username} 最后登录时间成功", user.UserName);
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

                if (!string.IsNullOrEmpty(user.JobTitle))
                    claims.Add(new Claim("job_title", user.JobTitle));

                if (!string.IsNullOrEmpty(user.EmployeeId))
                    claims.Add(new Claim("employee_id", user.EmployeeId));

                if (user.Age.HasValue)
                    claims.Add(new Claim("age", user.Age.Value.ToString()));

                if (user.DateOfBirth.HasValue)
                    claims.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.Value.ToString("yyyy-MM-dd")));

                if (!string.IsNullOrEmpty(user.Gender))
                    claims.Add(new Claim(ClaimTypes.Gender, user.Gender));

                // 添加地址声明
                if (!string.IsNullOrEmpty(user.Address))
                {
                    var addressClaim = new Claim(ClaimTypes.StreetAddress, user.Address);
                    claims.Add(addressClaim);
                }

                if (!string.IsNullOrEmpty(user.City))
                    claims.Add(new Claim(ClaimTypes.Locality, user.City));

                if (!string.IsNullOrEmpty(user.Country))
                    claims.Add(new Claim(ClaimTypes.Country, user.Country));

                if (!string.IsNullOrEmpty(user.PostalCode))
                    claims.Add(new Claim(ClaimTypes.PostalCode, user.PostalCode));

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

        /// <summary>
        /// 创建用户
        /// </summary>
        public async Task<(bool Success, string[] Errors)> CreateUserAsync(ApplicationUser user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("创建用户 {Username} 成功", user.UserName);
                    return (true, Array.Empty<string>());
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    _logger.LogWarning("创建用户 {Username} 失败: {Errors}", user.UserName, string.Join(", ", errors));
                    return (false, errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建用户 {Username} 时发生错误", user.UserName);
                return (false, new[] { "创建用户时发生系统错误" });
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public async Task<(bool Success, string[] Errors)> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("更新用户 {Username} 成功", user.UserName);
                    return (true, Array.Empty<string>());
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    _logger.LogWarning("更新用户 {Username} 失败: {Errors}", user.UserName, string.Join(", ", errors));
                    return (false, errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新用户 {Username} 时发生错误", user.UserName);
                return (false, new[] { "更新用户时发生系统错误" });
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("尝试删除不存在的用户: {UserId}", userId);
                    return false;
                }

                if (!user.CanDelete)
                {
                    _logger.LogWarning("用户 {Username} 不能被删除", user.UserName);
                    return false;
                }

                // 软删除：标记为非活跃状态
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("软删除用户 {Username} 成功", user.UserName);
                    return true;
                }
                else
                {
                    _logger.LogWarning("软删除用户 {Username} 失败", user.UserName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除用户 {UserId} 时发生错误", userId);
                return false;
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public async Task<(List<ApplicationUser> Users, int Total)> GetUsersAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            try
            {
                var query = _userManager.Users.Where(u => u.IsActive);

                // 搜索过滤
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => 
                        u.UserName!.Contains(search) ||
                        u.Email!.Contains(search) ||
                        u.DisplayName!.Contains(search) ||
                        u.FirstName!.Contains(search) ||
                        u.LastName!.Contains(search) ||
                        u.Department!.Contains(search) ||
                        u.JobTitle!.Contains(search));
                }

                var total = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.UserName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (users, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户列表时发生错误");
                return (new List<ApplicationUser>(), 0);
            }
        }

        /// <summary>
        /// 检查用户是否有特定权限
        /// </summary>
        public async Task<bool> HasPermissionAsync(ApplicationUser user, string permission)
        {
            try
            {
                // 检查用户权限
                if (user.HasPermission(permission))
                    return true;

                // 检查角色权限
                var roles = await GetUserRolesAsync(user);
                if (roles.Contains("SuperAdmin") || roles.Contains("Admin"))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查用户 {Username} 权限 {Permission} 时发生错误", user.UserName, permission);
                return false;
            }
        }
    }
}