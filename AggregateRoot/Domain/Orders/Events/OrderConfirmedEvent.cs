using System;
using AggregateRoot.Domain.Common;

namespace AggregateRoot.Domain.Orders.Events
{
    public sealed class OrderConfirmedEvent : DomainEvent
    {
        public Guid OrderId { get; }

        public OrderConfirmedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}