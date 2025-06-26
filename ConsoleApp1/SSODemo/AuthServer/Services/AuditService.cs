using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthServer.Services
{
    /// <summary>
    /// 审计服务实现
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<AuditService> _logger;
        private const string AUDIT_PREFIX = "audit:";
        private const string USER_AUDIT_PREFIX = "user_audit:";

        public AuditService(IDistributedCache cache, ILogger<AuditService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 记录登录事件
        /// </summary>
        public async Task LogLoginAsync(string userId, string clientId, string? ipAddress, bool success, string? failureReason = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "Login",
                Description = success ? "用户登录成功" : "用户登录失败",
                ClientId = clientId,
                IpAddress = ipAddress,
                EventType = AuditEventType.Login,
                Success = success,
                FailureReason = failureReason
            };

            await SaveAuditLogAsync(auditLog);
            _logger.LogInformation("记录登录事件: UserId={UserId}, Success={Success}, ClientId={ClientId}", 
                userId, success, clientId);
        }

        /// <summary>
        /// 记录登出事件
        /// </summary>
        public async Task LogLogoutAsync(string userId, string clientId, string? ipAddress)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "Logout",
                Description = "用户登出",
                ClientId = clientId,
                IpAddress = ipAddress,
                EventType = AuditEventType.Logout,
                Success = true
            };

            await SaveAuditLogAsync(auditLog);
            _logger.LogInformation("记录登出事件: UserId={UserId}, ClientId={ClientId}", userId, clientId);
        }

        /// <summary>
        /// 记录用户操作
        /// </summary>
        public async Task LogUserActionAsync(string userId, string action, string? details = null, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                Description = details ?? action,
                IpAddress = ipAddress,
                EventType = AuditEventType.DataAccess,
                Success = true
            };

            await SaveAuditLogAsync(auditLog);
        }

        /// <summary>
        /// 记录安全事件
        /// </summary>
        public async Task LogSecurityEventAsync(SecurityEventType eventType, string userId, string description, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = eventType.ToString(),
                Description = description,
                IpAddress = ipAddress,
                EventType = AuditEventType.SecurityEvent,
                Success = false,
                Properties = new Dictionary<string, object>
                {
                    ["SecurityEventType"] = eventType.ToString()
                }
            };

            await SaveAuditLogAsync(auditLog);
            _logger.LogWarning("记录安全事件: EventType={EventType}, UserId={UserId}, Description={Description}", 
                eventType, userId, description);
        }

        /// <summary>
        /// 获取用户审计日志
        /// </summary>
        public async Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var key = $"{USER_AUDIT_PREFIX}{userId}";
                var auditLogsData = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(auditLogsData))
                    return new List<AuditLog>();

                var allLogs = JsonSerializer.Deserialize<List<AuditLog>>(auditLogsData) ?? new List<AuditLog>();
                
                return allLogs
                    .OrderByDescending(log => log.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户审计日志失败: UserId={UserId}", userId);
                return new List<AuditLog>();
            }
        }

        /// <summary>
        /// 获取系统审计日志
        /// </summary>
        public async Task<List<AuditLog>> GetSystemAuditLogsAsync(DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            try
            {
                // 在实际项目中，这里应该从数据库或其他持久化存储中获取
                // 这里简化为从缓存获取最近的日志
                var logs = new List<AuditLog>();
                
                // 模拟获取系统日志的逻辑
                // 实际实现中应该根据日期范围和分页参数查询数据库
                
                return logs
                    .Where(log => (!startDate.HasValue || log.Timestamp >= startDate.Value) &&
                                 (!endDate.HasValue || log.Timestamp <= endDate.Value))
                    .OrderByDescending(log => log.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取系统审计日志失败");
                return new List<AuditLog>();
            }
        }

        /// <summary>
        /// 保存审计日志
        /// </summary>
        private async Task SaveAuditLogAsync(AuditLog auditLog)
        {
            try
            {
                // 保存到系统审计日志
                var systemKey = $"{AUDIT_PREFIX}system:{DateTime.UtcNow:yyyyMMdd}";
                await AppendToAuditLogAsync(systemKey, auditLog, TimeSpan.FromDays(90));

                // 保存到用户审计日志
                var userKey = $"{USER_AUDIT_PREFIX}{auditLog.UserId}";
                await AppendToAuditLogAsync(userKey, auditLog, TimeSpan.FromDays(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存审计日志失败: Action={Action}, UserId={UserId}", 
                    auditLog.Action, auditLog.UserId);
            }
        }

        /// <summary>
        /// 追加审计日志到缓存
        /// </summary>
        private async Task AppendToAuditLogAsync(string key, AuditLog auditLog, TimeSpan expiration)
        {
            try
            {
                var existingData = await _cache.GetStringAsync(key);
                var logs = new List<AuditLog>();

                if (!string.IsNullOrEmpty(existingData))
                {
                    logs = JsonSerializer.Deserialize<List<AuditLog>>(existingData) ?? new List<AuditLog>();
                }

                logs.Add(auditLog);

                // 限制日志数量，保留最新的1000条
                if (logs.Count > 1000)
                {
                    logs = logs.OrderByDescending(log => log.Timestamp).Take(1000).ToList();
                }

                await _cache.SetStringAsync(
                    key,
                    JsonSerializer.Serialize(logs),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "追加审计日志失败: Key={Key}", key);
            }
        }
    }
}