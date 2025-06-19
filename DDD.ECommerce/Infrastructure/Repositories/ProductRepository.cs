using DDD.ECommerce.Domain.Catalog;
using DDD.ECommerce.Domain.Catalog.Specifications;
using DDD.ECommerce.Infrastructure.Data;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DDD.ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// 产品仓储实现
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _dbContext;
        
        public ProductRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 按ID获取产品
        /// </summary>
        public async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// 更新产品
        /// </summary>
        public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        public Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Products.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据条件筛选产品
        /// </summary>
        public async Task<IEnumerable<Product>> FindAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(p => p.Images)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据类别获取产品
        /// </summary>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(CategoryType categoryType, CancellationToken cancellationToken = default)
        {
            var spec = new ProductInCategorySpecification(categoryType);
            return await _dbContext.Products
                .Include(p => p.Images)
                .Where(spec.ToExpression())
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 获取低库存产品
        /// </summary>
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int thresholdQuantity, CancellationToken cancellationToken = default)
        {
            var spec = new LowStockProductSpecification(thresholdQuantity);
            var activeSpec = new ActiveProductSpecification();
            
            // 组合规约：低库存并且处于活动状态
            var combinedSpec = spec.And(activeSpec);
            
            return await _dbContext.Products
                .Include(p => p.Images)
                .Where(combinedSpec.ToExpression())
                .ToListAsync(cancellationToken);
        }
    }
}