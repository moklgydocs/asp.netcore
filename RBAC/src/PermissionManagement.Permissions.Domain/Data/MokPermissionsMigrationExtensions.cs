using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Data
{
    public static class MokPermissionsMigrationExtensions
    {
        /// <summary>
        /// 添加权限初始化服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionInitialization(
            this IServiceCollection services,
            Action<PermissionInitializationOptions> configureOptions = null)
        {
            // 注册权限种子
            services.AddScoped<IPermissionDataSeeder, PermissionDataSeeder>();

            // 注册权限初始化器
            services.AddScoped<IPermissionInitializer, PermissionInitializer>();

            // 配置选项
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            return services;
        }
    }
}
