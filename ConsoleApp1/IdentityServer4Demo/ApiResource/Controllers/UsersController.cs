using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiResource.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// 演示基于权限的API保护
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "UserManagement")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        // 模拟用户数据
        private static readonly List<UserInfo> _users = new()
        {
            new UserInfo { Id = 1, Username = "admin", Email = "admin@example.com", Role = "Admin" },
            new UserInfo { Id = 2, Username = "manager", Email = "manager@example.com", Role = "Manager" },
            new UserInfo { Id = 3, Username = "user", Email = "user@example.com", Role = "User" },
            new UserInfo { Id = 4, Username = "developer", Email = "developer@example.com", Role = "Developer" },
            new UserInfo { Id = 5, Username = "tester", Email = "tester@example.com", Role = "Tester" }
        };

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<UserInfo>> GetAllUsers()
        {
            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            var scopes = User.FindAll("scope").Select(c => c.Value).ToList();
            
            _logger.LogInformation("用户 {Username} 获取用户列表，拥有作用域: {Scopes}", 
                currentUser, string.Join(", ", scopes));

            return Ok(_users);
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<UserInfo> GetUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"用户ID {id} 不存在");
            }

            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            _logger.LogInformation("用户 {Username} 获取用户 {UserId} 的信息", currentUser, id);

            return Ok(user);
        }

        /// <summary>
        /// 创建新用户（需要特定权限）
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "UserManagePermission")]
        public ActionResult<UserInfo> CreateUser([FromBody] CreateUserRequest request)
        {
            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            var permissions = User.FindAll("permission").Select(c => c.Value).ToList();
            
            _logger.LogInformation("用户 {Username} 创建新用户，拥有权限: {Permissions}", 
                currentUser, string.Join(", ", permissions));

            var newUser = new UserInfo
            {
                Id = _users.Max(u => u.Id) + 1,
                Username = request.Username,
                Email = request.Email,
                Role = request.Role
            };

            _users.Add(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        /// <summary>
        /// 更新用户信息（需要特定权限）
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "UserManagePermission")]
        public ActionResult<UserInfo> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"用户ID {id} 不存在");
            }

            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            _logger.LogInformation("用户 {Username} 更新用户 {UserId} 的信息", currentUser, id);

            user.Username = request.Username;
            user.Email = request.Email;
            user.Role = request.Role;

            return Ok(user);
        }

        /// <summary>
        /// 删除用户（仅管理员可操作）
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"用户ID {id} 不存在");
            }

            var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "未知角色";
            
            _logger.LogInformation("管理员 {Username} (角色: {Role}) 删除用户 {UserId}", 
                currentUser, role, id);

            _users.Remove(user);

            return Ok(new { Message = $"用户 {user.Username} 已被删除" });
        }

        /// <summary>
        /// 获取当前用户的令牌信息（调试用）
        /// </summary>
        [HttpGet("me/token-info")]
        public IActionResult GetTokenInfo()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var identity = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                Name = User.Identity?.Name,
                AuthenticationType = User.Identity?.AuthenticationType,
                Claims = claims
            };

            return Ok(identity);
        }
    }

    /// <summary>
    /// 用户信息模型
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// 创建用户请求
    /// </summary>
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新用户请求
    /// </summary>
    public class UpdateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}