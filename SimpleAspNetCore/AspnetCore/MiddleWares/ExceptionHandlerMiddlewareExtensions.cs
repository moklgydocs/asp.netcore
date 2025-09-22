using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore.MiddleWares
{
    // 异常处理中间件 - 捕获并处理管道中的异常
    // 对应ASP.NET Core中的UseExceptionHandler扩展方法
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static SimpleWebApplication UseExceptionHandler(this SimpleWebApplication app)
        {
            return app.Use(next => async context =>
            {
                try
                {
                    // 调用管道中的下一个中间件
                    await next(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ExceptionHandler] 捕获到异常: {ex.Message}");

                    // 设置500状态码并返回错误信息
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"服务器错误: {ex.Message}");
                }
            });
        }
    }
}
