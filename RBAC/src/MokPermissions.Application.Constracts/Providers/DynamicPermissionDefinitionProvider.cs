using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MokPermissions.Domain.Store;

namespace MokPermissions.Application.Contracts.Providers
{
    /// <summary>
    /// 动态权限提供者，从数据库加载动态定义的权限
    /// </summary>
    public class DynamicPermissionDefinitionProvider : IPermissionDefinitionProvider
    {
        private readonly IDynamicPermissionStore _dynamicPermissionStore;

        public DynamicPermissionDefinitionProvider(IDynamicPermissionStore dynamicPermissionStore)
        {
            _dynamicPermissionStore = dynamicPermissionStore;
        }

        public void Define(PermissionDefinitionContext context)
        {
            // 从存储中获取所有动态权限
            var dynamicPermissions = _dynamicPermissionStore.GetPermissionsAsync().GetAwaiter().GetResult();

            // 按组名称分组
            var groupedPermissions = dynamicPermissions
                .GroupBy(p => p.GroupName ?? "Default")
                .ToDictionary(g => g.Key, g => g.ToList());

            // 首先创建所有组
            foreach (var groupName in groupedPermissions.Keys)
            {
                if (!context.Groups.Any(g => g.Name == groupName))
                {
                    context.AddGroup(groupName, groupName);
                }
            }

            // 存储已创建的权限，用于后续查找父权限
            var createdPermissions = new Dictionary<string, PermissionDefinition>();

            // 创建没有父权限的权限
            foreach (var group in groupedPermissions)
            {
                var rootPermissions = group.Value
                    .Where(p => string.IsNullOrEmpty(p.ParentName))
                    .ToList();

                foreach (var record in rootPermissions)
                {
                    var permissionGroup = context.GetGroup(record.GroupName ?? "Default");
                    var permission = permissionGroup.AddPermission(
                        record.Name,
                        record.DisplayName,
                        record.Description,
                        record.IsGrantedByDefault);

                    createdPermissions[record.Name] = permission;
                }
            }

            // 创建有父权限的权限
            bool anyCreated;
            do
            {
                anyCreated = false;
                foreach (var group in groupedPermissions)
                {
                    var childPermissions = group.Value
                        .Where(p => !string.IsNullOrEmpty(p.ParentName) &&
                                    !createdPermissions.ContainsKey(p.Name) &&
                                    createdPermissions.ContainsKey(p.ParentName))
                        .ToList();

                    foreach (var record in childPermissions)
                    {
                        var parentPermission = createdPermissions[record.ParentName];
                        var permission = parentPermission.AddChild(
                            record.Name,
                            record.DisplayName,
                            record.Description,
                            record.IsGrantedByDefault);

                        createdPermissions[record.Name] = permission;
                        anyCreated = true;
                    }
                }
            } while (anyCreated);
        }
    }
}
