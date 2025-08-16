using MokPermissions.Domain.Shared.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 表示权限组，用于对权限进行分类
    /// </summary>
    public class PermissionGroupDefinition : IHasTenant
    {
        /// <summary>
        /// 权限组的名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 权限组的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 权限组中包含的权限列表
        /// </summary>
        public List<PermissionDefinition> Permissions { get; }
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建一个新的权限组
        /// </summary>
        /// <param name="name">组名称</param>
        /// <param name="displayName">显示名称</param>
        public PermissionGroupDefinition(string name, string displayName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? name;
            Permissions = new List<PermissionDefinition>();
        }

        /// <summary>
        /// 向组中添加权限
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <param name="displayName">显示名称</param>
        /// <param name="description">权限描述</param>
        /// <param name="isGrantedByDefault">是否默认授予</param>
        /// <returns>创建的权限</returns>
        public virtual PermissionDefinition AddPermission(
            string name,
            string displayName = null,
            string description = null,
            bool isGrantedByDefault = false)
        {
            var permission = new PermissionDefinition(
                name,
                displayName,
                description,
                isGrantedByDefault
            )
            {
                Group = Name
            };

            Permissions.Add(permission);
            return permission;
        }

        public virtual PermissionDefinition GetPermission(string name)
        {
            return Permissions.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentNullException(name);
        }
    }
}
