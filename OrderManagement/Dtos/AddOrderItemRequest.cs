using System.ComponentModel.DataAnnotations;

namespace DDD.OrderManagement.Dtos
{
    /// <summary>
    /// 添加订单项请求
    /// </summary>
    public class AddOrderItemRequest
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        [Required(ErrorMessage = "产品ID不能为空")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required(ErrorMessage = "数量不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
        public int Quantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200, ErrorMessage = "备注不能超过200个字符")]
        public string Remarks { get; set; }
    }
}
