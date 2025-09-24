using SimpleAspNetCore.AspnetCore;
using SimpleAspNetCore.AspnetCore.MiddleWares;
using System.Net;

namespace SimpleAspNetCore
{
    internal class Program
    {
        // Program.cs修改
        static async Task Main(string[] args)
        {
            Console.WriteLine("启动简化版ASP.NET Core与Kestrel示例");

            // 1. 创建构建器，Options 被初始化
            var builder = SimpleWebApplicationBuilder.CreateBuilder();

            // 2. 配置 Kestrel，直接修改 builder.Options
            builder.ConfigureKestrel(options =>
            {
                // 这里的 options 就是 builder.Options 的引用
                options.EndPoint = new IPEndPoint(IPAddress.Loopback, 5001);
                // 对 options 的任何修改都会直接反映到 builder.Options 上
            });

            // 3. 构建应用时，使用已配置的 Options
            var app = builder.Build(); // 内部会读取 builder.Options 的配

            // 配置中间件管道
            app.UseExceptionHandler()
               .UseLogging()
               .UseSimpleRouting();

            // 添加路由端点
            app.MapGet("/", async context =>
            {
                context.Response.Headers["Content-Type"] = "text/plain";
                await context.Response.WriteAsync("Hello from Simple ASP.NET Core!");
            });

            app.MapGet("/html", async context =>
            {
                context.Response.Headers["Content-Type"] = "text/html";
                await context.Response.WriteAsync("<html><body><h1>HTML Test Page</h1><p>This is HTML content from Simple ASP.NET Core</p></body></html>");
            });

            app.MapGet("/error", async context =>
            {
                throw new Exception("这是一个测试异常");
            });

            Console.WriteLine("应用程序已配置，准备启动");
            Console.WriteLine("服务器将在 http://localhost:5001 上运行");
            Console.WriteLine("可以访问的路由: /, /html, /error");
            Console.WriteLine("按Ctrl+C停止服务器");

            // 启动应用
            await app.RunAsync();
        }
    }
}
