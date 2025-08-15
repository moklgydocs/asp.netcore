using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 权限授权特性
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; }

        public PermissionAuthorizeAttribute(string permissionName) : base("Permission")
        {
            if (string.IsNullOrWhiteSpace(permissionName))
            {
                throw new ArgumentNullException(nameof(permissionName), "权限名称不能为空");
            }
            PermissionName = permissionName;
            Policy = permissionName; // 设置策略为权限名称
        }
    }
}
