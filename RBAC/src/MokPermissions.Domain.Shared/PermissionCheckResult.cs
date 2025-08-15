using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 权限检查结果
    /// </summary>
    public class PermissionCheckResult
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; }

        /// <summary>
        /// 是否被授予
        /// </summary>
        public bool IsGranted { get; }

        public PermissionCheckResult(string permissionName, bool isGranted)
        {
            PermissionName = permissionName;
            IsGranted = isGranted;
        }
    }
}
