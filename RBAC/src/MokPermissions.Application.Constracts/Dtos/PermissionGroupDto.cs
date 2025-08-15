using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Application.Contracts.Dtos
{
    /// <summary>
    /// 权限组DTO
    /// </summary>
    public class PermissionGroupDto
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<PermissionDto> Permissions { get; set; }
    }

}
