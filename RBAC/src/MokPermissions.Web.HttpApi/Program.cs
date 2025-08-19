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
            return 0;
        }
    }
}
