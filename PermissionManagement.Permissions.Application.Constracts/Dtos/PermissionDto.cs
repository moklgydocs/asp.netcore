using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts.Dtos
{
    /// <summary>
    /// 权限管理DTO
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 父权限名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 是否已授权
        /// </summary>
        public bool IsGranted { get; set; }

        /// <summary>
        /// 是否被禁止
        /// </summary>
        public bool IsProhibited { get; set; }
    }

}
