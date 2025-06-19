using System;
using System.ComponentModel.DataAnnotations;

namespace DDD.ECommerce.Application.DTOs
{
    /// <summary>
    /// 更新产品库存请求DTO
    /// </summary>
    public class UpdateProductStockDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public int QuantityToAdjust { get; set; }
    }
}