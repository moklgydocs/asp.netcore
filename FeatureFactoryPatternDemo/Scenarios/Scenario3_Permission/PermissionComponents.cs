using System;
using System.Collections.Generic;
using System.Reflection;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario3_Permission
{
    /// <summary>
    /// 权限验证场景 - 展示如何使用特性+工厂模式+管理器+扩展方法实现方法级权限控制
    /// 
    /// 设计思路：
    /// 1. 特性标记：[RequirePermission("Order.Create", UseRemoteService = true)] 声明方法所需权限
    /// 2. 工厂模式：LocalPermissionFactory、RemotePermissionFactory 分别创建本地和远程验证器
    /// 3. 管理器：PermissionManager 统一管理工厂注册和验证器获取
    /// 4. 扩展方法：ExecuteWithPermissionCheck 自动读取特性并执行权限验证
    /// 
    /// 业务价值：
    /// - 声明式权限控制：通过特性标记权限，代码更清晰
    /// - 策略解耦：本地验证和远程验证可以独立切换
    /// - AOP思想：权限验证逻辑与业务逻辑分离
    /// - 易于测试：可以模拟不同验证策略
    /// </summary>
    
    #region 1. 权限类型定义和核心接口

    /// <summary>
    /// 权限验证结果枚举
    /// </summary>
    public enum PermissionResult
    {
        /// <summary>
        /// 权限验证通过
        /// </summary>
        Granted,
        
        /// <summary>
        /// 权限验证拒绝
        /// </summary>
        Denied,
        
        /// <summary>
        /// 验证过程中发生错误
        /// </summary>
        Error
    }

    /// <summary>
    /// 权限验证接口 - 定义权限验证的核心行为
    /// 这是工厂模式中的"抽象产品"，所有具体验证实现都必须实现此接口
    /// </summary>
    public interface IPermissionValidator
    {
        /// <summary>
        /// 验证用户是否具有指定权限
        /// </summary>
        /// <param name="permissionName">权限名称，如 "Order.Create"</param>
        /// <param name="userId">用户ID</param>
        /// <param name="resourceId">可选的资源ID，用于基于资源的权限验证</param>
        /// <returns>权限验证结果</returns>
        PermissionResult Validate(string permissionName, string userId, string resourceId = null);
        
        /// <summary>
        /// 获取验证器的名称，用于日志记录和调试
        /// </summary>
        string ValidatorName { get; }
    }

    #endregion

    #region 2. 具体权限验证实现（具体产品）

    /// <summary>
    /// 本地角色权限验证器 - 基于角色的访问控制（RBAC）
    /// 适用于简单的权限系统，权限数据存储在本地内存或数据库中
    /// </summary>
    public class LocalPermissionValidator : IPermissionValidator
    {
        /// <summary>
        /// 模拟的用户角色映射表
        /// 实际项目中这通常来自数据库或缓存
        /// </summary>
        private readonly Dictionary<string, List<string>> _userRoles = new()
        {
            { "admin", new List<string> { "Administrator" } },
            { "user1", new List<string> { "User" } },
            { "manager", new List<string> { "Manager", "User" } }
        };

        /// <summary>
        /// 模拟的角色权限映射表
        /// 实际项目中这通常来自配置文件或数据库
        /// </summary>
        private readonly Dictionary<string, List<string>> _rolePermissions = new()
        {
            { "Administrator", new List<string> { "Order.Create", "Order.Cancel", "User.Manage", "Report.View" } },
            { "Manager", new List<string> { "Order.Create", "Order.Cancel", "Report.View" } },
            { "User", new List<string> { "Order.Create", "Report.View" } }
        };

        public string ValidatorName => "LocalPermissionValidator";

        public PermissionResult Validate(string permissionName, string userId, string resourceId = null)
        {
            try
            {
                Console.WriteLine($"[本地验证] 正在验证用户 '{userId}' 是否具有权限 '{permissionName}'");

                // 检查用户是否存在
                if (!_userRoles.TryGetValue(userId, out var userRoles))
                {
                    Console.WriteLine($"[本地验证] 用户 '{userId}' 不存在");
                    return PermissionResult.Denied;
                }

                // 检查用户角色是否具有所需权限
                foreach (var role in userRoles)
                {
                    if (_rolePermissions.TryGetValue(role, out var permissions) && 
                        permissions.Contains(permissionName))
                    {
                        Console.WriteLine($"[本地验证] 用户 '{userId}' 通过角色 '{role}' 获得权限 '{permissionName}'");
                        return PermissionResult.Granted;
                    }
                }

                Console.WriteLine($"[本地验证] 用户 '{userId}' 的角色没有权限 '{permissionName}'");
                return PermissionResult.Denied;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[本地验证] 验证过程中发生错误: {ex.Message}");
                return PermissionResult.Error;
            }
        }
    }

    /// <summary>
    /// 远程权限服务验证器 - 调用外部权限服务
    /// 适用于微服务架构，权限验证由专门的权限服务处理
    /// </summary>
    public class RemotePermissionValidator : IPermissionValidator
    {
        /// <summary>
        /// 模拟的远程服务端点
        /// 实际项目中这可能是 REST API 或 gRPC 服务
        /// </summary>
        private readonly string _serviceEndpoint;

        public string ValidatorName => "RemotePermissionValidator";

        public RemotePermissionValidator(string serviceEndpoint = "https://permission-service/api")
        {
            _serviceEndpoint = serviceEndpoint;
        }

        public PermissionResult Validate(string permissionName, string userId, string resourceId = null)
        {
            try
            {
                Console.WriteLine($"[远程验证] 正在调用权限服务 '{_serviceEndpoint}' 验证用户 '{userId}' 是否具有权限 '{permissionName}'");
                
                // 模拟网络延迟
                System.Threading.Thread.Sleep(100);
                
                // 模拟远程服务响应
                // 实际项目中这里会调用真实的远程API
                var random = new Random();
                var successRate = 0.8; // 80%的成功率模拟网络和服务的不确定性
                
                if (random.NextDouble() < successRate)
                {
                    Console.WriteLine($"[远程验证] 权限服务返回：用户 '{userId}' 具有权限 '{permissionName}'");
                    return PermissionResult.Granted;
                }
                else
                {
                    Console.WriteLine($"[远程验证] 权限服务返回：用户 '{userId}' 不具有权限 '{permissionName}'");
                    return PermissionResult.Denied;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[远程验证] 调用权限服务时发生错误: {ex.Message}");
                return PermissionResult.Error;
            }
        }
    }

    #endregion

    #region 3. 权限工厂接口和具体工厂（工厂模式）

    /// <summary>
    /// 权限验证工厂接口 - 工厂模式中的"抽象工厂"
    /// 定义创建权限验证器的契约
    /// </summary>
    public interface IPermissionFactory
    {
        /// <summary>
        /// 创建权限验证器实例
        /// </summary>
        /// <returns>权限验证器实例</returns>
        IPermissionValidator CreateValidator();
    }

    /// <summary>
    /// 本地权限验证工厂 - 工厂模式中的"具体工厂"
    /// 负责创建本地权限验证器
    /// </summary>
    public class LocalPermissionFactory : IPermissionFactory
    {
        public IPermissionValidator CreateValidator()
        {
            return new LocalPermissionValidator();
        }
    }

    /// <summary>
    /// 远程权限验证工厂 - 工厂模式中的"具体工厂"
    /// 负责创建远程权限验证器
    /// </summary>
    public class RemotePermissionFactory : IPermissionFactory
    {
        private readonly string _serviceEndpoint;

        public RemotePermissionFactory(string serviceEndpoint = null)
        {
            _serviceEndpoint = serviceEndpoint;
        }

        public IPermissionValidator CreateValidator()
        {
            return new RemotePermissionValidator(_serviceEndpoint);
        }
    }

    #endregion

    #region 4. 权限管理器（Manager）

    /// <summary>
    /// 权限验证管理器 - 负责工厂注册和验证器获取
    /// 这是系统的统一入口，实现了"管理器"模式
    /// </summary>
    public static class PermissionManager
    {
        /// <summary>
        /// 工厂注册表 - 存储不同类型的工厂创建函数
        /// 使用 Func<IPermissionFactory> 而不是直接存储工厂实例，
        /// 这样可以在需要时动态创建工厂，避免不必要的内存占用
        /// </summary>
        private static readonly Dictionary<string, Func<IPermissionFactory>> _factoryCreators = new()
        {
            { "local", () => new LocalPermissionFactory() },
            { "remote", () => new RemotePermissionFactory() }
        };

        /// <summary>
        /// 注册新的权限验证工厂
        /// 这个方法支持系统的扩展性，可以在运行时添加新的验证策略
        /// </summary>
        /// <param name="type">工厂类型标识</param>
        /// <param name="factoryCreator">工厂创建函数</param>
        public static void RegisterFactory(string type, Func<IPermissionFactory> factoryCreator)
        {
            _factoryCreators[type] = factoryCreator;
            Console.WriteLine($"已注册权限验证工厂: {type}");
        }

        /// <summary>
        /// 获取指定类型的权限验证器
        /// 这是系统的统一入口方法，隐藏了工厂创建的复杂性
        /// </summary>
        /// <param name="type">验证器类型 ("local" 或 "remote")</param>
        /// <returns>权限验证器实例</returns>
        /// <exception cref="NotSupportedException">当指定的类型不支持时抛出</exception>
        public static IPermissionValidator GetValidator(string type)
        {
            if (_factoryCreators.TryGetValue(type, out var factoryCreator))
            {
                var factory = factoryCreator();
                return factory.CreateValidator();
            }
            
            throw new NotSupportedException($"不支持的权限验证类型: {type}");
        }

        /// <summary>
        /// 获取所有可用的验证器类型
        /// 用于调试和配置验证
        /// </summary>
        /// <returns>可用的验证器类型列表</returns>
        public static IEnumerable<string> GetAvailableTypes()
        {
            return _factoryCreators.Keys;
        }
    }

    #endregion

    #region 5. 权限特性（Attribute）

    /// <summary>
    /// 权限验证特性 - 用于标记需要权限验证的方法
    /// 这是"元数据标记"的核心，将权限要求声明在方法上
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute
    {
        /// <summary>
        /// 所需的权限名称，如 "Order.Create"、"User.Delete" 等
        /// 这是权限验证的核心标识
        /// </summary>
        public string PermissionName { get; }

        /// <summary>
        /// 是否使用远程权限服务
        /// true = 使用远程验证，false = 使用本地验证
        /// 这个设计允许同一个方法在不同环境下使用不同的验证策略
        /// </summary>
        public bool UseRemoteService { get; set; } = false;

        /// <summary>
        /// 是否启用权限缓存（针对远程验证）
        /// 可以避免频繁调用远程服务
        /// </summary>
        public bool EnableCache { get; set; } = false;

        /// <summary>
        /// 缓存过期时间（秒），仅当 EnableCache = true 时有效
        /// </summary>
        public int CacheExpirationSeconds { get; set; } = 300; // 默认5分钟

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        public RequirePermissionAttribute(string permissionName)
        {
            PermissionName = permissionName ?? throw new ArgumentNullException(nameof(permissionName));
        }

        /// <summary>
        /// 带验证策略的构造函数
        /// </summary>
        /// <param name="permissionName">权限名称</param>
        /// <param name="useRemoteService">是否使用远程服务</param>
        public RequirePermissionAttribute(string permissionName, bool useRemoteService) : this(permissionName)
        {
            UseRemoteService = useRemoteService;
        }
    }

    #endregion

    #region 6. 权限验证扩展方法（Extension Methods）

    /// <summary>
    /// 权限验证扩展方法 - 为所有对象提供统一的权限验证接口
    /// 这是"扩展方法"模式的应用，让调用方无需关心内部实现细节
    /// </summary>
    public static class PermissionExtensions
    {
        /// <summary>
        /// 执行方法前进行权限验证的扩展方法
        /// 这是AOP（面向切面编程）思想的体现，将权限验证作为横切关注点
        /// </summary>
        /// <typeparam name="T">方法返回值类型</typeparam>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">要执行的方法名</param>
        /// <param name="userId">当前用户ID</param>
        /// <param name="parameters">方法参数</param>
        /// <returns>方法执行结果</returns>
        /// <exception cref="InvalidOperationException">当方法没有标记权限特性时</exception>
        /// <exception cref="UnauthorizedAccessException">当权限验证失败时</exception>
        public static T ExecuteWithPermissionCheck<T>(this object service, string methodName, string userId, params object[] parameters)
        {
            // 1. 通过反射获取方法信息
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"方法 '{methodName}' 在类型 '{service.GetType().Name}' 中不存在");
            }

            // 2. 读取权限特性
            var permissionAttr = method.GetCustomAttribute<RequirePermissionAttribute>();
            if (permissionAttr == null)
            {
                throw new InvalidOperationException($"方法 '{methodName}' 未标记 [RequirePermission] 特性");
            }

            // 3. 执行权限验证
            var validationResult = ValidatePermission(permissionAttr, userId);
            
            // 4. 根据验证结果决定是否执行方法
            if (validationResult != PermissionResult.Granted)
            {
                throw new UnauthorizedAccessException(
                    $"用户 '{userId}' 无权限执行 '{permissionAttr.PermissionName}' 操作。" +
                    $"验证结果: {validationResult}");
            }

            // 5. 权限验证通过，执行原方法
            Console.WriteLine($"[权限验证] 用户 '{userId}' 权限验证通过，正在执行方法 '{methodName}'");
            
            try
            {
                var result = (T)method.Invoke(service, parameters);
                Console.WriteLine($"[权限验证] 方法 '{methodName}' 执行成功");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[权限验证] 方法 '{methodName}' 执行失败: {ex.InnerException?.Message ?? ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 重载版本：支持无返回值的方法
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">要执行的方法名</param>
        /// <param name="userId">当前用户ID</param>
        /// <param name="parameters">方法参数</param>
        public static void ExecuteWithPermissionCheck(this object service, string methodName, string userId, params object[] parameters)
        {
            ExecuteWithPermissionCheck<object>(service, methodName, userId, parameters);
        }

        /// <summary>
        /// 执行权限验证的核心逻辑
        /// </summary>
        /// <param name="permissionAttr">权限特性</param>
        /// <param name="userId">用户ID</param>
        /// <returns>验证结果</returns>
        private static PermissionResult ValidatePermission(RequirePermissionAttribute permissionAttr, string userId)
        {
            Console.WriteLine($"\n=== 权限验证开始 ===");
            Console.WriteLine($"权限要求: {permissionAttr.PermissionName}");
            Console.WriteLine($"使用远程服务: {permissionAttr.UseRemoteService}");
            Console.WriteLine($"启用缓存: {permissionAttr.EnableCache}");

            // 选择验证器类型
            string validatorType = permissionAttr.UseRemoteService ? "remote" : "local";
            
            // 获取验证器
            var validator = PermissionManager.GetValidator(validatorType);
            Console.WriteLine($"使用的验证器: {validator.ValidatorName}");

            // 执行验证
            var result = validator.Validate(permissionAttr.PermissionName, userId);
            
            Console.WriteLine($"验证结果: {result}");
            Console.WriteLine($"=== 权限验证结束 ===\n");
            
            return result;
        }
    }

    #endregion
}
