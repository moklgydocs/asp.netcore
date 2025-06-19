using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog.Events
{
    /// <summary>
    /// 产品创建领域事件
    /// </summary>
    public class ProductCreatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public decimal Price { get; }
        public string Currency { get; }

        public ProductCreatedEvent(Guid productId, string productName, decimal price, string currency)
        {
            ProductId = productId;
            ProductName = productName;
            Price = price;
            Currency = currency;
        }
    }
}