using Mok.Modularity;

namespace HomeSys
{
    public class WebModule : MokModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Console.WriteLine("到达与配置服务点");
            base.PreConfigureServices(context);
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddControllers();
            context.Services.AddRazorPages();

        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.ApplicationBuilder;
            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMvc(); 
            //app.MapStaticAssets();
            //app.MapRazorPages()
            //   .WithStaticAssets();

        }
    }
}
