using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mok.Modularity
{
    public static class ModuleServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureMokModules<TRootModule>(
           this IServiceCollection services,
           ILoggerFactory loggerFactory = null,
           params Assembly[] assembliesToScan)
           where TRootModule : MokModule // 约束根模块类型
        {
            // 如果没有指定扫描程序集，默认扫描根模块所在的程序集
            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                assembliesToScan = new[] { typeof(TRootModule).Assembly };
            }

            // 1. 创建 MokModuleLoader 实例
            var moduleLoader = new ModuleLoader(services, loggerFactory);

            // 2. 执行模块的服务配置阶段 (LoadModulesAsync)
            // LoadModulesAsync 会发现、排序、实例化模块，并调用它们的 ConfigureServices 方法
            // 注意：这是一个异步方法，但在 ConfigureServices 阶段同步调用它需要 GetAwaiter().GetResult()。
            // 更好的做法是让模块的 ConfigureServicesAsync 返回 Task.CompletedTask，
            // 并在 OnApplicationInitialization 或 OnPostApplicationInitialization 中执行真正的异步初始化。
            // 或者，接受在 Program.cs 中使用异步 Main 的限制。
            // 为了简化集成到 builder.Services.Add... 的同步链，这里使用同步等待。
            // 生产环境更推荐异步集成方式（如使用 IHostedService 或在 app.Configure 内部处理）。
            // 这里为了示例简单，直接同步等待 LoadModulesAsync。
            moduleLoader.LoadModulesAsync(assembliesToScan).GetAwaiter().GetResult();

            // 3. 将 MokModuleLoader 实例注册到 DI 容器，以便后续初始化阶段使用
            services.AddSingleton(moduleLoader);
            services.AddSingleton(typeof(TRootModule)); // 注册根模块类型

            // ABP 框架还会注册一个 IAbpApplication 实例来表示整个应用程序，
            // 您也可以创建一个 IMokApplication 接口和实现，并注册到这里。
            // 为了本例简化，我们直接通过 MokModuleLoader 来触发初始化和关闭。

            return services;
        }

        public static async Task<IApplicationBuilder> AddApplicationAsync<TRootModule>(
           this IApplicationBuilder app,
           ILoggerFactory loggerFactory = null)
           where TRootModule : IMokModule // 约束根模块类型
        {
            var assembliesToScan = new[] { typeof(TRootModule).Assembly };

            // 1. 获取或创建日志工厂
            // 如果没有传入 loggerFactory，则从服务容器中获取默认的 ILoggerFactory
            if (loggerFactory == null)
            {
                loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            }
            // 创建 MokModuleLoader 实例
            var moduleLoader = app.ApplicationServices.GetRequiredService<ModuleLoader>();

            // 2. 执行模块的服务配置阶段 (LoadModulesAsync)
            // LoadModulesAsync 会发现、排序、实例化模块，并调用它们的 ConfigureServices 方法
            // 注意：这是一个异步方法，但在 ConfigureServices 阶段同步调用它需要 GetAwaiter().GetResult()。
            // 更好的做法是让模块的 ConfigureServicesAsync 返回 Task.CompletedTask，
            // 并在 OnApplicationInitialization 或 OnPostApplicationInitialization 中执行真正的异步初始化。
            // 或者，接受在 Program.cs 中使用异步 Main 的限制。
            // 为了简化集成到 builder.Services.Add... 的同步链，这里使用同步等待。
            // 生产环境更推荐异步集成方式（如使用 IHostedService 或在 app.Configure 内部处理）。
            // 这里为了示例简单，直接同步等待 LoadModulesAsync。
            await moduleLoader.LoadModulesAsync(assembliesToScan);

            //// 3. 将 MokModuleLoader 实例注册到 DI 容器，以便后续初始化阶段使用
            //app.AddSingleton(moduleLoader);
            //app.AddSingleton(typeof(TRootModule)); // 注册根模块类型

            // ABP 框架还会注册一个 IAbpApplication 实例来表示整个应用程序，
            // 您也可以创建一个 IMokApplication 接口和实现，并注册到这里。
            // 为了本例简化，我们直接通过 MokModuleLoader 来触发初始化和关闭。
            // 初始化模块
            //await moduleLoader.InitializeModulesAsync(app.ApplicationServices);
            return app;
        }


        // 针对 .NET Standard 2.0 和 2.1
        public static IServiceCollection AddApplicationAsync<TRootModule>(
            this IServiceCollection services,
            ILoggerFactory loggerFactory = null)
            where TRootModule : IMokModule // 约束根模块类型
        {
            var assembliesToScan = new[] { typeof(TRootModule).Assembly };

            // 1. 获取或创建日志工厂
            if (loggerFactory == null)
            {
                // 在 netstandard2.0 和 2.1 中，无法使用 WebApplicationBuilder，因此通过此方式获取 loggerFactory
                loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            }

            // 2. 创建 MokModuleLoader 实例
            var moduleLoader = new ModuleLoader(services, loggerFactory);

            // 3. 执行模块的服务配置阶段 (LoadModulesAsync)
            moduleLoader.LoadModulesAsync(assembliesToScan).GetAwaiter().GetResult();

            // 4. 将 MokModuleLoader 实例注册到 DI 容器，以便后续初始化阶段使用
            services.AddSingleton(moduleLoader);
            services.AddSingleton(typeof(TRootModule)); // 注册根模块类型

            // 返回 IServiceCollection，以便继续链式调用
            return services;
        }

    }
}
