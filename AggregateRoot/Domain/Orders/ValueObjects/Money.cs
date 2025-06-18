using System;
using System.Collections.Generic;
using AggregateRoot.Domain.Common;

namespace AggregateRoot.Domain.Orders.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(int multiplier)
        {
            return new Money(Amount * multiplier, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public static Money Zero(string currency) => new(0, currency);

        public override string ToString() => $"{Amount:F2} {Currency}";
    }
}