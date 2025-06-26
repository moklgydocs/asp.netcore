using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityServer.Models;

namespace IdentityServer.Services
{
    /// <summary>
    /// 用户档案服务
    /// 自定义Identity Server 4的用户声明逻辑
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            IUserService userService,
            ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 获取用户声明
        /// 在生成访问令牌和身份令牌时调用
        /// </summary>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var userId = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("未找到用户 {UserId}", userId);
                    return;
                }

                // 获取用户所有声明
                var claims = await _userService.GetUserClaimsAsync(user);

                // 根据请求的作用域过滤声明
                var requestedClaims = new List<Claim>();

                // 处理请求的声明类型
                foreach (var requestedClaimType in context.RequestedClaimTypes)
                {
                    var matchingClaims = claims.Where(c => c.Type == requestedClaimType);
                    requestedClaims.AddRange(matchingClaims);
                }

                // 添加额外的上下文声明
                if (context.RequestedClaimTypes.Contains("role"))
                {
                    var roles = await _userService.GetUserRolesAsync(user);
                    foreach (var role in roles)
                    {
                        requestedClaims.Add(new Claim("role", role));
                    }
                }

                if (context.RequestedClaimTypes.Contains("permission"))
                {
                    var permissions = user.GetPermissions();
                    foreach (var permission in permissions)
                    {
                        requestedClaims.Add(new Claim("permission", permission));
                    }
                }

                // 添加常用的标准声明
                if (context.RequestedClaimTypes.Contains(ClaimTypes.Email) && !string.IsNullOrEmpty(user.Email))
                {
                    requestedClaims.Add(new Claim(ClaimTypes.Email, user.Email));
                }

                if (context.RequestedClaimTypes.Contains(ClaimTypes.Name) && !string.IsNullOrEmpty(user.UserName))
                {
                    requestedClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
                }

                if (context.RequestedClaimTypes.Contains("given_name") && !string.IsNullOrEmpty(user.FirstName))
                {
                    requestedClaims.Add(new Claim("given_name", user.FirstName));
                }

                if (context.RequestedClaimTypes.Contains("family_name") && !string.IsNullOrEmpty(user.LastName))
                {
                    requestedClaims.Add(new Claim("family_name", user.LastName));
                }

                // 设置返回的声明
                context.IssuedClaims = requestedClaims.Distinct(new ClaimComparer()).ToList();

                _logger.LogInformation("为用户 {Username} 返回了 {ClaimCount} 个声明", 
                    user.UserName, context.IssuedClaims.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户档案数据时发生错误");
            }
        }

        /// <summary>
        /// 判断用户是否激活
        /// </summary>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var userId = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(userId);

                // 检查用户是否存在且激活
                context.IsActive = user != null && user.IsActive;

                if (user != null)
                {
                    _logger.LogInformation("用户 {Username} 激活状态检查: {IsActive}", 
                        user.UserName, context.IsActive);
                }
                else
                {
                    _logger.LogWarning("用户 {UserId} 不存在", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查用户激活状态时发生错误");
                context.IsActive = false;
            }
        }
    }

    /// <summary>
    /// 声明比较器，用于去重
    /// </summary>
    public class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim? x, Claim? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Type == y.Type && x.Value == y.Value;
        }

        public int GetHashCode(Claim obj)
        {
            return HashCode.Combine(obj.Type, obj.Value);
        }
    }
}