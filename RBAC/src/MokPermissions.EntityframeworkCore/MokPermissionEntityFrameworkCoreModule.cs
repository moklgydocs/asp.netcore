using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mok.Modularity;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    [DependsOn(typeof(MokPermissionsDomainModule))]
    public class MokPermissionEntityFrameworkCoreModule : MokModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            var Configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            // 添加EF Core存储
            services.AddPermissionManagementEntityFrameworkCore(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            //// 确保数据库已创建
            //using (var scope = app.ApplicationServices.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<MokPermissionDbContext>();
            //    dbContext.Database.EnsureCreated();
            //    // 初始化基本权限数据
            //    SeedData(scope.ServiceProvider);
            //}
        }
    }
}
