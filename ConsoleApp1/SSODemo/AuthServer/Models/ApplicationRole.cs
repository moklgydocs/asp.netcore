using Microsoft.AspNetCore.Identity;

namespace AuthServer.Models
{
    /// <summary>
    /// 应用程序角色模型
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// 角色显示名称
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 角色分类
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}