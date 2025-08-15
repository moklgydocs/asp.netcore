using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Cache
{
    /// <summary>
    /// 权限缓存选项
    /// </summary>
    public class PermissionCacheOptions
    {
        /// <summary>
        /// 缓存过期时间（分钟）
        /// </summary>
        public int ExpirationMinutes { get; set; } = 30;

        /// <summary>
        /// 滑动过期时间（分钟）
        /// </summary>
        public int SlidingExpirationMinutes { get; set; } = 10;

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

}
