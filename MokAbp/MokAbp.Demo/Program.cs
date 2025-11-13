using Microsoft.Extensions.DependencyInjection;
using MokAbp;
using MokAbp.Demo.Modules;
using MokAbp.Demo.Services;

namespace MokAbp.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MokAbp 框架演示 ===\n");

            // 1. 创建服务集合
            var services = new ServiceCollection();

            Console.WriteLine("【第一步】添加MokAbp应用程序，指定启动模块");
            Console.WriteLine("启动模块: ApplicationModule");
            Console.WriteLine("依赖链: ApplicationModule -> LoggingModule -> CoreModule\n");

            // 2. 添加MokAbp应用程序（会自动加载所有依赖模块）
            var app = services.AddMokAbpApplication<ApplicationModule>();

            Console.WriteLine("\n【第二步】构建ServiceProvider");
            // 3. 构建服务提供者
            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("\n【第三步】初始化应用程序");
            // 4. 初始化应用程序
            app.Initialize(serviceProvider);

            Console.WriteLine("\n=== 模块加载完成 ===");
            Console.WriteLine($"共加载了 {app.Modules.Count} 个模块:");
            foreach (var module in app.Modules)
            {
                Console.WriteLine($"  - {module.ModuleType.Name}");
            }

            Console.WriteLine("\n=== 开始使用服务 ===\n");

            // 5. 使用服务
            DemoServices(serviceProvider);

            Console.WriteLine("\n=== 演示完成 ===");
            Console.WriteLine("\n【第四步】关闭应用程序");
            
            // 6. 清理资源
            app.Dispose();

            Console.WriteLine("\n应用程序已正常退出");
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        static void DemoServices(IServiceProvider serviceProvider)
        {
            // 演示1: 日志服务（Singleton）
            Console.WriteLine("【演示1】日志服务 (Singleton)");
            var logger1 = serviceProvider.GetRequiredService<ILoggerService>();
            var logger2 = serviceProvider.GetRequiredService<ILoggerService>();
            Console.WriteLine($"logger1和logger2是同一实例: {ReferenceEquals(logger1, logger2)}");
            logger1.LogInfo("这是一条信息日志");
            logger1.LogWarning("这是一条警告日志");
            logger1.LogError("这是一条错误日志");

            Console.WriteLine("\n【演示2】用户服务 (Transient)");
            var userService1 = serviceProvider.GetRequiredService<IUserService>();
            var userService2 = serviceProvider.GetRequiredService<IUserService>();
            Console.WriteLine($"userService1和userService2是同一实例: {ReferenceEquals(userService1, userService2)}");
            userService1.CreateUser("张三");
            userService1.CreateUser("李四");
            var userInfo = userService1.GetUserInfo("张三");
            Console.WriteLine($"  {userInfo}");

            Console.WriteLine("\n【演示3】数据服务 (Scoped)");
            using (var scope1 = serviceProvider.CreateScope())
            {
                var dataService1 = scope1.ServiceProvider.GetRequiredService<IDataService>();
                var dataService2 = scope1.ServiceProvider.GetRequiredService<IDataService>();
                Console.WriteLine($"同一作用域内，dataService1和dataService2是同一实例: {ReferenceEquals(dataService1, dataService2)}");
                dataService1.SaveData("key1", "value1");
                var value = dataService2.GetData("key1");
            }

            using (var scope2 = serviceProvider.CreateScope())
            {
                var dataService3 = scope2.ServiceProvider.GetRequiredService<IDataService>();
                Console.WriteLine("不同作用域内，尝试获取之前保存的数据:");
                var value = dataService3.GetData("key1"); // 这会返回null，因为是新的实例
            }

            Console.WriteLine("\n【演示4】服务依赖注入");
            var userService = serviceProvider.GetRequiredService<IUserService>();
            Console.WriteLine("UserService成功注入了ILoggerService依赖");
            userService.CreateUser("王五");
            userService.DeleteUser("不存在的用户");
        }
    }
}
