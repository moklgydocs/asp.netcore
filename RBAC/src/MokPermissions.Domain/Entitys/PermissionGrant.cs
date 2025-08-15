using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 表示一个权限授权
    /// </summary>
    public class PermissionGrant
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提供者名称（如：R表示角色，U表示用户）
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 提供者键（如：角色名称或用户ID）
        /// </summary>
        public string ProviderKey { get; set; }

        /// <summary>
        /// 是否授予
        /// </summary>
        public bool IsGranted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 租户ID，null表示宿主
        /// </summary>
        public Guid? TenantId { get; set; }

        public PermissionGrant()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.Now;
        }

        public PermissionGrant(
            string name,
            string providerName,
            string providerKey,
             Guid? tenantId = null,
            bool isGranted = true)
            : this()
        {
            Name = name;
            ProviderName = providerName;
            ProviderKey = providerKey;
            IsGranted = isGranted;
            TenantId = tenantId;
        }
    }
}
