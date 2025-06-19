using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Application.DTOs;
using DDD.ECommerce.Application.Interfaces;
using DDD.ECommerce.Domain.Catalog;
using DDDCore.Infrastructure;

namespace DDD.ECommerce.Application.Services
{
    /// <summary>
    /// 产品应用服务实现
    /// 负责协调领域对象和基础设施以完成用例
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取所有产品
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.FindAsync(p => true, cancellationToken);
            return products.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 根据ID获取产品
        /// </summary>
        public async Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return null;
                
            return MapToDto(product);
        }

        /// <summary>
        /// 根据类别获取产品
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(CategoryType category, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetByCategoryAsync(category, cancellationToken);
            return products.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 创建产品
        /// </summary>
        public async Task<Guid> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default)
        {
            // 创建值对象
            var name = ProductName.Create(createProductDto.Name);
            var description = ProductDescription.Create(createProductDto.Description);
            var price = Money.Create(createProductDto.Price, createProductDto.Currency);
            var category = ProductCategory.Create(createProductDto.Category, createProductDto.SubCategory);
            
            // 创建产品实体(聚合根)
            var id = Guid.NewGuid();
            var product = new Product(id, name, description, price, createProductDto.StockQuantity, category);
            
            // 持久化并提交
            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return id;
        }

        /// <summary>
        /// 更新产品基本信息
        /// </summary>
        public async Task UpdateProductAsync(UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(updateProductDto.Id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {updateProductDto.Id} not found.");
            
            // 更新值对象
            var name = string.IsNullOrEmpty(updateProductDto.Name) 
                ? product.Name 
                : ProductName.Create(updateProductDto.Name);
                
            var description = string.IsNullOrEmpty(updateProductDto.Description)
                ? product.Description
                : ProductDescription.Create(updateProductDto.Description);
                
            var category = updateProductDto.Category == null
                ? product.Category
                : ProductCategory.Create(updateProductDto.Category.Value, updateProductDto.SubCategory);
            
            // 调用领域方法更新产品
            product.UpdateDetails(name, description, category);
            
            // 持久化并提交
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 更新产品价格
        /// </summary>
        public async Task UpdateProductPriceAsync(UpdateProductPriceDto updateProductPriceDto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(updateProductPriceDto.Id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {updateProductPriceDto.Id} not found.");
            
            // 创建新价格值对象
            var newPrice = Money.Create(updateProductPriceDto.Price, updateProductPriceDto.Currency);
            
            // 调用领域方法更新价格
            product.UpdatePrice(newPrice);
            
            // 持久化并提交
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 调整产品库存
        /// </summary>
        public async Task AdjustProductStockAsync(UpdateProductStockDto updateProductStockDto, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(updateProductStockDto.Id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {updateProductStockDto.Id} not found.");
            
            // 调用领域方法调整库存
            product.AdjustStock(updateProductStockDto.QuantityToAdjust);
            
            // 持久化并提交
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 获取低库存产品
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetLowStockProductsAsync(threshold, cancellationToken);
            return products.Select(MapToDto).ToList();
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                throw new ArgumentException($"Product with ID {id} not found.");
            
            await _productRepository.DeleteAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 将领域对象映射到DTO
        /// </summary>
        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name.Value,
                Description = product.Description.Value,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                StockQuantity = product.StockQuantity,
                Category = product.Category.Type.ToString(),
                SubCategory = product.Category.SubCategory,
                IsActive = product.IsActive,
                Images = product.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    Url = i.Url,
                    ContentType = i.ContentType,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder,
                    Description = i.Description
                }).ToList()
            };
        }
    }
}