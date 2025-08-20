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
using Microsoft.Extensions.Hosting;

namespace Mok.Modularity
{
    public class Application : IApplication
    {
        /// <summary>
        /// 根模块类型
        /// </summary>
        public Type RootModuleType { get; }
        /// <summary>
        /// 应用程序的服务提供程序
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; } // 在构建后设置

        /// <summary>
        /// 持有模块加载器实例
        /// </summary>
        private ModuleLoader ModuleLoader;
        /// <summary>
        /// 持有初始的服务集合
        /// </summary>
        private IServiceCollection Services { get; set; }
        /// <summary>
        /// 持有日志工厂
        /// </summary>
        private ILoggerFactory LoggerFactory;
        private IApplicationBuilder AppBuilder;
        private IHostEnvironment Env;
        private readonly ILogger<Application> Logger;
        private bool _isDisposed;


        /// <summary>
        /// 创建应用程序实例
        /// </summary>
        private Application(
            Type rootModuleType,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            IApplicationBuilder appBuilder,
            IHostEnvironment environment)
        {
            RootModuleType = rootModuleType ?? throw new ArgumentNullException(nameof(rootModuleType));
            services = services ?? throw new ArgumentNullException(nameof(services));
            LoggerFactory = loggerFactory;
            AppBuilder = appBuilder;
            Env = environment;

            Logger = loggerFactory.CreateLogger<Application>();
            //// 创建应用程序日志记录器 
            Logger?.LogDebug("Creating Mok Application instance for root module: {RootModuleType}", rootModuleType.FullName);
        }

        /// <summary>
        /// 创建并配置应用程序
        /// </summary>
        public static async Task<IApplication> CreateAsync(
            Type rootModuleType,
            IServiceCollection services,
            ILoggerFactory loggerFactory = null,
            Assembly[] assembliesToScan = null,
            IApplicationBuilder appBuilder = null,
            IHostEnvironment env = null)
        {
            // 验证参数
            if (rootModuleType == null) throw new ArgumentNullException(nameof(rootModuleType));
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 创建应用程序实例
            var application = new Application(rootModuleType, services, loggerFactory, appBuilder, env);

            // 如果没有指定扫描程序集，默认扫描根模块所在的程序集
            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                assembliesToScan = new[] { rootModuleType.Assembly };
            }

            try
            {
                // 先注册应用程序实例到服务容器
                services.AddSingleton<IApplication>(application);

                using var tempServiceProvider = services.BuildServiceProvider(validateScopes: false);
                application.ServiceProvider = tempServiceProvider; // 提供临时ServiceProvider

                // 创建模块加载器
                application.ModuleLoader = new ModuleLoader(services, loggerFactory);

                // 加载模块并配置服务
                await application.ModuleLoader.LoadModulesAsync(assembliesToScan); 

                // 构建服务提供程序
                application.ServiceProvider = services.BuildServiceProvider(validateScopes: false);  

                application.Logger?.LogInformation("Application created successfully with root module: {RootModuleType}",
                   rootModuleType.FullName);

                return application;
            }
            catch (Exception ex)
            {
                application.Logger?.LogError(ex, "Failed to create application with root module: {RootModuleType}",
                    rootModuleType.FullName);
                throw;
            }
        }

        /// <summary>
        /// 如果没有提供日志工厂或者日志工厂已被释放，创建一个新的
        /// </summary>
        /// <param name="loggerFactory"></param>
        private static void GetDefaultLoggerFactory(ILoggerFactory loggerFactory)
        {
            ILoggerFactory safeLoggerFactory;
            if (loggerFactory == null)
            {
                safeLoggerFactory = new LoggerFactory();
            }
            else
            {
                try
                {
                    // 测试日志工厂是否可用
                    loggerFactory.CreateLogger("Test");
                    safeLoggerFactory = loggerFactory;
                }
                catch (ObjectDisposedException)
                {
                    // 如果已被释放，创建新的
                    safeLoggerFactory = new LoggerFactory();
                }
            }
        }

        /// <summary>
        /// 初始化应用程序并执行模块的初始化阶段
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task InitializeApplicationAsync(IApplicationBuilder app = null)
        {
            ThrowIfDisposed();
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("ServiceProvider has not been built. Call CreateAsync first.");
            }
            if (ModuleLoader == null)
            {
                throw new InvalidOperationException("Module loader has not been created. Call CreateAsync first.");
            }
            try
            {

                // 使用传入的IApplicationBuilder而不是字段中存储的
                var appBuilderToUse = app ?? AppBuilder;

                // 如果环境为空，尝试从服务提供程序获取
                var envToUse = Env ?? ServiceProvider.GetService<IHostEnvironment>();
                // 初始化模块，执行 OnPreApplicationInitialization, OnApplicationInitialization, OnPostApplicationInitialization 
                await ModuleLoader.InitializeModulesAsync(ServiceProvider, appBuilderToUse, envToUse);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to initialize application.");
                throw;
            }


        }

        /// <summary>
        /// 关闭应用程序并执行模块的关闭阶段
        /// </summary>
        public async Task ShutdownAsync()
        {
            if (_isDisposed || ServiceProvider == null)
            {
                return;
            }

            try
            {
                Logger?.LogInformation("Shutting down application...");

                if (ModuleLoader != null)
                {
                    await ModuleLoader.ShutdownModulesAsync(ServiceProvider);
                    ModuleLoader.Dispose();
                    ModuleLoader = null;
                }

                Logger?.LogInformation("Application shutdown completed.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error during application shutdown.");
                throw;
            }
            finally
            {
                // 释放服务提供程序
                if (ServiceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }

                ServiceProvider = null;
                AppBuilder = null;
                Env = null;
            }
        }


        /// <summary>
        /// 异步释放资源
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                try
                {
                    await ShutdownAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error during async dispose.");
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                try
                {
                    // 同步等待异步操作完成
                    // 在真实环境中，最好使用DisposeAsync方法
                    ShutdownAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Error during dispose.");
                }
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 检查是否已释放资源
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(Application));
            }
        }
    }
}
