// 场景5：任务调度演示程序
// 展示"特性+工厂模式+管理器+扩展方法"范式在任务调度中的应用

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario5_Scheduling
{
    /// <summary>
    /// 任务调度场景演示类
    /// 提供完整的任务调度功能演示
    /// </summary>
    public class Scenario5Demo
    {
        private readonly ScheduledTaskService _taskService;
        private readonly List<ITrigger> _activeTriggers;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Scenario5Demo()
        {
            _taskService = new ScheduledTaskService();
            _activeTriggers = new List<ITrigger>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 运行场景5的完整演示
        /// </summary>
        public async Task RunAsync()
        {
            Console.Clear();
            Console.WriteLine("🎯 场景5：任务调度（不同触发器类型）");
            Console.WriteLine("=====================================");
            Console.WriteLine();
            Console.WriteLine("本场景演示如何使用'特性+工厂模式+管理器+扩展方法'范式");
            Console.WriteLine("实现灵活的任务调度系统，支持多种触发器类型。");
            Console.WriteLine();

            // 显示支持的触发器类型
            ShowSupportedTriggerTypes();

            // 初始化任务配置
            _taskService.InitializeTaskConfigurations();

            // 演示管理器功能
            await DemoManagerFeatures();

            // 演示单个任务调度
            await DemoIndividualTaskScheduling();

            // 演示批量任务调度
            await DemoBatchTaskScheduling();

            // 演示手动触发器创建
            await DemoManualTriggerCreation();

            // 运行交互式演示
            await RunInteractiveDemo();

            // 清理资源
            Cleanup();
        }

        /// <summary>
        /// 显示支持的触发器类型
        /// </summary>
        private void ShowSupportedTriggerTypes()
        {
            Console.WriteLine("📋 支持的触发器类型：");
            Console.WriteLine();

            var supportedTypes = TriggerManager.GetSupportedTriggerTypes().ToList();
            foreach (var triggerType in supportedTypes)
            {
                Console.WriteLine($"  {GetTriggerTypeIcon(triggerType)} {triggerType}");
                Console.WriteLine($"     {GetTriggerTypeDescription(triggerType)}");
                Console.WriteLine();
            }

            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// 获取触发器类型的图标
        /// </summary>
        private string GetTriggerTypeIcon(TriggerType triggerType)
        {
            return triggerType switch
            {
                TriggerType.FixedInterval => "⏱️",
                TriggerType.Cron => "📅",
                TriggerType.OneTime => "🎯",
                TriggerType.Conditional => "🔍",
                _ => "❓"
            };
        }

        /// <summary>
        /// 获取触发器类型的描述
        /// </summary>
        private string GetTriggerTypeDescription(TriggerType triggerType)
        {
            return triggerType switch
            {
                TriggerType.FixedInterval => "按固定时间间隔重复执行任务",
                TriggerType.Cron => "按CRON表达式定义的时间规则执行任务",
                TriggerType.OneTime => "在指定时间只执行一次任务",
                TriggerType.Conditional => "当满足特定条件时执行任务",
                _ => "未知触发器类型"
            };
        }

        /// <summary>
        /// 演示管理器功能
        /// </summary>
        private async Task DemoManagerFeatures()
        {
            Console.WriteLine("🔧 演示触发器管理器功能：");
            Console.WriteLine();

            // 检查支持的触发器类型
            Console.WriteLine("✅ 检查触发器类型支持情况：");
            var allTypes = Enum.GetValues<TriggerType>();
            foreach (var type in allTypes)
            {
                var isSupported = TriggerManager.IsTriggerTypeSupported(type);
                Console.WriteLine($"   {GetTriggerTypeIcon(type)} {type}: {(isSupported ? "✅ 支持" : "❌ 不支持")}");
            }
            Console.WriteLine();

            // 演示手动创建触发器
            Console.WriteLine("✅ 演示手动创建触发器：");

            // 创建固定间隔触发器
            var fixedTrigger = TriggerManager.CreateTrigger(
                TriggerType.FixedInterval,
                "Manual_Fixed_Interval",
                TimeSpan.FromSeconds(10)
            );
            Console.WriteLine($"   📝 创建固定间隔触发器: {fixedTrigger.Name}");
            Console.WriteLine($"   ⏱️  间隔: {fixedTrigger.GetNextExecutionTime()?.ToString("HH:mm:ss") ?? "未启动"}");

            // 创建CRON触发器
            var cronTrigger = TriggerManager.CreateTrigger(
                TriggerType.Cron,
                "Manual_Cron_Trigger",
                "0/30 * * * * *" // 每30秒执行一次
            );
            Console.WriteLine($"   📝 创建CRON触发器: {cronTrigger.Name}");
            Console.WriteLine($"   📅 下次执行: {cronTrigger.GetNextExecutionTime()?.ToString("HH:mm:ss") ?? "未启动"}");

            _activeTriggers.Add(fixedTrigger);
            _activeTriggers.Add(cronTrigger);

            Console.WriteLine();
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// 演示单个任务调度
        /// </summary>
        private async Task DemoIndividualTaskScheduling()
        {
            Console.WriteLine("🎯 演示单个任务调度：");
            Console.WriteLine();

            // 调度健康检查任务
            Console.WriteLine("📝 调度健康检查任务...");
            try
            {
                var healthTrigger = await _taskService.ScheduleMethodAsync(
                    nameof(ScheduledTaskService.HealthCheckTask),
                    _cancellationTokenSource.Token
                );
                _activeTriggers.Add(healthTrigger);
                Console.WriteLine($"✅ 健康检查任务已调度: {healthTrigger.Name}");
                Console.WriteLine($"⏱️  触发器类型: {healthTrigger.Type}");
                Console.WriteLine($"📅 下次执行: {healthTrigger.GetNextExecutionTime()?.ToString("HH:mm:ss") ?? "未知"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 调度失败: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// 演示批量任务调度
        /// </summary>
        private async Task DemoBatchTaskScheduling()
        {
            Console.WriteLine("📦 演示批量任务调度：");
            Console.WriteLine();

            var methodsToSchedule = new[]
            {
                nameof(ScheduledTaskService.DataBackupTask),
                nameof(ScheduledTaskService.DailyReportTask),
                nameof(ScheduledTaskService.WeeklyReportTask)
            };

            Console.WriteLine("📝 批量调度以下任务：");
            foreach (var method in methodsToSchedule)
            {
                Console.WriteLine($"   📋 {method}");
            }

            try
            {
                var triggers = await _taskService.ScheduleMethodsAsync(
                    methodsToSchedule,
                    _cancellationTokenSource.Token
                );

                Console.WriteLine();
                Console.WriteLine("✅ 批量调度完成：");
                foreach (var trigger in triggers)
                {
                    _activeTriggers.Add(trigger);
                    Console.WriteLine($"   🎯 {trigger.Name}");
                    Console.WriteLine($"   ⏱️  类型: {trigger.Type}");
                    Console.WriteLine($"   📅 下次执行: {trigger.GetNextExecutionTime()?.ToString("HH:mm:ss") ?? "未知"}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 批量调度失败: {ex.Message}");
            }

            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// 演示手动触发器创建
        /// </summary>
        private async Task DemoManualTriggerCreation()
        {
            Console.WriteLine("🛠️  演示手动触发器创建和管理：");
            Console.WriteLine();

            // 创建一次性触发器
            Console.WriteLine("📝 创建一次性系统维护任务...");
            var maintenanceTrigger = TriggerManager.CreateTrigger(
                TriggerType.OneTime,
                "Manual_Maintenance",
                DateTime.Now.AddMinutes(1) // 1分钟后执行
            );

            // 创建包装方法
            Action maintenanceAction = () =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔧 手动创建的系统维护任务执行中...");
                Console.WriteLine("   - 正在执行维护操作...");
                Console.WriteLine("   - 维护完成");
                Console.WriteLine();
            };

            // 启动触发器
            _ = maintenanceTrigger.StartAsync(maintenanceAction, _cancellationTokenSource.Token);
            _activeTriggers.Add(maintenanceTrigger);

            Console.WriteLine($"✅ 一次性触发器已创建并启动: {maintenanceTrigger.Name}");
            Console.WriteLine($"🎯 执行时间: {maintenanceTrigger.GetNextExecutionTime()?.ToString("HH:mm:ss")}");
            Console.WriteLine();

            // 创建条件触发器
            Console.WriteLine("📝 创建条件触发器...");
            var conditionTrigger = TriggerManager.CreateTrigger(
                TriggerType.Conditional,
                "Manual_Condition",
                () =>
                {
                    var random = new Random();
                    var value = random.Next(1, 100);
                    Console.WriteLine($"   [条件检查] 随机值: {value}");
                    return value > 80;
                }
            );

            Action conditionAction = () =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔍 条件满足，执行任务...");
                Console.WriteLine("   - 条件触发任务完成");
                Console.WriteLine();
            };

            _ = conditionTrigger.StartAsync(conditionAction, _cancellationTokenSource.Token);
            _activeTriggers.Add(conditionTrigger);

            Console.WriteLine($"✅ 条件触发器已创建并启动: {conditionTrigger.Name}");
            Console.WriteLine();

            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
            Console.WriteLine();
        }

        /// <summary>
        /// 运行交互式演示
        /// </summary>
        private async Task RunInteractiveDemo()
        {
            Console.WriteLine("🎮 交互式任务调度演示：");
            Console.WriteLine();
            Console.WriteLine("当前活跃的触发器：");
            ShowActiveTriggers();

            Console.WriteLine();
            Console.WriteLine("请选择操作：");
            Console.WriteLine("1. 查看活跃触发器状态");
            Console.WriteLine("2. 停止所有触发器");
            Console.WriteLine("3. 创建新的定时任务");
            Console.WriteLine("4. 跳过交互演示");
            Console.WriteLine();

            while (true)
            {
                var key = Console.ReadKey(true);
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.Clear();
                        ShowActiveTriggers();
                        Console.WriteLine("按任意键返回...");
                        Console.ReadKey();
                        Console.Clear();
                        ShowInteractiveMenu();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("🛑 正在停止所有触发器...");
                        StopAllTriggers();
                        Console.WriteLine("✅ 所有触发器已停止");
                        Console.WriteLine("按任意键返回...");
                        Console.ReadKey();
                        Console.Clear();
                        ShowInteractiveMenu();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        await CreateCustomTask();
                        Console.Clear();
                        ShowInteractiveMenu();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        Console.WriteLine("⏭️ 跳过交互演示");
                        return;

                    default:
                        Console.WriteLine("❌ 无效选择，请重新输入");
                        break;
                }
            }
        }

        /// <summary>
        /// 显示活跃触发器状态
        /// </summary>
        private void ShowActiveTriggers()
        {
            if (!_activeTriggers.Any())
            {
                Console.WriteLine("📭 暂无活跃触发器");
                return;
            }

            Console.WriteLine($"📊 当前有 {_activeTriggers.Count} 个活跃触发器：");
            Console.WriteLine();

            for (int i = 0; i < _activeTriggers.Count; i++)
            {
                var trigger = _activeTriggers[i];
                Console.WriteLine($"  {i + 1}. {GetTriggerTypeIcon(trigger.Type)} {trigger.Name}");
                Console.WriteLine($"     状态: {(trigger.IsRunning ? "🟢 运行中" : "🔴 已停止")}");
                Console.WriteLine($"     类型: {trigger.Type}");
                Console.WriteLine($"     下次执行: {trigger.GetNextExecutionTime()?.ToString("HH:mm:ss") ?? "未知"}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 显示交互式菜单
        /// </summary>
        private void ShowInteractiveMenu()
        {
            Console.WriteLine("🎯 交互式任务调度演示：");
            Console.WriteLine();
            Console.WriteLine("当前活跃的触发器：");
            ShowActiveTriggers();

            Console.WriteLine();
            Console.WriteLine("请选择操作：");
            Console.WriteLine("1. 查看活跃触发器状态");
            Console.WriteLine("2. 停止所有触发器");
            Console.WriteLine("3. 创建新的定时任务");
            Console.WriteLine("4. 跳过交互演示");
            Console.WriteLine();
        }

        /// <summary>
        /// 停止所有触发器
        /// </summary>
        private void StopAllTriggers()
        {
            foreach (var trigger in _activeTriggers)
            {
                if (trigger.IsRunning)
                {
                    trigger.Stop();
                }
            }
            _activeTriggers.Clear();
        }

        /// <summary>
        /// 创建自定义任务
        /// </summary>
        private async Task CreateCustomTask()
        {
            Console.WriteLine("📝 创建自定义定时任务：");
            Console.WriteLine();

            Console.WriteLine("请选择触发器类型：");
            Console.WriteLine("1. 固定间隔 (FixedInterval)");
            Console.WriteLine("2. CRON表达式 (Cron)");
            Console.WriteLine("3. 一次性 (OneTime)");
            Console.WriteLine("4. 条件触发 (Conditional)");

            var key = Console.ReadKey(true);
            Console.WriteLine();

            ITrigger trigger = null;

            switch (key.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine("⏱️  设置固定间隔（秒）:");
                    if (int.TryParse(Console.ReadLine(), out int intervalSeconds))
                    {
                        trigger = TriggerManager.CreateTrigger(
                            TriggerType.FixedInterval,
                            $"Custom_Fixed_{DateTime.Now:HHmmss}",
                            TimeSpan.FromSeconds(intervalSeconds)
                        );
                    }
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.WriteLine("📅 输入CRON表达式（例如：0/30 * * * * * 每30秒）:");
                    var cronExpr = Console.ReadLine();
                    if (!string.IsNullOrEmpty(cronExpr))
                    {
                        trigger = TriggerManager.CreateTrigger(
                            TriggerType.Cron,
                            $"Custom_Cron_{DateTime.Now:HHmmss}",
                            cronExpr
                        );
                    }
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.WriteLine("🎯 设置执行时间（分钟后）:");
                    if (int.TryParse(Console.ReadLine(), out int minutes))
                    {
                        trigger = TriggerManager.CreateTrigger(
                            TriggerType.OneTime,
                            $"Custom_OneTime_{DateTime.Now:HHmmss}",
                            DateTime.Now.AddMinutes(minutes)
                        );
                    }
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    trigger = TriggerManager.CreateTrigger(
                        TriggerType.Conditional,
                        $"Custom_Condition_{DateTime.Now:HHmmss}",
                        () =>
                        {
                            var random = new Random();
                            var value = random.Next(1, 100);
                            Console.WriteLine($"   [条件检查] 随机值: {value}");
                            return value > 70;
                        }
                    );
                    break;
            }

            if (trigger != null)
            {
                Action customAction = () =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🎯 自定义任务 '{trigger.Name}' 执行中...");
                    Console.WriteLine("   - 自定义任务完成");
                    Console.WriteLine();
                };

                _ = trigger.StartAsync(customAction, _cancellationTokenSource.Token);
                _activeTriggers.Add(trigger);

                Console.WriteLine($"✅ 自定义触发器已创建: {trigger.Name}");
                Console.WriteLine("按任意键返回...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("❌ 创建失败");
                Console.WriteLine("按任意键返回...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        private void Cleanup()
        {
            Console.WriteLine();
            Console.WriteLine("🧹 正在清理资源...");
            
            _cancellationTokenSource.Cancel();
            StopAllTriggers();
            
            _cancellationTokenSource.Dispose();
            
            Console.WriteLine("✅ 场景5演示完成！");
            Console.WriteLine();
            Console.WriteLine("按任意键返回主菜单...");
            Console.ReadKey();
        }
    }
}
