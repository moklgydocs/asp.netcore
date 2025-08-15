using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Data
{
    /// <summary>
    /// 权限初始化配置
    /// </summary>
    public class PermissionInitializationOptions
    {
        /// <summary>
        /// 默认角色列表
        /// </summary>
        public List<string> DefaultRoles { get; set; } = new List<string>();

        /// <summary>
        /// 角色权限映射
        /// </summary>
        public Dictionary<string, List<string>> RolePermissions { get; set; } = new Dictionary<string, List<string>>();
    }
}
