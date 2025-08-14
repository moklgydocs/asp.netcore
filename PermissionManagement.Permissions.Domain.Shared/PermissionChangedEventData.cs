using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 权限变更事件数据
    /// </summary>
    public class PermissionChangedEventData
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 提供者名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 提供者键
        /// </summary>
        public string ProviderKey { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
    }

    /// <summary>
    /// 权限授予事件数据
    /// </summary>
    public class PermissionGrantedEventData : PermissionChangedEventData
    {
    }

    /// <summary>
    /// 权限撤销事件数据
    /// </summary>
    public class PermissionRevokedEventData : PermissionChangedEventData
    {
    }

    /// <summary>
    /// 权限禁止事件数据
    /// </summary>
    public class PermissionProhibitedEventData : PermissionChangedEventData
    {
    }
}
