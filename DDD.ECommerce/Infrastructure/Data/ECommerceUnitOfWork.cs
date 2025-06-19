using System.Threading;
using System.Threading.Tasks;
using DDDCore.Domain;
using DDDCore.Infrastructure;

namespace DDD.ECommerce.Infrastructure.Data
{
    /// <summary>
    /// 电子商务应用的工作单元实现
    /// </summary>
    public class ECommerceUnitOfWork : UnitOfWork
    {
        private readonly ECommerceDbContext _dbContext;
        
        public ECommerceUnitOfWork(ECommerceDbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 保存数据库更改
        /// </summary>
        protected override async Task<int> SaveChangesInternalAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 获取所有待处理的领域事件
        /// </summary>
        protected override async Task<DomainEvent[]> GetDomainEventsAsync()
        {
            return await _dbContext.GetDomainEventsAsync();
        }

        /// <summary>
        /// 清除所有已处理的领域事件
        /// </summary>
        protected override Task ClearDomainEventsAsync()
        {
            _dbContext.ClearDomainEvents();
            return Task.CompletedTask;
        }
    }
}