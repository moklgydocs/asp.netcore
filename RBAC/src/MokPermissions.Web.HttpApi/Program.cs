using Mok.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace MokPermissions.Web.HttpApi
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration() // 注意，这里不能用var Log = new LoggerConfiguration
#if DEBUG
                    .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Async(c => c.File("Logs/logs.txt"))
                    .WriteTo.Async(x => x.Console())
                    .CreateLogger();

                Log.Information("Starting web host.");

                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                await builder.AddApplicationAsync<MokPermissionWebModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();
                await app.RunAsync();
                // 在应用程序资源释放后，释放Serilog资源
                Log.CloseAndFlush();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                // 确保在程序退出时释放日志资源
                Log.CloseAndFlush();
            }
        }
    }
}
