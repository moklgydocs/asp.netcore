namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 产品数据传输对象
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// SKU编码
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 格式化价格
        /// </summary>
        public string FormattedPrice { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// 产品分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 产品品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 产品图片URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 产品缩略图URL
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 是否有库存
        /// </summary>
        public bool InStock => StockQuantity > 0;

        /// <summary>
        /// 是否为活跃产品
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
