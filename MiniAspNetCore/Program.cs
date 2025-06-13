using System;
using System.Text;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            { 
                // 确保控制台能正确显示 Unicode 字符
                Console.OutputEncoding = Encoding.UTF8;
                // 可以使用 Unicode 图标（Emoji 或其他特殊符号）
                Console.WriteLine("🚀 MiniAspNetCore 服务启动中...");
                Console.WriteLine("⚙️ 初始化依赖注入容器...");
                // 创建Web应用程序构建器
                var builder = new WebApplicationBuilder();

                // 配置服务 - 注册所有需要的服务
                Console.WriteLine("=== 注册服务 ===");

                // 1. 注册基础服务
                builder.Services.AddSingleton<ILogger, ConsoleLogger>();
                Console.WriteLine("✓ 已注册 ILogger -> ConsoleLogger");

                // 2. 注册控制器
                builder.Services.AddTransient<HomeController>();
                Console.WriteLine("✓ 已注册 HomeController");

                // 3. 注册中间件 - 这是关键！
                builder.Services.AddTransient<LoggingMiddleware>();
                builder.Services.AddTransient<RoutingMiddleware>();
                builder.Services.AddTransient<ControllerMiddleware>();
                Console.WriteLine("✓ 已注册所有中间件");

                // 4. 使用工厂方法注册自定义服务
                builder.Services.AddSingleton<ICustomService>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger>();
                    return new CustomService(logger, "Factory Created");
                });
                Console.WriteLine("✓ 已注册 ICustomService");

                // 构建应用程序
                var app = builder.Build();
                Console.WriteLine("✓ 应用程序构建完成");

                // 测试依赖注入是否工作正常
                Console.WriteLine("\n=== 依赖注入测试 ===");
                var logger = app.Services.GetRequiredService<ILogger>();
                logger.LogInformation("依赖注入容器初始化成功！");

                // 测试中间件是否能正确解析
                var loggingMiddleware = app.Services.GetRequiredService<LoggingMiddleware>();
                Console.WriteLine("✓ LoggingMiddleware 解析成功");

                var customService = app.Services.GetRequiredService<ICustomService>();
                var info = await customService.GetInfoAsync();
                Console.WriteLine($"✓ 服务信息: {info}");

                // 配置中间件管道
                Console.WriteLine("\n=== 配置中间件管道 ===");
                app.UseMiddleware<LoggingMiddleware>();
                app.UseMiddleware<RoutingMiddleware>();
                app.UseMiddleware<ControllerMiddleware>();
                Console.WriteLine("✓ 中间件管道配置完成");

                // 配置路由
                Console.WriteLine("\n=== 配置路由 ===");
                // 添加路由
                app.MapGet("/", () => "Hello World! 🌍");
                app.MapGet("/home", async (context) =>
                {
                    var controller = context.RequestServices.GetRequiredService<HomeController>();
                    return await controller.Index();
                });

                app.MapGet("/services", async (context) =>
                {
                    var customServiceInScope = context.RequestServices.GetRequiredService<ICustomService>();
                    return await customServiceInScope.GetInfoAsync();
                });

                app.MapGet("/test", () => "测试端点 - 验证中间件管道工作正常");

                app.MapPost("/home/create", async (context) =>
                {
                    var controller = context.RequestServices.GetRequiredService<HomeController>();
                    return await controller.Create(context);
                });
                Console.WriteLine("✓ 路由配置完成");

                // 启动服务器
                Console.WriteLine("\n=== 启动服务器 ===");
                await app.RunAsync("http://localhost:8080");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 应用程序启动失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }

    // 演示接口和实现
    public interface ICustomService
    {
        Task<string> GetInfoAsync();
    }

    public class CustomService : ICustomService
    {
        private readonly ILogger _logger;
        private readonly string _message;

        public CustomService(ILogger logger, string message)
        {
            _logger = logger;
            _message = message;
        }

        public async Task<string> GetInfoAsync()
        {
            _logger.LogInformation("CustomService.GetInfoAsync called");
            await Task.Delay(50);
            return $"CustomService Info: {_message} - Created at {DateTime.Now}";
        }
    }
}