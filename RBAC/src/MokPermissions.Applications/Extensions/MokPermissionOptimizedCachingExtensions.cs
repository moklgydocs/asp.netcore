using MokPermissions.Domain.Shared.Cache;
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
    public static class MokPermissionOptimizedCachingExtensions
    {
        /// <summary>
        /// 添加优化的权限缓存服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddOptimizedPermissionCaching(
            this IServiceCollection services,
            Action<PermissionCacheOptions> configureOptions = null)
        {
            // 添加分布式缓存
            services.AddDistributedMemoryCache();

            // 配置选项
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<PermissionCacheOptions>(options => { });
            }

            // 使用装饰器模式包装权限存储
            services.Decorate<IPermissionStore, OptimizedCachedPermissionStore>();

            return services;
        }
    }
}
