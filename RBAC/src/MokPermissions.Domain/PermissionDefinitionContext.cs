using MokPermissions.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>  
    /// 权限定义存储，用于管理系统中所有权限的定义和分组。  
    /// </summary>  
    public class PermissionDefinitionContext
    {
        private readonly Dictionary<string, PermissionGroupDefinition> _groups;
        private readonly Dictionary<string, PermissionDefinition> _permissions;

        /// <summary>
        /// 获取所有权限组
        /// </summary>
        public IReadOnlyList<PermissionGroupDefinition> Groups => _groups.Values.ToList();

        /// <summary>
        /// 获取所有权限
        /// </summary>
        public IReadOnlyList<PermissionDefinition> Permissions => _permissions.Values.ToList();

        public PermissionDefinitionContext()
        {

            _groups = new Dictionary<string, PermissionGroupDefinition>();
            _permissions = new Dictionary<string, PermissionDefinition>();
        }

        /// <summary>
        /// 添加权限组
        /// </summary>
        /// <param name="name">组名称</param>
        /// <param name="displayName">显示名称</param>
        /// <returns>创建的权限组</returns>
        public virtual PermissionGroupDefinition AddGroup(
            string name,
            string displayName = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Group name cannot be null or empty.", nameof(name));
            }
            if (_groups.ContainsKey(name))
            {
                throw new InvalidOperationException($"Group '{name}' already exists.");
            }
            var group = new PermissionGroupDefinition(name, displayName);
            _groups[name] = group;
            return group;
        }

        /// <summary>
        /// 获取权限组
        /// </summary>
        /// <param name="name">组名称</param>
        /// <returns>权限组</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual PermissionGroupDefinition GetGroup(string name)
        {
            if (!_groups.ContainsKey(name))
            {
                throw new InvalidOperationException($"找不到权限组 '{name}'");
            }

            return _groups[name];
        }

        /// <summary>
        /// 注册权限定义
        /// </summary>
        /// <param name="permission">权限定义</param>
        public virtual void AddPermission(
          PermissionDefinition permission)
        {
            if (_permissions.ContainsKey(permission.Name))
            {
                throw new InvalidOperationException($"权限 '{permission.Name}' 已存在");
            }

            _permissions[permission.Name] = permission;
        }

        public virtual PermissionDefinition GetPermission(string name)
        {
            if (!_permissions.ContainsKey(name))
            {
                throw new InvalidOperationException($"找不到权限 '{name}'");
            }
            return _permissions[name];
        }

    }
}
