using AggregateRoot.Domain.Orders;
using AggregateRoot.Domain.Orders.ValueObjects;
using Xunit;

namespace AggregateRoot.Tests.Domain.UnitTests
{
    public class OrderTests
    {
        [Fact]
        public void CreateOrder_WithValidData_ShouldSucceed()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var orderNumber = "ORD-001";
            var createdBy = "TestUser";

            // Act
            var order = Order.Create(customerId, orderNumber, createdBy);

            // Assert
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(orderNumber, order.OrderNumber);
            Assert.Single(order.DomainEvents);
        }

        [Fact]
        public void AddItem_WhenOrderIsPending_ShouldSucceed()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), "ORD-001", "TestUser");
            var product = new ProductInfo(Guid.NewGuid(), "Test Product", "SKU-001");
            var price = new Money(100, "USD");

            // Act
            order.AddItem(product, price, 2);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(new Money(200, "USD"), order.TotalAmount);
        }
    }
}