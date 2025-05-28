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
            where TRootModule : IMokModule // 约束根模块类型
        {
            var assembliesToScan = new[] { typeof(TRootModule).Assembly };

            // 1. 获取或创建日志工厂
            if (loggerFactory == null)
            {
                // 在 netstandard2.0 和 2.1 中，无法使用 WebApplicationBuilder，因此通过此方式获取 loggerFactory
                loggerFactory = webBuilder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            }
            var applicationLifetime = webBuilder.Services.BuildServiceProvider().GetRequiredService<IHostApplicationLifetime>();
             
           await Application.CreateAsync(typeof(TRootModule), webBuilder.Services, loggerFactory, assembliesToScan);


            // 返回 IServiceCollection，以便继续链式调用
            return webBuilder;
        }

        public static async Task InitializeApplicationAsync([NotNull] this IApplicationBuilder app)
        {

            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var application = app.ApplicationServices.GetRequiredService<IApplication>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                AsyncHelper.RunSync(async () => await application.ShutdownAsync());
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                application.Dispose();
            });

            await application.InitializeApplicationAsync();
        }
    }
}
