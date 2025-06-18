using System;
using System.Threading;
using System.Threading.Tasks;
using AggregateRoot.Domain;
using AggregateRoot.Domain.Orders;
using AggregateRoot.Domain.Orders.ValueObjects;
using MediatR;

namespace AggregateRoot.Application.Orders.Commands
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 生成订单号
            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

            // 创建聚合根
            var order = Order.Create(
                request.CustomerId,
                orderNumber,
                "System"); // 实际应用中应该是当前用户

            // 添加订单项
            foreach (var item in request.Items)
            {
                var product = new ProductInfo(item.ProductId, item.ProductName, item.ProductSku);
                var unitPrice = new Money(item.UnitPrice, item.Currency);

                order.AddItem(product, unitPrice, item.Quantity);
            }

            // 保存聚合
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}