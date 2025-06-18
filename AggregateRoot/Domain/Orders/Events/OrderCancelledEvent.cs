using System;
using AggregateRoot.Domain.Common;

namespace AggregateRoot.Domain.Orders.Events
{
    public sealed class OrderCancelledEvent : DomainEvent
    {
        public Guid OrderId { get; }
        public string Reason { get; }

        public OrderCancelledEvent(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }
    }
}