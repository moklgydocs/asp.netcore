using Mok.Modularity;
using MokPermissions.Application.Contracts;
using MokPermissions.Application.Extensions;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MokPermissions.Application
{
    [DependsOn(typeof(MokPermissionsDomainModule), 
        typeof(MokPermissionsApplicationContractsModule))]
    public class MokPermissionApplicationModule : MokModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            // 添加权限管理
            services.AddPermissionManagement();

            // 添加权限授权
            services.AddPermissionAuthorization();

            // 添加权限缓存
            services.AddPermissionCaching(); 

            // 添加事件支持
            services.AddPermissionEvents();

            // 添加权限服务
            services.AddPermissionServices();

        }
    }
}
