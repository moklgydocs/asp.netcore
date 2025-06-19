namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 订单项数据传输对象
    /// </summary>
    public class OrderItemDto
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品SKU编码
        /// </summary>
        public string ProductSku { get; set; }

        /// <summary>
        /// 产品图片URL
        /// </summary>
        public string ProductImageUrl { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 小计金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 格式化的单价
        /// </summary>
        public string FormattedUnitPrice { get; set; }

        /// <summary>
        /// 格式化的小计
        /// </summary>
        public string FormattedTotalPrice { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddedAt { get; set; }
    }
}
