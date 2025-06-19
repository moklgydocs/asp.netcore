using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog.Events
{
    /// <summary>
    /// 产品价格变更领域事件
    /// </summary>
    public class ProductPriceChangedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public string Currency { get; }

        public ProductPriceChangedEvent(Guid productId, string productName, decimal oldPrice, decimal newPrice, string currency)
        {
            ProductId = productId;
            ProductName = productName;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            Currency = currency;
        }
    }
}