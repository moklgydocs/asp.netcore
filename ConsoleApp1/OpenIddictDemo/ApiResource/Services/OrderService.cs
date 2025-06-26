using Microsoft.EntityFrameworkCore;
using ApiResource.Data;
using ApiResource.Models;

namespace ApiResource.Services
{
    /// <summary>
    /// 订单服务实现
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly ApiDbContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApiDbContext context, IProductService productService, ILogger<OrderService> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        public async Task<(List<Order> Orders, int Total)> GetOrdersAsync(int page = 1, int pageSize = 10, OrderStatus? status = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .AsQueryable();

                if (status.HasValue)
                {
                    query = query.Where(o => o.Status == status.Value);
                }

                var total = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (orders, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单列表时发生错误");
                return (new List<Order>(), 0);
            }
        }

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取订单 {OrderId} 时发生错误", id);
                return null;
            }
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // 验证客户存在
                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null)
                    throw new ArgumentException("客户不存在");

                // 创建订单
                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    CustomerId = request.CustomerId,
                    Status = OrderStatus.Pending,
                    Notes = request.Notes,
                    ShippingAddress = request.ShippingAddress,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // 保存订单以获取ID

                // 创建订单项
                decimal totalAmount = 0;
                foreach (var itemRequest in request.Items)
                {
                    var product = await _productService.GetProductByIdAsync(itemRequest.ProductId);
                    if (product == null)
                        throw new ArgumentException($"产品 {itemRequest.ProductId} 不存在");

                    if (product.Stock < itemRequest.Quantity)
                        throw new InvalidOperationException($"产品 {product.Name} 库存不足");

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = itemRequest.ProductId,
                        Quantity = itemRequest.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * itemRequest.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;

                    // 更新库存
                    await _productService.UpdateStockAsync(itemRequest.ProductId, product.Stock - itemRequest.Quantity);
                }

                // 更新订单总金额
                order.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("创建订单成功: {OrderNumber} (ID: {OrderId})", order.OrderNumber, order.Id);
                
                return await GetOrderByIdAsync(order.Id) ?? order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "创建订单时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            try
            {
                var order = await GetOrderByIdAsync(id);
                if (order == null)
                    return false;

                var oldStatus = order.Status;
                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;

                // 设置特定状态的时间戳
                switch (status)
                {
                    case OrderStatus.Shipped:
                        order.ShippedAt = DateTime.UtcNow;
                        break;
                    case OrderStatus.Completed:
                        order.DeliveredAt = DateTime.UtcNow;
                        break;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("订单状态更新成功: {OrderNumber} (ID: {OrderId}) {OldStatus} -> {NewStatus}", 
                    order.OrderNumber, id, oldStatus, status);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新订单 {OrderId} 状态时发生错误", id);
                return false;
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        public async Task<bool> CancelOrderAsync(int id)
        {
            try
            {
                var order = await GetOrderByIdAsync(id);
                if (order == null)
                    return false;

                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
                {
                    _logger.LogWarning("订单 {OrderNumber} 状态为 {Status}，无法取消", order.OrderNumber, order.Status);
                    return false;
                }

                // 恢复库存
                foreach (var item in order.Items)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        await _productService.UpdateStockAsync(item.ProductId, product.Stock + item.Quantity);
                    }
                }

                // 更新订单状态
                await UpdateOrderStatusAsync(id, OrderStatus.Cancelled);

                _logger.LogInformation("取消订单成功: {OrderNumber} (ID: {OrderId})", order.OrderNumber, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消订单 {OrderId} 时发生错误", id);
                return false;
            }
        }

        /// <summary>
        /// 获取客户的订单
        /// </summary>
        public async Task<List<Order>> GetCustomerOrdersAsync(int customerId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取客户 {CustomerId} 订单时发生错误", customerId);
                return new List<Order>();
            }
        }

        /// <summary>
        /// 生成订单号
        /// </summary>
        private static string GenerateOrderNumber()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}