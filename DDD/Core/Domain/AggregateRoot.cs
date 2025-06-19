using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Core.Domain
{
    /// <summary>
    /// 聚合根基类 - DDD中的核心概念
    /// 聚合根是聚合的唯一入口，负责维护聚合内部的一致性
    /// 只有聚合根可以被仓储直接获取和持久化
    /// </summary>
    /// <typeparam name="TId">聚合根标识类型</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// 版本号 - 用于乐观锁控制并发
        /// </summary>
        public long Version { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        protected AggregateRoot(TId id) : base(id)
        {
            Version = 0;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 增加版本号 - 每次聚合状态改变时调用
        /// </summary>
        protected void IncrementVersion()
        {
            Version++;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 应用领域事件并增加版本号
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        protected void ApplyEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
            IncrementVersion();
        }

        /// <summary>
        /// 检查业务规则 - 用于维护聚合不变性
        /// </summary>
        /// <param name="rule">业务规则</param>
        /// <param name="message">违反规则时的错误消息</param>
        protected void CheckRule(IBusinessRule rule, string message = null)
        {
            if (!rule.IsSatisfied())
            {
                throw new BusinessRuleValidationException(message ?? rule.Message);
            }
        }

        /// <summary>
        /// 标记为已修改
        /// </summary>
        protected void MarkAsModified()
        {
            IncrementVersion();
        }
    }
}