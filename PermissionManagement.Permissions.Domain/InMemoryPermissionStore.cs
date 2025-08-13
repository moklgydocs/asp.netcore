using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using MokPermissions.Domain.Shared;
using System.Collections.Concurrent;
using System.Security;

namespace MokPermissions.Applications
{
    /// <summary>
    /// 基于内存的权限存储实现
    /// </summary>
    public class InMemoryPermissionStore : IPermissionStore
    {
        private readonly ConcurrentDictionary<string, PermissionGrant> _permissions;

        public InMemoryPermissionStore()
        {
            _permissions = new ConcurrentDictionary<string, PermissionGrant>();
        }
        public Task DeleteAsync(string permissionName, string providerName, string providerKey)
        {
            var key = GetKey(permissionName, providerName, providerKey);
            _permissions.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            var result = _permissions.Values.Where(x => x.ProviderName == providerKey && x.ProviderKey == providerKey).ToList();
            return Task.FromResult(result);
        }

        public Task<PermissionGrantStatus> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            var key = GetKey(name, providerName, providerKey);
            if (_permissions.TryGetValue(key, out var permissionGrant))
            {
                return Task.FromResult(permissionGrant.IsGranted ? PermissionGrantStatus.Granted : PermissionGrantStatus.Prohibited);
            }
            return Task.FromResult(PermissionGrantStatus.Undefined);
        }

        public Task SaveAsync(string permissionName, string providerName, string providerKey, bool isGranted)
        {
            var key = GetKey(permissionName, providerName, providerKey);
            var permissionGrant = new PermissionGrant(permissionName, providerName, providerKey, isGranted);
            //_permissions[key] = permissionGrant;
            _permissions.AddOrUpdate(key, permissionGrant, (k, v) => permissionGrant);
            return Task.CompletedTask;
        }

        private string GetKey(string permissionName, string providerName, string providerKey)
        {
            return $"{permissionName}:{providerName}:{providerKey}";
        }
    }
}
