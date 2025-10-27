using FeatureFactoryPatternDemo.Scenarios.Scenario2_Caching;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario2_Caching
{
    /// <summary>
    /// 业务服务示例 - 产品服务
    /// 展示如何在实际业务代码中使用缓存特性标记
    /// </summary>
    public class ProductService
    {
        /// <summary>
        /// 获取产品名称
        /// 使用内存缓存，5分钟过期，因为产品信息相对稳定
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>产品名称</returns>
        [Cache(CacheType.Memory, 300)] // 5分钟 = 300秒
        public string GetProductName(int productId)
        {
            Console.WriteLine($"[数据库查询] 获取产品 {productId} 名称");
            
            // 模拟数据库查询延迟
            Thread.Sleep(200);
            
            // 模拟从数据库获取产品名称
            return $"产品{productId} - 高性能笔记本电脑";
        }

        /// <summary>
        /// 获取产品详情
        /// 使用Redis缓存，10分钟过期，因为详情信息更复杂，适合分布式缓存
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>产品详情</returns>
        [Cache(CacheType.Redis, 600)] // 10分钟 = 600秒
        public string GetProductDetails(int productId)
        {
            Console.WriteLine($"[数据库查询] 获取产品 {productId} 详情");
            
            // 模拟复杂的数据库查询
            Thread.Sleep(500);
            
            return $"产品{productId}详情：\n品牌：TechBrand\n型号：ProX1000\n价格：9999.99元\n库存：100件";
        }

        /// <summary>
        /// 获取产品价格
        /// 使用内存缓存，2分钟过期，因为价格可能频繁变动
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>产品价格</returns>
        [Cache(CacheType.Memory, 120)] // 2分钟 = 120秒
        public decimal GetProductPrice(int productId)
        {
            Console.WriteLine($"[数据库查询] 获取产品 {productId} 价格");
            
            // 模拟价格计算
            Thread.Sleep(100);
            
            return 9999.99m;
        }

        /// <summary>
        /// 获取热门产品列表
        /// 使用Redis缓存，15分钟过期，因为热门产品列表变化较慢
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <returns>热门产品ID列表</returns>
        [Cache(CacheType.Redis, 900)] // 15分钟 = 900秒
        public List<int> GetHotProducts(int count)
        {
            Console.WriteLine($"[数据库查询] 获取热门产品列表，数量：{count}");
            
            // 模拟复杂的热门算法计算
            Thread.Sleep(800);
            
            // 返回模拟的热门产品ID列表
            return Enumerable.Range(1, count).ToList();
        }

        /// <summary>
        /// 搜索产品
        /// 使用内存缓存，3分钟过期，因为搜索结果可能包含个性化因素
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="category">产品类别</param>
        /// <returns>搜索结果</returns>
        [Cache(CacheType.Memory, 180)] // 3分钟 = 180秒
        public string SearchProducts(string keyword, string category)
        {
            Console.WriteLine($"[数据库查询] 搜索产品：关键词={keyword}, 类别={category}");
            
            // 模拟复杂的搜索算法
            Thread.Sleep(600);
            
            return $"搜索结果：关键词'{keyword}'在'{category}'类别下找到10个产品";
        }
    }
}
