using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    /// <summary>
    /// 应用程序用户模型
    /// 扩展ASP.NET Core Identity的IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 用户显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// 用户头像URL
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 用户年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 用户权限（以逗号分隔）
        /// </summary>
        public string? Permissions { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 获取完整姓名
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// 获取权限列表
        /// </summary>
        public List<string> GetPermissions()
        {
            if (string.IsNullOrEmpty(Permissions))
                return new List<string>();
                
            return Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(p => p.Trim())
                             .ToList();
        }

        /// <summary>
        /// 设置权限列表
        /// </summary>
        public void SetPermissions(IEnumerable<string> permissions)
        {
            Permissions = string.Join(",", permissions);
        }
    }
}