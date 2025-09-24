using SimpleAspNetCore.Kestrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore
{
    // 简化版WebApplication实现
    public class SimpleWebApplication : IDisposable
    {
        private readonly IServer _server;

        private readonly List<Func<RequestDelegate, RequestDelegate>> _middlewares = new List<Func<RequestDelegate, RequestDelegate>>();

        private RequestDelegate _pipeline;

        private bool _pipelineBuilt = false;

        // 用于处理特殊路径的处理器
        private readonly Dictionary<string, RequestDelegate> _endpoints = new Dictionary<string, RequestDelegate>();


        public SimpleWebApplication(SimpleWebApplicationBuilder builder)
        {
            // 从构建器获取服务器实例
            // 在实际ASP.NET Core中，这部分通过DI容器获取
            _server = (IServer)builder.Services[typeof(IServer)]();

            // 如果配置了自定义端点，重新创建服务器
            if (builder.Options.EndPoint != null)
            {
                _server = new SimpleKestrelServer(builder.Options.EndPoint);
            }
            Console.WriteLine("[Application] 应用程序已创建");

        }
        // 添加中间件到管道
        // 对应ASP.NET Core中的IApplicationBuilder.Use方法
        public SimpleWebApplication Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            _middlewares.Add(middleware);

            _pipelineBuilt = false;
            return this;
        }

        // 添加路由端点 - 简化版的Map方法
        // 对应ASP.NET Core中的IEndpointRouteBuilder.Map方法
        public SimpleWebApplication MapGet(string path, Func<SimpleHttpContext, Task> handler)
        {
            _endpoints[path] = context => handler(context);
            return this;
        }

        // 启动应用程序
        // 对应WebApplication.RunAsync
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            // 构建请求处理管道
            BuildPipeline();

            Console.WriteLine("[Application] 启动应用程序");


            // 创建HTTP应用实例并启动服务器
            var httpApp = new SimpleHostingApplication(this);
            await _server.StartAsync(httpApp, cancellationToken);


            // 保持应用程序运行，直到取消令牌触发
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => tcs.TrySetResult(null));
            await tcs.Task;

            // 停止服务器
            await _server.StopAsync(cancellationToken);
        }
        // 构建中间件管道
        // 对应ASP.NET Core中的ApplicationBuilder.Build方法
        private void BuildPipeline()
        {
            if (_pipelineBuilt) return;

            Console.WriteLine("[Application] 构建中间件管道");
            Console.WriteLine($"[Application] 已注册的端点数量: {_endpoints.Count}");

            foreach (var endpoint in _endpoints)
            {
                Console.WriteLine($"[Application] 已注册的端点: {endpoint.Key}");
            }

            // 创建端点路由中间件 - 这是管道的最终处理器
            RequestDelegate app = context =>
            {
                var path = context.Request.Path;
                Console.WriteLine($"[Application] 查找路由匹配: {path}");

                // 检查是否有匹配的路由
                if (_endpoints.TryGetValue(path, out var handler))
                {
                    Console.WriteLine($"[Application] 找到路由匹配: {path}");
                    return handler(context);
                }

                // 没有匹配的路由，返回404
                Console.WriteLine($"[Application] 未找到路由匹配: {path}");
                context.Response.StatusCode = 404;
                return context.Response.WriteAsync("Not Found - No route matches the specified path");
            };

            // 按照添加顺序的反向应用中间件
            for (int i = _middlewares.Count - 1; i >= 0; i--)
            {
                app = _middlewares[i](app);
            }

            _pipeline = app;
            _pipelineBuilt = true;
        }



        // 同步版本的Run方法
        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        // 在ExecuteAsync方法中添加更多日志
        internal async Task ExecuteAsync(SimpleHttpContext context)
        {
            if (!_pipelineBuilt)
            {
                BuildPipeline();
            }

            Console.WriteLine($"[Application] 执行请求管道, 路径: {context.Request.Path}");
            await _pipeline(context);
            Console.WriteLine($"[Application] 请求管道执行完成, 状态码: {context.Response.StatusCode}");
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
