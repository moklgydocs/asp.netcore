using System;
using System.ComponentModel.DataAnnotations;
using DDD.ECommerce.Domain.Catalog;

namespace DDD.ECommerce.Application.DTOs
{
    /// <summary>
    /// 更新产品请求DTO
    /// </summary>
    public class UpdateProductDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        
        [StringLength(2000)]
        public string Description { get; set; }
        
        public CategoryType? Category { get; set; }
        
        [StringLength(50)]
        public string SubCategory { get; set; }
    }
}