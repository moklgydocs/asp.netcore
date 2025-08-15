using MokPermissions.Domain.Shared;
using MokPermissions.EntityframeworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain.Store;
using MokPermissions.Domain.Entitys;

namespace MokPermissions.Application.Contracts.Extensions
{
    public static class MokPermissionsMultiTenancyExtensions
    {
        /// <summary>
        /// 添加权限管理多租户支持
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionMultiTenancy(this IServiceCollection services)
        {
            // 注册当前租户服务
            services.AddScoped<ICurrentTenant, CurrentTenant>();

            // 使用多租户权限存储
            services.AddScoped<IPermissionStore, MultiTenantEfCorePermissionStore>();

            return services;
        }
    }
}
