using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MokPermissions.Domain.Shared;
using MokPermissions.Application.Contracts;

namespace MokPermissions.Application.Contracts.Extensions
{
    public static class PermissionManagementAuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// 添加权限授权服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            // 注册权限存储
            services.AddSingleton<IPermissionStore, InMemoryPermissionStore>();

            // 注册当前用户
            services.AddHttpContextAccessor(); 
            services.AddScoped<ICurrentUser, CurrentUser>();

            // 注册权限检查器
            services.AddScoped<IPermissionChecker, PermissionChecker>();

            // 注册权限管理服务
            services.AddScoped<IPermissionManager, PermissionManager>();

            // 注册权限授权处理程序
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // 添加授权策略提供程序
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            return services;
        }
    }
}
