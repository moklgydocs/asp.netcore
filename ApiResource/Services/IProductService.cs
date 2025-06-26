using ApiResource.Models;

namespace ApiResource.Services
{
    /// <summary>
    /// 产品服务接口
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// 获取所有产品
        /// </summary>
        Task<(List<Product> Products, int Total)> GetProductsAsync(int page = 1, int pageSize = 10, string? category = null, string? search = null);

        /// <summary>
        /// 根据ID获取产品
        /// </summary>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// 创建产品
        /// </summary>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>
        /// 更新产品
        /// </summary>
        Task<Product?> UpdateProductAsync(int id, Product product);

        /// <summary>
        /// 删除产品
        /// </summary>
        Task<bool> DeleteProductAsync(int id);

        /// <summary>
        /// 获取产品分类列表
        /// </summary>
        Task<List<string>> GetCategoriesAsync();

        /// <summary>
        /// 更新产品库存
        /// </summary>
        Task<bool> UpdateStockAsync(int id, int quantity);
    }
}