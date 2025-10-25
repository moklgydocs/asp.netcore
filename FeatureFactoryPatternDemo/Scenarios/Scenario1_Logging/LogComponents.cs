using System.Reflection;
using System.IO;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario1_Logging
{
    /// <summary>
    /// 场景1：日志记录策略 - 多实现动态切换
    /// 这是第一个学习场景，展示了完整的"特性+工厂模式+管理器+扩展方法"范式
    /// </summary>
    
    #region 1. 核心枚举和接口定义
    
    /// <summary>
    /// 日志类型枚举 - 定义支持的日志输出方式
    /// 这是元数据的一部分，用于标记不同的日志实现
    /// </summary>
    public enum LogType
    {
        Console,   // 控制台日志 - 适用于开发环境调试
        File       // 文件日志 - 适用于生产环境持久化
    }

    /// <summary>
    /// 日志级别枚举 - 定义日志的重要程度
    /// </summary>
    public enum LogLevel
    {
        Information,  // 信息级别 - 一般操作日志
        Error         // 错误级别 - 异常和错误信息
    }

    /// <summary>
    /// 日志接口（抽象产品） - 定义所有日志实现的公共契约
    /// 这是工厂模式中的"产品接口"，隔离了不同实现的差异
    /// 好处：调用方只依赖抽象，不依赖具体实现，便于替换和扩展
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 记录日志消息
        /// </summary>
        /// <param name="message">日志消息内容</param>
        /// <param name="level">日志级别</param>
        void Log(string message, LogLevel level = LogLevel.Information);
    }

    #endregion

    #region 2. 具体日志实现（具体产品）

    /// <summary>
    /// 控制台日志实现（具体产品） - 将日志输出到控制台
    /// 这是工厂模式中的"具体产品"，实现了ILogger接口
    /// 适用场景：开发环境、调试阶段，便于实时查看日志
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LogLevel level)
        {
            // 控制台输出格式：[时间] [级别] 消息内容
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
        }
    }

    /// <summary>
    /// 文件日志实现（具体产品） - 将日志写入文件
    /// 这是另一个"具体产品"，同样实现了ILogger接口
    /// 适用场景：生产环境，需要持久化日志进行问题排查
    /// 好处：日志不会丢失，可以长期保存和分析
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        /// <summary>
        /// 构造函数接收文件路径，体现了依赖注入的思想
        /// 文件路径可以从配置文件读取，实现配置化管理
        /// </summary>
        /// <param name="filePath">日志文件的完整路径</param>
        public FileLogger(string filePath)
        {
            _filePath = filePath;
            // 确保日志目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        public void Log(string message, LogLevel level)
        {
            // 格式化日志内容，包含换行符
            var log = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
            // 追加写入文件，不会覆盖已有日志
            File.AppendAllText(_filePath, log);
        }
    }

    #endregion

    #region 3. 工厂接口和具体工厂

    /// <summary>
    /// 日志工厂接口（抽象工厂） - 定义创建日志实例的契约
    /// 这是工厂模式的核心，封装了对象创建的复杂性
    /// 好处：调用方不需要知道如何创建具体对象，只需要获取工厂实例
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建日志实例
        /// </summary>
        /// <returns>ILogger实例</returns>
        ILogger CreateLogger();
    }

    /// <summary>
    /// 控制台日志工厂（具体工厂） - 创建ConsoleLogger实例
    /// 这是工厂模式中的"具体工厂"，负责创建特定类型的日志实现
    /// </summary>
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger() => new ConsoleLogger();
    }

    /// <summary>
    /// 文件日志工厂（具体工厂） - 创建FileLogger实例
    /// 注意：这里需要传递文件路径参数，展示了工厂如何处理依赖
    /// </summary>
    public class FileLoggerFactory : ILoggerFactory
    {
        private readonly string _filePath;

        public FileLoggerFactory(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger() => new FileLogger(_filePath);
    }

    #endregion

    #region 4. 日志管理器（工厂管理器）

    /// <summary>
    /// 日志管理器 - 统一管理所有日志工厂的入口点
    /// 这是"管理器"模式的体现，负责注册、管理和提供工厂实例
    /// 设计好处：
    /// 1. 集中管理所有工厂，避免调用方直接依赖具体工厂
    /// 2. 支持动态配置和扩展，新增日志类型只需在这里注册
    /// 3. 可以集成配置管理、健康检查等功能
    /// </summary>
    public static class LogManager
    {
        // 工厂注册表 - 使用字典存储日志类型到工厂创建函数的映射
        // 使用Func<ILoggerFactory>而不是直接存储工厂实例的好处：
        // 1. 延迟初始化，按需创建工厂
        // 2. 支持工厂的参数化配置
        // 3. 便于单元测试和Mock
        private static readonly Dictionary<LogType, Func<ILoggerFactory>> _factoryCreators = new()
        {
            // 注册控制台日志工厂 - 无参数，直接创建
            { LogType.Console, () => new ConsoleLoggerFactory() },
            
            // 注册文件日志工厂 - 带参数，可以从配置读取路径
            { LogType.File, () => new FileLoggerFactory("logs/app.log") }
        };

        /// <summary>
        /// 根据日志类型获取对应的日志实例
        /// 这是管理器的核心方法，封装了"选择工厂->创建实例"的完整流程
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <returns>ILogger实例</returns>
        /// <exception cref="NotSupportedException">当不支持的日志类型时抛出</exception>
        public static ILogger GetLogger(LogType logType)
        {
            // 查找对应的工厂创建函数
            if (_factoryCreators.TryGetValue(logType, out var creator))
            {
                // 创建工厂并获取日志实例
                return creator().CreateLogger();
            }
            
            // 如果找不到对应的工厂，抛出异常
            // 在实际项目中，这里可以记录错误日志并返回默认实现
            throw new NotSupportedException($"不支持的日志类型：{logType}");
        }

        /// <summary>
        /// 动态注册新的日志工厂（扩展方法）
        /// 这展示了系统的可扩展性，可以在运行时添加新的日志实现
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="factoryCreator">工厂创建函数</param>
        public static void RegisterLoggerFactory(LogType logType, Func<ILoggerFactory> factoryCreator)
        {
            _factoryCreators[logType] = factoryCreator;
        }
    }

    #endregion

    #region 5. 日志特性（元数据标记）

    /// <summary>
    /// 日志特性 - 用于标记方法需要的日志配置
    /// 这是"特性"模式的体现，将元数据（配置信息）与业务逻辑分离
    /// 设计好处：
    /// 1. 声明式编程：通过特性标记表达意图，代码更清晰
    /// 2. 配置外置：日志配置与业务逻辑解耦，便于修改
    /// 3. 编译时检查：特性使用在编译时就能发现错误
    /// 4. 反射读取：运行时可以通过反射获取配置信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LogAttribute : Attribute
    {
        /// <summary>
        /// 日志类型 - 指定使用哪种日志实现
        /// </summary>
        public LogType LogType { get; }

        /// <summary>
        /// 日志级别 - 指定日志的重要程度
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logLevel">日志级别，默认为Information</param>
        public LogAttribute(LogType logType, LogLevel logLevel = LogLevel.Information)
        {
            LogType = logType;
            LogLevel = logLevel;
        }
    }

    #endregion

    #region 6. 扩展方法（简化使用）

    /// <summary>
    /// 日志扩展方法 - 为所有对象提供统一的日志执行能力
    /// 这是"扩展方法"模式的体现，为现有类型添加新功能
    /// 设计好处：
    /// 1. 无侵入性：不需要修改原有类就能添加功能
    /// 2. 语法糖：调用方式像原生方法一样自然
    /// 3. 封装复杂性：隐藏了反射、工厂选择等复杂逻辑
    /// 4. 统一入口：所有需要日志的方法都通过这个入口
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// 执行方法并自动记录日志
        /// 这是整个模式的集大成者，整合了特性读取、工厂选择、日志记录等所有环节
        /// </summary>
        /// <typeparam name="T">方法返回值类型</typeparam>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>方法执行结果</returns>
        /// <exception cref="ArgumentException">当方法不存在时</exception>
        /// <exception cref="InvalidOperationException">当方法没有标记Log特性时</exception>
        public static T ExecuteWithLog<T>(this object service, string methodName, params object[] parameters)
        {
            // 1. 反射获取方法信息
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException($"方法 {methodName} 不存在");

            // 2. 读取方法上的Log特性（元数据）
            var logAttr = method.GetCustomAttribute<LogAttribute>();
            if (logAttr == null)
                throw new InvalidOperationException($"方法 {methodName} 未标记 [Log] 特性");

            // 3. 通过管理器获取对应的日志实例（工厂模式）
            var logger = LogManager.GetLogger(logAttr.LogType);

            // 4. 执行前日志记录
            logger.Log($"开始执行 {methodName}", logAttr.LogLevel);

            try
            {
                // 5. 执行实际方法
                var result = (T)method.Invoke(service, parameters);

                // 6. 执行成功日志记录
                logger.Log($"执行 {methodName} 成功", logAttr.LogLevel);

                return result;
            }
            catch (Exception ex)
            {
                // 7. 异常日志记录 - 使用Error级别
                logger.Log($"执行 {methodName} 失败：{ex.Message}", LogLevel.Error);
                
                // 8. 重新抛出异常，保持原有异常语义
                throw;
            }
        }
    }

    #endregion
}
