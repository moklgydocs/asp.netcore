using System;
using System.Collections.Generic;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 金额值对象
    /// 表示货币金额，包含金额和货币类型
    /// </summary>
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }
        
        // 货币代码验证规则(ISO 4217)
        private static readonly HashSet<string> ValidCurrencies = new HashSet<string>
        {
            "USD", "EUR", "CNY", "GBP", "JPY", "CAD", "AUD"
            // 可以添加更多支持的货币
        };
        
        private Money() { }

        /// <summary>
        /// 创建金额值对象
        /// </summary>
        /// <param name="amount">金额数值</param>
        /// <param name="currency">货币代码(ISO 4217)</param>
        /// <exception cref="ArgumentException">如果金额或货币不符合业务规则</exception>
        public static Money Create(decimal amount, string currency)
        {
            // 验证金额
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));
                
            // 验证货币代码
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency code cannot be empty.", nameof(currency));
                
            string normalizedCurrency = currency.Trim().ToUpperInvariant();
            
            if (!ValidCurrencies.Contains(normalizedCurrency))
                throw new ArgumentException($"Currency '{normalizedCurrency}' is not supported.", nameof(currency));
                
            // 创建并返回实例
            return new Money
            {
                //Amount = Math.Round(amount, 2), // 保留两位小数
                //Currency = normalizedCurrency
            };
        }
        
        /// <summary>
        /// 零金额工厂方法
        /// </summary>
        public static Money Zero(string currency)
        {
            return Create(0, currency);
        }

        /// <summary>
        /// 加法运算
        /// </summary>
        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return Create(Amount + other.Amount, Currency);
        }

        /// <summary>
        /// 减法运算
        /// </summary>
        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return Create(Amount - other.Amount, Currency);
        }

        /// <summary>
        /// 乘以系数
        /// </summary>
        public Money Multiply(decimal factor)
        {
            return Create(Amount * factor, Currency);
        }

        /// <summary>
        /// 确保两个金额的货币类型相同
        /// </summary>
        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot perform operations on money with different currencies: {Currency} and {other.Currency}");
        }
        
        /// <summary>
        /// 实现ValueObject的抽象方法，提供用于比较的值
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        public override ValueObject Clone()
        {
            return Create(Amount, Currency);
        }
        
        /// <summary>
        /// 提供字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}