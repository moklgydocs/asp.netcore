namespace MiddleWare.IPFilterMiddleware
{
    public static class IPFilterMiddlewareExtensions
    {
        /// <summary>
        /// 配置IP过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureOption"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseIpFilter(this IApplicationBuilder app,Action<IPFilterOption> configureOption)
        {
            var options = new IPFilterOption();
            configureOption?.Invoke(options);
            return app.UseMiddleware<IPFilterMiddleware>(options);
        }
    }
}
