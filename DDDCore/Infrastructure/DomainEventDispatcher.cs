using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDDCore.Domain; 

namespace DDDCore.Infrastructure
{
    /// <summary>  
    /// 领域事件分发器，负责将领域事件派发给对应的处理器  
    /// </summary>  
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>  
        /// 分发领域事件到对应的处理器  
        /// </summary>  
        /// <param name="events">要分发的领域事件集合</param>  
        /// <param name="cancellationToken">取消令牌</param>  
        public async Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in events)
            {
                await DispatchEventAsync(domainEvent, cancellationToken);
            }
        }

        private async Task DispatchEventAsync(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            Type handlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));
            Type wrapperType = typeof(DomainEventHandlerWrapper<>).MakeGenericType(domainEvent.GetType());

            // 获取所有对应类型的事件处理器  
            var handlers = _serviceProvider.GetService(handlerType) as IEnumerable<object>;

            if (handlers == null)
            {
                throw new InvalidOperationException($"No handlers registered for domain event type {domainEvent.GetType().Name}");
            }

            var tasks = handlers
                .Select(handler =>
                {
                    // 创建包装器实例并调用处理方法  
                    var wrapper = (IDomainEventHandlerWrapper)Activator.CreateInstance(wrapperType, handler);
                    return wrapper.HandleAsync(domainEvent, cancellationToken);
                });

            // 等待所有处理器完成处理  
            await Task.WhenAll(tasks);
        }

        // 用于处理领域事件的内部接口  
        private interface IDomainEventHandlerWrapper
        {
            Task HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken);
        }

        // 处理领域事件的包装类  
        private class DomainEventHandlerWrapper<TDomainEvent> : IDomainEventHandlerWrapper
            where TDomainEvent : DomainEvent
        {
            private readonly IDomainEventHandler<TDomainEvent> _handler;

            public DomainEventHandlerWrapper(IDomainEventHandler<TDomainEvent> handler)
            {
                _handler = handler;
            }

            public Task HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken)
            {
                return _handler.HandleAsync((TDomainEvent)domainEvent, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 领域事件分发器接口
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
    }
}