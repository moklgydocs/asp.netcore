using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Shared;
using MokPermissions.Domain.Shared.Cache;
using MokPermissions.Domain.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 优化的缓存权限存储装饰器
    /// </summary>
    public class OptimizedCachedPermissionStore : IPermissionStore
    {
        private readonly IPermissionStore _permissionStore;
        private readonly IDistributedCache _cache;
        private readonly ILogger<OptimizedCachedPermissionStore> _logger;
        private readonly PermissionCacheOptions _options;

        public OptimizedCachedPermissionStore(
            IPermissionStore permissionStore,
            IDistributedCache cache,
            ILogger<OptimizedCachedPermissionStore> logger,
            IOptions<PermissionCacheOptions> options)
        {
            _permissionStore = permissionStore;
            _cache = cache;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<PermissionGrantStatus> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            if (!_options.IsEnabled)
            {
                return await _permissionStore.IsGrantedAsync(name, providerName, providerKey);
            }

            // 尝试从缓存获取
            var cacheKey = GetIsGrantedCacheKey(name, providerName, providerKey);

            try
            {
                var cachedValue = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedValue))
                {
                    return (PermissionGrantStatus)int.Parse(cachedValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "从缓存获取权限授权状态时出错: {CacheKey}", cacheKey);
            }

            // 从数据库获取
            var result = await _permissionStore.IsGrantedAsync(name, providerName, providerKey);

            // 存入缓存
            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    ((int)result).ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.ExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(_options.SlidingExpirationMinutes)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "将权限授权状态存入缓存时出错: {CacheKey}", cacheKey);
            }

            return result;
        }
        public async Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            if (!_options.IsEnabled)
            {
                return await _permissionStore.GetAllAsync(providerName, providerKey);
            }

            // 尝试从缓存获取
            var cacheKey = GetAllPermissionsCacheKey(providerName, providerKey);

            try
            {
                var cachedValue = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrEmpty(cachedValue))
                {
                    return JsonConvert.DeserializeObject<List<PermissionGrant>>(cachedValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "从缓存获取权限授权列表时出错: {CacheKey}", cacheKey);
            }

            // 从数据库获取
            var permissions = await _permissionStore.GetAllAsync(providerName, providerKey);

            // 存入缓存
            try
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonConvert.SerializeObject(permissions),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.ExpirationMinutes),
                        SlidingExpiration = TimeSpan.FromMinutes(_options.SlidingExpirationMinutes)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "将权限授权列表存入缓存时出错: {CacheKey}", cacheKey);
            }

            return permissions;
        }

        public async Task SaveAsync(string permissionName, string providerName, string providerKey, bool isGranted)
        {
            // 先保存到数据库
            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, isGranted);

            if (_options.IsEnabled)
            {
                // 清除相关缓存
                await InvalidatePermissionCacheAsync(permissionName, providerName, providerKey);
            }
        }

        public async Task DeleteAsync(string permissionName, string providerName, string providerKey)
        {
            // 先从数据库删除
            await _permissionStore.DeleteAsync(permissionName, providerName, providerKey);

            if (_options.IsEnabled)
            {
                // 清除相关缓存
                await InvalidatePermissionCacheAsync(permissionName, providerName, providerKey);
            }
        }

        /// <summary>
        /// 使特定权限的缓存失效
        /// </summary>
        private async Task InvalidatePermissionCacheAsync(string permissionName, string providerName, string providerKey)
        {
            try
            {
                // 清除单个权限缓存
                await _cache.RemoveAsync(GetIsGrantedCacheKey(permissionName, providerName, providerKey));

                // 清除所有权限缓存
                await _cache.RemoveAsync(GetAllPermissionsCacheKey(providerName, providerKey));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "清除权限缓存时出错");
            }
        }

        /// <summary>
        /// 获取单个权限的缓存键
        /// </summary>
        private string GetIsGrantedCacheKey(string name, string providerName, string providerKey)
        {
            return $"Permission:IsGranted:{name}:{providerName}:{providerKey}";
        }

        /// <summary>
        /// 获取所有权限的缓存键
        /// </summary>
        private string GetAllPermissionsCacheKey(string providerName, string providerKey)
        {
            return $"Permission:GetAll:{providerName}:{providerKey}";
        }
    }
}
