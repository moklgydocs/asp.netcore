using System;
using System.Collections.Generic;
using System.Linq;
using AggregateRoot.Domain.Orders.Entities;
using DDD.Core.Domain;
using DDD.OrderManagement;
using OrderManagement.Domain.Events; 
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities
{
    /// <summary>
    /// 订单聚合根 - 电商领域的核心聚合
    /// </summary>
    public class Order : AggregateRoot<OrderId>
    {
        private readonly List<OrderItem> _items = new();

        /// <summary>
        /// 客户ID
        /// </summary>
        public CustomerId CustomerId { get; private set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public Money TotalAmount { get; private set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public Address ShippingAddress { get; private set; }

        /// <summary>
        /// 订单项列表
        /// </summary>
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderDate { get; private set; }

        // 私有构造函数 - 用于Entity Framework
        private Order() : base(default) { }

        /// <summary>
        /// 创建新订单
        /// </summary>
        public Order(OrderId id, CustomerId customerId, Address shippingAddress) 
            : base(id)
        {
            CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
            Status = OrderStatus.Draft;
            TotalAmount = Money.Zero("CNY");
            OrderDate = DateTime.UtcNow;

            // 发布订单创建事件
            ApplyEvent(new OrderCreatedEvent(Id, CustomerId, OrderDate));
        }

        /// <summary>
        /// 添加订单项
        /// </summary>
        public void AddItem(ProductId productId, string productName, Money unitPrice, int quantity)
        {
            // 检查业务规则
            CheckRule(new OrderMustBeDraftRule(Status), "只有草稿状态的订单才能添加商品");
            CheckRule(new QuantityMustBePositiveRule(quantity), "商品数量必须大于0");

            var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var item = new OrderItem(OrderItemId.New(), productId, productName, unitPrice, quantity);
                _items.Add(item);
            }

            RecalculateTotalAmount();
            MarkAsModified();

            ApplyEvent(new OrderItemAddedEvent(Id, productId, quantity));
        }

        /// <summary>
        /// 移除订单项
        /// </summary>
        public void RemoveItem(ProductId productId)
        {
            CheckRule(new OrderMustBeDraftRule(Status), "只有草稿状态的订单才能移除商品");

            var item = _items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                RecalculateTotalAmount();
                MarkAsModified();

                ApplyEvent(new OrderItemRemovedEvent(Id, productId));
            }
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        public void Confirm()
        {
            CheckRule(new OrderMustBeDraftRule(Status), "只有草稿状态的订单才能确认");
            CheckRule(new OrderMustHaveItemsRule(_items), "订单必须包含至少一个商品");

            Status = OrderStatus.Confirmed;
            MarkAsModified();

            ApplyEvent(new OrderConfirmedEvent(Id, TotalAmount, OrderDate));
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        public void Pay()
        {
            CheckRule(new OrderMustBeConfirmedRule(Status), "只有已确认的订单才能支付");

            Status = OrderStatus.Paid;
            MarkAsModified();

            ApplyEvent(new OrderPaidEvent(Id, TotalAmount, DateTime.UtcNow));
        }

        /// <summary>
        /// 发货
        /// </summary>
        public void Ship()
        {
            CheckRule(new OrderMustBePaidRule(Status), "只有已支付的订单才能发货");

            Status = OrderStatus.Shipped;
            MarkAsModified();

            ApplyEvent(new OrderShippedEvent(Id, ShippingAddress, DateTime.UtcNow));
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public void Cancel(string reason)
        {
            CheckRule(new OrderCanBeCancelledRule(Status), "该状态的订单不能取消");

            Status = OrderStatus.Cancelled;
            MarkAsModified();

            ApplyEvent(new OrderCancelledEvent(Id, reason, DateTime.UtcNow));
        }

        /// <summary>
        /// 完成订单
        /// </summary>
        public void Complete()
        {
            CheckRule(new OrderMustBeShippedRule(Status), "只有已发货的订单才能完成");

            Status = OrderStatus.Completed;
            MarkAsModified();

            ApplyEvent(new OrderCompletedEvent(Id, DateTime.UtcNow));
        }

        /// <summary>
        /// 重新计算订单总金额
        /// </summary>
        private void RecalculateTotalAmount()
        {
            var total = _items.Sum(item => item.TotalPrice.Amount);
            TotalAmount = new Money(total, TotalAmount.Currency);
        }
    }
}