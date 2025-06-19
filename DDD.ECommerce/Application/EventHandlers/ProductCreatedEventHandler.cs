using System;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Domain.Catalog.Events;
using DDDCore.Domain;
using Microsoft.Extensions.Logging;

namespace DDD.ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 产品创建事件处理器
    /// </summary>
    public class ProductCreatedEventHandler : IDomainEventHandler<ProductCreatedEvent>
    {
        private readonly ILogger<ProductCreatedEventHandler> _logger;
        
        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task HandleAsync(ProductCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // 记录日志
            _logger.LogInformation(
                "Product created: {ProductId}, {ProductName}, Price: {Price} {Currency} at {OccurredOn}",
                domainEvent.ProductId,
                domainEvent.ProductName,
                domainEvent.Price,
                domainEvent.Currency,
                domainEvent.OccurredOn);
                
            // 可以在这里添加其他逻辑，如:
            // - 发送通知
            // - 更新搜索索引
            // - 触发集成事件
            // - 等等
                
            return Task.CompletedTask;
        }
    }
}