using System.Diagnostics;

namespace MiddleWare.HealthCheck
{
    public class HealthCheckMiddleware
    {
        private RequestDelegate next;
        private readonly HealthCheckOption HealthCheckOption;
        private readonly ILogger<HealthCheckMiddleware> Logger;
        public HealthCheckMiddleware(RequestDelegate _next, HealthCheckOption healthCheckOption, ILogger<HealthCheckMiddleware> logger)
        {
            this.next = _next;
            HealthCheckOption = healthCheckOption;
            this.Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Equals(HealthCheckOption.HealthEndpointPath, StringComparison.OrdinalIgnoreCase))
            {
                await HandleHealthCheckAsync(context);
                return;
            }

            await next(context);

        }

        private async Task HandleHealthCheckAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var results = new List<HealthCheckResult>();
            var overallStatus = HealthStatus.Healthy;

            foreach (var item in HealthCheckOption.HealthChecks)
            {
                var sw = Stopwatch.StartNew();
                HealthCheckResult result = await check(context);
                sw.Stop();
                result.Duration = sw.Elapsed;

                results.Add(result);

                // 如果任何检查是不健康的，整体状态就是不健康的
                if (result.HealthStatus == HealthStatus.Unhealthy)
                {
                    overallStatus = HealthStatus.Unhealthy;
                }
                // 如果当前是健康的，但有任何检查是降级的，则整体状态为降级
                else if (overallStatus == HealthStatus.Healthy && result.HealthStatus == HealthStatus.Degraded)
                {
                    overallStatus = HealthStatus.Degraded;
                }


            }
            var response = new
            {
                Status = overallStatus.ToString(),
                Results = results.Select(r => new
                {
                    Name = r.Name,
                    Status = r.HealthStatus.ToString(),
                    Description = r.Description,
                    Duration = $"{r.Duration.TotalMilliseconds}ms"
                }),
                Timestamp = DateTime.UtcNow
            };
            context.Response.StatusCode = overallStatus == HealthStatus.Healthy ? 200 :
                overallStatus == HealthStatus.Degraded ? 200 : 503;
            await context.Response.WriteAsJsonAsync(response);
        }
        private async Task<HealthCheckResult> check(HttpContext context)
        {
            // 取第一个健康检查委托并执行（仅作示例，实际应遍历所有）
            if (HealthCheckOption.HealthChecks.Count > 0)
            {
                return await HealthCheckOption.HealthChecks[0](context);
            }
            // 没有健康检查时返回默认结果
            return new HealthCheckResult
            {
                Name = "Default",
                HealthStatus = HealthStatus.Healthy,
                Description = "No health checks configured.",
                Duration = TimeSpan.Zero
            };
        }
    }
}
