using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts.Dtos
{
    /// <summary>
    /// 更新权限请求
    /// </summary>
    public class UpdatePermissionRequest
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否授予
        /// </summary>
        public bool IsGranted { get; set; }
    }
}
