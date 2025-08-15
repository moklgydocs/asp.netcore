using Microsoft.Extensions.Caching.Distributed;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Shared;
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
    /// 缓存的权限授权存储装饰器，用于包装实际的权限存储并提供缓存功能
    /// </summary>
    public class CachedPermissionStore : IPermissionStore
    {
        private readonly IPermissionStore _permissionStore;
        private readonly IDistributedCache _cache;

        // 缓存过期时间（分钟）
        private const int CacheExpirationMinutes = 30;

        public CachedPermissionStore(
            IPermissionStore permissionStore,
            IDistributedCache cache)
        {
            _permissionStore = permissionStore;
            _cache = cache;
        }

        public async Task<PermissionGrantStatus> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            // 尝试从缓存获取
            var cacheKey = GetIsGrantedCacheKey(name, providerName, providerKey);
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                return (PermissionGrantStatus)int.Parse(cachedValue);
            }

            // 从数据库获取
            var result = await _permissionStore.IsGrantedAsync(name, providerName, providerKey);

            // 存入缓存
            await _cache.SetStringAsync(
                cacheKey,
                ((int)result).ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                });

            return result;
        }

        public async Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            // 尝试从缓存获取
            var cacheKey = GetAllPermissionsCacheKey(providerName, providerKey);
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                return JsonConvert.DeserializeObject<List<PermissionGrant>>(cachedValue);
            }

            // 从数据库获取
            var permissions = await _permissionStore.GetAllAsync(providerName, providerKey);

            // 存入缓存
            await _cache.SetStringAsync(
                cacheKey,
                JsonConvert.SerializeObject(permissions),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                });

            return permissions;
        }

        public async Task SaveAsync(string permissionName, string providerName, string providerKey, bool isGranted)
        {
            // 先保存到数据库
            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, isGranted);

            // 清除相关缓存
            await InvalidatePermissionCacheAsync(permissionName, providerName, providerKey);
        }

        public async Task DeleteAsync(string permissionName, string providerName, string providerKey)
        {
            // 先从数据库删除
            await _permissionStore.DeleteAsync(permissionName, providerName, providerKey);

            // 清除相关缓存
            await InvalidatePermissionCacheAsync(permissionName, providerName, providerKey);
        }

        /// <summary>
        /// 使特定权限的缓存失效
        /// </summary>
        private async Task InvalidatePermissionCacheAsync(string permissionName, string providerName, string providerKey)
        {
            // 清除单个权限缓存
            await _cache.RemoveAsync(GetIsGrantedCacheKey(permissionName, providerName, providerKey));

            // 清除所有权限缓存
            await _cache.RemoveAsync(GetAllPermissionsCacheKey(providerName, providerKey));
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
