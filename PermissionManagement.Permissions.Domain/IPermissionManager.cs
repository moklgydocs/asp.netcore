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
    public interface IPermissionManager
    {
        /// <summary>
        /// 授予权限
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task GrantAsync(string permissionName, string providerName, string providerKey);

        /// <summary>
        /// 撤销权限
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task RevokeAsync(string permissionName, string providerName, string providerKey);

        /// <summary>
        /// 禁止权限
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task ProhibitAsync(string permissionName, string providerName, string providerKey);

        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        /// <returns>权限授权列表</returns>
        Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey);
    }
}
