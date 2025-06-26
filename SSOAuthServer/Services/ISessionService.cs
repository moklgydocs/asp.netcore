namespace AuthServer.Services
{
    /// <summary>
    /// 会话管理服务接口
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// 创建用户会话
        /// </summary>
        Task<string> CreateSessionAsync(string userId, string clientId, string? ipAddress = null);

        /// <summary>
        /// 获取用户会话信息
        /// </summary>
        Task<UserSession?> GetSessionAsync(string sessionId);

        /// <summary>
        /// 更新会话最后活动时间
        /// </summary>
        Task UpdateSessionActivityAsync(string sessionId);

        /// <summary>
        /// 结束用户会话
        /// </summary>
        Task EndSessionAsync(string sessionId);

        /// <summary>
        /// 结束用户所有会话
        /// </summary>
        Task EndAllUserSessionsAsync(string userId);

        /// <summary>
        /// 获取用户活跃会话列表
        /// </summary>
        Task<List<UserSession>> GetUserActiveSessionsAsync(string userId);

        /// <summary>
        /// 清理过期会话
        /// </summary>
        Task CleanupExpiredSessionsAsync();
    }

    /// <summary>
    /// 用户会话模型
    /// </summary>
    public class UserSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}