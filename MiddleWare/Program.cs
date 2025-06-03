using Mapster;
using MiddleWare.HealthCheck;
using MiddleWare.IPFilterMiddleware;
using MiddleWare.LoggerMiddleWare;
using MiddleWare.RequestTimingMiddleware;
using MiddleWare.ResponseCompresison;
using MiddleWare.SecurityHeadersMiddleware;
using System.Xml.Linq;

namespace MiddleWare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseRequestLogging();
            app.UseRequestTiming();
            app.UseSecurityHeaders(option =>
            {
                option.UseHsts = true;
                option.UseXssProtection = true;
                option.UseContentTypeOptions = true;
                option.UseFrameOptions = true;
                option.ContentSecurityPolicy = "default-src 'self'; script-src 'self'; https://trusted-cdn.com";
            });
            app.UseIpFilter(option =>
            {
                //option.AllowedIps.Add("127.0.0.1");
                option.AllowedIps.Add("::1");

                option.BannedIps.Add("192.168.42.52");
                option.BannedIps.Add("127.0.0.1");
            });

            app.UseCompression(option =>
            {
                option.CompressionLevel = System.IO.Compression.CompressionLevel.Fastest;
                option.MimeTypes.Add("application/vnd.ms-excel");
            });
            app.UseHealthCheck(options =>
            {
                options.HealthEndpointPath = "/api/health";
                options.HealthChecks.Add(async _ =>
                {
                    return new HealthCheck.HealthCheckResult
                    {
                        HealthStatus = (await HealthCheckMiddleExtensions.CheckRedisConnectionAsync()).HealthStatus,
                        Description = (await HealthCheckMiddleExtensions.CheckRedisConnectionAsync()).Description,
                        Duration = TimeSpan.Zero,
                        Name = "Redis"
                    };
                });

            });
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            });

            app.Run();
        }
    }
}
