using MokPermissions.Domain.Shared;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MokPermissions.Domain.Store;
using MokPermissions.Domain.Manager;

namespace MokPermissions.Tests
{
    [TestClass]
    public class PermissionManagementCheckerTests
    {
        private Mock<IPermissionStore> _permissionStoreMock;
        private Mock<PermissionDefinitionManager> _permissionDefinitionManagerMock;
        private Mock<ICurrentUser> _currentUserMock;
        private PermissionChecker _permissionChecker;

        [TestInitialize]
        public void Initialize()
        {
            _permissionStoreMock = new Mock<IPermissionStore>();
            _permissionDefinitionManagerMock = new Mock<PermissionDefinitionManager>(
                new List<IPermissionDefinitionProvider>());
            _currentUserMock = new Mock<ICurrentUser>();

            _permissionChecker = new PermissionChecker(
                _permissionStoreMock.Object,
                _permissionDefinitionManagerMock.Object,
                _currentUserMock.Object);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForUnauthenticatedUser_ShouldReturnFalse()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = await _permissionChecker.IsGrantedAsync(claimsPrincipal, "TestPermission");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForDefaultGrantedPermission_ShouldReturnTrue()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") }, "test"));

            var permissionDefinition = new PermissionDefinition("TestPermission")
            {
                IsGrantedByDefault = true
            };

            _permissionDefinitionManagerMock
                .Setup(m => m.GetPermission("TestPermission"))
                .Returns(permissionDefinition);

            // Act
            var result = await _permissionChecker.IsGrantedAsync(claimsPrincipal, "TestPermission");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForUserWithGrantedPermission_ShouldReturnTrue()
        {
            // Arrange
            var userId = "1";
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) }, "test"));

            var permissionDefinition = new PermissionDefinition("TestPermission");

            _permissionDefinitionManagerMock
                .Setup(m => m.GetPermission("TestPermission"))
                .Returns(permissionDefinition);

            _permissionStoreMock
                .Setup(m => m.IsGrantedAsync("TestPermission", "U", userId))
                .ReturnsAsync(PermissionGrantStatus.Granted);

            // Act
            var result = await _permissionChecker.IsGrantedAsync(claimsPrincipal, "TestPermission");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForUserWithProhibitedPermission_ShouldReturnFalse()
        {
            // Arrange
            var userId = "1";
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) }, "test"));

            var permissionDefinition = new PermissionDefinition("TestPermission");

            _permissionDefinitionManagerMock
                .Setup(m => m.GetPermission("TestPermission"))
                .Returns(permissionDefinition);

            _permissionStoreMock
                .Setup(m => m.IsGrantedAsync("TestPermission", "U", userId))
                .ReturnsAsync(PermissionGrantStatus.Prohibited);

            // Act
            var result = await _permissionChecker.IsGrantedAsync(claimsPrincipal, "TestPermission");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForUserWithRolePermission_ShouldReturnTrue()
        {
            // Arrange
            var userId = "1";
            var roleName = "Admin";
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, roleName)
                }, "test"));

            var permissionDefinition = new PermissionDefinition("TestPermission");

            _permissionDefinitionManagerMock
                .Setup(m => m.GetPermission("TestPermission"))
                .Returns(permissionDefinition);

            _permissionStoreMock
                .Setup(m => m.IsGrantedAsync("TestPermission", "U", userId))
                .ReturnsAsync(PermissionGrantStatus.Undefined);

            _permissionStoreMock
                .Setup(m => m.IsGrantedAsync("TestPermission", "R", roleName))
                .ReturnsAsync(PermissionGrantStatus.Granted);

            // Act
            var result = await _permissionChecker.IsGrantedAsync(claimsPrincipal, "TestPermission");

            // Assert
            Assert.IsTrue(result);
        }
    }
}
