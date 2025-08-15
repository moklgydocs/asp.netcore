using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Manager
{
    /// <summary>
    /// 批量权限管理扩展
    /// </summary>
    public interface IBatchPermissionManager : IPermissionManager
    {
        /// <summary>
        /// 批量授予权限
        /// </summary>
        /// <param name="permissionNames">权限名称列表</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task BatchGrantAsync(IEnumerable<string> permissionNames, string providerName, string providerKey);

        /// <summary>
        /// 批量撤销权限
        /// </summary>
        /// <param name="permissionNames">权限名称列表</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task BatchRevokeAsync(IEnumerable<string> permissionNames, string providerName, string providerKey);
    }
}
