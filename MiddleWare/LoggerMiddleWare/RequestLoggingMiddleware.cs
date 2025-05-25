namespace MiddleWare.LoggerMiddleWare
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate _next, ILogger<RequestLoggingMiddleware> _logger)
        {
            next = _next;
            this.logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            logger.LogInformation(
                        "HTTP {Method} {Url} - {StatusCode}-{contentType}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        context.Request.ContentType); 
            await next(context);

        }
    }
}
