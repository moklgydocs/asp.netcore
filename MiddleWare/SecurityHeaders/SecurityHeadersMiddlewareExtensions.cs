namespace MiddleWare.SecurityHeadersMiddleware
{
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Action<SecurityHeadersOptions> configOptions = null)
        {
            var options = new SecurityHeadersOptions();
            configOptions?.Invoke(options);
            app.UseMiddleware<SecurityHeadersMiddleware>(options);
            return app;

        }
    }
}
