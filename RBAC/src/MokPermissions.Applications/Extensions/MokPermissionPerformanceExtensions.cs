using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain.Manager;
using MokPermissions.Domain.Store;
using MokPermissions.EntityframeworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Extensions
{
    public static class MokPermissionPerformanceExtensions
    {
        /// <summary>
        /// 添加批量权限服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddBatchPermissionServices(this IServiceCollection services)
        {
            // 注册批量权限存储
            services.AddScoped<IBatchPermissionStore, BatchEfCorePermissionStore>();

            // 装饰权限管理器
            services.Decorate<IPermissionManager, BatchPermissionManager>();

            // 注册批量权限管理器
            services.AddScoped<IBatchPermissionManager>(sp =>
                (IBatchPermissionManager)sp.GetRequiredService<IPermissionManager>());

            return services;
        }
    }
}
