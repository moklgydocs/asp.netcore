using System;
using System.Collections.Generic;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario3_Permission
{
    /// <summary>
    /// 订单权限服务 - 展示如何在业务代码中使用权限验证特性
    /// 
    /// 业务场景说明：
    /// 在电商系统中，不同的订单操作需要不同的权限控制：
    /// - 创建订单：普通用户和管理员都可以
    /// - 取消订单：只有订单创建者或管理员可以
    /// - 查看订单详情：订单创建者、客服、管理员可以
    /// - 删除订单：只有管理员可以
    /// 
    /// 通过特性标记，我们可以清晰地表达每个方法的权限要求，
    /// 而不需要在方法内部编写复杂的权限验证逻辑。
    /// </summary>
    public class OrderPermissionService
    {
        /// <summary>
        /// 创建订单 - 需要 Order.Create 权限
        /// 使用本地验证，因为这是常见的操作，需要快速响应
        /// </summary>
        [RequirePermission("Order.Create")]
        public int CreateOrder(string productName, decimal price)
        {
            Console.WriteLine($"[业务逻辑] 正在创建订单：产品={productName}, 价格={price:C}");
            
            // 模拟订单创建逻辑
            var orderId = new Random().Next(10000, 99999);
            Console.WriteLine($"[业务逻辑] 订单创建成功，订单ID：{orderId}");
            
            return orderId;
        }

        /// <summary>
        /// 取消订单 - 需要 Order.Cancel 权限
        /// 使用本地验证，因为取消操作需要快速响应
        /// </summary>
        [RequirePermission("Order.Cancel")]
        public bool CancelOrder(int orderId, string reason)
        {
            Console.WriteLine($"[业务逻辑] 正在取消订单：订单ID={orderId}, 原因={reason}");
            
            // 模拟取消订单逻辑
            // 实际项目中这里会检查订单状态、用户权限等
            var success = new Random().NextDouble() > 0.1; // 90%成功率
            
            if (success)
            {
                Console.WriteLine($"[业务逻辑] 订单取消成功：订单ID={orderId}");
            }
            else
            {
                Console.WriteLine($"[业务逻辑] 订单取消失败：订单ID={orderId}");
            }
            
            return success;
        }

        /// <summary>
        /// 获取订单详情 - 需要 Order.View 权限
        /// 使用本地验证，因为查看详情是频繁操作
        /// </summary>
        [RequirePermission("Order.View")]
        public OrderDetail GetOrderDetail(int orderId)
        {
            Console.WriteLine($"[业务逻辑] 正在获取订单详情：订单ID={orderId}");
            
            // 模拟数据库查询
            System.Threading.Thread.Sleep(50); // 模拟查询延迟
            
            // 返回模拟的订单详情
            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductName = "示例商品",
                Price = 99.99m,
                Status = "已发货",
                CreatedAt = DateTime.Now.AddDays(-1),
                CustomerName = "张三"
            };
            
            Console.WriteLine($"[业务逻辑] 订单详情获取成功：{orderDetail.ProductName}");
            return orderDetail;
        }

        /// <summary>
        /// 删除订单 - 需要 Order.Delete 权限
        /// 使用远程验证，因为这是敏感操作，需要更严格的权限检查
        /// </summary>
        [RequirePermission("Order.Delete", UseRemoteService = true)]
        public bool DeleteOrder(int orderId)
        {
            Console.WriteLine($"[业务逻辑] 正在删除订单：订单ID={orderId}");
            
            // 模拟删除逻辑
            // 实际项目中这里会进行软删除或硬删除
            var success = new Random().NextDouble() > 0.05; // 95%成功率
            
            if (success)
            {
                Console.WriteLine($"[业务逻辑] 订单删除成功：订单ID={orderId}");
            }
            else
            {
                Console.WriteLine($"[业务逻辑] 订单删除失败：订单ID={orderId}");
            }
            
            return success;
        }

        /// <summary>
        /// 批量处理订单 - 需要 Order.BatchProcess 权限
        /// 使用远程验证，因为批量操作影响范围大，需要严格控制
        /// </summary>
        [RequirePermission("Order.BatchProcess", UseRemoteService = true)]
        public BatchProcessResult BatchProcessOrders(List<int> orderIds, string action)
        {
            Console.WriteLine($"[业务逻辑] 正在批量处理订单：操作={action}, 订单数量={orderIds.Count}");
            
            // 模拟批量处理逻辑
            System.Threading.Thread.Sleep(100); // 模拟处理时间
            
            var result = new BatchProcessResult
            {
                TotalCount = orderIds.Count,
                SuccessCount = 0,
                FailedCount = 0,
                Action = action
            };
            
            // 模拟部分成功
            foreach (var orderId in orderIds)
            {
                if (new Random().NextDouble() > 0.2) // 80%成功率
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                }
            }
            
            Console.WriteLine($"[业务逻辑] 批量处理完成：成功{result.SuccessCount}个，失败{result.FailedCount}个");
            return result;
        }
    }

    /// <summary>
    /// 订单详情模型
    /// </summary>
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
    }

    /// <summary>
    /// 批量处理结果模型
    /// </summary>
    public class BatchProcessResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public string Action { get; set; }
        
        public override string ToString()
        {
            return $"批量操作 '{Action}': 总数{TotalCount}, 成功{SuccessCount}, 失败{FailedCount}";
        }
    }
}
