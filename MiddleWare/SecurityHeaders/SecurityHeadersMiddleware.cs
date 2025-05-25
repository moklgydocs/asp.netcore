using Microsoft.AspNetCore.Http.Headers;

namespace MiddleWare.SecurityHeadersMiddleware
{
    /// <summary>
    /// 安全头部中间件，用于向HTTP响应添加常见的安全相关Header。
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        /// <summary>
        /// 下一个中间件的委托。
        /// </summary>
        public RequestDelegate next;

        /// <summary>
        /// 日志记录器，用于记录中间件的相关信息。
        /// </summary>
        public ILogger<SecurityHeadersMiddleware> Logger;

        /// <summary>
        /// 安全头部配置选项。
        /// </summary>
        public SecurityHeadersOptions Options;

        /// <summary>
        /// 构造函数，注入下一个中间件、日志记录器和安全头部配置。
        /// </summary>
        /// <param name="_next">下一个中间件的委托</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">安全头部配置选项</param>
        public SecurityHeadersMiddleware(RequestDelegate _next,
            ILogger<SecurityHeadersMiddleware> logger,
            SecurityHeadersOptions options)
        {
            this.next = _next;
            this.Logger = logger;
            this.Options = options;
        }

        /// <summary>
        /// 中间件主方法，在HTTP响应中添加安全相关Header。
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            Logger.LogInformation("安全头部中间件==start==");

            // 添加HSTS头部，强制客户端使用HTTPS
            if (Options.UseHsts)
            {
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }

            // 添加XSS防护头部
            if (Options.UseXssProtection)
            {
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            }

            // 添加内容类型嗅探防护头部
            if (Options.UseContentTypeOptions)
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            }

            // 添加防止页面被嵌入到iframe的头部
            if (Options.UseFrameOptions)
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
            }

            // 添加内容安全策略头部
            if (!string.IsNullOrEmpty(Options.ContentSecurityPolicy))
            {
                context.Response.Headers.Add("Content-Security-Policy", Options.ContentSecurityPolicy);
            }

            // 调用下一个中间件
            await next(context);
        }
    }
}
