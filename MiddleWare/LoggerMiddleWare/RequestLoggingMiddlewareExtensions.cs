namespace MiddleWare.LoggerMiddleWare
{
    /// <summary>
    /// 更简洁的日志使用方式
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
