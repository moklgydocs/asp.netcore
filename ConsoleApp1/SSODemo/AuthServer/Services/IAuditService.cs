namespace AuthServer.Services
{
    /// <summary>
    /// 审计服务接口
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// 记录登录事件
        /// </summary>
        Task LogLoginAsync(string userId, string clientId, string? ipAddress, bool success, string? failureReason = null);

        /// <summary>
        /// 记录登出事件
        /// </summary>
        Task LogLogoutAsync(string userId, string clientId, string? ipAddress);

        /// <summary>
        /// 记录用户操作
        /// </summary>
        Task LogUserActionAsync(string userId, string action, string? details = null, string? ipAddress = null);

        /// <summary>
        /// 记录安全事件
        /// </summary>
        Task LogSecurityEventAsync(SecurityEventType eventType, string userId, string description, string? ipAddress = null);

        /// <summary>
        /// 获取用户审计日志
        /// </summary>
        Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int page = 1, int pageSize = 20);

        /// <summary>
        /// 获取系统审计日志
        /// </summary>
        Task<List<AuditLog>> GetSystemAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50);
    }

    /// <summary>
    /// 审计日志模型
    /// </summary>
    public class AuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ClientId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public AuditEventType EventType { get; set; }
        public bool Success { get; set; } = true;
        public string? FailureReason { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    /// <summary>
    /// 审计事件类型
    /// </summary>
    public enum AuditEventType
    {
        Login,
        Logout,
        PasswordChange,
        UserCreated,
        UserUpdated,
        UserDeleted,
        RoleAssigned,
        RoleRemoved,
        PermissionGranted,
        PermissionRevoked,
        SecurityEvent,
        ApiAccess,
        DataAccess,
        SystemEvent
    }

    /// <summary>
    /// 安全事件类型
    /// </summary>
    public enum SecurityEventType
    {
        SuspiciousLogin,
        MultipleFailedLogins,
        UnauthorizedAccess,
        PasswordBreach,
        AccountLockout,
        UnusualActivity,
        DataExfiltration
    }
}