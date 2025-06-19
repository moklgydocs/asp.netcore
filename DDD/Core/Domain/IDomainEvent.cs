using System;

namespace DDD.Core.Domain
{
    /// <summary>
    /// 领域事件接口 - DDD中的核心概念
    /// 领域事件表示领域中发生的重要业务事件
    /// 用于解耦聚合之间的依赖关系
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// 事件唯一标识
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// 事件发生时间
        /// </summary>
        DateTime OccurredOn { get; }

        /// <summary>
        /// 事件版本 - 用于事件演化
        /// </summary>
        int Version { get; }
    }

    /// <summary>
    /// 领域事件基类
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        public int Version { get; }

        protected DomainEvent(int version = 1)
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Version = version;
        }
    }
}