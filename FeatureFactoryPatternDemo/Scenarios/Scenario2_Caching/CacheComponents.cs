using System.Reflection;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario2_Caching
{
    /// <summary>
    /// 场景2：缓存策略 - 方法结果自动缓存
    /// 这个场景在第一个场景的基础上增加了缓存键生成、过期时间管理等复杂特性
    /// 展示了如何处理更复杂的元数据和工厂参数
    /// </summary>
    
    #region 1. 核心枚举和接口定义

    /// <summary>
    /// 缓存类型枚举 - 定义不同的缓存实现
    /// </summary>
    public enum CacheType
    {
        Memory,  // 内存缓存 - 适用于单机应用，速度快
        Redis    // Redis缓存 - 适用于分布式应用，支持集群
    }

    /// <summary>
    /// 缓存接口（抽象产品） - 定义缓存操作的公共契约
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存的数据，如果不存在则返回默认值</returns>
        T Get<T>(string key);

        /// <summary>
        /// 将数据存入缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">要缓存的数据</param>
        /// <param name="expiration">过期时间</param>
        void Set<T>(string key, T value, TimeSpan expiration);
    }

    #endregion

    #region 2. 具体缓存实现（具体产品）

    /// <summary>
    /// 内存缓存实现 - 使用字典存储缓存数据
    /// 优点：访问速度快，无需网络开销
    /// 缺点：数据不持久，单机容量有限
    /// 适用场景：单机应用、临时数据缓存
    /// </summary>
    public class MemoryCache : ICache
    {
        // 使用字典存储缓存数据，包含值和过期时间
        private readonly Dictionary<string, (object Value, DateTime ExpiresAt)> _cache = new();

        public T Get<T>(string key)
        {
            // 检查缓存是否存在且未过期
            if (_cache.TryGetValue(key, out var item) && item.ExpiresAt > DateTime.Now)
            {
                // 返回解包后的值
                return (T)item.Value;
            }
            
            // 缓存不存在或已过期，返回默认值
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            // 存储值和计算出的过期时间
            _cache[key] = (value, DateTime.Now.Add(expiration));
        }
    }

    /// <summary>
    /// Redis缓存实现 - 模拟Redis缓存操作
    /// 在实际项目中，这里会使用StackExchange.Redis等客户端库
    /// 优点：支持分布式、数据持久化、高可用
    /// 缺点：需要网络连接，性能相对内存缓存较慢
    /// 适用场景：分布式应用、需要共享缓存的场景
    /// </summary>
    public class RedisCache : ICache
    {
        public T Get<T>(string key)
        {
            // 实际项目中这里会调用Redis客户端
            Console.WriteLine($"[Redis] 从Redis获取缓存：{key}");
            
            // 模拟缓存未命中或数据不存在
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            // 实际项目中这里会调用Redis客户端设置过期时间
            Console.WriteLine($"[Redis] 向Redis设置缓存：{key}，过期时间：{expiration}");
        }
    }

    #endregion

    #region 3. 缓存工厂接口和具体工厂

    /// <summary>
    /// 缓存工厂接口（抽象工厂） - 定义创建缓存实例的契约
    /// </summary>
    public interface ICacheFactory
    {
        /// <summary>
        /// 创建缓存实例
        /// </summary>
        /// <returns>ICache实例</returns>
        ICache CreateCache();
    }

    /// <summary>
    /// 内存缓存工厂 - 创建MemoryCache实例
    /// </summary>
    public class MemoryCacheFactory : ICacheFactory
    {
        public ICache CreateCache() => new MemoryCache();
    }

    /// <summary>
    /// Redis缓存工厂 - 创建RedisCache实例
    /// 在实际项目中，这里可能需要连接字符串等配置参数
    /// </summary>
    public class RedisCacheFactory : ICacheFactory
    {
        public ICache CreateCache() => new RedisCache();
    }

    #endregion

    #region 4. 缓存管理器（工厂管理器）

    /// <summary>
    /// 缓存管理器 - 统一管理缓存工厂
    /// 相比日志管理器，这里展示了更复杂的工厂管理场景
    /// </summary>
    public static class CacheManager
    {
        // 工厂注册表 - 简单的工厂映射
        private static readonly Dictionary<CacheType, ICacheFactory> _factories = new()
        {
            { CacheType.Memory, new MemoryCacheFactory() },
            { CacheType.Redis, new RedisCacheFactory() }
        };

        /// <summary>
        /// 根据缓存类型获取对应的缓存实例
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <returns>ICache实例</returns>
        public static ICache GetCache(CacheType cacheType)
        {
            return _factories[cacheType].CreateCache();
        }

        /// <summary>
        /// 动态注册缓存工厂
        /// 展示了系统的可扩展性
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="factory">缓存工厂</param>
        public static void RegisterCacheFactory(CacheType cacheType, ICacheFactory factory)
        {
            _factories[cacheType] = factory;
        }
    }

    #endregion

    #region 5. 缓存特性（元数据标记）

    /// <summary>
    /// 缓存特性 - 标记方法的缓存配置
    /// 相比日志特性，这里包含了更复杂的元数据：缓存类型和过期时间
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存类型 - 指定使用哪种缓存实现
        /// </summary>
        public CacheType CacheType { get; }

        /// <summary>
        /// 过期时间（秒） - 指定缓存的有效期
        /// 这是重要的业务配置，不同的数据有不同的缓存策略
        /// </summary>
        public int ExpirationSeconds { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="expirationSeconds">过期时间（秒）</param>
        public CacheAttribute(CacheType cacheType, int expirationSeconds)
        {
            CacheType = cacheType;
            ExpirationSeconds = expirationSeconds;
        }
    }

    #endregion

    #region 6. 缓存扩展方法（简化使用）

    /// <summary>
    /// 缓存扩展方法 - 为方法执行添加自动缓存功能
    /// 这个扩展方法比日志的更复杂，需要处理缓存键生成、缓存命中等逻辑
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 执行方法并自动处理缓存
        /// 这是缓存模式的核心方法，实现了透明的缓存代理
        /// </summary>
        /// <typeparam name="T">方法返回值类型</typeparam>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>方法执行结果或缓存结果</returns>
        public static T ExecuteWithCache<T>(this object service, string methodName, params object[] parameters)
        {
            // 1. 反射获取方法信息
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException($"方法 {methodName} 不存在");

            // 2. 读取方法上的Cache特性
            var cacheAttr = method.GetCustomAttribute<CacheAttribute>();
            if (cacheAttr == null)
                throw new InvalidOperationException($"方法 {methodName} 未标记 [Cache] 特性");

            // 3. 生成缓存键 - 这是缓存的核心技术点
            // 使用"类名_方法名_参数哈希"的格式确保键的唯一性
            var key = $"{method.DeclaringType.Name}_{methodName}_{string.Join("_", parameters)}";

            // 4. 获取缓存实例
            var cache = CacheManager.GetCache(cacheAttr.CacheType);

            // 5. 尝试从缓存获取数据
            var cachedValue = cache.Get<T>(key);
            if (cachedValue != null)
            {
                Console.WriteLine($"[缓存命中] {key}");
                return cachedValue;
            }

            // 6. 缓存未命中，执行方法并缓存结果
            Console.WriteLine($"[缓存未命中] {key}");
            var result = (T)method.Invoke(service, parameters);
            
            // 7. 将结果存入缓存
            cache.Set(key, result, TimeSpan.FromSeconds(cacheAttr.ExpirationSeconds));
            Console.WriteLine($"[缓存设置] {key}, 过期时间：{cacheAttr.ExpirationSeconds}秒");

            return result;
        }

        /// <summary>
        /// 清除指定方法的缓存
        /// 提供了缓存管理的额外功能
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        public static void ClearCache(this object service, string methodName, params object[] parameters)
        {
            var method = service.GetType().GetMethod(methodName);
            if (method == null) return;

            // 生成相同的缓存键
            var key = $"{method.DeclaringType.Name}_{methodName}_{string.Join("_", parameters)}";
            
            // 这里可以实现缓存清除逻辑
            // 实际项目中可能需要在ICache接口中添加Remove方法
            Console.WriteLine($"[缓存清除] {key}");
        }
    }

    #endregion

    #region 7. 高级特性：缓存键生成器

    /// <summary>
    /// 缓存键生成器接口 - 提供灵活的缓存键生成策略
    /// 这展示了模式的进一步扩展，可以支持不同的键生成算法
    /// </summary>
    public interface ICacheKeyGenerator
    {
        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>缓存键</returns>
        string GenerateKey(string methodName, object[] parameters);
    }

    /// <summary>
    /// 默认缓存键生成器 - 使用简单的字符串连接
    /// </summary>
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GenerateKey(string methodName, object[] parameters)
        {
            return $"{methodName}_{string.Join("_", parameters)}";
        }
    }

    /// <summary>
    /// 哈希缓存键生成器 - 使用哈希算法生成更短的键
    /// 适用于参数较多或参数较长的场景
    /// </summary>
    public class HashCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GenerateKey(string methodName, object[] parameters)
        {
            var paramStr = string.Join("_", parameters);
            var hash = paramStr.GetHashCode();
            return $"{methodName}_{hash}";
        }
    }

    #endregion
}
