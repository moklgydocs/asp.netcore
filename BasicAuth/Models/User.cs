using System.ComponentModel.DataAnnotations;

namespace BasicAuth.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 用户角色：Admin, User, Manager等
        /// </summary>
        [Required]
        public string Role { get; set; } = "User";

        /// <summary>
        /// 用户年龄（用于演示自定义授权策略）
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 用户权限列表（以逗号分隔）
        /// </summary>
        public string Permissions { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 登录请求模型
    /// </summary>
    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 是否记住登录状态
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }

    /// <summary>
    /// 登录响应模型
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public UserInfo? User { get; set; }
    }

    /// <summary>
    /// 用户信息模型（不包含敏感信息）
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}