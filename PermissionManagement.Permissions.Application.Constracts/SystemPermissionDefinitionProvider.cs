using MokPermissions.Domain;

namespace MokPermissions.Application.Contracts
{
    public class SystemPermissionDefinitionProvider : IPermissionDefinitionProvider
    {
        public void Define(PermissionDefinitionContext context)
        {

            // 添加系统管理权限组
            var adminGroup = context.AddGroup("Administration", "系统管理");
            var userManagement = adminGroup.AddPermission("UserManagement", "用户管理", "系统管理用户");

            // 添加用户管理的子权限
            userManagement.AddChild("UserManagement.Create", "创建用户");
            userManagement.AddChild("UserManagement.Update", "更新用户");
            userManagement.AddChild("UserManagement.Delete", "删除用户");
            userManagement.AddChild("UserManagement.View", "查看用户");


            // 添加角色管理权限
            var roleManagement = adminGroup.AddPermission(
                "RoleManagement",
                "角色管理",
                "管理系统角色");

            // 添加角色管理的子权限
            roleManagement.AddChild("RoleManagement.Create", "创建角色");
            roleManagement.AddChild("RoleManagement.Update", "更新角色");
            roleManagement.AddChild("RoleManagement.Delete", "删除角色");
            roleManagement.AddChild("RoleManagement.View", "查看角色");

            adminGroup.AddPermission("PermissionManagement", "权限管理", "管理系统权限");
        }
    }
}
