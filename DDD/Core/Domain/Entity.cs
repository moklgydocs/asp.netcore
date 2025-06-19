using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Core.Domain
{
    /// <summary>
    /// 实体基类 - DDD中的核心概念
    /// 实体具有唯一标识，生命周期中标识不变，但属性可能会改变
    /// </summary>
    /// <typeparam name="TId">标识符类型</typeparam>
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
        where TId : IEquatable<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// 实体唯一标识
        /// </summary>
        public TId Id { get; protected set; }

        /// <summary>
        /// 领域事件集合 - 用于记录实体状态变化产生的事件
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Entity(TId id)
        {
            if (id == null || id.Equals(default(TId)))
                throw new ArgumentException("实体ID不能为空", nameof(id));
            
            Id = id;
        }

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// 清除领域事件 - 通常在事件发布后调用
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// 实体相等性比较 - 基于ID比较
        /// </summary>
        public bool Equals(Entity<TId> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity<TId>);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !Equals(left, right);
        }
    }
}