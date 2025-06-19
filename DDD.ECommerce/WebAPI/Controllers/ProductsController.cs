using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDD.ECommerce.Application.DTOs;
using DDD.ECommerce.Application.Interfaces;
using DDD.ECommerce.Domain.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDD.ECommerce.WebAPI.Controllers
{
    /// <summary>
    /// 产品管理API控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        
        /// <summary>
        /// 获取所有产品
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        
        /// <summary>
        /// 根据ID获取产品
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
                
            return Ok(product);
        }
        
        /// <summary>
        /// 根据类别获取产品
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            if (!Enum.TryParse<CategoryType>(category, true, out var categoryType))
                return BadRequest($"Invalid category: {category}");
                
            var products = await _productService.GetProductsByCategoryAsync(categoryType);
            return Ok(products);
        }
        
        /// <summary>
        /// 创建新产品
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                var id = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id }, null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// 更新产品信息
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        {
            if (id != updateProductDto.Id)
                return BadRequest("ID mismatch");
                
            try
            {
                await _productService.UpdateProductAsync(updateProductDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        /// <summary>
        /// 更新产品价格
        /// </summary>
        [HttpPut("{id}/price")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductPrice(Guid id, UpdateProductPriceDto updateProductPriceDto)
        {
            if (id != updateProductPriceDto.Id)
                return BadRequest("ID mismatch");
                
            try
            {
                await _productService.UpdateProductPriceAsync(updateProductPriceDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        /// <summary>
        /// 调整产品库存
        /// </summary>
        [HttpPut("{id}/stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdjustProductStock(Guid id, UpdateProductStockDto updateProductStockDto)
        {
            if (id != updateProductStockDto.Id)
                return BadRequest("ID mismatch");
                
            try
            {
                await _productService.AdjustProductStockAsync(updateProductStockDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// 获取低库存产品
        /// </summary>
        [HttpGet("lowstock/{threshold:int=10}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts(int threshold)
        {
            var products = await _productService.GetLowStockProductsAsync(threshold);
            return Ok(products);
        }
        
        /// <summary>
        /// 删除产品
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}