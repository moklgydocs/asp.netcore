using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts
{
    /// <summary>
    /// 权限检查器接口
    /// </summary>
    public interface IPermissionChecker
    {
        /// <summary>
        /// 检查当前用户是否拥有指定权限
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <returns>是否拥有权限</returns>
        Task<bool> IsGrantedAsync(string name);

        /// <summary>
        /// 检查指定用户是否拥有指定权限
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="name">权限名称</param>
        /// <returns>是否拥有权限</returns>
        Task<bool> IsGrantedAsync(ClaimsPrincipal user, string name);

        /// <summary>
        /// 检查当前用户是否拥有多个权限
        /// </summary>
        /// <param name="names">权限名称列表</param>
        /// <returns>权限检查结果列表</returns>
        Task<List<PermissionCheckResult>> IsGrantedAsync(string[] names);

        /// <summary>
        /// 检查指定用户是否拥有多个权限
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="names">权限名称列表</param>
        /// <returns>权限检查结果列表</returns>
        Task<List<PermissionCheckResult>> IsGrantedAsync(ClaimsPrincipal user, string[] names);
    }
}
