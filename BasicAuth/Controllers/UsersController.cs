using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BasicAuth.Models;
using BasicAuth.Services;
using System.Security.Claims;

namespace BasicAuth.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// 演示不同级别的授权策略
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 控制器级别的授权，所有action都需要认证
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有用户（仅管理员可访问）
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")] // 基于策略的授权
        public async Task<ActionResult<List<UserInfo>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userInfos = users.Select(u => _userService.ToUserInfo(u)).ToList();
                
                _logger.LogInformation("管理员 {Username} 获取了所有用户列表", User.Identity?.Name);
                return Ok(userInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户列表时发生错误");
                return StatusCode(500, new { Message = "获取用户列表时发生错误" });
            }
        }

        /// <summary>
        /// 根据ID获取用户信息（需要用户管理权限）
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "CanManageUsers")] // 基于声明的授权
        public async Task<ActionResult<UserInfo>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "用户不存在" });
                }

                return Ok(_userService.ToUserInfo(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户 {UserId} 信息时发生错误", id);
                return StatusCode(500, new { Message = "获取用户信息时发生错误" });
            }
        }

        /// <summary>
        /// 删除用户（仅超级管理员可操作）
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "SuperAdmin")] // 复合策略授权
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = "用户不存在" });
                }

                // 这里只是演示，实际项目中应该软删除或记录删除日志
                _logger.LogWarning("超级管理员 {AdminUsername} 尝试删除用户 {TargetUsername}",
                    User.Identity?.Name, user.Username);

                return Ok(new { Message = $"用户 {user.Username} 已被标记为删除（演示）" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除用户 {UserId} 时发生错误", id);
                return StatusCode(500, new { Message = "删除用户时发生错误" });
            }
        }

        /// <summary>
        /// 成人内容访问（需要年龄验证）
        /// </summary>
        /// <returns>成人内容</returns>
        [HttpGet("adult-content")]
        [Authorize(Policy = "MinimumAge")] // 自定义授权策略
        public IActionResult GetAdultContent()
        {
            var username = User.Identity?.Name;
            var age = User.FindFirst("age")?.Value;
            
            _logger.LogInformation("用户 {Username}（年龄：{Age}）访问了成人内容", username, age);
            
            return Ok(new 
            { 
                Message = "这是需要年龄验证的内容",
                AccessTime = DateTime.UtcNow,
                UserAge = age
            });
        }

        /// <summary>
        /// 基于角色的简单授权示例
        /// </summary>
        /// <returns>管理员专属内容</returns>
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")] // 基于角色的简单授权
        public IActionResult GetAdminOnlyContent()
        {
            return Ok(new 
            { 
                Message = "这是管理员专属内容",
                AdminUser = User.Identity?.Name,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }

        /// <summary>
        /// 多角色授权示例
        /// </summary>
        /// <returns>管理员或经理可访问的内容</returns>
        [HttpGet("manager-content")]
        [Authorize(Roles = "Admin,Manager")] // 多角色授权
        public IActionResult GetManagerContent()
        {
            return Ok(new 
            { 
                Message = "这是管理员和经理可以访问的内容",
                User = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}