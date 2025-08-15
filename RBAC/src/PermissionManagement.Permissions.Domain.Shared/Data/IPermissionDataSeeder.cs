using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Data
{
    /// <summary>
    /// 权限数据种子接口
    /// </summary>
    public interface IPermissionDataSeeder
    {
        /// <summary>
        /// 创建默认角色
        /// </summary>
        /// <param name="roleNames">角色名称列表</param>
        Task CreateRolesAsync(IEnumerable<string> roleNames);

        /// <summary>
        /// 授予角色权限
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="permissionNames">权限名称列表</param>
        Task GrantRolePermissionsAsync(string roleName, IEnumerable<string> permissionNames);
    }
}
