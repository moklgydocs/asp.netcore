using Mapster;
using MiddleWare.HealthCheck;
using MiddleWare.IPFilterMiddleware;
using MiddleWare.LoggerMiddleWare;
using MiddleWare.RequestTimingMiddleware;
using MiddleWare.ResponseCompresison;
using MiddleWare.SecurityHeadersMiddleware;
using Mok.AspNetCore;
using Mok.Modularity;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddleWare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            //builder.Logging.SetMinimumLevel(LogLevel.Debug);


            await builder.AddApplicationAsync<MiddlewareModule>(); 
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            app.Run();
        }
    }
}
