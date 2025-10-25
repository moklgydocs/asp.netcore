using FeatureFactoryPatternDemo.Scenarios.Scenario1_Logging;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario1_Logging
{
    /// <summary>
    /// 场景1演示程序 - 日志记录策略
    /// 展示完整的"特性+工厂模式+管理器+扩展方法"范式使用
    /// </summary>
    public class Scenario1Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== 场景1：日志记录策略演示 ===");
            Console.WriteLine();

            // 创建订单服务实例
            var orderService = new OrderService();

            // 演示1：创建订单 - 使用文件日志
            Console.WriteLine("1. 创建订单（使用文件日志）：");
            var orderId = orderService.ExecuteWithLog<int>("CreateOrder", "iPhone 15");
            Console.WriteLine($"   订单创建成功，订单ID：{orderId}");
            Console.WriteLine();

            // 演示2：查询订单 - 使用控制台日志
            Console.WriteLine("2. 查询订单（使用控制台日志）：");
            var orderInfo = orderService.ExecuteWithLog<string>("GetOrderInfo", orderId);
            Console.WriteLine($"   订单信息：{orderInfo}");
            Console.WriteLine();

            // 演示3：处理支付 - 使用文件日志
            Console.WriteLine("3. 处理支付（使用文件日志）：");
            var paymentResult = orderService.ExecuteWithLog<bool>("ProcessPayment", orderId, 9999.99m);
            Console.WriteLine($"   支付结果：{(paymentResult ? "成功" : "失败")}");
            Console.WriteLine();

            // 演示4：取消订单 - 使用控制台日志
            Console.WriteLine("4. 取消订单（使用控制台日志）：");
            orderService.ExecuteWithLog<object>("CancelOrder", orderId);
            Console.WriteLine("   订单取消完成");
            Console.WriteLine();

            // 演示5：展示文件日志效果
            Console.WriteLine("5. 文件日志效果展示：");
            ShowFileLogContent();
            Console.WriteLine();

            // 演示6：展示模式的灵活性
            Console.WriteLine("6. 模式灵活性展示：");
            DemonstrateFlexibility();
            Console.WriteLine();

            Console.WriteLine("=== 场景1演示完成 ===");
            Console.WriteLine();
        }

        /// <summary>
        /// 展示文件日志的内容
        /// </summary>
        private static void ShowFileLogContent()
        {
            var logFilePath = "logs/app.log";
            if (File.Exists(logFilePath))
            {
                Console.WriteLine($"   文件日志位置：{logFilePath}");
                Console.WriteLine("   最近3条日志记录：");
                
                var lines = File.ReadAllLines(logFilePath);
                var recentLines = lines.TakeLast(3);
                
                foreach (var line in recentLines)
                {
                    Console.WriteLine($"   {line}");
                }
            }
            else
            {
                Console.WriteLine("   文件日志尚未生成（可能是因为使用了控制台日志）");
            }
        }

        /// <summary>
        /// 展示模式的灵活性
        /// </summary>
        private static void DemonstrateFlexibility()
        {
            Console.WriteLine("   1. 可以通过特性轻松切换日志类型");
            Console.WriteLine("   2. 可以通过管理器动态注册新的日志实现");
            Console.WriteLine("   3. 扩展方法提供了统一的调用接口");
            Console.WriteLine("   4. 业务代码无需修改即可支持新的日志方式");
            
            // 演示动态注册新的日志工厂
            LogManager.RegisterLoggerFactory(LogType.Console, () => new ConsoleLoggerFactory());
            Console.WriteLine("   5. 已成功动态注册了新的控制台日志工厂");
        }
    }
}
