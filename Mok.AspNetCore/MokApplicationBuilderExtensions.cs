using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;
using Mok.Modularity;

namespace Mok.AspNetCore
{
    public static class MokApplicationBuilderExtensions
    {
        // 针对 .NET Standard 2.0 和 2.1  
        public static async Task<WebApplicationBuilder> AddApplicationAsync<TRootModule>(
            this WebApplicationBuilder webBuilder,
            ILoggerFactory loggerFactory = null)
            where TRootModule : MokModule // 约束根模块类型  
        {
            if (webBuilder == null)
            {
                throw new ArgumentNullException(nameof(webBuilder));
            }

            var assembliesToScan = new[] { typeof(TRootModule).Assembly };

            // 获取或创建日志工厂
            if (loggerFactory == null)
            { 
                loggerFactory = webBuilder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            }

            var application = await Application.CreateAsync(typeof(TRootModule), webBuilder.Services, loggerFactory, assembliesToScan);
            // 返回 IServiceCollection，以便继续链式调用  
            return webBuilder;
        }

        public static async Task InitializeApplicationAsync([NotNull] this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var application = app.ApplicationServices.GetRequiredService<IApplication>();

            // 设置应用程序停止时的回调
            applicationLifetime.ApplicationStopping.Register(async () =>
            {
                // 使用无等待模式避免可能的死锁
                try
                {
                    await application.ShutdownAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // 记录关闭时的异常，但不阻止进程退出
                    var logger = app.ApplicationServices.GetService<ILogger<Application>>();
                    logger?.LogError(ex, "Application shutdown error");
                }
            });

            // 应用程序完全停止后的资源释放
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    (application as IDisposable)?.Dispose();
                }
                catch (Exception ex)
                {
                    var logger = app.ApplicationServices.GetService<ILogger<Application>>();
                    logger?.LogError(ex, "Application dispose error");
                }
            });
            // 初始化应用程序
            await application.InitializeApplicationAsync(app);
        }
    }
}
