using Microsoft.Extensions.Caching.Distributed;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限缓存失效事件处理器
    /// </summary>
    public class PermissionCacheInvalidationEventHandler :
        IEventHandler<PermissionGrantedEventData>,
        IEventHandler<PermissionRevokedEventData>,
        IEventHandler<PermissionProhibitedEventData>
    {
        private readonly IDistributedCache _cache;

        public PermissionCacheInvalidationEventHandler(IDistributedCache cache)
        {
            _cache = cache;
        }

        public Task HandleAsync(PermissionGrantedEventData eventData)
        {
            return InvalidatePermissionCacheAsync(eventData);
        }

        public Task HandleAsync(PermissionRevokedEventData eventData)
        {
            return InvalidatePermissionCacheAsync(eventData);
        }

        public Task HandleAsync(PermissionProhibitedEventData eventData)
        {
            return InvalidatePermissionCacheAsync(eventData);
        }

        private Task InvalidatePermissionCacheAsync(PermissionChangedEventData eventData)
        {
            // 清除单个权限缓存
            var isGrantedCacheKey = GetIsGrantedCacheKey(
                eventData.PermissionName,
                eventData.ProviderName,
                eventData.ProviderKey,
                eventData.TenantId);

            // 清除所有权限缓存
            var allPermissionsCacheKey = GetAllPermissionsCacheKey(
                eventData.ProviderName,
                eventData.ProviderKey,
                eventData.TenantId);

            return Task.WhenAll(
                _cache.RemoveAsync(isGrantedCacheKey),
                _cache.RemoveAsync(allPermissionsCacheKey)
            );
        }

        /// <summary>
        /// 获取单个权限的缓存键
        /// </summary>
        private string GetIsGrantedCacheKey(string name, string providerName, string providerKey, Guid? tenantId)
        {
            return $"Permission:IsGranted:{tenantId}:{name}:{providerName}:{providerKey}";
        }

        /// <summary>
        /// 获取所有权限的缓存键
        /// </summary>
        private string GetAllPermissionsCacheKey(string providerName, string providerKey, Guid? tenantId)
        {
            return $"Permission:GetAll:{tenantId}:{providerName}:{providerKey}";
        }
    }
}
