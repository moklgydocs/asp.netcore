// 场景5：任务调度（不同触发器类型）
// 实现"特性+工厂模式+管理器+扩展方法"编程范式
// 用于支持不同触发策略的任务调度系统

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario5_Scheduling
{
    #region 1. 核心枚举定义

    /// <summary>
    /// 触发器类型枚举
    /// 定义支持的不同触发器类型
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// 固定间隔触发器 - 按固定时间间隔执行
        /// </summary>
        FixedInterval,
        
        /// <summary>
        /// CRON表达式触发器 - 按CRON表达式执行
        /// </summary>
        Cron,
        
        /// <summary>
        /// 一次性触发器 - 只执行一次
        /// </summary>
        OneTime,
        
        /// <summary>
        /// 条件触发器 - 满足特定条件时执行
        /// </summary>
        Conditional
    }

    #endregion

    #region 2. 核心接口定义

    /// <summary>
    /// 任务触发器接口（抽象产品）
    /// 定义触发器的核心行为
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// 触发器名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 触发器类型
        /// </summary>
        TriggerType Type { get; }
        
        /// <summary>
        /// 是否已启动
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// 启动触发器
        /// </summary>
        /// <param name="action">要执行的任务</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task StartAsync(Action action, CancellationToken cancellationToken);
        
        /// <summary>
        /// 停止触发器
        /// </summary>
        void Stop();
        
        /// <summary>
        /// 获取下次执行时间
        /// </summary>
        /// <returns>下次执行时间，如果无法确定则返回null</returns>
        DateTime? GetNextExecutionTime();
    }

    /// <summary>
    /// 触发器工厂接口（抽象工厂）
    /// 负责创建不同类型的触发器
    /// </summary>
    public interface ITriggerFactory
    {
        /// <summary>
        /// 创建触发器
        /// </summary>
        /// <returns>触发器实例</returns>
        ITrigger CreateTrigger();
    }

    #endregion

    #region 3. 具体触发器实现（具体产品）

    /// <summary>
    /// 固定间隔触发器实现
    /// 按固定时间间隔重复执行任务
    /// </summary>
    public class FixedIntervalTrigger : ITrigger
    {
        private readonly TimeSpan _interval;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning;

        public string Name { get; }
        public TriggerType Type => TriggerType.FixedInterval;

        public bool IsRunning => _isRunning;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="interval">执行间隔</param>
        public FixedIntervalTrigger(string name, TimeSpan interval)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _interval = interval > TimeSpan.Zero ? interval : throw new ArgumentException("间隔必须大于零", nameof(interval));
        }

        public async Task StartAsync(Action action, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_lock)
            {
                if (_isRunning)
                    throw new InvalidOperationException("触发器已经在运行中");

                _isRunning = true;
            }

            try
            {
                // 使用异步方式启动定时器
                await Task.Run(() =>
                {
                    _timer = new Timer(_ =>
                    {
                        try
                        {
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                action();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"固定间隔触发器 '{Name}' 执行任务时发生异常: {ex.Message}");
                        }
                    }, null, TimeSpan.Zero, _interval);

                    // 等待取消令牌或直到停止
                    cancellationToken.WaitHandle.WaitOne();
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                
                _isRunning = false;
                
                _timer?.Dispose();
                _timer = null;
                
                Console.WriteLine($"固定间隔触发器 '{Name}' 已停止");
            }
        }

        public DateTime? GetNextExecutionTime()
        {
            return _isRunning ? DateTime.Now.Add(_interval) : (DateTime?)null;
        }
    }

    /// <summary>
    /// CRON表达式触发器实现
    /// 按CRON表达式定义的时间规则执行任务
    /// </summary>
    public class CronTrigger : ITrigger
    {
        private readonly string _cronExpression;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning;

        public string Name { get; }
        public TriggerType Type => TriggerType.Cron;

        public bool IsRunning => _isRunning;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="cronExpression">CRON表达式</param>
        public CronTrigger(string name, string cronExpression)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _cronExpression = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
            
            // 简单验证CRON表达式格式（这里只做基本检查）
            if (string.IsNullOrWhiteSpace(cronExpression) || cronExpression.Split(' ').Length != 5)
            {
                throw new ArgumentException("CRON表达式格式无效，应为 '分 时 日 月 星期' 格式", nameof(cronExpression));
            }
        }

        public async Task StartAsync(Action action, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_lock)
            {
                if (_isRunning)
                    throw new InvalidOperationException("触发器已经在运行中");

                _isRunning = true;
            }

            try
            {
                await Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested && _isRunning)
                    {
                        var nextExecution = CalculateNextExecutionTime();
                        var delay = nextExecution - DateTime.Now;

                        if (delay > TimeSpan.Zero)
                        {
                            // 等待到下次执行时间
                            var waitTask = Task.Delay(delay, cancellationToken);
                            waitTask.Wait(cancellationToken);
                        }

                        if (!cancellationToken.IsCancellationRequested && _isRunning)
                        {
                            try
                            {
                                action();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"CRON触发器 '{Name}' 执行任务时发生异常: {ex.Message}");
                            }
                        }
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                
                _isRunning = false;
                
                _timer?.Dispose();
                _timer = null;
                
                Console.WriteLine($"CRON触发器 '{Name}' 已停止");
            }
        }

        public DateTime? GetNextExecutionTime()
        {
            return _isRunning ? CalculateNextExecutionTime() : (DateTime?)null;
        }

        /// <summary>
        /// 计算下次执行时间（简化实现）
        /// 实际项目中可以使用更强大的CRON解析库如NCrontab
        /// </summary>
        /// <returns>下次执行时间</returns>
        private DateTime CalculateNextExecutionTime()
        {
            // 简化实现：解析CRON表达式并计算下次执行时间
            // 这里只处理简单的星号和数字
            var parts = _cronExpression.Split(' ');
            if (parts.Length != 5) return DateTime.Now.AddMinutes(1);

            var now = DateTime.Now;
            var next = now;

            try
            {
                // 解析分钟
                if (parts[0] != "*")
                {
                    var minute = int.Parse(parts[0]);
                    if (minute >= 0 && minute <= 59)
                    {
                        next = next.Minute < minute ? 
                            next.Date.AddHours(next.Hour).AddMinutes(minute) :
                            next.Date.AddHours(next.Hour).AddMinutes(minute).AddHours(1);
                    }
                }

                // 解析小时
                if (parts[1] != "*")
                {
                    var hour = int.Parse(parts[1]);
                    if (hour >= 0 && hour <= 23)
                    {
                        next = next.Hour < hour ? 
                            next.Date.AddHours(hour) :
                            next.Date.AddDays(1).AddHours(hour);
                    }
                }

                // 如果计算出的时间已过，则加一天
                if (next <= now)
                {
                    next = next.AddDays(1);
                }
            }
            catch
            {
                // 解析失败，使用默认值
                next = now.AddMinutes(1);
            }

            return next;
        }
    }

    /// <summary>
    /// 一次性触发器实现
    /// 只执行一次任务
    /// </summary>
    public class OneTimeTrigger : ITrigger
    {
        private readonly DateTime _executionTime;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning;

        public string Name { get; }
        public TriggerType Type => TriggerType.OneTime;

        public bool IsRunning => _isRunning;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="executionTime">执行时间</param>
        public OneTimeTrigger(string name, DateTime executionTime)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _executionTime = executionTime;
        }

        public async Task StartAsync(Action action, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_lock)
            {
                if (_isRunning)
                    throw new InvalidOperationException("触发器已经在运行中");

                _isRunning = true;
            }

            try
            {
                await Task.Run(() =>
                {
                    var delay = _executionTime - DateTime.Now;
                    
                    if (delay > TimeSpan.Zero)
                    {
                        // 等待到执行时间
                        var waitTask = Task.Delay(delay, cancellationToken);
                        waitTask.Wait(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested && _isRunning)
                    {
                        try
                        {
                            action();
                            Console.WriteLine($"一次性触发器 '{Name}' 已执行");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"一次性触发器 '{Name}' 执行任务时发生异常: {ex.Message}");
                        }
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                
                _isRunning = false;
                
                _timer?.Dispose();
                _timer = null;
                
                Console.WriteLine($"一次性触发器 '{Name}' 已停止");
            }
        }

        public DateTime? GetNextExecutionTime()
        {
            return _isRunning ? _executionTime : (DateTime?)null;
        }
    }

    /// <summary>
    /// 条件触发器实现
    /// 当满足特定条件时执行任务
    /// </summary>
    public class ConditionalTrigger : ITrigger
    {
        private readonly Func<bool> _condition;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning;

        public string Name { get; }
        public TriggerType Type => TriggerType.Conditional;

        public bool IsRunning => _isRunning;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="condition">执行条件函数</param>
        public ConditionalTrigger(string name, Func<bool> condition)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public async Task StartAsync(Action action, CancellationToken cancellationToken)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_lock)
            {
                if (_isRunning)
                    throw new InvalidOperationException("触发器已经在运行中");

                _isRunning = true;
            }

            try
            {
                await Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested && _isRunning)
                    {
                        // 检查条件
                        if (_condition())
                        {
                            try
                            {
                                action();
                                Console.WriteLine($"条件触发器 '{Name}' 已执行（条件满足）");
                                break; // 执行一次后退出
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"条件触发器 '{Name}' 执行任务时发生异常: {ex.Message}");
                            }
                        }

                        // 等待一段时间后再次检查
                        var waitTask = Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                        waitTask.Wait(cancellationToken);
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            finally
            {
                Stop();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                
                _isRunning = false;
                
                _timer?.Dispose();
                _timer = null;
                
                Console.WriteLine($"条件触发器 '{Name}' 已停止");
            }
        }

        public DateTime? GetNextExecutionTime()
        {
            return _isRunning ? DateTime.Now.AddSeconds(5) : (DateTime?)null;
        }
    }

    #endregion

    #region 4. 触发器工厂实现（具体工厂）

    /// <summary>
    /// 固定间隔触发器工厂
    /// </summary>
    public class FixedIntervalTriggerFactory : ITriggerFactory
    {
        private readonly string _name;
        private readonly TimeSpan _interval;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="interval">执行间隔</param>
        public FixedIntervalTriggerFactory(string name, TimeSpan interval)
        {
            _name = name;
            _interval = interval;
        }

        public ITrigger CreateTrigger()
        {
            return new FixedIntervalTrigger(_name, _interval);
        }
    }

    /// <summary>
    /// CRON触发器工厂
    /// </summary>
    public class CronTriggerFactory : ITriggerFactory
    {
        private readonly string _name;
        private readonly string _cronExpression;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="cronExpression">CRON表达式</param>
        public CronTriggerFactory(string name, string cronExpression)
        {
            _name = name;
            _cronExpression = cronExpression;
        }

        public ITrigger CreateTrigger()
        {
            return new CronTrigger(_name, _cronExpression);
        }
    }

    /// <summary>
    /// 一次性触发器工厂
    /// </summary>
    public class OneTimeTriggerFactory : ITriggerFactory
    {
        private readonly string _name;
        private readonly DateTime _executionTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="executionTime">执行时间</param>
        public OneTimeTriggerFactory(string name, DateTime executionTime)
        {
            _name = name;
            _executionTime = executionTime;
        }

        public ITrigger CreateTrigger()
        {
            return new OneTimeTrigger(_name, _executionTime);
        }
    }

    /// <summary>
    /// 条件触发器工厂
    /// </summary>
    public class ConditionalTriggerFactory : ITriggerFactory
    {
        private readonly string _name;
        private readonly Func<bool> _condition;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">触发器名称</param>
        /// <param name="condition">执行条件函数</param>
        public ConditionalTriggerFactory(string name, Func<bool> condition)
        {
            _name = name;
            _condition = condition;
        }

        public ITrigger CreateTrigger()
        {
            return new ConditionalTrigger(_name, _condition);
        }
    }

    #endregion

    #region 5. 触发器管理器（工厂管理器）

    /// <summary>
    /// 触发器管理器
    /// 统一管理所有触发器工厂，提供触发器创建和管理功能
    /// </summary>
    public static class TriggerManager
    {
        // 工厂注册字典
        private static readonly Dictionary<TriggerType, Func<string, object[], ITriggerFactory>> _factoryCreators = new()
        {
            { TriggerType.FixedInterval, (name, args) => new FixedIntervalTriggerFactory(name, (TimeSpan)args[0]) },
            { TriggerType.Cron, (name, args) => new CronTriggerFactory(name, (string)args[0]) },
            { TriggerType.OneTime, (name, args) => new OneTimeTriggerFactory(name, (DateTime)args[0]) },
            { TriggerType.Conditional, (name, args) => new ConditionalTriggerFactory(name, (Func<bool>)args[0]) }
        };

        /// <summary>
        /// 创建触发器工厂
        /// </summary>
        /// <param name="triggerType">触发器类型</param>
        /// <param name="name">触发器名称</param>
        /// <param name="factoryArgs">工厂参数</param>
        /// <returns>触发器工厂</returns>
        public static ITriggerFactory CreateFactory(TriggerType triggerType, string name, params object[] factoryArgs)
        {
            if (!_factoryCreators.TryGetValue(triggerType, out var creator))
            {
                throw new NotSupportedException($"不支持的触发器类型: {triggerType}");
            }

            return creator(name, factoryArgs);
        }

        /// <summary>
        /// 创建触发器实例
        /// </summary>
        /// <param name="triggerType">触发器类型</param>
        /// <param name="name">触发器名称</param>
        /// <param name="factoryArgs">工厂参数</param>
        /// <returns>触发器实例</returns>
        public static ITrigger CreateTrigger(TriggerType triggerType, string name, params object[] factoryArgs)
        {
            var factory = CreateFactory(triggerType, name, factoryArgs);
            return factory.CreateTrigger();
        }

        /// <summary>
        /// 获取支持的触发器类型列表
        /// </summary>
        /// <returns>支持的触发器类型</returns>
        public static IEnumerable<TriggerType> GetSupportedTriggerTypes()
        {
            return _factoryCreators.Keys;
        }

        /// <summary>
        /// 验证触发器类型是否支持
        /// </summary>
        /// <param name="triggerType">触发器类型</param>
        /// <returns>是否支持</returns>
        public static bool IsTriggerTypeSupported(TriggerType triggerType)
        {
            return _factoryCreators.ContainsKey(triggerType);
        }
    }

    #endregion

    #region 6. 任务调度特性（元数据标记）

    /// <summary>
    /// 任务调度特性
    /// 用于标记方法的调度配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScheduleAttribute : Attribute
    {
        /// <summary>
        /// 触发器类型
        /// </summary>
        public TriggerType TriggerType { get; }

        /// <summary>
        /// 执行间隔（仅用于FixedInterval触发器）
        /// </summary>
        public TimeSpan? Interval { get; set; }

        /// <summary>
        /// CRON表达式（仅用于Cron触发器）
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// 执行时间（仅用于OneTime触发器）
        /// </summary>
        public DateTime? ExecutionTime { get; set; }

        /// <summary>
        /// 条件函数（仅用于Conditional触发器）
        /// </summary>
        public Func<bool> Condition { get; set; }

        /// <summary>
        /// 是否自动启动
        /// </summary>
        public bool AutoStart { get; set; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="triggerType">触发器类型</param>
        public ScheduleAttribute(TriggerType triggerType)
        {
            TriggerType = triggerType;
        }
    }

    #endregion

    #region 7. 任务调度扩展方法（简化使用）

    /// <summary>
    /// 任务调度扩展方法
    /// 为对象提供便捷的任务调度功能
    /// </summary>
    public static class ScheduleExtensions
    {
        /// <summary>
        /// 为方法创建调度任务
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>触发器实例</returns>
        public static async Task<ITrigger> ScheduleMethodAsync(this object service, string methodName, CancellationToken cancellationToken = default)
        {
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
                throw new ArgumentException($"方法 '{methodName}' 在类型 '{service.GetType().Name}' 中不存在");

            // 读取调度特性
            var scheduleAttr = method.GetCustomAttribute<ScheduleAttribute>();
            if (scheduleAttr == null)
                throw new InvalidOperationException($"方法 '{methodName}' 未标记 [Schedule] 特性");

            // 创建触发器
            var trigger = CreateTriggerFromAttribute(scheduleAttr);

            // 如果需要自动启动，则启动触发器
            if (scheduleAttr.AutoStart)
            {
                // 创建包装方法，包含异常处理
                Action wrappedAction = () =>
                {
                    try
                    {
                        method.Invoke(service, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"调度任务执行失败: {ex.InnerException?.Message ?? ex.Message}");
                    }
                };

                // 启动触发器
                _ = trigger.StartAsync(wrappedAction, cancellationToken);
            }

            return trigger;
        }

        /// <summary>
        /// 根据特性创建触发器
        /// </summary>
        /// <param name="attribute">调度特性</param>
        /// <returns>触发器实例</returns>
        private static ITrigger CreateTriggerFromAttribute(ScheduleAttribute attribute)
        {
            var methodName = "Unknown";
            var triggerName = $"Trigger_{methodName}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            switch (attribute.TriggerType)
            {
                case TriggerType.FixedInterval:
                    if (attribute.Interval == null)
                        throw new InvalidOperationException("FixedInterval触发器必须指定Interval参数");
                    return TriggerManager.CreateTrigger(TriggerType.FixedInterval, triggerName, attribute.Interval.Value);

                case TriggerType.Cron:
                    if (string.IsNullOrEmpty(attribute.CronExpression))
                        throw new InvalidOperationException("Cron触发器必须指定CronExpression参数");
                    return TriggerManager.CreateTrigger(TriggerType.Cron, triggerName, attribute.CronExpression);

                case TriggerType.OneTime:
                    if (attribute.ExecutionTime == null)
                        throw new InvalidOperationException("OneTime触发器必须指定ExecutionTime参数");
                    return TriggerManager.CreateTrigger(TriggerType.OneTime, triggerName, attribute.ExecutionTime.Value);

                case TriggerType.Conditional:
                    if (attribute.Condition == null)
                        throw new InvalidOperationException("Conditional触发器必须指定Condition参数");
                    return TriggerManager.CreateTrigger(TriggerType.Conditional, triggerName, attribute.Condition);

                default:
                    throw new NotSupportedException($"不支持的触发器类型: {attribute.TriggerType}");
            }
        }

        /// <summary>
        /// 批量调度多个方法
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="methodNames">方法名称列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>触发器列表</returns>
        public static async Task<List<ITrigger>> ScheduleMethodsAsync(this object service, IEnumerable<string> methodNames, CancellationToken cancellationToken = default)
        {
            var triggers = new List<ITrigger>();

            foreach (var methodName in methodNames)
            {
                var trigger = await service.ScheduleMethodAsync(methodName, cancellationToken);
                triggers.Add(trigger);
            }

            return triggers;
        }

        /// <summary>
        /// 调度所有标记了Schedule特性的方法
        /// </summary>
        /// <param name="service">服务实例</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>触发器列表</returns>
        public static async Task<List<ITrigger>> ScheduleAllMethodsAsync(this object service, CancellationToken cancellationToken = default)
        {
            var methods = service.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ScheduleAttribute>() != null)
                .Select(m => m.Name);

            return await service.ScheduleMethodsAsync(methods, cancellationToken);
        }
    }

    #endregion
}
