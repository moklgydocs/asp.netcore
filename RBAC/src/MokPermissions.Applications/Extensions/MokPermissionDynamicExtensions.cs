using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using MokPermissions.EntityframeworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain.Store;

namespace MokPermissions.Application.Extensions
{
    public static class MokPermissionDynamicExtensions
    {
        /// <summary>
        /// 添加动态权限支持
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDynamicPermissions(this IServiceCollection services)
        {
            // 注册动态权限存储
            services.AddScoped<IDynamicPermissionStore, EfCoreDynamicPermissionStore>();

            // 注册动态权限提供者
            services.AddSingleton<IPermissionDefinitionProvider, DynamicPermissionDefinitionProvider>();

            return services;
        }
    }
}
