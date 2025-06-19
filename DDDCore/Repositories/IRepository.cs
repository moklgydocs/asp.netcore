using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DDDCore.Domain;

namespace DDDCore.Repositories
{
    /// <summary>
    /// 仓储接口 - DDD中的模式，用于封装领域对象的持久化和检索逻辑
    /// </summary>
    /// <typeparam name="T">聚合根类型</typeparam>
    /// <typeparam name="TId">聚合根ID类型</typeparam>
    public interface IRepository<T, TId> where T : AggregateRoot<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 按ID获取聚合根
        /// </summary>
        /// <param name="id">聚合根ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>找到的聚合根，如果未找到则为null</returns>
        Task<T> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加聚合根
        /// </summary>
        /// <param name="entity">要添加的聚合根</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新聚合根
        /// </summary>
        /// <param name="entity">要更新的聚合根</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除聚合根
        /// </summary>
        /// <param name="entity">要删除的聚合根</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 按条件查找聚合根
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>符合条件的聚合根集合</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}