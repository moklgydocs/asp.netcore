using Microsoft.Extensions.Options;
using MokPermissions.Domain.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Data
{
    /// <summary>
    /// 权限初始化器实现
    /// </summary>
    public class PermissionInitializer : IPermissionInitializer
    {
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly PermissionInitializationOptions _options;

        public PermissionInitializer(
            IPermissionDataSeeder permissionDataSeeder,
            IOptions<PermissionInitializationOptions> options)
        {
            _permissionDataSeeder = permissionDataSeeder;
            _options = options.Value;
        }

        public async Task InitializeAsync()
        {
            // 创建默认角色
            await _permissionDataSeeder.CreateRolesAsync(_options.DefaultRoles);

            // 授予角色权限
            foreach (var rolePermission in _options.RolePermissions)
            {
                await _permissionDataSeeder.GrantRolePermissionsAsync(
                    rolePermission.Key,
                    rolePermission.Value);
            }
        }
    }
}
