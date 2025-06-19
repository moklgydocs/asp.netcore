using System;

namespace DDDCore.Domain
{
    /// <summary>
    /// 领域事件基类 - 表示领域中发生的事件
    /// 领域事件用于在聚合之间通信和集成
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 事件发生的时间
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}