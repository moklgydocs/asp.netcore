using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionManagement.Permissions.Domain
{
    /// <summary>
    /// 权限提供者管理器，用于管理和获取所有权限提供者
    /// </summary>
    public class PermissionDefinitionManager
    {
        private readonly IEnumerable<IPermissionDefinitionProvider> _providers;

        private readonly PermissionDefinitionContext _context;

        private bool _isInitialized;

        public PermissionDefinitionManager(IEnumerable<IPermissionDefinitionProvider> providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
            _context = new PermissionDefinitionContext();
        }
        /// <summary>
        /// 获取所有权限定义
        /// </summary>
        /// <returns>所有权限定义</returns>
        public IReadOnlyList<PermissionDefinition> GetPermissions()
        {
            EnsureInitialized();
            return _context.Permissions.ToList();
        }
        /// <summary>
        /// 获取所有权限组
        /// </summary>
        /// <returns>所有权限组</returns>
        public IReadOnlyList<PermissionGroupDefinition> GetGroups()
        {
            EnsureInitialized();
            return _context.Groups;
        }

        /// <summary>
        /// 获取指定名称的权限定义
        /// </summary>
        /// <param name="name">权限名称</param>
        /// <returns>权限定义</returns>
        public PermissionDefinition GetPermission(string name)
        {
            EnsureInitialized();
            return _context.GetPermission(name);
        }
        /// <summary>
        /// 确保权限已经初始化
        /// </summary>
        public void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }
            foreach (var provider in _providers)
            {
                provider.Define(_context);
            }
            _isInitialized = true;
        }
    }
}
