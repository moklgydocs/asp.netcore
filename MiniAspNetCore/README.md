我来帮你创建一个完整的ASP.NET Core框架核心功能的模拟实现。这将是一个教育性的项目，帮助你理解ASP.NET Core的核心原理。

```csharp name=Program.cs
using System;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 创建Web应用程序构建器
            var builder = new WebApplicationBuilder();
            
            // 配置服务
            builder.Services.AddSingleton<ILogger, ConsoleLogger>();
            builder.Services.AddTransient<HomeController>();
            
            // 构建应用程序
            var app = builder.Build();
            
            // 配置中间件管道
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<RoutingMiddleware>();
            app.UseMiddleware<ControllerMiddleware>();
            
            // 配置路由
            app.MapGet("/", () => "Hello World!");
            app.MapGet("/home", async (context) => 
            {
                var controller = context.RequestServices.GetService<HomeController>();
                return await controller.Index();
            });
            app.MapPost("/home/create", async (context) =>
            {
                var controller = context.RequestServices.GetService<HomeController>();
                return await controller.Create(context);
            });
            
            // 启动服务器
            await app.RunAsync("http://localhost:8080");
        }
    }
}
```

```csharp name=WebApplicationBuilder.cs
using System;
using System.Collections.Generic;

namespace CustomAspNetCore
{
    /// <summary>
    /// Web应用程序构建器 - 模拟ASP.NET Core的WebApplicationBuilder
    /// 核心职责：配置服务、中间件和应用程序设置
    /// </summary>
    public class WebApplicationBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        
        public WebApplicationBuilder()
        {
            Services = new ServiceCollection();
            Configuration = new Configuration();
            
            // 添加框架核心服务
            Services.AddSingleton<IServiceProvider>(provider => new ServiceProvider(Services));
            Services.AddSingleton<IConfiguration>(Configuration);
        }
        
        /// <summary>
        /// 构建Web应用程序
        /// </summary>
        public WebApplication Build()
        {
            var serviceProvider = new ServiceProvider(Services);
            return new WebApplication(serviceProvider, Configuration);
        }
    }
    
    /// <summary>
    /// 服务集合接口 - 依赖注入容器的服务注册
    /// </summary>
    public interface IServiceCollection
    {
        void AddSingleton<T>(T instance);
        void AddSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface;
        void AddTransient<T>() where T : class;
        void AddScoped<T>() where T : class;
    }
    
    /// <summary>
    /// 服务集合实现 - 管理服务的生命周期和注册
    /// </summary>
    public class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();
        
        public void AddSingleton<T>(T instance)
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), instance, ServiceLifetime.Singleton);
        }
        
        public void AddSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            _services[typeof(TInterface)] = new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), ServiceLifetime.Singleton);
        }
        
        public void AddTransient<T>() where T : class
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), typeof(T), ServiceLifetime.Transient);
        }
        
        public void AddScoped<T>() where T : class
        {
            _services[typeof(T)] = new ServiceDescriptor(typeof(T), typeof(T), ServiceLifetime.Scoped);
        }
        
        public Dictionary<Type, ServiceDescriptor> GetServices() => _services;
    }
    
    /// <summary>
    /// 服务描述符 - 描述服务的类型、实现和生命周期
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public object Instance { get; }
        public ServiceLifetime Lifetime { get; }
        
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
        
        public ServiceDescriptor(Type serviceType, object instance, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Instance = instance;
            Lifetime = lifetime;
        }
    }
    
    /// <summary>
    /// 服务生命周期枚举
    /// </summary>
    public enum ServiceLifetime
    {
        Singleton,  // 单例：整个应用程序生命周期内只创建一次
        Transient,  // 瞬态：每次请求都创建新实例
        Scoped      // 作用域：在同一个请求范围内是同一个实例
    }
}
```

```csharp name=ServiceProvider.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CustomAspNetCore
{
    /// <summary>
    /// 服务提供者接口 - 依赖注入容器的核心
    /// </summary>
    public interface IServiceProvider
    {
        T GetService<T>();
        object GetService(Type serviceType);
        IServiceScope CreateScope();
    }
    
    /// <summary>
    /// 服务提供者实现 - ASP.NET Core DI容器的核心实现
    /// 职责：管理对象的创建、生命周期和依赖关系解析
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services;
        private readonly ConcurrentDictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, object> _scopedInstances = new();
        
        public ServiceProvider(ServiceCollection services)
        {
            _services = services.GetServices();
        }
        
        /// <summary>
        /// 获取指定类型的服务实例
        /// 核心原理：根据服务的生命周期决定如何创建和管理实例
        /// </summary>
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
        
        public object GetService(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
            }
            
            // 根据生命周期返回不同的实例
            return descriptor.Lifetime switch
            {
                ServiceLifetime.Singleton => GetSingleton(descriptor),
                ServiceLifetime.Transient => CreateInstance(descriptor),
                ServiceLifetime.Scoped => GetScoped(descriptor),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <summary>
        /// 获取单例服务 - 整个应用程序生命周期内只创建一次
        /// </summary>
        private object GetSingleton(ServiceDescriptor descriptor)
        {
            return _singletonInstances.GetOrAdd(descriptor.ServiceType, _ => 
                descriptor.Instance ?? CreateInstance(descriptor));
        }
        
        /// <summary>
        /// 获取作用域服务 - 在同一个请求范围内是同一个实例
        /// </summary>
        private object GetScoped(ServiceDescriptor descriptor)
        {
            if (!_scopedInstances.TryGetValue(descriptor.ServiceType, out var instance))
            {
                instance = CreateInstance(descriptor);
                _scopedInstances[descriptor.ServiceType] = instance;
            }
            return instance;
        }
        
        /// <summary>
        /// 创建服务实例 - 使用反射创建对象并解析构造函数依赖
        /// </summary>
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.Instance != null)
                return descriptor.Instance;
                
            var constructors = descriptor.ImplementationType.GetConstructors();
            var constructor = constructors[0]; // 简化：取第一个构造函数
            
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];
            
            // 递归解析构造函数依赖
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = GetService(parameters[i].ParameterType);
            }
            
            return Activator.CreateInstance(descriptor.ImplementationType, args);
        }
        
        /// <summary>
        /// 创建服务作用域 - 用于控制Scoped服务的生命周期
        /// </summary>
        public IServiceScope CreateScope()
        {
            return new ServiceScope(this);
        }
    }
    
    /// <summary>
    /// 服务作用域接口
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
    
    /// <summary>
    /// 服务作用域实现 - 管理Scoped服务的生命周期
    /// </summary>
    public class ServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; }
        
        public ServiceScope(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        public void Dispose()
        {
            // 在实际实现中，这里会清理Scoped服务
            Console.WriteLine("ServiceScope disposed");
        }
    }
}
```

```csharp name=WebApplication.cs
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
                var middleware = (T)_serviceProvider.GetService(typeof(T));
                await middleware.InvokeAsync(context, next);
            });
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
        }
        
        /// <summary>
        /// 构建中间件管道
        /// 核心原理：将所有中间件组合成一个处理管道，形成请求处理链
        /// </summary>
        private void BuildPipeline()
        {
            RequestDelegate pipeline = context =>
            {
                Console.WriteLine("End of pipeline reached");
                return Task.CompletedTask;
            };
            
            // 反向构建管道（洋葱模型的核心）
            for (int i = _middlewares.Count - 1; i >= 0; i--)
            {
                pipeline = _middlewares[i](pipeline);
            }
            
            _pipeline = pipeline;
        }
        
        /// <summary>
        /// 启动Web服务器
        /// </summary>
        public async Task RunAsync(string url)
        {
            BuildPipeline();
            
            var server = new SimpleHttpServer(url, _pipeline, _routes, _serviceProvider);
            
            Console.WriteLine($"服务器启动在: {url}");
            Console.WriteLine("ASP.NET Core 核心功能演示:");
            Console.WriteLine("1. 依赖注入容器");
            Console.WriteLine("2. 中间件管道（洋葱模型）");
            Console.WriteLine("3. 路由系统");
            Console.WriteLine("4. HTTP请求处理");
            Console.WriteLine("\n可访问的端点:");
            Console.WriteLine("GET  /        - Hello World");
            Console.WriteLine("GET  /home    - 控制器方法");
            Console.WriteLine("POST /home/create - 创建数据");
            Console.WriteLine("\n按 Ctrl+C 停止服务器");
            
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
```

```csharp name=HttpContext.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomAspNetCore
{
    /// <summary>
    /// HTTP上下文 - 封装HTTP请求和响应的核心对象
    /// 在ASP.NET Core中，这是处理Web请求的中心对象
    /// </summary>
    public class HttpContext
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public IServiceProvider RequestServices { get; set; }
        public Dictionary<object, object> Items { get; } = new();
        
        public HttpContext()
        {
            Request = new HttpRequest();
            Response = new HttpResponse();
        }
    }
    
    /// <summary>
    /// HTTP请求对象 - 封装客户端发送的HTTP请求信息
    /// </summary>
    public class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Headers { get; } = new();
        public Dictionary<string, string> Query { get; } = new();
        public Stream Body { get; set; }
        
        /// <summary>
        /// 读取请求体内容
        /// </summary>
        public async Task<string> ReadBodyAsync()
        {
            if (Body == null) return string.Empty;
            
            using var reader = new StreamReader(Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
    
    /// <summary>
    /// HTTP响应对象 - 封装发送给客户端的HTTP响应信息
    /// </summary>
    public class HttpResponse
    {
        public int StatusCode { get; set; } = 200;
        public Dictionary<string, string> Headers { get; } = new();
        public MemoryStream Body { get; } = new();
        
        /// <summary>
        /// 写入响应内容
        /// </summary>
        public async Task WriteAsync(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            await Body.WriteAsync(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// 获取响应内容
        /// </summary>
        public string GetContent()
        {
            return Encoding.UTF8.GetString(Body.ToArray());
        }
    }
    
    /// <summary>
    /// 请求委托 - 中间件管道中的核心委托类型
    /// 代表处理HTTP请求的方法
    /// </summary>
    public delegate Task RequestDelegate(HttpContext context);
}
```

```csharp name=Middleware.cs
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
            _logger = logger;
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
                _logger.Log($"[请求异常] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds}ms - 错误: {ex.Message}");
                
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error");
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
```

```csharp name=SimpleHttpServer.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// 简单HTTP服务器 - 模拟ASP.NET Core的Web服务器
    /// 核心职责：监听HTTP请求、创建HttpContext、执行中间件管道
    /// </summary>
    public class SimpleHttpServer
    {
        private readonly string _url;
        private readonly RequestDelegate _pipeline;
        private readonly Dictionary<string, RouteHandler> _routes;
        private readonly IServiceProvider _serviceProvider;
        private HttpListener _listener;
        
        public SimpleHttpServer(string url, RequestDelegate pipeline, 
            Dictionary<string, RouteHandler> routes, IServiceProvider serviceProvider)
        {
            _url = url;
            _pipeline = pipeline;
            _routes = routes;
            _serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// 启动HTTP服务器
        /// </summary>
        public async Task StartAsync()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url.EndsWith("/") ? _url : _url + "/");
            _listener.Start();
            
            // 持续监听请求
            while (true)
            {
                try
                {
                    var listenerContext = await _listener.GetContextAsync();
                    
                    // 不等待，允许并发处理多个请求
                    _ = Task.Run(() => ProcessRequestAsync(listenerContext));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"服务器错误: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 处理单个HTTP请求
        /// 核心流程：创建HttpContext -> 执行中间件管道 -> 处理路由 -> 返回响应
        /// </summary>
        private async Task ProcessRequestAsync(HttpListenerContext listenerContext)
        {
            try
            {
                // 创建自定义的HttpContext
                var context = await CreateHttpContextAsync(listenerContext);
                
                // 执行中间件管道
                await _pipeline(context);
                
                // 处理路由
                await HandleRoutingAsync(context);
                
                // 发送响应
                await SendResponseAsync(listenerContext, context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理请求时出错: {ex.Message}");
                
                // 发送500错误响应
                listenerContext.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes("Internal Server Error");
                await listenerContext.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                listenerContext.Response.Close();
            }
        }
        
        /// <summary>
        /// 从HttpListenerContext创建自定义HttpContext
        /// </summary>
        private async Task<HttpContext> CreateHttpContextAsync(HttpListenerContext listenerContext)
        {
            var context = new HttpContext();
            var request = listenerContext.Request;
            
            // 映射请求信息
            context.Request.Method = request.HttpMethod;
            context.Request.Path = request.Url?.AbsolutePath ?? "/";
            context.RequestServices = _serviceProvider;
            
            // 复制请求头
            foreach (string key in request.Headers.AllKeys)
            {
                context.Request.Headers[key] = request.Headers[key];
            }
            
            // 解析查询参数
            foreach (string key in request.QueryString.AllKeys ?? new string[0])
            {
                if (key != null)
                {
                    context.Request.Query[key] = request.QueryString[key];
                }
            }
            
            // 复制请求体
            if (request.HasEntityBody)
            {
                var bodyStream = new MemoryStream();
                await request.InputStream.CopyToAsync(bodyStream);
                bodyStream.Position = 0;
                context.Request.Body = bodyStream;
            }
            
            return context;
        }
        
        /// <summary>
        /// 处理路由匹配和执行
        /// </summary>
        private async Task HandleRoutingAsync(HttpContext context)
        {
            var routeKey = $"{context.Request.Method}:{context.Request.Path}";
            
            if (_routes.TryGetValue(routeKey, out var handler))
            {
                try
                {
                    var result = await handler.Handler(context);
                    await context.Response.WriteAsync(result);
                    Console.WriteLine($"[路由处理] 匹配路由: {routeKey}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[路由错误] {routeKey}: {ex.Message}");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("路由处理出错");
                }
            }
            else
            {
                // 404 Not Found
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"路由未找到: {routeKey}");
                Console.WriteLine($"[路由未找到] {routeKey}");
            }
        }
        
        /// <summary>
        /// 发送HTTP响应
        /// </summary>
        private async Task SendResponseAsync(HttpListenerContext listenerContext, HttpContext context)
        {
            var response = listenerContext.Response;
            
            // 设置响应状态码
            response.StatusCode = context.Response.StatusCode;
            
            // 设置响应头
            foreach (var header in context.Response.Headers)
            {
                try
                {
                    response.Headers[header.Key] = header.Value;
                }
                catch
                {
                    // 某些头可能不能直接设置，忽略错误
                }
            }
            
            // 设置内容类型
            response.ContentType = "text/plain; charset=utf-8";
            
            // 写入响应体
            var content = context.Response.GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                response.ContentLength64 = bytes.Length;
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            }
            
            response.Close();
        }
        
        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            _listener?.Stop();
        }
    }
}
```

```csharp name=Controllers.cs
using System;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// 控制器基类 - 提供控制器的基础功能
    /// 模拟ASP.NET Core MVC中的Controller基类
    /// </summary>
    public abstract class ControllerBase
    {
        protected ILogger Logger { get; }
        
        protected ControllerBase(ILogger logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// 返回字符串结果
        /// </summary>
        protected string Ok(string content)
        {
            return content;
        }
        
        /// <summary>
        /// 返回JSON结果（简化版）
        /// </summary>
        protected string Json(object data)
        {
            // 简化的JSON序列化
            return $"{{\"data\": \"{data}\"}}";
        }
    }
    
    /// <summary>
    /// 首页控制器 - 演示MVC控制器的使用
    /// </summary>
    public class HomeController : ControllerBase
    {
        public HomeController(ILogger logger) : base(logger)
        {
        }
        
        /// <summary>
        /// 首页动作方法
        /// </summary>
        public async Task<string> Index()
        {
            Logger.Log("[控制器] HomeController.Index 被调用");
            
            // 模拟异步操作
            await Task.Delay(100);
            
            return Ok("欢迎来到自定义 ASP.NET Core 框架！\n" +
                     "这是一个完整的框架核心功能演示，包括：\n" +
                     "✓ 依赖注入容器\n" +
                     "✓ 中间件管道\n" +
                     "✓ 路由系统\n" +
                     "✓ MVC控制器\n" +
                     "✓ HTTP请求处理");
        }
        
        /// <summary>
        /// 创建数据的动作方法
        /// </summary>
        public async Task<string> Create(HttpContext context)
        {
            Logger.Log("[控制器] HomeController.Create 被调用");
            
            // 读取请求体
            var body = await context.Request.ReadBodyAsync();
            
            // 模拟数据处理
            await Task.Delay(50);
            
            return Json(new { 
                Message = "数据创建成功", 
                Data = body.Length > 0 ? body : "无数据",
                Timestamp = DateTime.Now 
            });
        }
    }
}
```

```csharp name=Configuration.cs
using System;
using System.Collections.Generic;

namespace CustomAspNetCore
{
    /// <summary>
    /// 配置接口 - 管理应用程序配置
    /// </summary>
    public interface IConfiguration
    {
        string GetValue(string key);
        void SetValue(string key, string value);
    }
    
    /// <summary>
    /// 配置实现 - 简单的键值对配置系统
    /// 模拟ASP.NET Core的IConfiguration
    /// </summary>
    public class Configuration : IConfiguration
    {
        private readonly Dictionary<string, string> _values = new();
        
        public Configuration()
        {
            // 默认配置值
            _values["Environment"] = "Development";
            _values["ApplicationName"] = "Custom ASP.NET Core";
            _values["Version"] = "1.0.0";
        }
        
        public string GetValue(string key)
        {
            return _values.TryGetValue(key, out var value) ? value : null;
        }
        
        public void SetValue(string key, string value)
        {
            _values[key] = value;
        }
    }
}
```

```csharp name=Logger.cs
using System;

namespace CustomAspNetCore
{
    /// <summary>
    /// 日志接口 - 定义日志记录的契约
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message, Exception exception = null);
        void LogWarning(string message);
        void LogInformation(string message);
    }
    
    /// <summary>
    /// 控制台日志实现 - 将日志输出到控制台
    /// 模拟ASP.NET Core的日志系统
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            LogInformation(message);
        }
        
        public void LogError(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception}");
            }
            Console.ResetColor();
        }
        
        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN]  {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
        
        public void LogInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO]  {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }
}
```

```xml name=CustomAspNetCore.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

```markdown name=README.md
# 自定义 ASP.NET Core 框架核心实现

这是一个完整的 ASP.NET Core 框架核心功能的教育性实现，帮助理解框架的内部工作原理。

## 🎯 项目目标

通过从零开始实现 ASP.NET Core 的核心功能，深入理解：
- 依赖注入（DI）容器的工作原理
- 中间件管道的洋葱模型
- HTTP请求处理流程
- 路由系统的实现
- MVC控制器模式

## 🏗️ 核心架构

### 1. 依赖注入容器 (ServiceProvider.cs)
```
应用程序启动 → 服务注册 → 服务解析 → 对象创建 → 生命周期管理
```

**核心特性：**
- **Singleton（单例）**：整个应用程序生命周期内只创建一次
- **Transient（瞬态）**：每次请求都创建新实例
- **Scoped（作用域）**：在同一个请求范围内是同一个实例

**工作原理：**
```csharp
// 服务注册
builder.Services.AddSingleton<ILogger, ConsoleLogger>();
builder.Services.AddTransient<HomeController>();

// 服务解析（自动处理构造函数依赖注入）
var logger = serviceProvider.GetService<ILogger>();
```

### 2. 中间件管道 (Middleware.cs)
```
请求 → 中间件1 → 中间件2 → 中间件3 → 控制器 → 中间件3 → 中间件2 → 中间件1 → 响应
```

**洋葱模型核心原理：**
- 每个中间件可以在请求处理前后执行逻辑
- 中间件通过 `next()` 调用下一个中间件
- 响应时按相反顺序返回，形成洋葱层结构

**实现示例：**
```csharp
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
    // 请求处理前的逻辑
    Console.WriteLine("请求开始");
    
    await next(context); // 调用下一个中间件
    
    // 请求处理后的逻辑
    Console.WriteLine("请求结束");
}
```

### 3. HTTP请求处理 (SimpleHttpServer.cs)
```
HTTP请求 → 创建HttpContext → 执行中间件管道 → 路由匹配 → 控制器执行 → 返回响应
```

**请求处理流程：**
1. **监听HTTP请求**：使用 HttpListener 监听指定端口
2. **创建上下文**：将原生HTTP请求转换为自定义 HttpContext
3. **执行管道**：按顺序执行所有已注册的中间件
4. **路由处理**：匹配URL路径和HTTP方法到对应的处理器
5. **发送响应**：将处理结果返回给客户端

### 4. 路由系统
```csharp
// 简单路由注册
app.MapGet("/", () => "Hello World!");
app.MapGet("/home", async (context) => {
    var controller = context.RequestServices.GetService<HomeController>();
    return await controller.Index();
});
```

**路由匹配原理：**
- 使用 "HTTP方法:路径" 作为路由键
- 支持lambda表达式和控制器方法
- 自动处理依赖注入

## 🚀 运行项目

1. **编译项目**
```bash
dotnet build
```

2. **运行应用程序**
```bash
dotnet run
```

3. **测试端点**
```bash
# 基础端点
curl http://localhost:8080/

# 控制器端点
curl http://localhost:8080/home

# POST请求
curl -X POST http://localhost:8080/home/create -d "test data"
```

## 📚 学习要点

### 1. 依赖注入的好处
- **松耦合**：组件之间不直接依赖具体实现
- **可测试性**：容易进行单元测试和Mock
- **可维护性**：修改实现不影响使用方
- **生命周期管理**：框架自动管理对象生命周期

### 2. 中间件的设计模式
- **责任链模式**：每个中间件处理特定职责
- **装饰器模式**：为请求处理添加额外功能
- **可组合性**：可以灵活组合不同中间件

### 3. HTTP抽象的重要性
- **跨平台兼容**：统一的HTTP抽象层
- **易于测试**：可以轻松Mock HTTP上下文
- **框架无关**：业务逻辑不依赖具体HTTP实现

## 🔧 扩展点

1. **添加更多中间件**
   - 身份认证中间件
   - CORS中间件
   - 异常处理中间件

2. **完善路由系统**
   - 路由参数支持
   - 路由约束
   - 动态路由

3. **增强依赖注入**
   - 泛型服务注册
   - 条件注册
   - 装饰器模式

## 💡 核心概念总结

| 概念 | 作用 | 实现要点 |
|------|------|----------|
| **依赖注入** | 管理对象创建和依赖关系 | 服务注册、生命周期管理、递归解析 |
| **中间件管道** | 处理HTTP请求的洋葱模型 | 责任链模式、可组合、前后处理 |
| **HTTP抽象** | 统一的请求响应模型 | HttpContext封装、跨平台兼容 |
| **路由系统** | URL到处理器的映射 | 模式匹配、参数提取、方法绑定 |
| **配置系统** | 应用程序设置管理 | 键值对存储、环境配置、类型安全 |

这个实现虽然简化，但包含了 ASP.NET Core 的核心设计理念和架构模式，是学习现代Web框架设计的绝佳起点。
```

这个完整的实现展示了ASP.NET Core框架的核心功能：

## 🎯 **核心特性**

1. **依赖注入容器** - 支持Singleton、Transient、Scoped三种生命周期
2. **中间件管道** - 实现洋葱模型的请求处理链
3. **HTTP请求处理** - 完整的请求-响应生命周期
4. **路由系统** - 支持GET/POST路由映射
5. **MVC控制器** - 模拟控制器模式
6. **配置系统** - 简单的配置管理
7. **日志系统** - 结构化日志记录

## 🏗️ **架构精髓**

- **洋葱模型**：中间件的核心设计模式
- **依赖注入**：松耦合的组件设计
- **请求管道**：从HTTP到业务逻辑的完整流程
- **生命周期管理**：不同作用域的对象管理

## 🚀 **运行说明**

```bash
dotnet run
```

然后访问：
- `http://localhost:8080/` - 基础端点
- `http://localhost:8080/home` - 控制器方法
- `POST http://localhost:8080/home/create` - POST请求

这个实现虽然简化，但包含了ASP.NET Core的核心设计理念，是学习现代Web框架架构的绝佳教材！