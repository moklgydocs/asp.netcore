using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// Web应用程序 - ASP.NET Core应用程序的核心
    /// 职责：管理HTTP请求处理管道、路由和中间件
    /// </summary>
    public class WebApplication
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly List<Func<RequestDelegate, RequestDelegate>> _middlewares = new();
        private readonly Dictionary<string, RouteHandler> _routes = new();
        private RequestDelegate _pipeline;

        public IServiceProvider Services => _serviceProvider;

        public WebApplication(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        /// <summary>
        /// 添加中间件到管道
        /// 核心原理：中间件采用洋葱模型，每个中间件可以在请求处理前后执行逻辑
        /// </summary>
        public void UseMiddleware<T>() where T : IMiddleware
        {
            _middlewares.Add(next => async context =>
            {
                try
                {
                    // 修复：改进错误处理和调试信息
                    var middleware = _serviceProvider.GetService<T>();
                    if (middleware == null)
                    {
                        throw new InvalidOperationException(
                            $"中间件 {typeof(T).Name} 未注册到依赖注入容器。" +
                            $"请确保在 Services 中注册了该中间件：builder.Services.AddTransient<{typeof(T).Name}>();");
                    }

                    Console.WriteLine($"[中间件管道] 执行中间件: {typeof(T).Name}");
                    await middleware.InvokeAsync(context, next);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 中间件 {typeof(T).Name} 执行失败: {ex.Message}");

                    // 设置错误响应
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($"中间件错误: {ex.Message}");
                }
            });

            Console.WriteLine($"✓ 中间件 {typeof(T).Name} 已添加到管道");
        }

        /// <summary>
        /// 映射GET路由
        /// </summary>
        public void MapGet(string path, Func<string> handler)
        {
            _routes[GenerateRouteKey("GET", path)] = new RouteHandler
            {
                Method = "GET",
                Path = path,
                Handler = context => Task.FromResult(handler())
            };
            Console.WriteLine($"✓ 已注册路由: GET {path}");
        }

        /// <summary>
        /// 映射GET路由（异步处理器）
        /// </summary>
        public void MapGet(string path, Func<HttpContext, Task<string>> handler)
        {
            _routes[GenerateRouteKey("GET", path)] = new RouteHandler
            {
                Method = "GET",
                Path = path,
                Handler = handler
            };
            Console.WriteLine($"✓ 已注册路由: GET {path}");
        }

        /// <summary>
        /// 映射POST路由
        /// </summary>
        public void MapPost(string path, Func<HttpContext, Task<string>> handler)
        {
            _routes[GenerateRouteKey("POST", path)] = new RouteHandler
            {
                Method = "POST",
                Path = path,
                Handler = handler
            };
            Console.WriteLine($"✓ 已注册路由: POST {path}");
        }

        /// <summary>
        /// 构建中间件管道
        /// 核心原理：将所有中间件组合成一个处理管道，形成请求处理链
        /// </summary>
        private void BuildPipeline()
        {
            Console.WriteLine("\n=== 构建中间件管道 ===");

            RequestDelegate pipeline = context =>
            {
                Console.WriteLine("[管道结束] 到达管道末端");
                return Task.CompletedTask;
            };

            // 反向构建管道（洋葱模型的核心）
            for (int i = _middlewares.Count - 1; i >= 0; i--)
            {
                var currentMiddleware = _middlewares[i];
                var previousPipeline = pipeline;

                pipeline = currentMiddleware(previousPipeline);
                Console.WriteLine($"✓ 中间件 #{i + 1} 已添加到管道");
            }

            _pipeline = pipeline;
            Console.WriteLine($"✓ 管道构建完成，共 {_middlewares.Count} 个中间件");
        }

        /// <summary>
        /// 启动Web服务器
        /// </summary>
        public async Task RunAsync(string url)
        {
            BuildPipeline();

            var server = new SimpleHttpServer(url, _pipeline, _routes, _serviceProvider);

            Console.WriteLine($"\n🚀 服务器启动在: {url}");
            Console.WriteLine("📖 ASP.NET Core 核心功能演示:");
            Console.WriteLine("   1. ✅ 依赖注入容器");
            Console.WriteLine("   2. ✅ 中间件管道（洋葱模型）");
            Console.WriteLine("   3. ✅ 路由系统");
            Console.WriteLine("   4. ✅ HTTP请求处理");
            Console.WriteLine("   5. ✅ 错误处理和调试");
            Console.WriteLine("\n🌐 可访问的端点:");
            Console.WriteLine("   GET  /        - Hello World");
            Console.WriteLine("   GET  /home    - 控制器方法");
            Console.WriteLine("   GET  /services - 自定义服务");
            Console.WriteLine("   GET  /test    - 测试端点");
            Console.WriteLine("   POST /home/create - 创建数据");
            Console.WriteLine("\n💡 测试命令:");
            Console.WriteLine("   curl http://localhost:8080/");
            Console.WriteLine("   curl http://localhost:8080/home");
            Console.WriteLine("   curl http://localhost:8080/services");
            Console.WriteLine("   curl -X POST http://localhost:8080/home/create -d \"test data\"");
            Console.WriteLine("\n⏹️  按 Ctrl+C 停止服务器");

            await server.StartAsync();
        }

        private string GenerateRouteKey(string method, string path)
        {
            return $"{method}:{path}";
        }
    }

    /// <summary>
    /// 路由处理器
    /// </summary>
    public class RouteHandler
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public Func<HttpContext, Task<string>> Handler { get; set; }
    }
}