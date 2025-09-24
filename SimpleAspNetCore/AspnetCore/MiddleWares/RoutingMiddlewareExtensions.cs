using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore.MiddleWares
{
    // 路由中间件 - 简化版的路由功能
    // 对应ASP.NET Core中的UseRouting扩展方法
    public static class RoutingMiddlewareExtensions
    {
        public static SimpleWebApplication UseSimpleRouting(this SimpleWebApplication app)
        {
            return app.Use(next => async context =>
            {
                Console.WriteLine($"[Routing] 处理路由: {context.Request.Path}");

                // 在真实的ASP.NET Core中，这里会解析路由模板并匹配处理器
                // 在我们的简化实现中，直接调用下一个中间件
                await next(context);
            });
        }
    }
}
