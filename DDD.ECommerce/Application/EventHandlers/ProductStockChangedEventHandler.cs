using System;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Domain.Catalog.Events;
using DDDCore.Domain;
using Microsoft.Extensions.Logging;

namespace DDD.ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 产品库存变更事件处理器
    /// </summary>
    public class ProductStockChangedEventHandler : IDomainEventHandler<ProductStockChangedEvent>
    {
        private readonly ILogger<ProductStockChangedEventHandler> _logger;
        
        public ProductStockChangedEventHandler(ILogger<ProductStockChangedEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task HandleAsync(ProductStockChangedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // 记录日志
            _logger.LogInformation(
                "Product stock changed: {ProductId}, {ProductName}, Current Stock: {CurrentStock}, Changed By: {QuantityChanged} at {OccurredOn}",
                domainEvent.ProductId,
                domainEvent.ProductName,
                domainEvent.CurrentStock,
                domainEvent.QuantityChanged,
                domainEvent.OccurredOn);
                
            // 如果库存过低，可以发送提醒
            if (domainEvent.CurrentStock <= 5 && domainEvent.QuantityChanged < 0)
            {
                _logger.LogWarning(
                    "Low stock alert! Product {ProductId}, {ProductName} has only {CurrentStock} items left.",
                    domainEvent.ProductId,
                    domainEvent.ProductName,
                    domainEvent.CurrentStock);
                    
                // 在实际应用中，这里可以:
                // - 发送电子邮件给库存管理员
                // - 自动生成采购订单
                // - 推送通知到仪表板
                // - 等等
            }
                
            return Task.CompletedTask;
        }
    }
}