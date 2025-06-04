using Mok.Modularity;
using Microsoft.AspNetCore.Builder;

namespace HomeSys
{
    public class WebModule : MokModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Console.WriteLine("到达与配置服务点");
            context.Services.AddRazorPages();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddControllers();
            context.Services.AddRazorPages();

        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Console.WriteLine("应用程序初始化");

            var app = context.ApplicationBuilder;
            //// Configure the HTTP request pipeline.
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseWelcomePage(); 
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();


            app.UseAuthorization();
            app.UseRouting(); 
            // 如何让我的框架支持IEndpointRouteBuilder
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            }); 
        }
    }
}
