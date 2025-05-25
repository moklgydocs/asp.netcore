using Microsoft.AspNetCore.Builder;

namespace MiddleWare.RequestTimingMiddleware
{
    public static class RequestTimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestTimingMiddleware>();
        }
    }
}
