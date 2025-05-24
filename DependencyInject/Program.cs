// 1. 创建服务集合
using DependencyInject.Business;
using DependencyInject.Core;

// 定义一个集合，集合只含服务描述符
var services = new ServiceCollection();

// 2. 注册服务
services.AddSingleton<ILogger, ConsoleLogger>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddTransient<IUserService, UserService>();

services.AddSingleton<ILogger>(new ConsoleLogger());
// 修改以下代码行：  
// services.AddTransient<UserService>( );  
// 替换为：  
services.AddTransient(provider =>
{
    var userRepository = provider.GetService(typeof(IUserRepository)) as IUserRepository;
    var logger = provider.GetService(typeof(IUserRepository));
    return new UserService(userRepository) { Logger = (ILogger)logger };
});

// 3. 构建容器
var rootProvider = services.BuildServiceProvider();
// 4. 创建作用域
using (var scope = rootProvider.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;

    // 5. 解析服务
    var userService = (UserService)scopedProvider.GetRequiredService(typeof(IUserService));

    // 6. 使用服务
    userService.RegisterUser("john_doe");

    Console.WriteLine("First scope completed");
} // 作用域结束，Scoped服务被释放

// 创建另一个作用域
using (var scope = rootProvider.CreateScope())
{
    var userRepo = (IUserRepository)scope.ServiceProvider.GetService(typeof(IUserRepository));
    userRepo.SaveUser("jane_doe");

    Console.WriteLine("Second scope completed");
}

// 4. 创建第一个作用域
using (var scope1 = rootProvider.CreateScope())
{
    var provider1 = scope1.ServiceProvider;
    var logger1 = provider1.GetService(typeof(ILogger));
    var repo1 = provider1.GetService(typeof(IUserRepository));
    var service1 = provider1.GetService(typeof(IUserService));

    Console.WriteLine($"[Scope1] ILogger: {logger1.GetHashCode()}");
    Console.WriteLine($"[Scope1] IUserRepository: {repo1.GetHashCode()}");
    Console.WriteLine($"[Scope1] IUserService: {service1.GetHashCode()}");
}

// 5. 创建第二个作用域
using (var scope2 = rootProvider.CreateScope())
{
    var provider2 = scope2.ServiceProvider;
    var logger2 = provider2.GetService(typeof(ILogger));
    var repo2 = provider2.GetService(typeof(IUserRepository));
    var service2 = provider2.GetService(typeof(IUserService));

    Console.WriteLine($"[Scope2] ILogger: {logger2.GetHashCode()}");
    Console.WriteLine($"[Scope2] IUserRepository: {repo2.GetHashCode()}");
    Console.WriteLine($"[Scope2] IUserService: {service2.GetHashCode()}");
}


Console.WriteLine("Program completed,press any key to exit...");
Console.ReadLine();