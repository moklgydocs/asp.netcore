using System;
using AggregateRoot.Domain.Common;
using AggregateRoot.Domain.Orders.ValueObjects;

namespace AggregateRoot.Domain.Orders.Entities
{
    public sealed class OrderItem : Entity<Guid>
    {
        public ProductInfo Product { get; private set; }
        public Money UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public Money TotalPrice => UnitPrice.Multiply(Quantity);

        private OrderItem() : base() { } // For EF Core

        internal OrderItem(ProductInfo product, Money unitPrice, int quantity) 
            : base(Guid.NewGuid())
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            Product = product ?? throw new ArgumentNullException(nameof(product));
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            Quantity = quantity;
        }

        internal void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

            Quantity = newQuantity;
        }
    }
}