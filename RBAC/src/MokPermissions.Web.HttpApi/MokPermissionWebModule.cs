using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Mok.Modularity;
using MokPermissions.Application;
using MokPermissions.Application.Contracts;
using MokPermissions.Application.Contracts.Middleware;
using MokPermissions.Domain.Manager;
using MokPermissions.EntityframeworkCore;

namespace MokPermissions.Web.HttpApi
{
    [DependsOn(typeof(MokPermissionEntityFrameworkCoreModule),
        typeof(MokPermissionsApplicationContractsModule),
        typeof(MokPermissionApplicationModule)
        )]
    public class MokPermissionWebModule : MokModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
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
        // 如果需要在应用程序初始化时执行某些操作，可以重写 OnApplicationInitialization 方法
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.ApplicationBuilder;
            var env = context.Enviroment;

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
            app.UseMiddleware<MultiTenancyMiddleware>();// 这里的注入方式存在问题

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
