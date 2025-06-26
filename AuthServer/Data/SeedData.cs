using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using AuthServer.Models;
using AuthServer.Services;

namespace AuthServer.Data
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
            RoleManager<IdentityRole> roleManager,
            IClientService clientService,
            ILogger logger)
        {
            try
            {
                // 确保数据库已创建
                await context.Database.EnsureCreatedAsync();

                // 创建角色
                await CreateRolesAsync(roleManager, logger);

                // 创建用户
                await CreateUsersAsync(userManager, logger);

                // 创建OpenIddict客户端和应用程序
                await CreateClientsAsync(clientService, logger);

                logger.LogInformation("数据库种子数据初始化完成");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "数据库种子数据初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 创建系统角色
        /// </summary>
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                "SuperAdmin",   // 超级管理员
                "Admin",        // 系统管理员
                "Manager",      // 部门经理
                "User",         // 普通用户
                "Guest",        // 访客
                "Developer",    // 开发者
                "Tester",       // 测试员
                "Analyst",      // 分析师
                "Support"       // 技术支持
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    
                    if (result.Succeeded)
                    {
                        logger.LogInformation("创建角色成功: {RoleName}", roleName);
                    }
                    else
                    {
                        logger.LogWarning("创建角色失败: {RoleName}, 错误: {Errors}", 
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        /// <summary>
        /// 创建系统用户
        /// </summary>
        private static async Task CreateUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var users = new[]
            {
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "superadmin",
                        Email = "superadmin@company.com",
                        DisplayName = "超级管理员",
                        FirstName = "Super",
                        LastName = "Admin",
                        Department = "IT部门",
                        JobTitle = "系统架构师",
                        EmployeeId = "EMP001",
                        HireDate = DateTime.UtcNow.AddYears(-3),
                        EmailConfirmed = true,
                        IsActive = true,
                        CanDelete = false
                    },
                    Password = "SuperAdmin123!",
                    Roles = new[] { "SuperAdmin", "Admin" },
                    Permissions = new[] { "system.full", "user.manage", "client.manage", "audit.view", "settings.manage" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@company.com",
                        DisplayName = "系统管理员",
                        FirstName = "System",
                        LastName = "Administrator",
                        Department = "IT部门",
                        JobTitle = "系统管理员",
                        EmployeeId = "EMP002",
                        HireDate = DateTime.UtcNow.AddYears(-2),
                        EmailConfirmed = true,
                        IsActive = true
                    },
                    Password = "Admin123!",
                    Roles = new[] { "Admin" },
                    Permissions = new[] { "user.manage", "api.full", "data.read", "data.write" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "manager",
                        Email = "manager@company.com",
                        DisplayName = "部门经理",
                        FirstName = "John",
                        LastName = "Manager",
                        Department = "销售部门",
                        JobTitle = "销售经理",
                        EmployeeId = "EMP003",
                        HireDate = DateTime.UtcNow.AddYears(-1),
                        EmailConfirmed = true,
                        IsActive = true
                    },
                    Password = "Manager123!",
                    Roles = new[] { "Manager" },
                    Permissions = new[] { "team.manage", "report.view", "data.read", "api.read" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "developer",
                        Email = "developer@company.com",
                        DisplayName = "开发工程师",
                        FirstName = "Alice",
                        LastName = "Developer",
                        Department = "技术部门",
                        JobTitle = "高级开发工程师",
                        EmployeeId = "EMP004",
                        HireDate = DateTime.UtcNow.AddMonths(-6),
                        EmailConfirmed = true,
                        IsActive = true
                    },
                    Password = "Developer123!",
                    Roles = new[] { "Developer" },
                    Permissions = new[] { "code.write", "debug.access", "api.test", "data.read", "data.write" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "user",
                        Email = "user@company.com",
                        DisplayName = "普通用户",
                        FirstName = "Jane",
                        LastName = "User",
                        Department = "市场部门",
                        JobTitle = "市场专员",
                        EmployeeId = "EMP005",
                        HireDate = DateTime.UtcNow.AddMonths(-3),
                        EmailConfirmed = true,
                        IsActive = true
                    },
                    Password = "User123!",
                    Roles = new[] { "User" },
                    Permissions = new[] { "data.read", "profile.edit" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "tester",
                        Email = "tester@company.com",
                        DisplayName = "测试工程师",
                        FirstName = "Bob",
                        LastName = "Tester",
                        Department = "质量部门",
                        JobTitle = "测试工程师",
                        EmployeeId = "EMP006",
                        HireDate = DateTime.UtcNow.AddMonths(-4),
                        EmailConfirmed = true,
                        IsActive = true
                    },
                    Password = "Tester123!",
                    Roles = new[] { "Tester" },
                    Permissions = new[] { "test.execute", "bug.report", "data.read", "api.test" }
                }
            };

            foreach (var userInfo in users)
            {
                await CreateUserIfNotExistsAsync(userManager, userInfo, logger);
            }
        }

        /// <summary>
        /// 创建用户（如果不存在）
        /// </summary>
        private static async Task CreateUserIfNotExistsAsync(
            UserManager<ApplicationUser> userManager,
            UserCreationInfo userInfo,
            ILogger logger)
        {
            var existingUser = await userManager.FindByNameAsync(userInfo.User.UserName!);
            if (existingUser == null)
            {
                // 设置权限
                userInfo.User.SetPermissions(userInfo.Permissions);

                var result = await userManager.CreateAsync(userInfo.User, userInfo.Password);
                if (result.Succeeded)
                {
                    // 分配角色
                    foreach (var role in userInfo.Roles)
                    {
                        await userManager.AddToRoleAsync(userInfo.User, role);
                    }

                    logger.LogInformation("创建用户成功: {Username} ({Email}) - 角色: {Roles}",
                        userInfo.User.UserName, userInfo.User.Email, string.Join(", ", userInfo.Roles));
                }
                else
                {
                    logger.LogWarning("创建用户失败: {Username}, 错误: {Errors}",
                        userInfo.User.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("用户已存在: {Username}", userInfo.User.UserName);
            }
        }

        /// <summary>
        /// 创建OpenIddict客户端
        /// </summary>
        private static async Task CreateClientsAsync(IClientService clientService, ILogger logger)
        {
            await clientService.CreateClientsAsync();
            logger.LogInformation("OpenIddict客户端创建完成");
        }

        /// <summary>
        /// 用户创建信息
        /// </summary>
        private class UserCreationInfo
        {
            public ApplicationUser User { get; set; } = null!;
            public string Password { get; set; } = string.Empty;
            public string[] Roles { get; set; } = Array.Empty<string>();
            public string[] Permissions { get; set; } = Array.Empty<string>();
        }
    }
}