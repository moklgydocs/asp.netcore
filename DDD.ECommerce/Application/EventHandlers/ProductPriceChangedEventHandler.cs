using System;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Domain.Catalog.Events;
using DDDCore.Domain;
using Microsoft.Extensions.Logging;

namespace DDD.ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 产品价格变更事件处理器
    /// </summary>
    public class ProductPriceChangedEventHandler : IDomainEventHandler<ProductPriceChangedEvent>
    {
        private readonly ILogger<ProductPriceChangedEventHandler> _logger;
        
        public ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task HandleAsync(ProductPriceChangedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // 记录价格变动日志
            _logger.LogInformation(
                "Product price changed: {ProductId}, {ProductName}, Old Price: {OldPrice}, New Price: {NewPrice} {Currency} at {OccurredOn}",
                domainEvent.ProductId,
                domainEvent.ProductName,
                domainEvent.OldPrice,
                domainEvent.NewPrice,
                domainEvent.Currency,
                domainEvent.OccurredOn);
                
            // 计算价格变动百分比
            decimal changePercent = 0;
            if (domainEvent.OldPrice > 0)
            {
                changePercent = (domainEvent.NewPrice - domainEvent.OldPrice) / domainEvent.OldPrice * 100;
            }
            
            // 如果价格大幅变动，记录警告
            if (Math.Abs(changePercent) > 20)
            {
                _logger.LogWarning(
                    "Significant price change detected! Product {ProductId}, {ProductName} price changed by {ChangePercent:F2}%",
                    domainEvent.ProductId,
                    domainEvent.ProductName,
                    changePercent);
                    
                // 在实际应用中，这里可以:
                // - 通知营销部门
                // - 更新促销信息
                // - 向客户发送价格变动通知
                // - 等等
            }
                
            return Task.CompletedTask;
        }
    }
}