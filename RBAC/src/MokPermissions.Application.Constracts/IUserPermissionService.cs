using MokPermissions.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts
{
    /// <summary>
    /// 用户权限管理服务接口
    /// </summary>
    public interface IUserPermissionService
    {
        /// <summary>
        /// 获取用户的所有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限授权列表</returns>
        Task<List<PermissionGrant>> GetPermissionsAsync(Guid userId);

        /// <summary>
        /// 设置用户权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionNames">权限名称列表</param>
        Task SetPermissionsAsync(Guid userId, List<string> permissionNames);
    }
}
