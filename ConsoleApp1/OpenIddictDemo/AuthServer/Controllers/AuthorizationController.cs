using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using System.Security.Claims;
using AuthServer.Models;
using AuthServer.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthServer.Controllers
{
    /// <summary>
    /// OpenIddict授权控制器
    /// 处理OAuth2/OpenID Connect授权流程
    /// </summary>
    public class AuthorizationController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthorizationController> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        /// 授权端点
        /// 处理OAuth2/OpenID Connect授权请求
        /// </summary>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("无法获取OpenID Connect请求");

            try
            {
                // 检查用户是否已认证
                var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
                
                if (!result.Succeeded || request.HasPrompt(Prompts.Login))
                {
                    // 如果用户未认证或明确要求登录，重定向到登录页面
                    if (request.HasPrompt(Prompts.None))
                    {
                        // 如果prompt=none但用户未认证，返回错误
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户必须登录"
                            }));
                    }

                    // 构建登录URL并重定向
                    var loginUrl = Url.Action("Login", "Account", new { returnUrl = Request.PathBase + Request.Path + QueryString.Create(Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList()) });
                    return Challenge(new AuthenticationProperties { RedirectUri = loginUrl }, IdentityConstants.ApplicationScheme);
                }

                // 获取当前用户
                var user = await _userManager.GetUserAsync(result.Principal) ??
                    throw new InvalidOperationException("无法获取用户信息");

                // 创建新的ClaimsIdentity，包含所需的声明
                var identity = new ClaimsIdentity(
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                // 添加用户声明
                var claims = await _userService.GetUserClaimsAsync(user);
                identity.AddClaims(claims);

                // 确保包含必需的声明
                identity.SetClaim(Claims.Subject, user.Id);
                identity.SetClaim(Claims.Email, user.Email);
                identity.SetClaim(Claims.Name, user.UserName);
                identity.SetClaim(Claims.PreferredUsername, user.UserName);

                // 添加其他标准声明
                if (!string.IsNullOrEmpty(user.DisplayName))
                    identity.SetClaim(Claims.Nickname, user.DisplayName);

                if (!string.IsNullOrEmpty(user.FirstName))
                    identity.SetClaim(Claims.GivenName, user.FirstName);

                if (!string.IsNullOrEmpty(user.LastName))
                    identity.SetClaim(Claims.FamilyName, user.LastName);

                // 设置作用域
                identity.SetScopes(request.GetScopes());

                // 设置资源
                identity.SetResources(await GetResourcesAsync(request.GetScopes()));

                // 设置目标受众
                identity.SetDestinations(GetDestinations);

                var principal = new ClaimsPrincipal(identity);

                _logger.LogInformation("为用户 {Username} 创建授权响应，作用域: {Scopes}", 
                    user.UserName, string.Join(", ", request.GetScopes()));

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理授权请求时发生错误");

                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ServerError,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "处理授权请求时发生内部错误"
                    }));
            }
        }

        /// <summary>
        /// 登出端点
        /// 处理OpenID Connect登出请求
        /// </summary>
        [HttpGet("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("无法获取OpenID Connect请求");

            try
            {
                // 登出用户
                await _signInManager.SignOutAsync();

                _logger.LogInformation("用户登出成功");

                // 如果有post_logout_redirect_uri，重定向到该地址
                if (!string.IsNullOrEmpty(request.PostLogoutRedirectUri))
                {
                    return Redirect(request.PostLogoutRedirectUri);
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理登出请求时发生错误");
                return View();
            }
        }

        /// <summary>
        /// 令牌端点
        /// 处理令牌请求
        /// </summary>
        [HttpPost("~/connect/token")]
        [IgnoreAntiforgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("无法获取OpenID Connect请求");

            try
            {
                if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
                {
                    // 从授权码或刷新令牌中获取主体
                    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                    
                    if (!result.Succeeded)
                    {
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "令牌无效或已过期"
                            }));
                    }

                    var userId = result.Principal.GetClaim(Claims.Subject);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "无法获取用户标识"
                            }));
                    }

                    // 获取用户信息
                    var user = await _userService.GetUserByIdAsync(userId);
                    if (user == null || !user.IsActive)
                    {
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户不存在或已被禁用"
                            }));
                    }

                    // 创建新的身份，包含最新的用户信息
                    var identity = new ClaimsIdentity(result.Principal.Claims,
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    // 更新用户声明
                    var claims = await _userService.GetUserClaimsAsync(user);
                    identity.AddClaims(claims);

                    // 设置目标受众
                    identity.SetDestinations(GetDestinations);

                    var principal = new ClaimsPrincipal(identity);

                    _logger.LogInformation("为用户 {Username} 颁发令牌", user.UserName);

                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsClientCredentialsGrantType())
                {
                    // 客户端凭据流程
                    var identity = new ClaimsIdentity(
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    // 添加客户端声明
                    identity.SetClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException("客户端ID不能为空"));
                    identity.SetClaim(Claims.Name, request.ClientId);

                    // 设置作用域
                    identity.SetScopes(request.GetScopes());

                    // 设置资源
                    identity.SetResources(await GetResourcesAsync(request.GetScopes()));

                    // 设置目标受众
                    identity.SetDestinations(GetDestinations);

                    var principal = new ClaimsPrincipal(identity);

                    _logger.LogInformation("为客户端 {ClientId} 颁发令牌", request.ClientId);

                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsPasswordGrantType())
                {
                    // 密码流程（不推荐，仅用于演示）
                    var user = await _userService.GetUserByUsernameAsync(request.Username!);
                    if (user == null || !await _userService.ValidatePasswordAsync(user, request.Password!))
                    {
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户名或密码错误"
                            }));
                    }

                    if (!user.IsActive)
                    {
                        return Forbid(
                            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                            properties: new AuthenticationProperties(new Dictionary<string, string?>
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "用户账户已被禁用"
                            }));
                    }

                    // 更新最后登录时间
                    await _userService.UpdateLastLoginAsync(user);

                    // 创建身份
                    var identity = new ClaimsIdentity(
                        authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                        nameType: Claims.Name,
                        roleType: Claims.Role);

                    // 添加用户声明
                    var claims = await _userService.GetUserClaimsAsync(user);
                    identity.AddClaims(claims);

                    // 设置作用域
                    identity.SetScopes(request.GetScopes());

                    // 设置资源
                    identity.SetResources(await GetResourcesAsync(request.GetScopes()));

                    // 设置目标受众
                    identity.SetDestinations(GetDestinations);

                    var principal = new ClaimsPrincipal(identity);

                    _logger.LogInformation("为用户 {Username} 颁发令牌（密码流程）", user.UserName);

                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.UnsupportedGrantType,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "不支持的授权类型"
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理令牌请求时发生错误");

                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ServerError,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "处理令牌请求时发生内部错误"
                    }));
            }
        }

        /// <summary>
        /// 用户信息端点
        /// 返回当前用户的信息
        /// </summary>
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        [HttpPost("~/connect/userinfo")]
        [Produces("application/json")]
        public async Task<IActionResult> Userinfo()
        {
            try
            {
                var userId = User.GetClaim(Claims.Subject);
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new
                    {
                        error = "invalid_request",
                        error_description = "无法获取用户标识"
                    });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "invalid_request",
                        error_description = "用户不存在"
                    });
                }

                var claims = new Dictionary<string, object>
                {
                    [Claims.Subject] = user.Id,
                };

                // 根据作用域返回相应的声明
                if (User.HasScope(Scopes.Email))
                {
                    claims[Claims.Email] = user.Email ?? "";
                    claims[Claims.EmailVerified] = user.EmailConfirmed;
                }

                if (User.HasScope(Scopes.Profile))
                {
                    claims[Claims.Name] = user.UserName ?? "";
                    claims[Claims.PreferredUsername] = user.UserName ?? "";
                    
                    if (!string.IsNullOrEmpty(user.DisplayName))
                        claims[Claims.Nickname] = user.DisplayName;
                    
                    if (!string.IsNullOrEmpty(user.FirstName))
                        claims[Claims.GivenName] = user.FirstName;
                    
                    if (!string.IsNullOrEmpty(user.LastName))
                        claims[Claims.FamilyName] = user.LastName;
                }

                if (User.HasScope(Scopes.Roles))
                {
                    var roles = await _userService.GetUserRolesAsync(user);
                    if (roles.Any())
                    {
                        claims[Claims.Role] = roles.Count == 1 ? roles[0] : roles.ToArray();
                    }
                }

                // 添加自定义声明
                if (!string.IsNullOrEmpty(user.Department))
                    claims["department"] = user.Department;

                if (!string.IsNullOrEmpty(user.JobTitle))
                    claims["job_title"] = user.JobTitle;

                var permissions = user.GetPermissions();
                if (permissions.Any())
                {
                    claims["permissions"] = permissions.ToArray();
                }

                _logger.LogInformation("返回用户 {Username} 的信息", user.UserName);

                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户信息时发生错误");
                return StatusCode(500, new
                {
                    error = "server_error",
                    error_description = "获取用户信息时发生内部错误"
                });
            }
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        private async Task<IEnumerable<string>> GetResourcesAsync(IEnumerable<string> scopes)
        {
            // 这里可以根据作用域返回相应的资源
            // 简单起见，我们返回固定的资源列表
            await Task.CompletedTask;
            return new[] { "api1" };
        }

        /// <summary>
        /// 确定声明的目标受众
        /// </summary>
        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            // 决定哪些声明应该包含在访问令牌或身份令牌中
            switch (claim.Type)
            {
                case Claims.Name:
                case Claims.PreferredUsername:
                    yield return Destinations.AccessToken;
                    
                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;
                    
                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;
                    
                    if (claim.Subject.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;
                    
                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;
                    
                    if (claim.Subject.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;
                    
                    yield break;

                // 自定义声明默认只包含在访问令牌中
                case "permission":
                case "department":
                case "job_title":
                    yield return Destinations.AccessToken;
                    yield break;

                // 标准OpenID Connect声明
                case Claims.Subject:
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                    yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}