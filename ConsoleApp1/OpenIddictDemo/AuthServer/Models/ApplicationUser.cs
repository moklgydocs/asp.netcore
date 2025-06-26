using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models
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
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [MaxLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// 姓氏
        /// </summary>
        [MaxLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// 用户头像URL
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [MaxLength(10)]
        public string? Gender { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [MaxLength(100)]
        public string? City { get; set; }

        /// <summary>
        /// 国家/地区
        /// </summary>
        [MaxLength(100)]
        public string? Country { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [MaxLength(20)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// 用户权限（JSON格式存储）
        /// </summary>
        public string? Permissions { get; set; }

        /// <summary>
        /// 用户部门
        /// </summary>
        [MaxLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [MaxLength(100)]
        public string? JobTitle { get; set; }

        /// <summary>
        /// 管理员用户ID（如果有上级）
        /// </summary>
        public string? ManagerId { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginCount { get; set; } = 0;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 是否可以删除
        /// </summary>
        public bool CanDelete { get; set; } = true;

        /// <summary>
        /// 备注信息
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// 获取完整姓名
        /// </summary>
        public string FullName => !string.IsNullOrEmpty(DisplayName) 
            ? DisplayName 
            : $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// 获取用户年龄
        /// </summary>
        public int? Age => DateOfBirth.HasValue 
            ? DateTime.Today.Year - DateOfBirth.Value.Year - (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
            : null;

        /// <summary>
        /// 获取权限列表
        /// </summary>
        public List<string> GetPermissions()
        {
            if (string.IsNullOrEmpty(Permissions))
                return new List<string>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(Permissions) ?? new List<string>();
            }
            catch
            {
                // 如果JSON反序列化失败，尝试按逗号分割
                return Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(p => p.Trim())
                                 .ToList();
            }
        }

        /// <summary>
        /// 设置权限列表
        /// </summary>
        public void SetPermissions(IEnumerable<string> permissions)
        {
            Permissions = System.Text.Json.JsonSerializer.Serialize(permissions.ToList());
        }

        /// <summary>
        /// 检查是否有特定权限
        /// </summary>
        public bool HasPermission(string permission)
        {
            return GetPermissions().Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        public void AddPermission(string permission)
        {
            var permissions = GetPermissions();
            if (!permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
            {
                permissions.Add(permission);
                SetPermissions(permissions);
            }
        }

        /// <summary>
        /// 移除权限
        /// </summary>
        public void RemovePermission(string permission)
        {
            var permissions = GetPermissions();
            permissions.RemoveAll(p => string.Equals(p, permission, StringComparison.OrdinalIgnoreCase));
            SetPermissions(permissions);
        }

        /// <summary>
        /// 更新最后登录信息
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            LoginCount++;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}