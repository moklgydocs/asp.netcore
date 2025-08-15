using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 事件发布接口
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="eventData">事件数据</param>
        Task PublishAsync<TEvent>(TEvent eventData);
    }

    /// <summary>
    /// 事件处理接口
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    public interface IEventHandler<TEvent>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        Task HandleAsync(TEvent eventData);
    }

}
