using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog.Events
{
    /// <summary>
    /// 产品更新领域事件
    /// </summary>
    public class ProductUpdatedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }

        public ProductUpdatedEvent(Guid productId, string productName)
        {
            ProductId = productId;
            ProductName = productName;
        }
    }
}