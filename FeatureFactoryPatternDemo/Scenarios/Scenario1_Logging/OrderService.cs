using FeatureFactoryPatternDemo.Scenarios.Scenario1_Logging;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario1_Logging
{
    /// <summary>
    /// 业务服务示例 - 订单服务
    /// 展示如何在实际业务代码中使用日志特性标记
    /// </summary>
    public class OrderService
    {
        /// <summary>
        /// 创建订单方法
        /// 使用文件日志记录，因为订单创建是重要业务操作，需要持久化日志
        /// </summary>
        /// <param name="productName">产品名称</param>
        /// <returns>订单ID</returns>
        [Log(LogType.File, LogLevel.Information)]
        public int CreateOrder(string productName)
        {
            Console.WriteLine($"创建订单：{productName}");
            
            // 模拟业务逻辑处理时间
            Thread.Sleep(100);
            
            // 模拟生成订单ID
            return new Random().Next(1000, 9999);
        }

        /// <summary>
        /// 查询订单方法
        /// 使用控制台日志，因为查询操作频繁，使用控制台日志性能更好
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单信息</returns>
        [Log(LogType.Console, LogLevel.Information)]
        public string GetOrderInfo(int orderId)
        {
            Console.WriteLine($"查询订单：{orderId}");
            
            // 模拟数据库查询
            Thread.Sleep(200);
            
            return $"订单{orderId} - 状态：已创建 - 产品：手机";
        }

        /// <summary>
        /// 取消订单方法
        /// 使用控制台日志记录，因为取消操作相对简单，且可能涉及用户交互
        /// 使用Error级别，因为取消操作通常是因为出现了问题
        /// </summary>
        /// <param name="orderId">订单ID</param>
        [Log(LogType.Console, LogLevel.Error)]
        public void CancelOrder(int orderId)
        {
            Console.WriteLine($"取消订单：{orderId}");
            
            // 模拟业务逻辑处理时间
            Thread.Sleep(50);
            
            // 这里可以添加实际的取消逻辑
            // 比如检查订单状态、通知库存系统等
        }

        /// <summary>
        /// 处理支付方法
        /// 使用文件日志，因为支付涉及资金安全，需要详细记录
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="amount">支付金额</param>
        /// <returns>支付结果</returns>
        [Log(LogType.File, LogLevel.Information)]
        public bool ProcessPayment(int orderId, decimal amount)
        {
            Console.WriteLine($"处理支付：订单{orderId}, 金额{amount:C}");
            
            // 模拟支付处理
            Thread.Sleep(300);
            
            // 模拟支付成功
            return true;
        }
    }
}
