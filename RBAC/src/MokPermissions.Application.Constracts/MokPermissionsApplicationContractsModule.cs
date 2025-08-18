using Mok.Modularity;
using MokPermissions.Application.Contracts.Middleware;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts
{
    [DependsOn(typeof(MokPermissionsDomainSharedModule))]
    public class MokPermissionsApplicationContractsModule : MokModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // 添加多租户支持
            context.Services.AddPermissionMultiTenancy();
        }
    }
}
