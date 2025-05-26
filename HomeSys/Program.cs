using System.Threading.Tasks;
using Mok.AspNetCore;

namespace HomeSys
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            await builder.AddApplicationAsync<WebModule>();
            // Add services to the container.

            var app = builder.Build();
            app.MapGet("/",async (context) =>
            {
               await context.Response.WriteAsync("hello world");
            });
           await app.RunAsync();
           
        }
    }
}
