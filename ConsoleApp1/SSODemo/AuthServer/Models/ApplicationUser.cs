using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models
{
    /// <summary>
    /// SSO应用程序用户模型
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 员工编号
        /// </summary>
        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// 用户显示名称
        /// </summary>
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 中文姓名
        /// </summary>
        [MaxLength(50)]
        public string? ChineseName { get; set; }

        /// <summary>
        /// 英文名字
        /// </summary>
        [MaxLength(50)]
        public string? EnglishName { get; set; }

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
        /// 身份证号
        /// </summary>
        [MaxLength(18)]
        public string? IdCard { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [MaxLength(100)]
        public string? DepartmentName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [MaxLength(100)]
        public string? Position { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
        [MaxLength(50)]
        public string? Level { get; set; }

        /// <summary>
        /// 直属上司ID
        /// </summary>
        public string? ManagerId { get; set; }

        /// <summary>
        /// 直属上司姓名
        /// </summary>
        [MaxLength(100)]
        public string? ManagerName { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// 员工状态
        /// </summary>
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        /// <summary>
        /// 工作地点
        /// </summary>
        [MaxLength(100)]
        public string? WorkLocation { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        [MaxLength(20)]
        public string? OfficePhone { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [MaxLength(100)]
        public string? EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        [MaxLength(500)]
        public string? Address { get; set; }

        /// <summary>
        /// 用户权限（JSON格式）
        /// </summary>
        public string? Permissions { get; set; }

        /// <summary>
        /// 用户偏好设置（JSON格式）
        /// </summary>
        public string? Preferences { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [MaxLength(45)]
        public string? LastLoginIp { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginCount { get; set; } = 0;

        /// <summary>
        /// 密码最后修改时间
        /// </summary>
        public DateTime? PasswordChangedAt { get; set; }

        /// <summary>
        /// 是否强制修改密码
        /// </summary>
        public bool ForcePasswordChange { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 创建者ID
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// 获取完整显示名称
        /// </summary>
        public string FullDisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(DisplayName))
                    return DisplayName;
                if (!string.IsNullOrEmpty(ChineseName))
                    return ChineseName;
                if (!string.IsNullOrEmpty(EnglishName))
                    return EnglishName;
                return UserName ?? Email ?? "未知用户";
            }
        }

        /// <summary>
        /// 获取用户年龄
        /// </summary>
        public int? Age => DateOfBirth.HasValue 
            ? DateTime.Today.Year - DateOfBirth.Value.Year - (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0)
            : null;

        /// <summary>
        /// 获取工作年限
        /// </summary>
        public TimeSpan? WorkExperience => HireDate.HasValue 
            ? DateTime.Today - HireDate.Value
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
                return new List<string>();
            }
        }

        /// <summary>
        /// 设置权限列表
        /// </summary>
        public void SetPermissions(IEnumerable<string> permissions)
        {
            Permissions = System.Text.Json.JsonSerializer.Serialize(permissions.ToList());
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 获取用户偏好设置
        /// </summary>
        public Dictionary<string, object> GetPreferences()
        {
            if (string.IsNullOrEmpty(Preferences))
                return new Dictionary<string, object>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(Preferences) ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// 设置用户偏好
        /// </summary>
        public void SetPreferences(Dictionary<string, object> preferences)
        {
            Preferences = System.Text.Json.JsonSerializer.Serialize(preferences);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新登录信息
        /// </summary>
        public void UpdateLoginInfo(string? ipAddress = null)
        {
            LastLoginAt = DateTime.UtcNow;
            LoginCount++;
            if (!string.IsNullOrEmpty(ipAddress))
                LastLoginIp = ipAddress;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 检查是否有特定权限
        /// </summary>
        public bool HasPermission(string permission)
        {
            return GetPermissions().Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// 员工状态枚举
    /// </summary>
    public enum EmployeeStatus
    {
        /// <summary>
        /// 在职
        /// </summary>
        Active = 1,

        /// <summary>
        /// 试用期
        /// </summary>
        Probation = 2,

        /// <summary>
        /// 停职
        /// </summary>
        Suspended = 3,

        /// <summary>
        /// 离职
        /// </summary>
        Terminated = 4,

        /// <summary>
        /// 退休
        /// </summary>
        Retired = 5
    }
}