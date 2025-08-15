using Mok.AspNetCore;
using System.Threading.Tasks;

namespace MokPermissions.Web.HttpApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Host.CreateDefaultBuilder(args)
            //   .ConfigureWebHostDefaults(webBuilder =>
            //   {
            //       // ¹ØÁª Startup Àà
            //       webBuilder.UseStartup<Startup>();
            //   }).Build().Run(); 
            var builder = WebApplication.CreateBuilder(args);
            await builder.AddApplicationAsync<MokPermissionWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
        }
    }
}
