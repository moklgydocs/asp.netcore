using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.PortableExecutable;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Mok.Modularity
{
    public class Application : IApplication
    {
        public Type RootModuleType { get; }
        public IServiceProvider ServiceProvider { get; private set; } // 在构建后设置

        private ModuleLoader ModuleLoader; // 持有模块加载器实例
        private IServiceCollection Services; // 持有初始的服务集合
        private ILoggerFactory LoggerFactory; // 持有日志工厂
        private IApplicationBuilder AppBuilder; // 持有日志工厂
        private IHostingEnvironment Env; // 持有日志工厂

        // 私有构造函数，通过 CreateAsync 工厂方法创建
        private Application(Type rootModuleType,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            IApplicationBuilder builder,
            IHostingEnvironment environment)
        {
            AppBuilder = builder;
            Env = environment;
            RootModuleType = rootModuleType ?? throw new ArgumentNullException(nameof(rootModuleType));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            LoggerFactory = loggerFactory; // 允许 loggerFactory 为 null
        }

        // 工厂方法：用于创建 MokApplication 实例并执行模块的 ConfigureServices 阶段
        public static async Task<IApplication> CreateAsync(
            Type rootModuleType,
            IServiceCollection services,
            ILoggerFactory loggerFactory = null, // 添加可选的日志工厂
            Assembly[] assembliesToScan = null,
            IApplicationBuilder AppBuilder = null,
            IHostingEnvironment Env = null) // 添加可选的扫描程序集参数
        {
            var application = new Application(rootModuleType, services, loggerFactory, AppBuilder, Env);

            // 如果没有指定扫描程序集，默认扫描根模块所在的程序集
            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                assembliesToScan = new[] { rootModuleType.Assembly };
            }

            // 实例化模块加载器，负责发现、排序和调用 ConfigureServices
            application.ModuleLoader = new ModuleLoader(services, loggerFactory);

            // 加载模块并配置服务
            // LoadModulesAsync 会执行 PreConfigureServices, ConfigureServices, PostConfigureServices
            await application.ModuleLoader.LoadModulesAsync(assembliesToScan);
            services.AddSingleton(application.ModuleLoader);
            services.AddSingleton(rootModuleType); // 注册根模块类型 
            // 构建服务提供程序
            application.ServiceProvider = services.BuildServiceProvider();

            // 返回应用程序实例，后续由调用方调用 InitializeAsync
            return application;
        }

        // 初始化应用程序并执行模块的 Initialization 阶段
        public async Task InitializeApplicationAsync()
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider has not been built. Call CreateAsync first.");
            }
            if (ModuleLoader == null)
            {
                throw new InvalidOperationException("Module loader has not been created. Call CreateAsync first.");
            }

            // 初始化模块，执行 OnPreApplicationInitialization, OnApplicationInitialization, OnPostApplicationInitialization
            // 传入构建好的 ServiceProvider
            // 如果是 ASP.NET Core 应用，这里可能还需要 ApplicationBuilder, Environment 等上下文信息
             
            await ModuleLoader.InitializeModulesAsync(ServiceProvider, AppBuilder, Env);
        }

        // 关闭应用程序并执行模块的 Shutdown 阶段
        public async Task ShutdownAsync()
        {
            if (ServiceProvider == null)
            {
                // 已经关闭或从未成功创建
                return;
            }

            if (ModuleLoader != null)
            {
                await ModuleLoader.ShutdownModulesAsync(ServiceProvider);
                ModuleLoader.Dispose(); // Dispose the loader which might dispose disposable modules
                ModuleLoader = null;
            }

            // 释放 ServiceProvider (如果它是 IDisposable)
            if (ServiceProvider is IDisposable spDisposable)
            {
                spDisposable.Dispose();
            }
            ServiceProvider = null; // 清空引用
            Services = null; // 清空引用
        }

        public void Dispose()
        {
            // 确保 ShutdownAsync 被调用
            ShutdownAsync().GetAwaiter().GetResult();
        }
    }
}
