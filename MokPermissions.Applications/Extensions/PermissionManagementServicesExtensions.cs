using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Extensions
{
    public static class PermissionManagementServicesExtensions
    {
        /// <summary>
        /// 添加权限管理服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionServices(this IServiceCollection services)
        {
            // 注册角色权限管理服务
            services.AddScoped<IRolePermissionService, RolePermissionService>();

            // 注册用户权限管理服务
            services.AddScoped<IUserPermissionService, UserPermissionService>();

            return services;
        }
    }
}
