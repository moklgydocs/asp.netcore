using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限授权存储接口，用于存储和检索权限授权信息
    /// </summary>
    public interface IPermissionStore
    {
        /// <summary>
        /// 检查是否授予了权限
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <param name="providerName">提供者名称（如：R表示角色，U表示用户）</param>
        /// <param name="providerKey">提供者键（如：角色名称或用户ID）</param>
        /// <returns>权限授权状态</returns>
        Task<PermissionGrantStatus> IsGrantedAsync(
            string name,
            string providerName,
            string providerKey
            );

        /// <summary>
        /// 获取所有权限授权
        /// </summary>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        /// <returns>权限授权列表</returns>
        Task<List<PermissionGrant>> GetAllAsync(
            string providerName,
            string providerKey
            );

        /// <summary>
        /// 保存权限授权
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        /// <param name="isGranted">是否授予</param>
        Task SaveAsync(
            string permissionName,
            string providerName,
            string providerKey,
            bool isGranted
            );

        /// <summary>
        /// 删除权限授权
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        Task DeleteAsync(
            string permissionName,
            string providerName,
            string providerKey
            );
    }
}
