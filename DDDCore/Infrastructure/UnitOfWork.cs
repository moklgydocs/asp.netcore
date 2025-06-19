using System.Threading;
using System.Threading.Tasks;
using DDDCore.Domain;

namespace DDDCore.Infrastructure
{
    /// <summary>
    /// 工作单元接口
    /// 工作单元模式协调领域事件分发和事务管理
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交所有更改并分发领域事件
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 实现了工作单元模式的基类
    /// 注意：此实现假设使用EF Core作为ORM
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        protected UnitOfWork(IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        /// <summary>
        /// 由派生类实现，保存底层数据库更改
        /// </summary>
        protected abstract Task<int> SaveChangesInternalAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 由派生类实现，获取所有待处理的领域事件
        /// </summary>
        protected abstract Task<DomainEvent[]> GetDomainEventsAsync();
        
        /// <summary>
        /// 由派生类实现，清除所有已处理的领域事件
        /// </summary>
        protected abstract Task ClearDomainEventsAsync();

        /// <summary>
        /// 保存更改并分发领域事件
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 首先保存所有数据库更改
            var result = await SaveChangesInternalAsync(cancellationToken);
            
            // 获取所有已保存实体的领域事件
            var domainEvents = await GetDomainEventsAsync();
            
            // 分发所有领域事件
            await _domainEventDispatcher.DispatchEventsAsync(domainEvents, cancellationToken);
            
            // 清除已处理的领域事件
            await ClearDomainEventsAsync();
            
            return result;
        }
    }
}