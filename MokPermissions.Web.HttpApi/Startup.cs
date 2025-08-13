using MokPermissions.EntityframeworkCore;
using MokPermissions.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MokPermissions.Web.HttpApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 添加权限管理
            services.AddPermissionManagement();

            // 添加权限授权
            services.AddPermissionAuthorization();

            // 添加EF Core存储
            services.AddPermissionManagementEntityFrameworkCore(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            // 添加控制器和Razor页面
            services.AddControllers();
            services.AddRazorPages();

            // 添加认证
            services.AddAuthentication("Cookie")
                .AddCookie("Cookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // 添加授权
            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 确保数据库已创建
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PermissionManagementDbContext>();
                dbContext.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
