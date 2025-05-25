namespace MiddleWare.IPFilterMiddleware
{
    public class IPFilterMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<IPFilterMiddleware> logger;
        private IPFilterOption IPFilterOption;

        public IPFilterMiddleware(RequestDelegate _next, ILogger<IPFilterMiddleware> logger,
            IPFilterOption iPFilterOption)
        {
            this.next = _next;
            this.logger = logger;
            this.IPFilterOption = iPFilterOption;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("非法地址");
                await next(context);
            }
            var isAllowed = IsAllowd(ip);
            if (!isAllowed)
            {
                logger.LogInformation($"请求的IP被禁止： {ip}");
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied based on IP address");
                return;
            }
            await next(context);

        }

        private bool IsAllowd(string ip)
        {
            // 如果在黑名单中，拒绝访问
            if (IPFilterOption.BannedIps.Contains(ip))
            {
                return false;
            }
            // 如果白名单为空且设置为允许，则允许所有IP
            if (IPFilterOption.AllowedIps.Count == 0 && IPFilterOption.AllowAllIPsIfListEmpty)
            {
                return true;
            }
            return IPFilterOption.AllowedIps.Contains(ip);
        }
    }
}
