using System.ComponentModel.DataAnnotations;

namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 创建订单请求
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        [Required(ErrorMessage = "客户ID不能为空")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [Required(ErrorMessage = "收货地址不能为空")]
        public CreateAddressRequest ShippingAddress { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(500, ErrorMessage = "备注信息不能超过500个字符")]
        public string Remarks { get; set; }
    }

    /// <summary>
    /// 创建地址请求
    /// </summary>
    public class CreateAddressRequest
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [Required(ErrorMessage = "收货人姓名不能为空")]
        [StringLength(50, ErrorMessage = "收货人姓名不能超过50个字符")]
        public string RecipientName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        [Required(ErrorMessage = "收货人电话不能为空")]
        [Phone(ErrorMessage = "电话号码格式不正确")]
        public string Phone { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [Required(ErrorMessage = "省份不能为空")]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [Required(ErrorMessage = "城市不能为空")]
        public string City { get; set; }

        /// <summary>
        /// 区县
        /// </summary>
        [Required(ErrorMessage = "区县不能为空")]
        public string District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [Required(ErrorMessage = "详细地址不能为空")]
        [StringLength(200, ErrorMessage = "详细地址不能超过200个字符")]
        public string DetailAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [RegularExpression(@"^\d{6}$", ErrorMessage = "邮政编码格式不正确")]
        public string PostalCode { get; set; }
    }
}
