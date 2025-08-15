using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using MokPermissions.Application.Extensions;
using MokPermissions.Domain;
using MokPermissions.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using MokPermissions.Domain.Store;
using MokPermissions.Domain.Manager;

namespace PermissionManagement.Tests.Integration
{
    [TestClass]
    public class MokPermissionIntegrationTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();

            // 添加权限管理
            services.AddPermissionManagement();

            // 添加权限授权
            services.AddPermissionAuthorization();

            // 添加内存权限存储
            services.AddSingleton<IPermissionStore, InMemoryPermissionStore>();

            // 添加系统权限提供者
            services.AddSingleton<IPermissionDefinitionProvider, TestPermissionDefinitionProvider>();

            // 用于测试的当前用户
            services.AddScoped<ICurrentUser>(sp => new TestCurrentUser());

            _serviceProvider = services.BuildServiceProvider();

            // 初始化权限
            var permissionManager = _serviceProvider.GetRequiredService<IPermissionManager>();
            permissionManager.GrantAsync("TestPermission", "R", "Admin").Wait();
            permissionManager.GrantAsync("TestPermission.Create", "R", "Admin").Wait();
            permissionManager.GrantAsync("TestPermission.View", "R", "User").Wait();
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForAdminRole_ShouldReturnTrue()
        {
            // Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "test"));

            var permissionChecker = _serviceProvider.GetRequiredService<IPermissionChecker>();

            // Act
            var result = await permissionChecker.IsGrantedAsync(user, "TestPermission");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForUserRole_ShouldReturnTrueForViewPermission()
        {
            // Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "2"),
                    new Claim(ClaimTypes.Role, "User")
                }, "test"));

            var permissionChecker = _serviceProvider.GetRequiredService<IPermissionChecker>();

            // Act
            var resultForView = await permissionChecker.IsGrantedAsync(user, "TestPermission.View");
            var resultForCreate = await permissionChecker.IsGrantedAsync(user, "TestPermission.Create");

            // Assert
            Assert.IsTrue(resultForView);
            Assert.IsFalse(resultForCreate);
        }

        [TestMethod]
        public async Task AuthorizationHandler_ShouldSucceedForAuthorizedUser()
        {
            // Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "test"));

            var authorizationService = _serviceProvider.GetRequiredService<IAuthorizationService>();
            var requirement = new PermissionRequirement("TestPermission");
            var authorizationPolicy = new AuthorizationPolicy(new[] { requirement }, new[] { "test" });

            // Act
            var result = await authorizationService.AuthorizeAsync(user, authorizationPolicy);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }
    }

    public class TestPermissionDefinitionProvider : IPermissionDefinitionProvider
    {
        public void Define(PermissionDefinitionContext context)
        {
            var testGroup = context.AddGroup("Test", "测试");

            var testPermission = testGroup.AddPermission(
                "TestPermission",
                "测试权限",
                "用于测试的权限");

            testPermission.AddChild("TestPermission.Create", "创建");
            testPermission.AddChild("TestPermission.Update", "更新");
            testPermission.AddChild("TestPermission.Delete", "删除");
            testPermission.AddChild("TestPermission.View", "查看");
        }
    }

    public class TestCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => true;

        public string Id => "1";

        public string UserName => "TestUser";

        public string[] Roles => new[] { "Admin" };

        public ClaimsPrincipal Principal => new ClaimsPrincipal(
            new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id),
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Role, "Admin")
            }, "test"));

        public string FindClaimValue(string claimType)
        {
            return Principal?.FindFirst(claimType)?.Value;
        }

        public string[] FindClaimValues(string claimType)
        {
            return Principal?.FindAll(claimType).Select(c => c.Value).ToArray() ?? Array.Empty<string>();
        }
    }
}
