using Microsoft.AspNetCore.Identity;
using AuthServer.Models;
using AuthServer.Services;

namespace AuthServer.Data
{
    /// <summary>
    /// SSO系统种子数据初始化
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// 初始化种子数据
        /// </summary>
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IClientService clientService,
            ILogger logger)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();

                // 创建角色
                await CreateRolesAsync(roleManager, logger);

                // 创建用户
                await CreateUsersAsync(userManager, logger);

                // 创建SSO客户端应用
                await CreateSSOClientsAsync(clientService, logger);

                logger.LogInformation("SSO系统种子数据初始化完成");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SSO系统种子数据初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 创建系统角色
        /// </summary>
        private static async Task CreateRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "SuperAdmin",
                        DisplayName = "超级管理员",
                        Description = "系统超级管理员，拥有所有权限",
                        Category = "系统管理",
                        IsSystemRole = true
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "SystemAdmin",
                        DisplayName = "系统管理员",
                        Description = "系统管理员，负责系统配置和用户管理",
                        Category = "系统管理",
                        IsSystemRole = true
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "UserAdmin",
                        DisplayName = "用户管理员",
                        Description = "用户管理员，负责用户账户管理",
                        Category = "用户管理",
                        IsSystemRole = false
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "HRManager",
                        DisplayName = "人事经理",
                        Description = "人力资源部门经理",
                        Category = "业务管理",
                        IsSystemRole = false
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "HRStaff",
                        DisplayName = "人事专员",
                        Description = "人力资源部门专员",
                        Category = "业务管理",
                        IsSystemRole = false
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "FinanceManager",
                        DisplayName = "财务经理",
                        Description = "财务部门经理",
                        Category = "业务管理",
                        IsSystemRole = false
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "FinanceStaff",
                        DisplayName = "财务专员",
                        Description = "财务部门专员",
                        Category = "业务管理",
                        IsSystemRole = false
                    }
                },
                new RoleCreationInfo
                {
                    Role = new ApplicationRole
                    {
                        Name = "Employee",
                        DisplayName = "普通员工",
                        Description = "公司普通员工",
                        Category = "基础角色",
                        IsSystemRole = false
                    }
                }
            };

            foreach (var roleInfo in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleInfo.Role.Name!))
                {
                    var result = await roleManager.CreateAsync(roleInfo.Role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("创建角色成功: {RoleName}", roleInfo.Role.Name);
                    }
                    else
                    {
                        logger.LogWarning("创建角色失败: {RoleName}, 错误: {Errors}",
                            roleInfo.Role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
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
                        EmployeeId = "SA001",
                        ChineseName = "超级管理员",
                        EnglishName = "Super Admin",
                        DisplayName = "超级管理员",
                        DepartmentName = "信息技术部",
                        Position = "系统架构师",
                        Level = "P10",
                        HireDate = new DateTime(2020, 1, 1),
                        Status = EmployeeStatus.Active,
                        WorkLocation = "总部大厦A座",
                        EmailConfirmed = true
                    },
                    Password = "SuperAdmin@2024!",
                    Roles = new[] { "SuperAdmin", "SystemAdmin" },
                    Permissions = new[] { "system.full", "user.manage", "app.manage", "audit.view", "settings.manage" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@company.com",
                        EmployeeId = "AD001",
                        ChineseName = "张三",
                        EnglishName = "Zhang San",
                        DisplayName = "张三",
                        DepartmentName = "信息技术部",
                        Position = "系统管理员",
                        Level = "P8",
                        HireDate = new DateTime(2021, 3, 15),
                        Status = EmployeeStatus.Active,
                        WorkLocation = "总部大厦A座",
                        EmailConfirmed = true
                    },
                    Password = "Admin@2024!",
                    Roles = new[] { "SystemAdmin" },
                    Permissions = new[] { "user.manage", "system.config", "audit.view" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "hrmanager",
                        Email = "hrmanager@company.com",
                        EmployeeId = "HR001",
                        ChineseName = "李四",
                        EnglishName = "Li Si",
                        DisplayName = "李四",
                        DepartmentName = "人力资源部",
                        Position = "人事经理",
                        Level = "M2",
                        HireDate = new DateTime(2019, 6, 1),
                        Status = EmployeeStatus.Active,
                        WorkLocation = "总部大厦B座",
                        EmailConfirmed = true
                    },
                    Password = "HRManager@2024!",
                    Roles = new[] { "HRManager" },
                    Permissions = new[] { "hr.manage", "employee.view", "report.generate" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "financemanager",
                        Email = "financemanager@company.com",
                        EmployeeId = "FN001",
                        ChineseName = "王五",
                        EnglishName = "Wang Wu",
                        DisplayName = "王五",
                        DepartmentName = "财务部",
                        Position = "财务经理",
                        Level = "M2",
                        HireDate = new DateTime(2018, 9, 1),
                        Status = EmployeeStatus.Active,
                        WorkLocation = "总部大厦C座",
                        EmailConfirmed = true
                    },
                    Password = "FinanceManager@2024!",
                    Roles = new[] { "FinanceManager" },
                    Permissions = new[] { "finance.manage", "budget.approve", "report.generate" }
                },
                new UserCreationInfo
                {
                    User = new ApplicationUser
                    {
                        UserName = "employee1",
                        Email = "employee1@company.com",
                        EmployeeId = "EMP001",
                        ChineseName = "赵六",
                        EnglishName = "Zhao Liu",
                        DisplayName = "赵六",
                        DepartmentName = "市场部",
                        Position = "市场专员",
                        Level = "P3",
                        HireDate = new DateTime(2022, 8, 15),
                        Status = EmployeeStatus.Active,
                        WorkLocation = "总部大厦D座",
                        EmailConfirmed = true
                    },
                    Password = "Employee@2024!",
                    Roles = new[] { "Employee" },
                    Permissions = new[] { "profile.edit", "portal.access" }
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

                    logger.LogInformation("创建用户成功: {Username} ({ChineseName}) - 角色: {Roles}",
                        userInfo.User.UserName, userInfo.User.ChineseName, string.Join(", ", userInfo.Roles));
                }
                else
                {
                    logger.LogWarning("创建用户失败: {Username}, 错误: {Errors}",
                        userInfo.User.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        /// <summary>
        /// 创建SSO客户端应用
        /// </summary>
        private static async Task CreateSSOClientsAsync(IClientService clientService, ILogger logger)
        {
            var clients = new[]
            {
                // 企业门户
                new ClientDefinition
                {
                    ClientId = "portal-web",
                    ClientSecret = "portal-web-secret-2024-sso",
                    DisplayName = "企业门户网站",
                    Description = "公司内部门户网站，员工信息查询和公告浏览",
                    GrantTypes = new[] { "authorization_code", "refresh_token" },
                    Scopes = new[] { "openid", "profile", "email", "roles", "portal", "user.read" },
                    RedirectUris = new[] { 
                        "https://localhost:7010/signin-oidc",
                        "https://localhost:7010/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "https://localhost:7010/signout-callback-oidc",
                        "https://localhost:7010/" 
                    },
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 人力资源系统
                new ClientDefinition
                {
                    ClientId = "hr-system",
                    ClientSecret = "hr-system-secret-2024-sso",
                    DisplayName = "人力资源管理系统",
                    Description = "HR系统，员工管理、考勤、薪资等",
                    GrantTypes = new[] { "authorization_code", "refresh_token" },
                    Scopes = new[] { "openid", "profile", "email", "roles", "hr", "user.read", "user.write" },
                    RedirectUris = new[] { 
                        "https://localhost:7011/signin-oidc",
                        "https://localhost:7011/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "https://localhost:7011/signout-callback-oidc",
                        "https://localhost:7011/" 
                    },
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 财务系统
                new ClientDefinition
                {
                    ClientId = "finance-system",
                    ClientSecret = "finance-system-secret-2024-sso",
                    DisplayName = "财务管理系统",
                    Description = "财务系统，预算管理、报销、财务报表等",
                    GrantTypes = new[] { "authorization_code", "refresh_token" },
                    Scopes = new[] { "openid", "profile", "email", "roles", "finance", "user.read" },
                    RedirectUris = new[] { 
                        "https://localhost:7012/signin-oidc",
                        "https://localhost:7012/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "https://localhost:7012/signout-callback-oidc",
                        "https://localhost:7012/" 
                    },
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 移动端API
                new ClientDefinition
                {
                    ClientId = "mobile-app",
                    ClientSecret = null, // 移动应用是公共客户端
                    DisplayName = "企业移动应用",
                    Description = "企业移动端应用，支持iOS和Android",
                    GrantTypes = new[] { "authorization_code", "refresh_token", "password" },
                    Scopes = new[] { "openid", "profile", "email", "roles", "mobile", "user.read", "offline_access" },
                    RedirectUris = new[] { 
                        "com.company.mobile://callback",
                        "https://localhost:7013/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "com.company.mobile://logout",
                        "https://localhost:7013/logout" 
                    },
                    RequireConsent = false,
                    RequirePkce = true
                },

                // 管理后台
                new ClientDefinition
                {
                    ClientId = "admin-panel",
                    ClientSecret = "admin-panel-secret-2024-sso",
                    DisplayName = "系统管理后台",
                    Description = "系统管理后台，用户管理、系统配置等",
                    GrantTypes = new[] { "authorization_code", "refresh_token" },
                    Scopes = new[] { "openid", "profile", "email", "roles", "admin", "system.admin", "user.write" },
                    RedirectUris = new[] { 
                        "https://localhost:7014/signin-oidc",
                        "https://localhost:7014/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "https://localhost:7014/signout-callback-oidc",
                        "https://localhost:7014/" 
                    },
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 服务间通信客户端
                new ClientDefinition
                {
                    ClientId = "service-client",
                    ClientSecret = "service-client-secret-2024-sso",
                    DisplayName = "服务间通信客户端",
                    Description = "用于后台服务之间的API调用",
                    GrantTypes = new[] { "client_credentials" },
                    Scopes = new[] { "portal", "hr", "finance", "user.read", "user.write" },
                    RedirectUris = Array.Empty<string>(),
                    PostLogoutRedirectUris = Array.Empty<string>(),
                    RequireConsent = false,
                    RequirePkce = false
                }
            };

            foreach (var client in clients)
            {
                await clientService.CreateOrUpdateClientAsync(client.ClientId, client);
                logger.LogInformation("创建SSO客户端成功: {ClientId} - {DisplayName}", 
                    client.ClientId, client.DisplayName);
            }
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

        /// <summary>
        /// 角色创建信息
        /// </summary>
        private class RoleCreationInfo
        {
            public ApplicationRole Role { get; set; } = null!;
        }
    }
}