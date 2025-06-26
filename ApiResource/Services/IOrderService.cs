using ApiResource.Models;

namespace ApiResource.Services
{
    /// <summary>
    /// 订单服务接口
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// 获取订单列表
        /// </summary>
        Task<(List<Order> Orders, int Total)> GetOrdersAsync(int page = 1, int pageSize = 10, OrderStatus? status = null);

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        Task<Order?> GetOrderByIdAsync(int id);

        /// <summary>
        /// 创建订单
        /// </summary>
        Task<Order> CreateOrderAsync(CreateOrderRequest request);

        /// <summary>
        /// 更新订单状态
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status);

        /// <summary>
        /// 取消订单
        /// </summary>
        Task<bool> CancelOrderAsync(int id);

        /// <summary>
        /// 获取客户的订单
        /// </summary>
        Task<List<Order>> GetCustomerOrdersAsync(int customerId);
    }

    /// <summary>
    /// 创建订单请求
    /// </summary>
    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public string? Notes { get; set; }
        public string? ShippingAddress { get; set; }
    }

    /// <summary>
    /// 订单项请求
    /// </summary>
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}