using System;
using System.ComponentModel.DataAnnotations;

namespace DDD.ECommerce.Application.DTOs
{
    /// <summary>
    /// 更新产品价格请求DTO
    /// </summary>
    public class UpdateProductPriceDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; }
    }
}