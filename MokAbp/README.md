# MokAbp - 简化的ABP框架学习项目

## 简介

MokAbp是一个简化的ABP框架实现，用于学习ABP框架的核心设计思想。它实现了ABP的以下核心组件：

1. **依赖注入框架** - 基于特性的服务注册
2. **模块化系统** - 模块化应用程序设计
3. **约定注册** - 自动服务注册
4. **应用程序生命周期** - 初始化和关闭钩子

## 核心组件

### 1. 依赖注入 (DependencyInjection)

#### 特性标记
- `[Dependency]` - 通用依赖注入标记
- `[SingletonDependency]` - 单例模式
- `[ScopedDependency]` - 作用域模式
- `[TransientDependency]` - 瞬时模式
- `[ExposeServices]` - 指定要公开的服务类型

#### 示例
```csharp
// 自动注册为瞬时服务
[TransientDependency]
public class MyService : IMyService
{
    public void DoSomething()
    {
        // 实现
    }
}

// 只公开特定接口
[SingletonDependency]
[ExposeServices(typeof(IUserService))]
public class UserService : IUserService, IInternalService
{
    // 只会注册为IUserService
}
```

### 2. 模块化系统 (Modularity)

#### 核心类
- `MokAbpModule` - 模块基类
- `DependsOnAttribute` - 模块依赖声明
- `ModuleLoader` - 模块加载器（支持依赖解析和拓扑排序）

#### 示例
```csharp
// 基础模块
public class CoreModule : MokAbpModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // 配置核心服务
    }
}

// 依赖于CoreModule的模块
[DependsOn(typeof(CoreModule))]
public class ApplicationModule : MokAbpModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // 配置应用服务
    }
}
```

### 3. 应用程序 (Application)

#### 核心类
- `MokAbpApplication` - 应用程序主类
- `MokAbpApplicationFactory` - 应用程序工厂
- `IApplicationInitialization` - 初始化接口
- `IApplicationShutdown` - 关闭接口

#### 示例
```csharp
// 创建应用程序
var services = new ServiceCollection();
var app = services.AddMokAbpApplication<MyStartupModule>();
var serviceProvider = services.BuildServiceProvider();
app.Initialize(serviceProvider);

// 使用应用程序
// ...

// 关闭应用程序
app.Dispose();
```

## 完整使用示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using MokAbp;
using MokAbp.Modularity;
using MokAbp.DependencyInjection.Attributes;

// 1. 定义服务接口和实现
public interface IHelloService
{
    void SayHello();
}

[TransientDependency]
public class HelloService : IHelloService
{
    public void SayHello()
    {
        Console.WriteLine("Hello from MokAbp!");
    }
}

// 2. 定义模块
public class MyAppModule : MokAbpModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        // 可以在这里手动注册服务
        // context.Services.AddTransient<IOtherService, OtherService>();
    }
}

// 3. 创建和运行应用程序
class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        
        // 添加MokAbp应用程序
        var app = services.AddMokAbpApplication<MyAppModule>();
        
        // 构建服务提供者
        var serviceProvider = services.BuildServiceProvider();
        
        // 初始化应用程序
        app.Initialize(serviceProvider);
        
        // 使用服务
        var helloService = serviceProvider.GetRequiredService<IHelloService>();
        helloService.SayHello();
        
        // 清理
        app.Dispose();
    }
}
```

## 设计模式

### 1. 模块化设计
- 模块间可以声明依赖关系
- 自动解析模块依赖并按顺序加载
- 检测循环依赖

### 2. 约定优于配置
- 通过特性自动注册服务
- 自动扫描程序集
- 减少样板代码

### 3. 生命周期管理
- PreConfigureServices - 服务配置前
- ConfigureServices - 服务配置
- PostConfigureServices - 服务配置后
- OnApplicationInitialization - 应用初始化
- OnApplicationShutdown - 应用关闭

## 与ABP的对比

### 实现的功能
- ✅ 模块化系统
- ✅ 依赖注入特性标记
- ✅ 约定注册
- ✅ 应用程序生命周期
- ✅ 模块依赖管理

### 简化的部分
- ❌ 不包含具体的IOC容器实现（使用Microsoft.Extensions.DependencyInjection）
- ❌ 不包含AOP（面向切面编程）
- ❌ 不包含动态代理
- ❌ 不包含多租户
- ❌ 不包含领域驱动设计组件
- ❌ 不包含审计日志
- ❌ 不包含权限系统

## 学习要点

1. **模块化架构** - 如何设计可插拔的模块系统
2. **依赖注入** - 如何通过特性实现自动注册
3. **约定优于配置** - 减少配置，提高开发效率
4. **拓扑排序** - 解决模块依赖顺序问题
5. **生命周期管理** - 应用程序的启动和关闭流程

## 项目结构

```
MokAbp/
├── Application/                    # 应用程序层
│   ├── MokAbpApplication.cs       # 应用程序主类
│   ├── MokAbpApplicationFactory.cs # 应用程序工厂
│   ├── ApplicationInitializationContext.cs
│   └── ApplicationShutdownContext.cs
├── DependencyInjection/            # 依赖注入层
│   ├── Attributes/                 # 特性标记
│   │   ├── DependencyAttribute.cs
│   │   ├── SingletonDependencyAttribute.cs
│   │   ├── ScopedDependencyAttribute.cs
│   │   ├── TransientDependencyAttribute.cs
│   │   └── ExposeServicesAttribute.cs
│   ├── ConventionalRegistrar.cs    # 约定注册器
│   ├── IServiceConvention.cs
│   └── ServiceRegistrationContext.cs
├── Modularity/                     # 模块化层
│   ├── MokAbpModule.cs            # 模块基类
│   ├── IMokAbpModule.cs           # 模块接口
│   ├── DependsOnAttribute.cs      # 依赖特性
│   ├── ModuleDescriptor.cs        # 模块描述符
│   ├── ModuleLoader.cs            # 模块加载器
│   └── ModuleContext.cs           # 模块上下文
└── ServiceCollectionExtensions.cs  # 扩展方法
```

## 总结

MokAbp是一个精简的ABP框架实现，专注于核心的模块化和依赖注入功能。通过学习这个项目，可以理解：

- ABP的模块化设计思想
- 如何实现基于特性的依赖注入
- 如何管理模块间的依赖关系
- 应用程序生命周期管理

这个框架可以作为学习ABP设计思想的起点，为深入理解ABP框架打下基础。
