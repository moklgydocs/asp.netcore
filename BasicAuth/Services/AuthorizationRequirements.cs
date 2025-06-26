using Microsoft.AspNetCore.Authorization;

namespace BasicAuth.Services
{
    /// <summary>
    /// 最小年龄要求
    /// </summary>
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }

    /// <summary>
    /// 最小年龄授权处理器
    /// </summary>
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgeHandler> _logger;

        public MinimumAgeHandler(ILogger<MinimumAgeHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MinimumAgeRequirement requirement)
        {
            // 获取用户年龄声明
            var ageClaim = context.User.FindFirst("age");
            
            if (ageClaim == null)
            {
                _logger.LogWarning("用户 {User} 缺少年龄声明", context.User.Identity?.Name);
                return Task.CompletedTask;
            }

            if (!int.TryParse(ageClaim.Value, out var age))
            {
                _logger.LogWarning("用户 {User} 年龄声明格式无效: {Age}", 
                    context.User.Identity?.Name, ageClaim.Value);
                return Task.CompletedTask;
            }

            if (age >= requirement.MinimumAge)
            {
                _logger.LogInformation("用户 {User} 年龄 {Age} 满足最小年龄要求 {MinAge}",
                    context.User.Identity?.Name, age, requirement.MinimumAge);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("用户 {User} 年龄 {Age} 不满足最小年龄要求 {MinAge}",
                    context.User.Identity?.Name, age, requirement.MinimumAge);
            }

            return Task.CompletedTask;
        }
    }
}