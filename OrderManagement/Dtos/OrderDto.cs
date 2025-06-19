namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 订单数据传输对象
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 客户姓名（冗余字段，便于显示）
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 订单状态显示名称
        /// </summary>
        public string StatusDisplayName { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 格式化的总金额
        /// </summary>
        public string FormattedTotalAmount { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        /// 订单项列表
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new();

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 版本号（用于乐观锁）
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// 订单项数量
        /// </summary>
        public int ItemCount => Items?.Count ?? 0;

        /// <summary>
        /// 商品总数量
        /// </summary>
        public int TotalQuantity => Items?.Sum(x => x.Quantity) ?? 0;

        /// <summary>
        /// 是否可以编辑
        /// </summary>
        public bool CanEdit => Status == "Draft";

        /// <summary>
        /// 是否可以确认
        /// </summary>
        public bool CanConfirm => Status == "Draft" && Items?.Any() == true;

        /// <summary>
        /// 是否可以支付
        /// </summary>
        public bool CanPay => Status == "Confirmed";

        /// <summary>
        /// 是否可以取消
        /// </summary>
        public bool CanCancel => Status is "Draft" or "Confirmed";

        /// <summary>
        /// 是否可以发货
        /// </summary>
        public bool CanShip => Status == "Paid";
    }
}
