using System.Diagnostics;
using StackExchange.Redis;
namespace MiddleWare.HealthCheck
{
    public static class HealthCheckMiddleExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, Action<HealthCheckOption> healthCheckOption)
        {
            var options = new HealthCheckOption();
            healthCheckOption?.Invoke(options);
            // 如果没有设置健康检查路径，则使用默认路径
            if (options.HealthChecks.Count == 0)
            {
                options.HealthChecks.Add(_ => Task.FromResult(new HealthCheckResult
                {
                    HealthStatus = HealthStatus.Healthy,
                    Description = "Application is running",
                    Duration = TimeSpan.Zero,
                    Name = "System"
                }));
            }

            if (app == null) throw new ArgumentNullException(nameof(app));
            if (healthCheckOption == null) throw new ArgumentNullException(nameof(healthCheckOption));
            return app.UseMiddleware<HealthCheckMiddleware>(options);
        }


        public static async Task<HealthCheckResult> CheckRedisConnectionAsync()
        {
            // 你可以将连接字符串放到配置文件或环境变量中
            var redisConnectionString = "localhost:6379"; // 替换为你的 Redis 地址
            var result = new HealthCheckResult
            {
                Name = "Redis"
            };

            try
            {
                var sw = Stopwatch.StartNew();
                using var redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
                var db = redis.GetDatabase();
                // 简单的 PING 命令
                var pong = await db.PingAsync();
                sw.Stop();

                result.HealthStatus = HealthStatus.Healthy;
                result.Description = $"Redis 响应时间: {pong.TotalMilliseconds}ms";
                result.Duration = sw.Elapsed;
                Console.WriteLine(result.Description);
            }
            catch (Exception ex)
            {
                result.HealthStatus = HealthStatus.Unhealthy;
                result.Description = $"Redis 连接失败: {ex.Message}";
                result.Duration = TimeSpan.Zero;
                Console.WriteLine(result.Description); 
            }

            return result;
        }
    }
}
