using MediatR;
using System;
using System.Collections.Generic;

namespace AggregateRoot.Application.Orders.Commands
{
    public sealed class CreateOrderCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; }
    }

    public sealed class CreateOrderItemRequest
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public int Quantity { get; set; }
    }
}