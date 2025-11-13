# MokAbp 项目使用指南

## 快速开始

### 1. 运行演示程序

```bash
cd MokAbp/MokAbp.Demo
dotnet run
```

### 2. 创建自己的模块

```csharp
using MokAbp.Modularity;
using MokAbp.DependencyInjection.Attributes;

// 定义一个服务
public interface IMyService
{
    void DoWork();
}

// 使用特性标记自动注册
[TransientDependency]
public class MyService : IMyService
{
    public void DoWork()
    {
        Console.WriteLine("正在工作...");
    }
}

// 创建模块
public class MyModule : MokAbpModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // 这里可以手动注册额外的服务
        // context.Services.AddSingleton<IOtherService, OtherService>();
    }
}

// 在应用程序中使用
class Program
{
    static void Main()
    {
        var services = new ServiceCollection();
        var app = services.AddMokAbpApplication<MyModule>();
        var serviceProvider = services.BuildServiceProvider();
        app.Initialize(serviceProvider);
        
        // 使用服务
        var myService = serviceProvider.GetRequiredService<IMyService>();
        myService.DoWork();
        
        app.Dispose();
    }
}
```

## 核心概念

### 服务注册特性

1. **`[TransientDependency]`** - 瞬时生命周期
   - 每次请求都创建新实例
   - 适用于轻量级、无状态服务

2. **`[ScopedDependency]`** - 作用域生命周期
   - 在同一作用域内共享实例
   - 适用于请求级别的服务

3. **`[SingletonDependency]`** - 单例生命周期
   - 全局唯一实例
   - 适用于缓存、配置等服务

4. **`[ExposeServices]`** - 控制公开的服务类型
   ```csharp
   [SingletonDependency]
   [ExposeServices(typeof(IUserService))]
   public class UserService : IUserService, IInternalService
   {
       // 只会注册为IUserService，不会注册IInternalService
   }
   ```

### 模块系统

#### 模块依赖
```csharp
// ModuleA
public class ModuleA : MokAbpModule { }

// ModuleB 依赖于 ModuleA
[DependsOn(typeof(ModuleA))]
public class ModuleB : MokAbpModule { }

// ModuleC 依赖于 ModuleB
[DependsOn(typeof(ModuleB))]
public class ModuleC : MokAbpModule { }

// 加载顺序: ModuleA -> ModuleB -> ModuleC
```

#### 模块生命周期钩子

```csharp
public class MyModule : MokAbpModule
{
    // 1. 服务配置前
    public override void PreConfigureServices(ModuleContext context)
    {
        // 在ConfigureServices之前执行
    }
    
    // 2. 配置服务
    public override void ConfigureServices(ModuleContext context)
    {
        // 注册服务
        context.Services.AddSingleton<IMyService, MyService>();
    }
    
    // 3. 服务配置后
    public override void PostConfigureServices(ModuleContext context)
    {
        // 在ConfigureServices之后执行
    }
}
```

#### 应用程序生命周期

```csharp
public class MyModule : MokAbpModule, 
    IApplicationInitialization, 
    IApplicationShutdown
{
    public void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // 应用程序启动时执行
        var logger = context.ServiceProvider.GetRequiredService<ILogger>();
        logger.Log("应用程序已启动");
    }
    
    public void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        // 应用程序关闭时执行
        var logger = context.ServiceProvider.GetRequiredService<ILogger>();
        logger.Log("应用程序正在关闭");
    }
}
```

## 高级用法

### 1. 条件注册

```csharp
[Dependency]
public class MyService : IMyService
{
    // TryRegister = true: 如果已存在则不注册
    // ReplaceServices = true: 替换已有的注册
}

// 在特性中设置
[Dependency(ServiceLifetime.Singleton)]
public class ConfigService : IConfigService
{
    // 自定义生命周期
}
```

### 2. 多接口实现

```csharp
public interface IService1 { }
public interface IService2 { }

// 默认会注册所有接口和实现类本身
[SingletonDependency]
public class MultiService : IService1, IService2
{
    // 会注册为: IService1, IService2, MultiService
}

// 只公开特定接口
[SingletonDependency]
[ExposeServices(typeof(IService1))]
public class SingleInterfaceService : IService1, IService2
{
    // 只会注册为: IService1
}
```

### 3. 依赖注入

```csharp
[TransientDependency]
public class OrderService : IOrderService
{
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IDataService _dataService;
    
    // 构造函数注入
    public OrderService(
        ILogger logger,
        IUserService userService,
        IDataService dataService)
    {
        _logger = logger;
        _userService = userService;
        _dataService = dataService;
    }
    
    public void CreateOrder(int userId)
    {
        var user = _userService.GetUser(userId);
        _logger.LogInfo($"为用户 {user.Name} 创建订单");
        _dataService.SaveOrder(/* ... */);
    }
}
```

## 设计模式解析

### 1. 依赖反转原则 (DIP)
- 高层模块不依赖于低层模块，都依赖于抽象
- 使用接口定义契约，实现解耦

### 2. 模块化设计
- 将系统划分为独立的模块
- 模块间通过明确的依赖关系连接
- 支持可插拔架构

### 3. 约定优于配置
- 通过特性减少配置代码
- 自动扫描和注册服务
- 减少重复的样板代码

### 4. 生命周期管理
- 统一的初始化和清理流程
- 钩子方法支持扩展
- 保证资源正确释放

## 常见问题

### Q1: 为什么我的服务没有被注册？
A: 确保：
1. 服务类添加了正确的特性标记（如 `[TransientDependency]`）
2. 服务类所在的程序集被包含在模块中
3. 服务类不是抽象类或接口

### Q2: 如何处理循环依赖？
A: 
1. 使用工厂模式延迟创建
2. 使用 `IServiceProvider` 手动解析
3. 重新设计以消除循环依赖

### Q3: 模块加载顺序是怎样的？
A: 
- 使用拓扑排序算法
- 依赖项总是先于依赖它的模块加载
- 会检测并报告循环依赖

### Q4: 如何手动注册服务？
A:
```csharp
public override void ConfigureServices(ModuleContext context)
{
    // 直接使用 IServiceCollection
    context.Services.AddSingleton<IMyService, MyService>();
    context.Services.AddTransient<IOtherService, OtherService>();
}
```

## 与真实ABP的对比

### 相似之处
- ✅ 模块化系统设计
- ✅ 依赖注入特性标记
- ✅ 应用程序生命周期管理
- ✅ 约定优于配置的理念

### 简化之处
- MokAbp 使用 Microsoft.Extensions.DependencyInjection
- ABP 有自己的 IOC 容器抽象层
- MokAbp 没有实现 AOP（面向切面）
- MokAbp 没有实现动态代理
- MokAbp 专注于核心概念学习

## 学习路径

1. **第一步**: 运行并理解 Demo 项目
   - 观察模块加载顺序
   - 理解服务注册和依赖注入
   - 体验不同的生命周期

2. **第二步**: 创建自己的模块
   - 定义服务接口和实现
   - 使用特性标记注册服务
   - 创建模块并声明依赖

3. **第三步**: 深入理解源码
   - 研究 `ModuleLoader` 的拓扑排序算法
   - 理解 `ConventionalRegistrar` 的服务发现机制
   - 学习 `MokAbpApplication` 的生命周期管理

4. **第四步**: 扩展框架
   - 添加自己的约定注册器
   - 实现自定义的生命周期钩子
   - 尝试添加更多 ABP 特性

## 总结

MokAbp 是一个精简的学习项目，它帮助你理解：
- ABP 框架的核心设计思想
- 模块化架构的实现方式
- 依赖注入和生命周期管理
- 约定优于配置的实践

通过学习 MokAbp，你将为深入理解 ABP 框架和企业级应用开发打下坚实的基础。
