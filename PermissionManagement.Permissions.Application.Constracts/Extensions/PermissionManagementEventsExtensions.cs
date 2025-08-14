using MokPermissions.Domain.Shared;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MokPermissions.Application.Contracts.Extensions
{
    public static class PermissionManagementEventsExtensions
    {
        /// <summary>
        /// 添加权限管理事件支持
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPermissionEvents(this IServiceCollection services)
        {
            // 注册事件发布器
            services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();

            // 注册事件处理器
            services.AddTransient<IEventHandler<PermissionGrantedEventData>, PermissionCacheInvalidationEventHandler>();
            services.AddTransient<IEventHandler<PermissionRevokedEventData>, PermissionCacheInvalidationEventHandler>();
            services.AddTransient<IEventHandler<PermissionProhibitedEventData>, PermissionCacheInvalidationEventHandler>();

            // 使用装饰器模式包装权限管理服务
            services.Decorate<IPermissionManager, EventPublishingPermissionManager>();

            return services;
        }
    }
}
