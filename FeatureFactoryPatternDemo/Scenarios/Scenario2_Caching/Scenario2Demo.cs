using FeatureFactoryPatternDemo.Scenarios.Scenario2_Caching;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario2_Caching
{
    /// <summary>
    /// 场景2演示程序 - 缓存策略
    /// 展示缓存模式的完整使用和性能提升效果
    /// </summary>
    public class Scenario2Demo
    {
        public static void Run()
        {
            Console.WriteLine("=== 场景2：缓存策略演示 ===");
            Console.WriteLine();

            var productService = new ProductService();

            // 演示1：首次调用 - 无缓存
            Console.WriteLine("1. 首次调用GetProductName（无缓存）：");
            var startTime = DateTime.Now;
            var productName = productService.ExecuteWithCache<string>("GetProductName", 100);
            var endTime = DateTime.Now;
            Console.WriteLine($"   结果：{productName}");
            Console.WriteLine($"   耗时：{(endTime - startTime).TotalMilliseconds}ms");
            Console.WriteLine();

            // 演示2：第二次调用 - 命中缓存
            Console.WriteLine("2. 第二次调用GetProductName（命中缓存）：");
            startTime = DateTime.Now;
            productName = productService.ExecuteWithCache<string>("GetProductName", 100);
            endTime = DateTime.Now;
            Console.WriteLine($"   结果：{productName}");
            Console.WriteLine($"   耗时：{(endTime - startTime).TotalMilliseconds}ms");
            Console.WriteLine();

            // 演示3：不同参数 - 无缓存
            Console.WriteLine("3. 调用GetProductName with different ID（无缓存）：");
            startTime = DateTime.Now;
            productName = productService.ExecuteWithCache<string>("GetProductName", 101);
            endTime = DateTime.Now;
            Console.WriteLine($"   结果：{productName}");
            Console.WriteLine($"   耗时：{(endTime - startTime).TotalMilliseconds}ms");
            Console.WriteLine();

            // 演示4：Redis缓存演示
            Console.WriteLine("4. Redis缓存演示 - GetProductDetails：");
            var productDetails = productService.ExecuteWithCache<string>("GetProductDetails", 200);
            Console.WriteLine($"   结果：{productDetails}");
            Console.WriteLine();

            // 演示5：缓存清除功能
            Console.WriteLine("5. 缓存清除功能演示：");
            Console.WriteLine("   清除GetProductName(100)的缓存...");
            productService.ClearCache("GetProductName", 100);
            Console.WriteLine("   重新调用GetProductName(100)：");
            startTime = DateTime.Now;
            productName = productService.ExecuteWithCache<string>("GetProductName", 100);
            endTime = DateTime.Now;
            Console.WriteLine($"   结果：{productName}");
            Console.WriteLine($"   耗时：{(endTime - startTime).TotalMilliseconds}ms（缓存已清除）");
            Console.WriteLine();

            // 演示6：不同缓存策略对比
            Console.WriteLine("6. 不同缓存策略对比：");
            DemonstrateCacheStrategies();
            Console.WriteLine();

            Console.WriteLine("=== 场景2演示完成 ===");
            Console.WriteLine();
        }

        /// <summary>
        /// 展示不同缓存策略的效果
        /// </summary>
        private static void DemonstrateCacheStrategies()
        {
            var productService = new ProductService();

            Console.WriteLine("   内存缓存 vs Redis缓存性能对比：");

            // 内存缓存性能测试
            Console.WriteLine("   - 内存缓存（GetProductPrice）：");
            var startTime = DateTime.Now;
            for (int i = 0; i < 5; i++)
            {
                productService.ExecuteWithCache<decimal>("GetProductPrice", 300);
            }
            var endTime = DateTime.Now;
            Console.WriteLine($"     5次调用耗时：{(endTime - startTime).TotalMilliseconds}ms");

            // Redis缓存性能测试（模拟）
            Console.WriteLine("   - Redis缓存（GetProductDetails）：");
            startTime = DateTime.Now;
            for (int i = 0; i < 5; i++)
            {
                productService.ExecuteWithCache<string>("GetProductDetails", 300);
            }
            endTime = DateTime.Now;
            Console.WriteLine($"     5次调用耗时：{(endTime - startTime).TotalMilliseconds}ms");

            Console.WriteLine("   缓存策略总结：");
            Console.WriteLine("   1. 内存缓存：速度快，适合单机应用");
            Console.WriteLine("   2. Redis缓存：支持分布式，适合集群环境");
            Console.WriteLine("   3. 合理设置过期时间：避免数据过期和内存占用");
            Console.WriteLine("   4. 缓存键设计：确保唯一性和可读性");
        }
    }
}
