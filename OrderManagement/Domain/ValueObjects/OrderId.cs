using System;
using System.Collections.Generic;
using DDD.Core.Domain;

namespace OrderManagement.Domain.ValueObjects
{
    /// <summary>
    /// 订单ID值对象
    /// </summary>
    public class OrderId : ValueObject
    {
        public Guid Value { get; }

        public OrderId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("订单ID不能为空", nameof(value));
            
            Value = value;
        }

        public static OrderId New() => new(Guid.NewGuid());
        
        public static OrderId From(Guid value) => new(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();

        // 隐式转换
        public static implicit operator Guid(OrderId orderId) => orderId.Value;
        public static implicit operator OrderId(Guid value) => new(value);
    }
}