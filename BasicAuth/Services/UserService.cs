using BasicAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicAuth.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 验证用户凭据
        /// </summary>
        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            try
            {
                // 根据用户名查找用户
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("用户 {Username} 不存在或已被禁用", username);
                    return null;
                }

                // 验证密码
                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("用户 {Username} 密码验证失败", username);
                    return null;
                }

                _logger.LogInformation("用户 {Username} 登录成功", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证用户 {Username} 时发生错误", username);
                return null;
            }
        }

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        /// <summary>
        /// 将用户实体转换为用户信息模型
        /// </summary>
        public UserInfo ToUserInfo(User user)
        {
            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Permissions = string.IsNullOrEmpty(user.Permissions) 
                    ? new List<string>() 
                    : user.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            };
        }
    }
}