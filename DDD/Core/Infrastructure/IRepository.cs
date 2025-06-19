using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DDD.Core.Domain;

namespace DDD.Core.Infrastructure
{
    /// <summary>
    /// 仓储模式接口 - DDD中的基础设施概念
    /// 仓储提供类似集合的接口来访问聚合根
    /// 封装了数据访问的复杂性
    /// </summary>
    /// <typeparam name="TEntity">聚合根实体类型</typeparam>
    /// <typeparam name="TId">聚合根标识类型</typeparam>
    public interface IRepository<TEntity, TId>
        where TEntity : AggregateRoot<TId>
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// 根据ID获取聚合根
        /// </summary>
        Task<TEntity> GetByIdAsync(TId id);

        /// <summary>
        /// 根据规约查找聚合根
        /// </summary>
        Task<TEntity> FindAsync(Specification<TEntity> specification);

        /// <summary>
        /// 根据规约查找多个聚合根
        /// </summary>
        Task<IEnumerable<TEntity>> FindAllAsync(Specification<TEntity> specification);

        /// <summary>
        /// 获取所有聚合根
        /// </summary>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// 添加聚合根
        /// </summary>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// 更新聚合根
        /// </summary>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// 删除聚合根
        /// </summary>
        Task RemoveAsync(TEntity entity);

        /// <summary>
        /// 根据ID删除聚合根
        /// </summary>
        Task RemoveAsync(TId id);

        /// <summary>
        /// 检查是否存在满足条件的聚合根
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 统计满足条件的聚合根数量
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    }
}