using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog.Events
{
    /// <summary>
    /// 产品库存变更领域事件
    /// </summary>
    public class ProductStockChangedEvent : DomainEvent
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public int CurrentStock { get; }
        public int QuantityChanged { get; }

        public ProductStockChangedEvent(Guid productId, string productName, int currentStock, int quantityChanged)
        {
            ProductId = productId;
            ProductName = productName;
            CurrentStock = currentStock;
            QuantityChanged = quantityChanged;
        }
    }
}