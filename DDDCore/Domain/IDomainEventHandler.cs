using System.Threading;
using System.Threading.Tasks;

namespace DDDCore.Domain
{
    /// <summary>
    /// 领域事件处理接口
    /// 实现此接口以处理特定的领域事件
    /// </summary>
    /// <typeparam name="TDomainEvent">要处理的领域事件类型</typeparam>
    public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : DomainEvent
    {
        /// <summary>
        /// 处理领域事件
        /// </summary>
        /// <param name="domainEvent">要处理的领域事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}