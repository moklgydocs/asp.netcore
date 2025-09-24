using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore.MiddleWares
{
    // 日志中间件 - 记录请求信息
    // 对应ASP.NET Core中的UseLogging扩展方法
    public static class LoggingMiddlewareExtensions
    {
        public static SimpleWebApplication UseLogging(this SimpleWebApplication app)
        {
            return app.Use(next => async context =>
            {
                Console.WriteLine($"[Logging] 收到请求: {context.Request.Method} {context.Request.Path}");

                // 调用管道中的下一个中间件
                await next(context);

                Console.WriteLine($"[Logging] 请求完成: {context.Response.StatusCode}");
            });
        }
    }
}
