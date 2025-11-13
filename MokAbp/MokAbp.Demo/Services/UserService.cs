using MokAbp.DependencyInjection.Attributes;

namespace MokAbp.Demo.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        void CreateUser(string username);
        string GetUserInfo(string username);
        void DeleteUser(string username);
    }

    /// <summary>
    /// 用户服务实现
    /// </summary>
    [TransientDependency]
    public class UserService : IUserService
    {
        private readonly ILoggerService _logger;
        private static readonly Dictionary<string, DateTime> _users = new();

        public UserService(ILoggerService logger)
        {
            _logger = logger;
        }

        public void CreateUser(string username)
        {
            if (_users.ContainsKey(username))
            {
                _logger.LogWarning($"用户 '{username}' 已存在");
                return;
            }

            _users[username] = DateTime.Now;
            _logger.LogInfo($"用户 '{username}' 创建成功");
        }

        public string GetUserInfo(string username)
        {
            if (!_users.TryGetValue(username, out var createTime))
            {
                _logger.LogWarning($"用户 '{username}' 不存在");
                return $"用户 '{username}' 不存在";
            }

            return $"用户: {username}, 创建时间: {createTime:yyyy-MM-dd HH:mm:ss}";
        }

        public void DeleteUser(string username)
        {
            if (_users.Remove(username))
            {
                _logger.LogInfo($"用户 '{username}' 已删除");
            }
            else
            {
                _logger.LogWarning($"用户 '{username}' 不存在，无法删除");
            }
        }
    }
}
