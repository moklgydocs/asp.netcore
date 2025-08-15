using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain;
using MokPermissions.EntityframeworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Extensions
{
    public static class PermissionManagementCachingExtensions
    {
        /// <summary>
        /// 添加权限缓存服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionCaching(this IServiceCollection services)
        {
            // 添加分布式缓存
            services.AddDistributedMemoryCache();

            // 使用装饰器模式包装权限存储
            services.Decorate<IPermissionStore, CachedPermissionStore>();

            return services;
        }
    }
}
