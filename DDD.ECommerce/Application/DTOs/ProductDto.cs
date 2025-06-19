using System;
using System.Collections.Generic;

namespace DDD.ECommerce.Application.DTOs
{
    /// <summary>
    /// 产品数据传输对象
    /// </summary>
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public bool IsActive { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }

    /// <summary>
    /// 产品图片数据传输对象
    /// </summary>
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
    }
}