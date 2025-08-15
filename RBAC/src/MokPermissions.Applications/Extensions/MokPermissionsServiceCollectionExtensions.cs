using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using MokPermissions.Domain.Manager;

namespace MokPermissions.Application.Extensions
{

    public static class MokPermissionsServiceCollectionExtensions
    {
        /// <summary>
        /// 添加权限管理服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionManagement(this IServiceCollection services)
        {
            // 注册权限管理器
            services.AddSingleton<PermissionDefinitionManager>();

            // 注册系统权限提供者
            services.AddSingleton<IPermissionDefinitionProvider, SystemPermissionDefinitionProvider>();

            return services;
        }
    }
}
