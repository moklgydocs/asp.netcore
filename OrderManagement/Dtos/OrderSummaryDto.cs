namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 订单摘要DTO - 用于列表显示
    /// </summary>
    public class OrderSummaryDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 订单编号（显示用）
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 状态显示名称
        /// </summary>
        public string StatusDisplayName { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 格式化的总金额
        /// </summary>
        public string FormattedTotalAmount { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 格式化的订单日期
        /// </summary>
        public string FormattedOrderDate { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// 收货地址（简化）
        /// </summary>
        public string ShippingAddress { get; set; }
    }
}
