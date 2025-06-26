using Microsoft.AspNetCore.Identity;
using IdentityServer.Models;

namespace IdentityServer.Data
{
    /// <summary>
    /// 数据库种子数据初始化
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// 初始化数据库种子数据
        /// </summary>
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // 确保数据库已创建
            await context.Database.EnsureCreatedAsync();

            // 创建角色
            await CreateRolesAsync(roleManager);

            // 创建用户
            await CreateUsersAsync(userManager);
        }

        /// <summary>
        /// 创建系统角色
        /// </summary>
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                "Admin",        // 系统管理员
                "Manager",      // 管理员
                "User",         // 普通用户
                "Guest",        // 访客
                "Developer",    // 开发者
                "Tester"        // 测试员
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        /// <summary>
        /// 创建系统用户
        /// </summary>
        private static async Task CreateUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // 管理员用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                DisplayName = "系统管理员",
                FirstName = "Admin",
                LastName = "User",
                Age = 30,
                Department = "IT部门",
                Permissions = "user.manage,system.admin,data.read,data.write,api.full",
                EmailConfirmed = true,
                IsActive = true
            }, "Admin123!", "Admin");

            // 管理员用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "manager",
                Email = "manager@example.com",
                DisplayName = "部门经理",
                FirstName = "John",
                LastName = "Manager",
                Age = 35,
                Department = "销售部门",
                Permissions = "user.manage,data.read,data.write,api.read,api.write",
                EmailConfirmed = true,
                IsActive = true
            }, "Manager123!", "Manager");

            // 普通用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "user",
                Email = "user@example.com",
                DisplayName = "普通用户",
                FirstName = "Jane",
                LastName = "User",
                Age = 28,
                Department = "市场部门",
                Permissions = "data.read,api.read",
                EmailConfirmed = true,
                IsActive = true
            }, "User123!", "User");

            // 开发者用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "developer",
                Email = "developer@example.com",
                DisplayName = "开发工程师",
                FirstName = "Bob",
                LastName = "Developer",
                Age = 26,
                Department = "技术部门",
                Permissions = "data.read,data.write,api.read,api.write,debug.access",
                EmailConfirmed = true,
                IsActive = true
            }, "Dev123!", "Developer");

            // 测试用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "tester",
                Email = "tester@example.com",
                DisplayName = "测试工程师",
                FirstName = "Alice",
                LastName = "Tester",
                Age = 24,
                Department = "质量部门",
                Permissions = "data.read,api.read,test.access",
                EmailConfirmed = true,
                IsActive = true
            }, "Test123!", "Tester");

            // 访客用户
            await CreateUserIfNotExistsAsync(userManager, new ApplicationUser
            {
                UserName = "guest",
                Email = "guest@example.com",
                DisplayName = "访客用户",
                FirstName = "Guest",
                LastName = "User",
                Age = 20,
                Department = "访客",
                Permissions = "data.read",
                EmailConfirmed = true,
                IsActive = true
            }, "Guest123!", "Guest");
        }

        /// <summary>
        /// 创建用户（如果不存在）
        /// </summary>
        private static async Task CreateUserIfNotExistsAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByNameAsync(user.UserName!);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // 分配角色
                    await userManager.AddToRoleAsync(user, role);
                    
                    Console.WriteLine($"创建用户成功: {user.UserName} ({user.Email}) - 角色: {role}");
                }
                else
                {
                    Console.WriteLine($"创建用户失败: {user.UserName}");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  错误: {error.Description}");
                    }
                }
            }
        }
    }
}