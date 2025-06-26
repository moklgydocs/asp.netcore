using Microsoft.EntityFrameworkCore;
using ApiResource.Data;
using ApiResource.Models;

namespace ApiResource.Services
{
    /// <summary>
    /// 产品服务实现
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApiDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有产品
        /// </summary>
        public async Task<(List<Product> Products, int Total)> GetProductsAsync(int page = 1, int pageSize = 10, string? category = null, string? search = null)
        {
            try
            {
                var query = _context.Products.Where(p => p.IsActive);

                // 分类过滤
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(p => p.Category == category);
                }

                // 搜索过滤
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        p.Name.Contains(search) ||
                        p.Description!.Contains(search) ||
                        p.Brand!.Contains(search));
                }

                var total = await query.CountAsync();
                var products = await query
                    .OrderBy(p => p.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取产品列表时发生错误");
                return (new List<Product>(), 0);
            }
        }

        /// <summary>
        /// 根据ID获取产品
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取产品 {ProductId} 时发生错误", id);
                return null;
            }
        }

        /// <summary>
        /// 创建产品
        /// </summary>
        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("创建产品成功: {ProductName} (ID: {ProductId})", product.Name, product.Id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建产品 {ProductName} 时发生错误", product.Name);
                throw;
            }
        }

        /// <summary>
        /// 更新产品
        /// </summary>
        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            try
            {
                var existingProduct = await GetProductByIdAsync(id);
                if (existingProduct == null)
                    return null;

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;
                existingProduct.Stock = product.Stock;
                existingProduct.Brand = product.Brand;
                existingProduct.Model = product.Model;
                existingProduct.Weight = product.Weight;
                existingProduct.Specifications = product.Specifications;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("更新产品成功: {ProductName} (ID: {ProductId})", existingProduct.Name, id);
                return existingProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新产品 {ProductId} 时发生错误", id);
                throw;
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await GetProductByIdAsync(id);
                if (product == null)
                    return false;

                // 软删除
                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("删除产品成功: {ProductName} (ID: {ProductId})", product.Name, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除产品 {ProductId} 时发生错误", id);
                return false;
            }
        }

        /// <summary>
        /// 获取产品分类列表
        /// </summary>
        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Products
                    .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Category))
                    .Select(p => p.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取产品分类列表时发生错误");
                return new List<string>();
            }
        }

        /// <summary>
        /// 更新产品库存
        /// </summary>
        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            try
            {
                var product = await GetProductByIdAsync(id);
                if (product == null)
                    return false;

                product.Stock = quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("更新产品库存成功: {ProductName} (ID: {ProductId}), 新库存: {Stock}", 
                    product.Name, id, quantity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新产品 {ProductId} 库存时发生错误", id);
                return false;
            }
        }
    }
}