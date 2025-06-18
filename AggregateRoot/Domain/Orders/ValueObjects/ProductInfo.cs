using System;
using System.Collections.Generic;
using AggregateRoot.Domain.Common;

namespace AggregateRoot.Domain.Orders.ValueObjects
{
    public sealed class ProductInfo : ValueObject
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Sku { get; }

        public ProductInfo(Guid productId, string name, string sku)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty", nameof(productId));
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

            ProductId = productId;
            Name = name;
            Sku = sku;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Name;
            yield return Sku;
        }
    }
}