using MokPermissions.Domain.Shared.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role: IHasTenant
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否是默认角色
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否是静态角色（不可删除）
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Role(
            Guid id,
            string name,
            string displayName = null,
            string description = null,
            Guid? tenantId = null,
            bool isDefault = false,
            bool isStatic = false)
        {
            Id = id;
            Name = name;
            DisplayName = displayName ?? name;
            Description = description;
            TenantId = tenantId;
            IsDefault = isDefault;
            IsStatic = isStatic;
        }
    }
}
