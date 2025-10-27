using System;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario3_Permission
{
    /// <summary>
    /// 权限验证场景演示程序
    /// 
    /// 演示目标：
    /// 1. 展示如何使用特性标记方法权限要求
    /// 2. 演示本地验证和远程验证的区别
    /// 3. 展示权限验证失败时的处理
    /// 4. 展示不同用户权限的差异
    /// 
    /// 学习要点：
    /// - 声明式权限控制的优势
    /// - 工厂模式如何实现验证策略的解耦
    /// - 扩展方法如何简化权限验证的使用
    /// - 管理器如何统一管理不同的验证策略
    /// </summary>
    public class Scenario3Demo
    {
        private readonly OrderPermissionService _orderService;

        public Scenario3Demo()
        {
            _orderService = new OrderPermissionService();
        }

        /// <summary>
        /// 运行权限验证场景演示
        /// </summary>
        public void RunDemo()
        {
            Console.WriteLine("=== 场景3：权限验证演示 ===\n");
            
            // 显示可用的验证器类型
            ShowAvailableValidators();
            
            Console.WriteLine("\n=== 权限验证测试开始 ===\n");
            
            // 测试不同用户的权限
            TestUserPermissions();
            
            Console.WriteLine("\n=== 权限验证演示结束 ===\n");
        }

        /// <summary>
        /// 显示系统中可用的权限验证器类型
        /// </summary>
        private void ShowAvailableValidators()
        {
            Console.WriteLine("系统支持的权限验证器类型：");
            foreach (var validatorType in PermissionManager.GetAvailableTypes())
            {
                Console.WriteLine($"  - {validatorType}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 测试不同用户的权限验证
        /// </summary>
        private void TestUserPermissions()
        {
            // 定义测试用户
            var testUsers = new[]
            {
                new { UserId = "admin", Role = "管理员" },
                new { UserId = "user1", Role = "普通用户" },
                new { UserId = "manager", Role = "经理" },
                new { UserId = "guest", Role = "访客" }
            };

            // 定义测试用例
            var testCases = new[]
            {
                new { Method = "CreateOrder", Args = new object[] { "测试商品", 99.99m } },
                new { Method = "CancelOrder", Args = new object[] { 12345, "不想要了" } },
                new { Method = "GetOrderDetail", Args = new object[] { 12345 } },
                new { Method = "DeleteOrder", Args = new object[] { 12345 } },
                new { Method = "BatchProcessOrders", Args = new object[] { new int[] { 12345, 12346, 12347 }.ToList(), "发货" } }
            };

            // 为每个用户测试所有方法
            foreach (var user in testUsers)
            {
                Console.WriteLine($"\n--- 测试用户：{user.UserId} ({user.Role}) ---");
                
                foreach (var testCase in testCases)
                {
                    Console.WriteLine($"\n测试方法：{testCase.Method}");
                    Console.WriteLine($"参数：{string.Join(", ", testCase.Args)}");
                    
                    try
                    {
                        // 使用扩展方法执行带权限验证的方法
                        if (testCase.Method == "CreateOrder")
                        {
                            var result = _orderService.ExecuteWithPermissionCheck<int>(
                                testCase.Method, user.UserId, testCase.Args);
                            Console.WriteLine($"✓ 执行成功，返回值：{result}");
                        }
                        else if (testCase.Method == "CancelOrder" || testCase.Method == "DeleteOrder")
                        {
                            var result = _orderService.ExecuteWithPermissionCheck<bool>(
                                testCase.Method, user.UserId, testCase.Args);
                            Console.WriteLine($"✓ 执行成功，返回值：{result}");
                        }
                        else if (testCase.Method == "GetOrderDetail")
                        {
                            var result = _orderService.ExecuteWithPermissionCheck<OrderDetail>(
                                testCase.Method, user.UserId, testCase.Args);
                            Console.WriteLine($"✓ 执行成功，返回值：订单{result.OrderId} - {result.ProductName}");
                        }
                        else if (testCase.Method == "BatchProcessOrders")
                        {
                            var result = _orderService.ExecuteWithPermissionCheck<BatchProcessResult>(
                                testCase.Method, user.UserId, testCase.Args);
                            Console.WriteLine($"✓ 执行成功，返回值：{result}");
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"✗ 权限验证失败：{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ 执行异常：{ex.Message}");
                    }
                    
                    // 添加分隔线，让输出更清晰
                    Console.WriteLine(new string('-', 50));
                }
            }
        }

        /// <summary>
        /// 演示权限验证的业务价值
        /// </summary>
        public void ShowBusinessValue()
        {
            Console.WriteLine("\n=== 权限验证场景的业务价值 ===\n");
            
            Console.WriteLine("1. 声明式权限控制：");
            Console.WriteLine("   - 通过 [RequirePermission] 特性直接在方法上声明权限要求");
            Console.WriteLine("   - 代码更清晰，权限要求一目了然");
            Console.WriteLine("   - 避免在方法内部编写复杂的权限验证逻辑");
            Console.WriteLine();
            
            Console.WriteLine("2. 策略解耦：");
            Console.WriteLine("   - 本地验证：快速响应，适合频繁操作");
            Console.WriteLine("   - 远程验证：严格检查，适合敏感操作");
            Console.WriteLine("   - 可以根据操作的重要程度选择不同的验证策略");
            Console.WriteLine();
            
            Console.WriteLine("3. 易于扩展：");
            Console.WriteLine("   - 可以通过 PermissionManager.RegisterFactory 添加新的验证策略");
            Console.WriteLine("   - 支持数据库验证、LDAP验证、OAuth验证等多种方式");
            Console.WriteLine();
            
            Console.WriteLine("4. AOP思想：");
            Console.WriteLine("   - 权限验证作为横切关注点，与业务逻辑分离");
            Console.WriteLine("   - 可以轻松添加日志、监控、审计等功能");
            Console.WriteLine();
            
            Console.WriteLine("5. 实际应用场景：");
            Console.WriteLine("   - 电商平台：订单操作权限控制");
            Console.WriteLine("   - 企业系统：数据访问权限控制");
            Console.WriteLine("   - 金融系统：交易操作权限控制");
            Console.WriteLine("   - 内容管理系统：内容编辑权限控制");
        }
    }
}
