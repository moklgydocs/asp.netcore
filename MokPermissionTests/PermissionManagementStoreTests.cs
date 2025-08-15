using MokPermissions.Domain.Shared;
using MokPermissions.Domain.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Tests
{
    [TestClass]
    public class PermissionManagementStoreTests
    {

        private InMemoryPermissionStore _permissionStore;

        [TestInitialize]
        public void Initialize()
        {
            _permissionStore = new InMemoryPermissionStore();
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForNonExistingPermission_ShouldReturnUndefined()
        {
            // Arrange
            var permissionName = "TestPermission";
            var providerName = "R";
            var providerKey = "Admin";

            // Act
            var result = await _permissionStore.IsGrantedAsync(permissionName, providerName, providerKey);

            // Assert
            Assert.AreEqual(PermissionGrantStatus.Undefined, result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForGrantedPermission_ShouldReturnGranted()
        {
            // Arrange
            var permissionName = "TestPermission";
            var providerName = "R";
            var providerKey = "Admin";

            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, true);

            // Act
            var result = await _permissionStore.IsGrantedAsync(permissionName, providerName, providerKey);

            // Assert
            Assert.AreEqual(PermissionGrantStatus.Granted, result);
        }

        [TestMethod]
        public async Task IsGrantedAsync_ForProhibitedPermission_ShouldReturnProhibited()
        {
            // Arrange
            var permissionName = "TestPermission";
            var providerName = "R";
            var providerKey = "Admin";

            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, false);

            // Act
            var result = await _permissionStore.IsGrantedAsync(permissionName, providerName, providerKey);

            // Assert
            Assert.AreEqual(PermissionGrantStatus.Prohibited, result);
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllPermissionsForProvider()
        {
            // Arrange
            var providerName = "R";
            var providerKey = "Admin";

            await _permissionStore.SaveAsync("Permission1", providerName, providerKey, true);
            await _permissionStore.SaveAsync("Permission2", providerName, providerKey, false);
            await _permissionStore.SaveAsync("Permission3", "U", "User1", true);

            // Act
            var permissions = await _permissionStore.GetAllAsync(providerName, providerKey);

            // Assert
            Assert.AreEqual(2, permissions.Count);
            Assert.IsTrue(permissions.Any(p => p.Name == "Permission1" && p.IsGranted));
            Assert.IsTrue(permissions.Any(p => p.Name == "Permission2" && !p.IsGranted));
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemovePermission()
        {
            // Arrange
            var permissionName = "TestPermission";
            var providerName = "R";
            var providerKey = "Admin";

            await _permissionStore.SaveAsync(permissionName, providerName, providerKey, true);

            // Act
            await _permissionStore.DeleteAsync(permissionName, providerName, providerKey);
            var result = await _permissionStore.IsGrantedAsync(permissionName, providerName, providerKey);

            // Assert
            Assert.AreEqual(PermissionGrantStatus.Undefined, result);
        }
    }

}
