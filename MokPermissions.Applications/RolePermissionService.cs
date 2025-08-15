using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application
{
    /// <summary>
    /// 角色权限管理服务实现
    /// </summary>
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IPermissionManager _permissionManager;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;

        public RolePermissionService(
            IPermissionManager permissionManager,
            PermissionDefinitionManager permissionDefinitionManager)
        {
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task<List<PermissionGrant>> GetPermissionsAsync(Guid roleId)
        {
            return await _permissionManager.GetAllAsync("R", roleId.ToString());
        }

        public async Task<List<PermissionGrant>> GetPermissionsAsync(string roleName)
        {
            return await _permissionManager.GetAllAsync("R", roleName);
        }

        public async Task SetPermissionsAsync(Guid roleId, List<string> permissionNames)
        {
            await SetPermissionsAsync(roleId.ToString(), permissionNames);
        }

        public async Task SetPermissionsAsync(string roleName, List<string> permissionNames)
        {
            // 获取现有权限
            var existingPermissions = await _permissionManager.GetAllAsync("R", roleName);
            var existingPermissionNames = existingPermissions
                .Where(p => p.IsGranted)
                .Select(p => p.Name)
                .ToHashSet();

            // 计算需要添加和删除的权限
            var addedPermissions = permissionNames
                .Where(p => !existingPermissionNames.Contains(p))
                .ToList();

            var removedPermissions = existingPermissionNames
                .Where(p => !permissionNames.Contains(p))
                .ToList();

            // 添加新权限
            foreach (var permissionName in addedPermissions)
            {
                // 验证权限名称是否存在
                _permissionDefinitionManager.GetPermission(permissionName);

                await _permissionManager.GrantAsync(permissionName, "R", roleName);
            }

            // 删除移除的权限
            foreach (var permissionName in removedPermissions)
            {
                await _permissionManager.RevokeAsync(permissionName, "R", roleName);
            }
        }
    }

}
