using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Application.DTOs;
using DDD.ECommerce.Domain.Catalog;

namespace DDD.ECommerce.Application.Interfaces
{
    /// <summary>
    /// 产品应用服务接口
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// 获取所有产品
        /// </summary>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 根据ID获取产品
        /// </summary>
        Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 根据类别获取产品
        /// </summary>
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(CategoryType category, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 创建产品
        /// </summary>
        Task<Guid> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 更新产品基本信息
        /// </summary>
        Task UpdateProductAsync(UpdateProductDto updateProductDto, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 更新产品价格
        /// </summary>
        Task UpdateProductPriceAsync(UpdateProductPriceDto updateProductPriceDto, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 调整产品库存
        /// </summary>
        Task AdjustProductStockAsync(UpdateProductStockDto updateProductStockDto, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 获取低库存产品
        /// </summary>
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 删除产品
        /// </summary>
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    }
}