using System;
using System.Collections.Generic;

namespace DDDCore.Domain
{
    /// <summary>
    /// 聚合根基类 - DDD中的核心构建块
    /// 聚合根是一种特殊的实体，它是聚合的入口点和一致性边界
    /// 聚合根负责确保聚合内所有实体的业务规则始终得到满足
    /// </summary>
    /// <typeparam name="TId">聚合根ID类型</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : IEquatable<TId>
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

        /// <summary>
        /// 提供聚合内发生的领域事件的只读集合
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="domainEvent">要添加的领域事件</param>
        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// 清除所有未处理的领域事件
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// 聚合根版本，用于支持乐观并发控制
        /// </summary>
        public int Version { get; protected set; }
    }
}