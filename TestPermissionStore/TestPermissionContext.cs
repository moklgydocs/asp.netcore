using MokPermissions.Domain;

namespace TestPermissionStore
{
    [TestClass]
    public sealed class TestPermissionContext
    {
        [TestMethod]
        public void Test_Permission_Context()
        {
            // Arrange
            var context = new PermissionDefinitionContext();
            // Act
            context.AddGroup("TestGroup", "测试组")
                .AddPermission("TestPermission", "测试权限", "测试权限描述")
                .AddChild("TestPermission.Child1", "子权限1")
                .AddChild("TestPermission.Child2", "子权限2");
            // Assert
            var group = context.GetGroup("TestGroup");
            Assert.IsNotNull(group);
            Assert.AreEqual("测试组", group.DisplayName);
            var permission = group.GetPermission("TestPermission");
            Assert.IsNotNull(permission);
            Assert.AreEqual("测试权限", permission.DisplayName);
            Assert.AreEqual("测试权限描述", permission.Description);
            var child1 = permission.Children.FirstOrDefault(x=>x.Name=="TestPermission.Child1");
            Assert.IsNotNull(child1);
            Assert.AreEqual("子权限1", child1.DisplayName);
            var child2 = child1.Children.FirstOrDefault(x => x.Name == "TestPermission.Child2");
            Assert.IsNotNull(child2);
            Assert.AreEqual("子权限2", child2.DisplayName);
        }
    }
}
