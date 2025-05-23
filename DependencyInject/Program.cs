// 1. 创建服务集合
using DependencyInject.Business;
using DependencyInject.Core;

var services = new ServiceCollection();

// 2. 注册服务
services.AddSingleton<ILogger, ConsoleLogger>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddTransient<IUserService, UserService>();

// 修改以下代码行：  
// services.AddTransient<UserService>( );  
// 替换为：  
services.AddTransient<UserService>(provider =>
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
    var userService = (UserService)scopedProvider.GetService(typeof(IUserService));

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

Console.WriteLine("Program completed");