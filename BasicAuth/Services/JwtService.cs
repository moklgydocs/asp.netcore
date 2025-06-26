using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BasicAuth.Models;

namespace BasicAuth.Services
{
    /// <summary>
    /// JWT服务实现
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // 从配置中读取JWT设置
            var jwtSettings = _configuration.GetSection("JwtSettings");
            _secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JWT SecretKey 未配置");
            _issuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("JWT Issuer 未配置");
            _audience = jwtSettings["Audience"] ?? throw new ArgumentNullException("JWT Audience 未配置");
            _expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");
        }

        /// <summary>
        /// 生成JWT Token
        /// </summary>
        public string GenerateJwtToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                // 创建用户声明
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role),
                    new("age", user.Age.ToString())
                };

                // 添加权限声明
                if (!string.IsNullOrEmpty(user.Permissions))
                {
                    var permissions = user.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var permission in permissions)
                    {
                        claims.Add(new Claim("permission", permission.Trim()));
                    }
                }

                // 创建Token描述符
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // 创建并写入Token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("为用户 {Username} 生成JWT Token成功", user.Username);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为用户 {Username} 生成JWT Token时发生错误", user.Username);
                throw;
            }
        }

        /// <summary>
        /// 从JWT Token中获取用户声明
        /// </summary>
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "解析JWT Token失败");
                return null;
            }
        }

        /// <summary>
        /// 验证JWT Token是否有效
        /// </summary>
        public bool ValidateToken(string token)
        {
            return GetPrincipalFromToken(token) != null;
        }
    }
}