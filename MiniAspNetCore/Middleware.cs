using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// 中间件接口 - 定义中间件的标准契约
    /// ASP.NET Core中间件的核心特征：
    /// 1. 可以处理请求和响应
    /// 2. 可以决定是否调用下一个中间件
    /// 3. 形成处理管道的洋葱模型
    /// </summary>
    public interface IMiddleware
    {
        Task InvokeAsync(HttpContext context, RequestDelegate next);
    }

    /// <summary>
    /// 日志中间件 - 记录请求处理时间和基本信息
    /// 演示：中间件如何在请求处理前后执行逻辑
    /// </summary>
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            // 请求处理前的逻辑
            _logger.Log($"[请求开始] {context.Request.Method} {context.Request.Path}");

            try
            {
                // 调用管道中的下一个中间件
                await next(context);

                // 请求处理后的逻辑
                stopwatch.Stop();
                _logger.Log($"[请求完成] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds}ms - 状态码: {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                // 异常处理逻辑
                stopwatch.Stop();
                _logger.LogError($"[请求异常] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds}ms", ex);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 路由中间件 - 解析URL路径和HTTP方法
    /// 演示：如何解析和匹配路由
    /// </summary>
    public class RoutingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // 简单的路由解析逻辑
            var path = context.Request.Path ?? "/";
            var method = context.Request.Method ?? "GET";

            // 将路由信息添加到上下文中供后续中间件使用
            context.Items["Route"] = $"{method}:{path}";
            context.Items["RouteMatched"] = true;

            Console.WriteLine($"[路由中间件] 解析路由: {method} {path}");

            // 继续到下一个中间件
            await next(context);
        }
    }

    /// <summary>
    /// 控制器中间件 - 处理控制器路由和方法调用
    /// 演示：MVC模式中的控制器处理逻辑
    /// </summary>
    public class ControllerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var route = context.Items["Route"]?.ToString();

            // 检查是否为控制器路由
            if (route != null && (route.Contains("/home") || route.Contains("/api")))
            {
                Console.WriteLine($"[控制器中间件] 处理控制器路由: {route}");

                // 设置控制器处理标记
                context.Items["ControllerHandled"] = true;
            }

            await next(context);
        }
    }
}