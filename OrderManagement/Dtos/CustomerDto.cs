namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 客户数据传输对象
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 客户等级
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisteredAt { get; set; }

        /// <summary>
        /// 是否为VIP客户
        /// </summary>
        public bool IsVip { get; set; }

        /// <summary>
        /// 累计消费金额
        /// </summary>
        public decimal TotalSpent { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount { get; set; }

        /// <summary>
        /// 地址列表
        /// </summary>
        public List<AddressDto> Addresses { get; set; } = new();
    }
}
