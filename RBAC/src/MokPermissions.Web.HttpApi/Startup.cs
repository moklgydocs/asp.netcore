using Microsoft.EntityFrameworkCore;
using MokPermissions.Application.Contracts.Middleware;
using MokPermissions.Application.Extensions;
using MokPermissions.Domain.Manager;
using MokPermissions.EntityframeworkCore;

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

            // 添加权限缓存
            services.AddPermissionCaching();

            // 添加多租户支持
            services.AddPermissionMultiTenancy();

            // 添加事件支持
            services.AddPermissionEvents();

            // 添加权限服务
            services.AddPermissionServices();

            // 添加控制器和Razor页面
            services.AddControllers();
            services.AddRazorPages();

            // 添加认证
            //services.AddAuthentication("Cookie")
            //    .AddCookie("Cookie", options =>
            //    {
            //        options.LoginPath = "/Account/Login";
            //        options.AccessDeniedPath = "/Account/AccessDenied";
            //    });

            // 添加授权
            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 确保数据库已创建
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MokPermissionDbContext>();
                dbContext.Database.EnsureCreated();
                // 初始化基本权限数据
                SeedData(scope.ServiceProvider);
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

            // 多租户中间件（提取当前租户信息）
            app.UseMiddleware<MultiTenancyMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }


        private void SeedData(IServiceProvider serviceProvider)
        {
            var permissionManager = serviceProvider.GetRequiredService<IPermissionManager>();

            // 创建管理员角色并授予所有权限
            var permissionDefinitionManager = serviceProvider.GetRequiredService<PermissionDefinitionManager>();
            var permissions = permissionDefinitionManager.GetPermissions();

            foreach (var permission in permissions)
            {
                permissionManager.GrantAsync(permission.Name, "R", "Admin").Wait();
            }
        }
    }
}
