using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthServer.Services
{
    /// <summary>
    /// 会话管理服务实现
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<SessionService> _logger;
        private const string SESSION_PREFIX = "session:";
        private const string USER_SESSIONS_PREFIX = "user_sessions:";
        private const int DEFAULT_SESSION_TIMEOUT_HOURS = 8;

        public SessionService(IDistributedCache cache, ILogger<SessionService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 创建用户会话
        /// </summary>
        public async Task<string> CreateSessionAsync(string userId, string clientId, string? ipAddress = null)
        {
            try
            {
                var sessionId = Guid.NewGuid().ToString("N");
                var now = DateTime.UtcNow;
                
                var session = new UserSession
                {
                    SessionId = sessionId,
                    UserId = userId,
                    ClientId = clientId,
                    IpAddress = ipAddress,
                    CreatedAt = now,
                    LastActivityAt = now,
                    ExpiresAt = now.AddHours(DEFAULT_SESSION_TIMEOUT_HOURS),
                    IsActive = true
                };

                // 存储会话信息
                await _cache.SetStringAsync(
                    $"{SESSION_PREFIX}{sessionId}",
                    JsonSerializer.Serialize(session),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(DEFAULT_SESSION_TIMEOUT_HOURS)
                    });

                // 添加到用户会话列表
                await AddToUserSessionsAsync(userId, sessionId);

                _logger.LogInformation("创建用户会话成功: UserId={UserId}, SessionId={SessionId}, ClientId={ClientId}", 
                    userId, sessionId, clientId);

                return sessionId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建用户会话失败: UserId={UserId}, ClientId={ClientId}", userId, clientId);
                throw;
            }
        }

        /// <summary>
        /// 获取用户会话信息
        /// </summary>
        public async Task<UserSession?> GetSessionAsync(string sessionId)
        {
            try
            {
                var sessionData = await _cache.GetStringAsync($"{SESSION_PREFIX}{sessionId}");
                if (string.IsNullOrEmpty(sessionData))
                    return null;

                var session = JsonSerializer.Deserialize<UserSession>(sessionData);
                
                // 检查会话是否过期
                if (session != null && session.ExpiresAt < DateTime.UtcNow)
                {
                    await EndSessionAsync(sessionId);
                    return null;
                }

                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取会话信息失败: SessionId={SessionId}", sessionId);
                return null;
            }
        }

        /// <summary>
        /// 更新会话最后活动时间
        /// </summary>
        public async Task UpdateSessionActivityAsync(string sessionId)
        {
            try
            {
                var session = await GetSessionAsync(sessionId);
                if (session == null)
                    return;

                session.LastActivityAt = DateTime.UtcNow;
                session.ExpiresAt = DateTime.UtcNow.AddHours(DEFAULT_SESSION_TIMEOUT_HOURS);

                await _cache.SetStringAsync(
                    $"{SESSION_PREFIX}{sessionId}",
                    JsonSerializer.Serialize(session),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(DEFAULT_SESSION_TIMEOUT_HOURS)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新会话活动时间失败: SessionId={SessionId}", sessionId);
            }
        }

        /// <summary>
        /// 结束用户会话
        /// </summary>
        public async Task EndSessionAsync(string sessionId)
        {
            try
            {
                var session = await GetSessionAsync(sessionId);
                if (session == null)
                    return;

                // 从缓存中移除会话
                await _cache.RemoveAsync($"{SESSION_PREFIX}{sessionId}");

                // 从用户会话列表中移除
                await RemoveFromUserSessionsAsync(session.UserId, sessionId);

                _logger.LogInformation("结束用户会话成功: SessionId={SessionId}, UserId={UserId}", 
                    sessionId, session.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "结束会话失败: SessionId={SessionId}", sessionId);
            }
        }

        /// <summary>
        /// 结束用户所有会话
        /// </summary>
        public async Task EndAllUserSessionsAsync(string userId)
        {
            try
            {
                var userSessions = await GetUserActiveSessionsAsync(userId);
                
                var tasks = userSessions.Select(session => EndSessionAsync(session.SessionId));
                await Task.WhenAll(tasks);

                _logger.LogInformation("结束用户所有会话成功: UserId={UserId}, SessionCount={Count}", 
                    userId, userSessions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "结束用户所有会话失败: UserId={UserId}", userId);
            }
        }

        /// <summary>
        /// 获取用户活跃会话列表
        /// </summary>
        public async Task<List<UserSession>> GetUserActiveSessionsAsync(string userId)
        {
            try
            {
                var sessionIds = await GetUserSessionIdsAsync(userId);
                var sessions = new List<UserSession>();

                foreach (var sessionId in sessionIds)
                {
                    var session = await GetSessionAsync(sessionId);
                    if (session != null && session.IsActive)
                    {
                        sessions.Add(session);
                    }
                }

                return sessions.OrderByDescending(s => s.LastActivityAt).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户活跃会话失败: UserId={UserId}", userId);
                return new List<UserSession>();
            }
        }

        /// <summary>
        /// 清理过期会话
        /// </summary>
        public async Task CleanupExpiredSessionsAsync()
        {
            try
            {
                // 这里可以实现定时清理逻辑
                // 由于使用了分布式缓存，过期会话会自动被清理
                _logger.LogInformation("会话清理任务执行完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理过期会话失败");
            }
        }

        /// <summary>
        /// 添加会话到用户会话列表
        /// </summary>
        private async Task AddToUserSessionsAsync(string userId, string sessionId)
        {
            try
            {
                var key = $"{USER_SESSIONS_PREFIX}{userId}";
                var sessionIds = await GetUserSessionIdsAsync(userId);
                
                if (!sessionIds.Contains(sessionId))
                {
                    sessionIds.Add(sessionId);
                    
                    await _cache.SetStringAsync(
                        key,
                        JsonSerializer.Serialize(sessionIds),
                        new DistributedCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromDays(1)
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加用户会话ID失败: UserId={UserId}, SessionId={SessionId}", userId, sessionId);
            }
        }

        /// <summary>
        /// 从用户会话列表中移除会话
        /// </summary>
        private async Task RemoveFromUserSessionsAsync(string userId, string sessionId)
        {
            try
            {
                var key = $"{USER_SESSIONS_PREFIX}{userId}";
                var sessionIds = await GetUserSessionIdsAsync(userId);
                
                if (sessionIds.Remove(sessionId))
                {
                    if (sessionIds.Any())
                    {
                        await _cache.SetStringAsync(
                            key,
                            JsonSerializer.Serialize(sessionIds),
                            new DistributedCacheEntryOptions
                            {
                                SlidingExpiration = TimeSpan.FromDays(1)
                            });
                    }
                    else
                    {
                        await _cache.RemoveAsync(key);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除用户会话ID失败: UserId={UserId}, SessionId={SessionId}", userId, sessionId);
            }
        }

        /// <summary>
        /// 获取用户会话ID列表
        /// </summary>
        private async Task<List<string>> GetUserSessionIdsAsync(string userId)
        {
            try
            {
                var key = $"{USER_SESSIONS_PREFIX}{userId}";
                var sessionIdsData = await _cache.GetStringAsync(key);
                
                if (string.IsNullOrEmpty(sessionIdsData))
                    return new List<string>();

                return JsonSerializer.Deserialize<List<string>>(sessionIdsData) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户会话ID列表失败: UserId={UserId}", userId);
                return new List<string>();
            }
        }
    }
}