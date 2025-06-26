using System.Security.Claims;
using BasicAuth.Models;

namespace BasicAuth.Services
{
    /// <summary>
    /// JWT服务接口
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 生成JWT Token
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>JWT Token字符串</returns>
        string GenerateJwtToken(User user);

        /// <summary>
        /// 从JWT Token中获取用户声明
        /// </summary>
        /// <param name="token">JWT Token</param>
        /// <returns>声明集合</returns>
        ClaimsPrincipal? GetPrincipalFromToken(string token);

        /// <summary>
        /// 验证JWT Token是否有效
        /// </summary>
        /// <param name="token">JWT Token</param>
        /// <returns>是否有效</returns>
        bool ValidateToken(string token);
    }
}