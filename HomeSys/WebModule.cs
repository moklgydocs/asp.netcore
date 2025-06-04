using Mok.Modularity;
using Microsoft.AspNetCore.Builder;

namespace HomeSys
{
    public class WebModule : MokModule
    { 
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddControllers(); 
            ConfigureSwaggerServices(context.Services);


        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            Console.WriteLine("应用程序初始化");

            var app = context.ApplicationBuilder;
            //// Configure the HTTP request pipeline.
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            //app.UseWelcomePage(); 
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHttpsRedirection();

             
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Docs");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/hello", (HttpContext context) =>
                {
                    context.Response.WriteAsJsonAsync("I Love U");
                });
            });
            // 项目启动的时候跳转到swagger
            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path == "/")
                {
                    ctx.Response.Redirect("/swagger/index.html");
                    return;
                }
                await next();
            });
        }

        /// <summary>
        /// 配置 Swagger 服务，
        /// <para>用于生成 API 文档</para>
        /// <para>注册了 Swagger 文档的基本信息（如标题、版本等），并确保所有控制器都包含在文档中</para>
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Backend API",
                        Version = "v1",
                        Description = "Blog API"
                    });
                // 确保包含所有控制器
                options.DocInclusionPredicate((docName, description) => true);
                //options.HideAbpEndpoints(); // 排除 ABP 内置接口（可选）  
                // 自动加载所有 xml 注释文件
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var xmlFile in xmlFiles)
                {
                    options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
                }

            });
        }
    }
}
