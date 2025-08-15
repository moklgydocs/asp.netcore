using Microsoft.VisualStudio.TestTools.UnitTesting;
using MokPermissions.Domain;

namespace MokPermissions.Tests
{
    [TestClass]
    public class PermissionDefinitionTests
    {
        [TestMethod]
        public void Create_WithValidName_ShouldSucceed()
        {
            // Arrange & Act
            var permission = new PermissionDefinition("TestPermission");

            // Assert
            Assert.AreEqual("TestPermission", permission.Name);
            Assert.AreEqual("TestPermission", permission.DisplayName);
            Assert.IsFalse(permission.IsGrantedByDefault);
            Assert.IsNull(permission.Description);
            Assert.IsNotNull(permission.Children);
            Assert.AreEqual(0, permission.Children.Count);
        }

        [TestMethod]
        public void Create_WithNullName_ShouldThrowException()
        {
            // Arrange, Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new PermissionDefinition(null));
        }

        [TestMethod]
        public void Create_WithAllParameters_ShouldSetProperties()
        {
            // Arrange & Act
            var permission = new PermissionDefinition(
                "TestPermission",
                "Test Permission",
                "This is a test permission",
                true);

            // Assert
            Assert.AreEqual("TestPermission", permission.Name);
            Assert.AreEqual("Test Permission", permission.DisplayName);
            Assert.AreEqual("This is a test permission", permission.Description);
            Assert.IsTrue(permission.IsGrantedByDefault);
        }

        [TestMethod]
        public void AddChild_ShouldAddChildPermission()
        {
            // Arrange
            var parent = new PermissionDefinition("Parent");

            // Act
            var child = parent.AddChild("Child");

            // Assert
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(child, parent.Children[0]);
            Assert.AreEqual(parent, child.Parent);
        }

        [TestMethod]
        public void GetFullName_WithNoParent_ShouldReturnName()
        {
            // Arrange
            var permission = new PermissionDefinition("TestPermission");

            // Act
            var fullName = permission.GetFullName();

            // Assert
            Assert.AreEqual("TestPermission", fullName);
        }

        [TestMethod]
        public void GetFullName_WithParent_ShouldReturnFullName()
        {
            // Arrange
            var parent = new PermissionDefinition("Parent");
            var child = parent.AddChild("Child");
            var grandChild = child.AddChild("GrandChild");

            // Act
            var fullName = grandChild.GetFullName();

            // Assert
            Assert.AreEqual("Parent.Child.GrandChild", fullName);
        }

    }
}
