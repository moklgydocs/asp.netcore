using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 动态权限记录，用于存储动态定义的权限
    /// </summary>
    public class DynamicPermissionRecord
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
        /// 是否默认授予
        /// </summary>
        public bool IsGrantedByDefault { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }
    }

}
