using Microsoft.EntityFrameworkCore;
using Yarp.ReverseProxy.Configuration;

namespace GatewayCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            //builder.Services.AddReverseProxy()
            //    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
            // 1. 注册数据库上下文（示例使用SQL Server）
            builder.Services.AddDbContext<YarpConfigDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("YarpConfigDb")));
            // 2. 注册自定义配置提供器
            builder.Services.AddSingleton<IProxyConfigProvider, DatabaseProxyConfigProvider>();


            var app = builder.Build();

            // 5. 启动时初始化配置（可选：定时刷新配置）
            var configProvider = app.Services.GetRequiredService<IProxyConfigProvider>() as DatabaseProxyConfigProvider;
            if (configProvider != null)
            {
                // 定时刷新配置（例如每30秒）
                var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
                _ = Task.Run(async () =>
                {
                    while (await timer.WaitForNextTickAsync())
                    {
                        await configProvider.RefreshConfigAsync();
                    }
                });
            }

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapReverseProxy();

            app.Run();
        }
    }
}
