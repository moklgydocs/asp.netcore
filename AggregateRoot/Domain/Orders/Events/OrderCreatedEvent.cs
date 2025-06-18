using System;
using AggregateRoot.Domain.Common;

namespace AggregateRoot.Domain.Orders.Events
{
    public sealed class OrderCreatedEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public decimal TotalAmount { get; }

        public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount)
        {
            OrderId = orderId;
            CustomerId = customerId;
            TotalAmount = totalAmount;
        }
    }
}