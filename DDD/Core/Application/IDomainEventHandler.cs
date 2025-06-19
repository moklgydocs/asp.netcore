using System.Threading.Tasks;
using DDD.Core.Domain;
using MediatR;

namespace DDD.Core.Application
{
    /// <summary>  
    /// 领域事件处理器接口  
    /// 用于处理领域事件，实现最终一致性  
    /// </summary>  
    /// <typeparam name="TDomainEvent">领域事件类型</typeparam>  
    public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent, INotification // 添加了 INotification 约束以解决 CS0314 错误  
    {
    }
}