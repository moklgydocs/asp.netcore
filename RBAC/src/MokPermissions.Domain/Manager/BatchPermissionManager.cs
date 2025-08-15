using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Manager
{
    /// <summary>
    /// 批量权限管理实现
    /// </summary>
    public class BatchPermissionManager : IPermissionManager, IBatchPermissionManager
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IPermissionStore _permissionStore;

        public BatchPermissionManager(
            IPermissionManager permissionManager,
            IPermissionStore permissionStore)
        {
            _permissionManager = permissionManager;
            _permissionStore = permissionStore;
        }

        public Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            return _permissionManager.GetAllAsync(providerName, providerKey);
        }

        public Task GrantAsync(string permissionName, string providerName, string providerKey)
        {
            return _permissionManager.GrantAsync(permissionName, providerName, providerKey);
        }

        public Task ProhibitAsync(string permissionName, string providerName, string providerKey)
        {
            return _permissionManager.ProhibitAsync(permissionName, providerName, providerKey);
        }

        public Task RevokeAsync(string permissionName, string providerName, string providerKey)
        {
            return _permissionManager.RevokeAsync(permissionName, providerName, providerKey);
        }

        public async Task BatchGrantAsync(IEnumerable<string> permissionNames, string providerName, string providerKey)
        {
            // 如果权限存储支持批量操作，则直接调用
            if (_permissionStore is IBatchPermissionStore batchStore)
            {
                await batchStore.BatchSaveAsync(permissionNames, providerName, providerKey, true);
                return;
            }

            // 否则逐个授予
            foreach (var permissionName in permissionNames)
            {
                await _permissionManager.GrantAsync(permissionName, providerName, providerKey);
            }
        }

        public async Task BatchRevokeAsync(IEnumerable<string> permissionNames, string providerName, string providerKey)
        {
            // 如果权限存储支持批量操作，则直接调用
            if (_permissionStore is IBatchPermissionStore batchStore)
            {
                await batchStore.BatchDeleteAsync(permissionNames, providerName, providerKey);
                return;
            }

            // 否则逐个撤销
            foreach (var permissionName in permissionNames)
            {
                await _permissionManager.RevokeAsync(permissionName, providerName, providerKey);
            }
        }
    }
}
