using System;
using System.Collections.Generic;
using System.Linq;
using AggregateRoot.Domain.Common;
using AggregateRoot.Domain.Orders.Entities;
using AggregateRoot.Domain.Orders.Enums;
using AggregateRoot.Domain.Orders.ValueObjects;
using Domain.Orders.Events;

namespace AggregateRoot.Domain.Orders
{
    public sealed class Order : AggregateRoot<Guid>
    {
        private readonly List<OrderItem> _items = new();

        public Guid CustomerId { get; private set; }
        public string OrderNumber { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money TotalAmount { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // 私有构造函数确保只能通过工厂方法创建
        private Order() : base() { }

        private Order(Guid customerId, string orderNumber, string createdBy) 
            : base(Guid.NewGuid())
        {
            CustomerId = customerId;
            OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
            Status = OrderStatus.Pending;
            TotalAmount = Money.Zero("USD");
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

            AddDomainEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount.Amount));
        }

        // 工厂方法
        public static Order Create(Guid customerId, string orderNumber, string createdBy)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Order number cannot be null or empty", nameof(orderNumber));

            return new Order(customerId, orderNumber, createdBy);
        }

        public void AddItem(ProductInfo product, Money unitPrice, int quantity)
        {
            ValidateOrderCanBeModified();

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (unitPrice == null)
                throw new ArgumentNullException(nameof(unitPrice));

            // 检查是否已存在相同产品
            var existingItem = _items.FirstOrDefault(x => x.Product.ProductId == product.ProductId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = new OrderItem(product, unitPrice, quantity);
                _items.Add(newItem);
            }

            RecalculateTotalAmount();
        }

        public void RemoveItem(Guid productId)
        {
            ValidateOrderCanBeModified();

            var item = _items.FirstOrDefault(x => x.Product.ProductId == productId);
            if (item == null)
                throw new InvalidOperationException($"Product {productId} not found in order");

            _items.Remove(item);
            RecalculateTotalAmount();
        }

        public void UpdateItemQuantity(Guid productId, int newQuantity)
        {
            ValidateOrderCanBeModified();

            var item = _items.FirstOrDefault(x => x.Product.ProductId == productId);
            if (item == null)
                throw new InvalidOperationException($"Product {productId} not found in order");

            if (newQuantity <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.UpdateQuantity(newQuantity);
                RecalculateTotalAmount();
            }
        }

        public void Confirm(string confirmedBy)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Order cannot be confirmed. Current status: {Status}");

            if (!_items.Any())
                throw new InvalidOperationException("Cannot confirm order without items");

            Status = OrderStatus.Confirmed;
            UpdateTimestamp(confirmedBy);

            AddDomainEvent(new OrderConfirmedEvent(Id));
        }

        public void Ship(string shippedBy)
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException($"Order cannot be shipped. Current status: {Status}");

            Status = OrderStatus.Shipped;
            UpdateTimestamp(shippedBy);
        }

        public void Deliver(string deliveredBy)
        {
            if (Status != OrderStatus.Shipped)
                throw new InvalidOperationException($"Order cannot be delivered. Current status: {Status}");

            Status = OrderStatus.Delivered;
            UpdateTimestamp(deliveredBy);
        }

        public void Cancel(string reason, string cancelledBy)
        {
            if (Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel delivered order");

            if (Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = OrderStatus.Cancelled;
            UpdateTimestamp(cancelledBy);

            AddDomainEvent(new OrderCancelledEvent(Id, reason));
        }

        private void ValidateOrderCanBeModified()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Order cannot be modified. Current status: {Status}");
        }

        private void RecalculateTotalAmount()
        {
            if (!_items.Any())
            {
                TotalAmount = Money.Zero("USD");
                return;
            }

            var total = _items.First().TotalPrice;
            foreach (var item in _items.Skip(1))
            {
                total = total.Add(item.TotalPrice);
            }

            TotalAmount = total;
        }
    }
}