using DDD.Core.Domain;
using OrderManagement.Domain.ValueObjects;

namespace DDD.OrderManagement
{
    public class Address : AggregateRoot<OrderId>
    {
        public Address(OrderId id) : base(id)
        {
        }


        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区县
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string DetailAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 完整地址（格式化）
        /// </summary>
        public string FullAddress => $"{Province} {City} {District} {DetailAddress}";

        /// <summary>
        /// 是否为默认地址
        /// </summary>
        public bool IsDefault { get; set; }
    }
}