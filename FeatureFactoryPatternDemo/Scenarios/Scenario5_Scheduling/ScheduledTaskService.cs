// 场景5：任务调度演示服务
// 展示如何使用"特性+工厂模式+管理器+扩展方法"范式进行任务调度

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario5_Scheduling
{
    /// <summary>
    /// 定时任务服务示例
    /// 演示不同触发器类型的实际应用
    /// </summary>
    public class ScheduledTaskService
    {
        private readonly object _consoleLock = new object();

        #region 1. 固定间隔任务示例

        /// <summary>
        /// 每30秒执行一次的健康检查任务
        /// 使用FixedInterval触发器
        /// </summary>
        [Schedule(TriggerType.FixedInterval)]
        public void HealthCheckTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🏥 健康检查任务执行中...");
                Console.WriteLine("   - 检查CPU使用率: 正常");
                Console.WriteLine("   - 检查内存使用: 正常");
                Console.WriteLine("   - 检查磁盘空间: 正常");
                Console.WriteLine("   - 检查网络连接: 正常");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 每2分钟执行一次的数据备份任务
        /// </summary>
        [Schedule(TriggerType.FixedInterval)]
        public void DataBackupTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 💾 数据备份任务执行中...");
                Console.WriteLine("   - 正在备份用户数据...");
                Console.WriteLine("   - 正在备份配置文件...");
                Console.WriteLine("   - 正在验证备份完整性...");
                Console.WriteLine("   - 备份完成，文件已加密存储");
                Console.WriteLine();
            }
        }

        #endregion

        #region 2. CRON表达式任务示例

        /// <summary>
        /// 每天上午9点执行的日报生成任务
        /// 使用Cron触发器，CRON表达式: "0 9 * * *"
        /// </summary>
        [Schedule(TriggerType.Cron)]
        public void DailyReportTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📊 日报生成任务执行中...");
                Console.WriteLine("   - 收集昨日销售数据...");
                Console.WriteLine("   - 生成销售报表...");
                Console.WriteLine("   - 发送邮件给管理层...");
                Console.WriteLine("   - 保存报表到文件系统...");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 每周一上午10点执行的周报任务
        /// 使用Cron触发器，CRON表达式: "0 10 * * 1"
        /// </summary>
        [Schedule(TriggerType.Cron)]
        public void WeeklyReportTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📈 周报任务执行中...");
                Console.WriteLine("   - 收集本周数据...");
                Console.WriteLine("   - 生成周度分析报告...");
                Console.WriteLine("   - 发送团队邮件...");
                Console.WriteLine();
            }
        }

        #endregion

        #region 3. 一次性任务示例

        /// <summary>
        /// 系统维护任务 - 在指定时间执行一次
        /// 使用OneTime触发器
        /// </summary>
        [Schedule(TriggerType.OneTime)]
        public void SystemMaintenanceTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔧 系统维护任务执行中...");
                Console.WriteLine("   - 正在更新系统补丁...");
                Console.WriteLine("   - 正在清理临时文件...");
                Console.WriteLine("   - 正在优化数据库...");
                Console.WriteLine("   - 系统维护完成");
                Console.WriteLine();
            }
        }

        #endregion

        #region 4. 条件触发任务示例

        /// <summary>
        /// 高负载告警任务 - 当CPU使用率超过80%时触发
        /// 使用Conditional触发器
        /// </summary>
        [Schedule(TriggerType.Conditional)]
        public void HighLoadAlertTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ⚠️  高负载告警任务执行中...");
                Console.WriteLine("   - 检测到系统负载过高");
                Console.WriteLine("   - 已发送告警邮件给运维团队");
                Console.WriteLine("   - 已启动自动扩容");
                Console.WriteLine("   - 正在监控系统状态...");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 低库存告警任务 - 当库存低于阈值时触发
        /// </summary>
        [Schedule(TriggerType.Conditional)]
        public void LowStockAlertTask()
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 📦 低库存告警任务执行中...");
                Console.WriteLine("   - 检测到商品库存不足");
                Console.WriteLine("   - 已通知采购部门");
                Console.WriteLine("   - 已启动紧急补货流程");
                Console.WriteLine();
            }
        }

        #endregion

        #region 5. 配置方法 - 为特性提供具体参数

        /// <summary>
        /// 配置健康检查任务的间隔
        /// </summary>
        public void ConfigureHealthCheck()
        {
            // 通过反射获取特性并设置参数
            var method = GetType().GetMethod(nameof(HealthCheckTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                attr.Interval = TimeSpan.FromSeconds(30); // 每30秒执行一次
                attr.AutoStart = true; // 自动启动
            }
        }

        /// <summary>
        /// 配置数据备份任务的间隔
        /// </summary>
        public void ConfigureDataBackup()
        {
            var method = GetType().GetMethod(nameof(DataBackupTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                attr.Interval = TimeSpan.FromMinutes(2); // 每2分钟执行一次
                attr.AutoStart = false; // 不自动启动，需要手动触发
            }
        }

        /// <summary>
        /// 配置日报任务的CRON表达式
        /// </summary>
        public void ConfigureDailyReport()
        {
            var method = GetType().GetMethod(nameof(DailyReportTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                attr.CronExpression = "0 9 * * *"; // 每天上午9点
                attr.AutoStart = true;
            }
        }

        /// <summary>
        /// 配置周报任务的CRON表达式
        /// </summary>
        public void ConfigureWeeklyReport()
        {
            var method = GetType().GetMethod(nameof(WeeklyReportTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                attr.CronExpression = "0 10 * * 1"; // 每周一上午10点
                attr.AutoStart = true;
            }
        }

        /// <summary>
        /// 配置系统维护任务的执行时间
        /// </summary>
        public void ConfigureSystemMaintenance()
        {
            var method = GetType().GetMethod(nameof(SystemMaintenanceTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                // 设置为5分钟后执行
                attr.ExecutionTime = DateTime.Now.AddMinutes(5);
                attr.AutoStart = true;
            }
        }

        /// <summary>
        /// 配置高负载告警任务的条件
        /// </summary>
        public void ConfigureHighLoadAlert()
        {
            var method = GetType().GetMethod(nameof(HighLoadAlertTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                // 模拟CPU使用率检查，实际项目中可以从系统监控获取
                attr.Condition = () =>
                {
                    // 模拟随机高负载情况
                    var random = new Random();
                    var cpuUsage = random.Next(50, 100);
                    Console.WriteLine($"   [条件检查] 当前CPU使用率: {cpuUsage}%");
                    return cpuUsage > 80;
                };
                attr.AutoStart = true;
            }
        }

        /// <summary>
        /// 配置低库存告警任务的条件
        /// </summary>
        public void ConfigureLowStockAlert()
        {
            var method = GetType().GetMethod(nameof(LowStockAlertTask));
            var attr = method.GetCustomAttribute<ScheduleAttribute>();
            if (attr != null)
            {
                // 模拟库存检查
                attr.Condition = () =>
                {
                    var random = new Random();
                    var stockLevel = random.Next(0, 100);
                    Console.WriteLine($"   [条件检查] 当前库存水平: {stockLevel}件");
                    return stockLevel < 20; // 库存低于20件时触发
                };
                attr.AutoStart = true;
            }
        }

        #endregion

        #region 6. 初始化方法

        /// <summary>
        /// 初始化所有任务的配置
        /// </summary>
        public void InitializeTaskConfigurations()
        {
            ConfigureHealthCheck();
            ConfigureDataBackup();
            ConfigureDailyReport();
            ConfigureWeeklyReport();
            ConfigureSystemMaintenance();
            ConfigureHighLoadAlert();
            ConfigureLowStockAlert();

            Console.WriteLine("✅ 所有定时任务配置已完成");
            Console.WriteLine();
        }

        #endregion
    }
}
