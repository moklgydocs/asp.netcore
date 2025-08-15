using Microsoft.EntityFrameworkCore;
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
    public static class PermissionManagementEntityFrameworkCoreExtensions
    {
        /// <summary>
        /// 添加权限管理EF Core服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="optionsAction">数据库配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionManagementEntityFrameworkCore(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
        {
            // 注册DbContext
            services.AddDbContext<PermissionManagementDbContext>(optionsAction);

            // 替换权限存储实现
            services.AddScoped<IPermissionStore, EfCorePermissionStore>();

            return services;
        }
    }

}