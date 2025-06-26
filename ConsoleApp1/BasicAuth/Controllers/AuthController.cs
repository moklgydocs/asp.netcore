using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BasicAuth.Models;
using BasicAuth.Services;

namespace BasicAuth.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Cookie认证登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>登录结果</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // 验证用户凭据
                var user = await _userService.ValidateUserAsync(request.Username, request.Password);
                if (user == null)
                {
                    return BadRequest(new LoginResponse 
                    { 
                        Success = false, 
                        Message = "用户名或密码错误" 
                    });
                }

                // 创建用户声明
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role),
                    new("age", user.Age.ToString())
                };

                // 添加权限声明
                if (!string.IsNullOrEmpty(user.Permissions))
                {
                    var permissions = user.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var permission in permissions)
                    {
                        claims.Add(new Claim("permission", permission.Trim()));
                    }
                }

                // 创建身份标识
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // 设置认证属性
                var authProperties = new AuthenticationProperties
                {
                    // 是否持久化Cookie
                    IsPersistent = request.RememberMe,
                    // 过期时间
                    ExpiresUtc = request.RememberMe 
                        ? DateTimeOffset.UtcNow.AddDays(30) 
                        : DateTimeOffset.UtcNow.AddHours(24)
                };

                // 执行登录
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                _logger.LogInformation("用户 {Username} Cookie认证登录成功", user.Username);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "登录成功",
                    User = _userService.ToUserInfo(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户 {Username} 登录时发生错误", request.Username);
                return StatusCode(500, new LoginResponse 
                { 
                    Success = false, 
                    Message = "登录过程中发生错误" 
                });
            }
        }

        /// <summary>
        /// JWT Token登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>登录结果（包含JWT Token）</returns>
        [HttpPost("login-jwt")]
        public async Task<ActionResult<LoginResponse>> LoginJwt([FromBody] LoginRequest request)
        {
            try
            {
                // 验证用户凭据
                var user = await _userService.ValidateUserAsync(request.Username, request.Password);
                if (user == null)
                {
                    return BadRequest(new LoginResponse 
                    { 
                        Success = false, 
                        Message = "用户名或密码错误" 
                    });
                }

                // 生成JWT Token
                var token = _jwtService.GenerateJwtToken(user);

                _logger.LogInformation("用户 {Username} JWT认证登录成功", user.Username);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "登录成功",
                    Token = token,
                    User = _userService.ToUserInfo(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户 {Username} JWT登录时发生错误", request.Username);
                return StatusCode(500, new LoginResponse 
                { 
                    Success = false, 
                    Message = "登录过程中发生错误" 
                });
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns>登出结果</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var username = User.Identity?.Name;
                
                // 清除Cookie认证
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("用户 {Username} 登出成功", username);

                return Ok(new { Success = true, Message = "登出成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户登出时发生错误");
                return StatusCode(500, new { Success = false, Message = "登出过程中发生错误" });
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>当前用户信息</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserInfo>> GetCurrentUser()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { Message = "无法获取用户信息" });
                }

                var user = await _userService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return NotFound(new { Message = "用户不存在" });
                }

                return Ok(_userService.ToUserInfo(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取当前用户信息时发生错误");
                return StatusCode(500, new { Message = "获取用户信息时发生错误" });
            }
        }

        /// <summary>
        /// 获取用户声明信息（用于调试）
        /// </summary>
        /// <returns>用户声明列表</returns>
        [HttpGet("claims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}