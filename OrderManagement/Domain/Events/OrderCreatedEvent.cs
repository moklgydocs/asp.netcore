using System;
using DDD.Core.Domain;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Events
{
    /// <summary>
    /// 订单创建事件
    /// </summary>
    public class OrderCreatedEvent : DomainEvent
    {
        public OrderId OrderId { get; }
        public CustomerId CustomerId { get; }
        public DateTime OrderDate { get; }

        public OrderCreatedEvent(OrderId orderId, CustomerId customerId, DateTime orderDate)
        {
            OrderId = orderId;
            CustomerId = customerId;
            OrderDate = orderDate;
        }
    }

    /// <summary>
    /// 订单确认事件
    /// </summary>
    public class OrderConfirmedEvent : DomainEvent
    {
        public OrderId OrderId { get; }
        public Money TotalAmount { get; }
        public DateTime ConfirmedAt { get; }

        public OrderConfirmedEvent(OrderId orderId, Money totalAmount, DateTime confirmedAt)
        {
            OrderId = orderId;
            TotalAmount = totalAmount;
            ConfirmedAt = confirmedAt;
        }
    }

    /// <summary>
    /// 订单支付事件
    /// </summary>
    public class OrderPaidEvent : DomainEvent
    {
        public OrderId OrderId { get; }
        public Money Amount { get; }
        public DateTime PaidAt { get; }

        public OrderPaidEvent(OrderId orderId, Money amount, DateTime paidAt)
        {
            OrderId = orderId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}