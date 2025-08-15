using MokPermissions.Domain.Manager;
using MokPermissions.Domain.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Data
{
    /// <summary>
    /// 权限数据种子实现
    /// </summary>
    public class PermissionDataSeeder : IPermissionDataSeeder
    {
        private readonly IPermissionManager _permissionManager;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;

        public PermissionDataSeeder(
            IPermissionManager permissionManager,
            PermissionDefinitionManager permissionDefinitionManager)
        {
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public Task CreateRolesAsync(IEnumerable<string> roleNames)
        {
            // 实际项目中，这里应该调用角色管理服务创建角色
            // 在本示例中，我们假设角色已经存在
            return Task.CompletedTask;
        }

        public async Task GrantRolePermissionsAsync(string roleName, IEnumerable<string> permissionNames)
        {
            foreach (var permissionName in permissionNames)
            {
                try
                {
                    // 验证权限是否存在
                    _permissionDefinitionManager.GetPermission(permissionName);

                    // 授予权限
                    await _permissionManager.GrantAsync(permissionName, "R", roleName);
                }
                catch (Exception ex)
                {
                    // 记录日志但不抛出异常
                    Console.WriteLine($"无法授予权限 {permissionName} 给角色 {roleName}: {ex.Message}");
                }
            }
        }
    }
}
