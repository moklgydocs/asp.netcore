using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Domain.Catalog;
using DDDCore.Repositories;

namespace Domain.Catalog
{
    /// <summary>
    /// 产品仓储接口
    /// </summary>
    public interface IProductRepository : IRepository<Product, Guid>
    {
        /// <summary>
        /// 根据类别获取产品列表
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryAsync(CategoryType categoryType, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 获取缺货产品列表
        /// </summary>
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int thresholdQuantity, CancellationToken cancellationToken = default);
    }
}