using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts
{
    /// <summary>
    /// 角色权限管理服务接口
    /// </summary>
    public interface IRolePermissionService
    {
        /// <summary>
        /// 获取角色的所有权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限授权列表</returns>
        Task<List<PermissionGrant>> GetPermissionsAsync(Guid roleId);

        /// <summary>
        /// 获取角色的所有权限
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns>权限授权列表</returns>
        Task<List<PermissionGrant>> GetPermissionsAsync(string roleName);

        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionNames">权限名称列表</param>
        Task SetPermissionsAsync(Guid roleId, List<string> permissionNames);

        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="permissionNames">权限名称列表</param>
        Task SetPermissionsAsync(string roleName, List<string> permissionNames);
    }
}
