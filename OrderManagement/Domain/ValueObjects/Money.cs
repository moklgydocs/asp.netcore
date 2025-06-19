using System;
using System.Collections.Generic;
using DDD.Core.Domain;

namespace OrderManagement.Domain.ValueObjects
{
    /// <summary>
    /// 金钱值对象 - 封装金额和货币
    /// </summary>
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("金额不能为负数", nameof(amount));
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("货币代码不能为空", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpper();
        }

        public static Money Zero(string currency) => new(0, currency);

        /// <summary>
        /// 加法运算 - 相同货币才能相加
        /// </summary>
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("不同货币不能进行运算");

            return new Money(Amount + other.Amount, Currency);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("不同货币不能进行运算");

            return new Money(Amount - other.Amount, Currency);
        }

        /// <summary>
        /// 乘法运算
        /// </summary>
        public Money Multiply(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        /// <summary>
        /// 除法运算
        /// </summary>
        public Money Divide(decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("除数不能为零");

            return new Money(Amount / divisor, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Amount:F2} {Currency}";

        // 操作符重载
        public static Money operator +(Money left, Money right) => left.Add(right);
        public static Money operator -(Money left, Money right) => left.Subtract(right);
        public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
        public static Money operator /(Money money, decimal divisor) => money.Divide(divisor);

        public static bool operator >(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("不同货币不能比较");
            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("不同货币不能比较");
            return left.Amount < right.Amount;
        }

        public static bool operator >=(Money left, Money right) => !(left < right);
        public static bool operator <=(Money left, Money right) => !(left > right);
    }
}