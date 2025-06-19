using System;
using System.ComponentModel.DataAnnotations;
using DDD.ECommerce.Domain.Catalog;

namespace DDD.ECommerce.Application.DTOs
{
    /// <summary>
    /// 创建产品请求DTO
    /// </summary>
    public class CreateProductDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        
        [StringLength(2000)]
        public string Description { get; set; }
        
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "USD";
        
        [Required]
        [Range(0, 1000000)]
        public int StockQuantity { get; set; }
        
        [Required]
        public CategoryType Category { get; set; }
        
        [StringLength(50)]
        public string SubCategory { get; set; }
    }
}