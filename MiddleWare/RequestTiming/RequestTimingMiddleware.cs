using System.Diagnostics;

namespace MiddleWare.RequestTimingMiddleware
{
    /// <summary>
    /// 请求时间监控中间件
    /// </summary>
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<RequestTimingMiddleware> logger;

        public RequestTimingMiddleware(RequestDelegate _next, ILogger<RequestTimingMiddleware> _logger)
        {
            this.next = _next;
            this.logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                sw.Stop();
                logger.LogInformation("Request {Method} {Path} completed in {ElapsedMilliseconds}ms"
                    , context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
            }
        }
    }
}
