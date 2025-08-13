using MokPermissions.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限管理服务实现
    /// </summary>
    public class PermissionManager : IPermissionManager
    {
        private readonly IPermissionStore _permissionStore;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;

        public PermissionManager(
            IPermissionStore permissionStore,
            PermissionDefinitionManager permissionDefinitionManager)
        {
            _permissionStore = permissionStore;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task GrantAsync(string permissionName, string providerName, string providerKey)
        {
            // 验证权限名称是否存在
            _permissionDefinitionManager.GetPermission(permissionName);

            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, true);
        }

        public async Task RevokeAsync(string permissionName, string providerName, string providerKey)
        {
            // 验证权限名称是否存在
            _permissionDefinitionManager.GetPermission(permissionName);

            await _permissionStore.DeleteAsync(permissionName, providerName, providerKey);
        }

        public async Task ProhibitAsync(string permissionName, string providerName, string providerKey)
        {
            // 验证权限名称是否存在
            _permissionDefinitionManager.GetPermission(permissionName);

            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, false);
        }

        public async Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            return await _permissionStore.GetAllAsync(providerName, providerKey);
        }
    }
}
