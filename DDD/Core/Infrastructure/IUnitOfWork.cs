using System;
using System.Threading.Tasks;

namespace DDD.Core.Infrastructure
{
    /// <summary>
    /// 工作单元模式接口 - 用于管理事务和保证数据一致性
    /// 协调多个仓储的操作，确保它们在同一个事务中执行
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 开始事务
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// 提交事务 - 保存所有更改
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// 回滚事务 - 撤销所有更改
        /// </summary>
        Task RollbackAsync();

        /// <summary>
        /// 保存更改但不提交事务
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// 获取仓储实例
        /// </summary>
        TRepository GetRepository<TRepository>() where TRepository : class;
    }
}