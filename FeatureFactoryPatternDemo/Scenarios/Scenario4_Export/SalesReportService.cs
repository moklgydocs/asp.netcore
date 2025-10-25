using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario4_Export
{
    /// <summary>
    /// 销售报告服务 - 演示数据导出功能的业务服务
    /// 这个服务展示了如何在实际业务场景中使用"特性+工厂模式+管理器+扩展方法"范式
    /// 
    /// 业务场景说明：
    /// 在企业应用中，销售团队经常需要将销售数据导出为不同格式：
    /// - Excel格式：用于详细分析和图表制作
    /// - CSV格式：用于数据导入到其他系统或进行批量处理
    /// - PDF格式：用于打印报告和分享给客户
    /// 
    /// 使用这种范式的好处：
    /// 1. 业务逻辑与导出格式解耦：销售报告生成逻辑不依赖具体的导出实现
    /// 2. 灵活配置：通过特性可以轻松更改默认导出格式，无需修改业务代码
    /// 3. 易于扩展：新增导出格式（如JSON、XML）只需添加新的工厂和导出器
    /// 4. 统一接口：所有导出操作都通过扩展方法提供一致的API
    /// </summary>
    public class SalesReportService
    {
        /// <summary>
        /// 生成月度销售报告数据
        /// 这是一个典型的业务方法，负责生成需要导出的数据
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>销售报告数据</returns>
        [Exportable(ExportType.Excel)]
        public List<Dictionary<string, object>> GenerateMonthlyReport(int year, int month)
        {
            Console.WriteLine($"正在生成{year}年{month}月的销售报告数据...");
            
            // 模拟生成销售数据
            var reportData = new List<Dictionary<string, object>>();
            
            // 添加12个产品的销售数据
            var products = new[] { "产品A", "产品B", "产品C", "产品D", "产品E" };
            var regions = new[] { "华东", "华南", "华北", "西南", "西北" };
            
            for (int i = 1; i <= 31; i++) // 模拟31天的数据
            {
                foreach (var product in products)
                {
                    foreach (var region in regions)
                    {
                        reportData.Add(new Dictionary<string, object>
                        {
                            { "日期", $"{year}-{month:D2}-{i:D2}" },
                            { "产品", product },
                            { "地区", region },
                            { "销量", new Random().Next(10, 100) },
                            { "销售额", Math.Round(new Random().NextDouble() * 10000, 2) },
                            { "利润率", Math.Round(new Random().NextDouble() * 0.3 + 0.1, 3) } // 10%-40%
                        });
                    }
                }
            }
            
            Console.WriteLine($"成功生成{reportData.Count}条销售数据记录");
            return reportData;
        }

        /// <summary>
        /// 生成年度销售总结报告
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>年度总结数据</returns>
        [Exportable(ExportType.Excel, true, "annual_reports")]
        public List<Dictionary<string, object>> GenerateAnnualSummary(int year)
        {
            Console.WriteLine($"正在生成{year}年度销售总结...");
            
            var summaryData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "年份", year },
                    { "总销售额", Math.Round(new Random().NextDouble() * 10000000 + 5000000, 2) },
                    { "总订单数", new Random().Next(1000, 5000) },
                    { "平均订单价值", Math.Round(new Random().NextDouble() * 5000 + 1000, 2) },
                    { "最畅销产品", "智能手机" },
                    { "同比增长率", Math.Round((new Random().NextDouble() - 0.5) * 0.4, 4) }, // -20% 到 +20%
                    { "客户满意度", Math.Round(new Random().NextDouble() * 0.2 + 0.7, 3) }, // 70% 到 90%
                    { "市场份额", Math.Round(new Random().NextDouble() * 0.15 + 0.05, 3) } // 5% 到 20%
                }
            };

            Console.WriteLine($"成功生成年度总结数据");
            return summaryData;
        }

        /// <summary>
        /// 生成客户销售排行榜
        /// 这个方法使用CSV作为默认导出格式，便于导入到其他分析工具
        /// </summary>
        /// <param name="topN">前N名客户</param>
        /// <returns>客户销售排行榜数据</returns>
        [Exportable(ExportType.Csv, true, "exports/rankings")]
        public List<Dictionary<string, object>> GenerateCustomerRanking(int topN)
        {
            Console.WriteLine($"正在生成前{topN}名客户销售排行榜...");
            
            var rankingData = new List<Dictionary<string, object>>();
            
            // 生成客户排行榜数据
            for (int i = 1; i <= topN; i++)
            {
                rankingData.Add(new Dictionary<string, object>
                {
                    { "排名", i },
                    { "客户ID", $"CUST_{new Random().Next(1000, 9999)}" },
                    { "客户名称", $"客户{i}" },
                    { "所属行业", new[] { "制造业", "零售业", "服务业", "科技", "金融" }[new Random().Next(5)] },
                    { "年度采购额", Math.Round(new Random().NextDouble() * 10000000 + 1000000, 2) },
                    { "合作年限", new Random().Next(1, 10) },
                    { "客户等级", new[] { "A", "B", "C", "D" }[new Random().Next(4)] }
                });
            }
            
            Console.WriteLine($"成功生成客户排行榜数据");
            return rankingData;
        }

        /// <summary>
        /// 生成产品库存报告
        /// 这个方法不允许覆盖默认导出格式，确保重要报告的一致性
        /// </summary>
        /// <returns>产品库存数据</returns>
        [Exportable(ExportType.Excel, false, "exports/inventory")]
        public List<Dictionary<string, object>> GenerateInventoryReport()
        {
            Console.WriteLine($"正在生成产品库存报告...");
            
            var inventoryData = new List<Dictionary<string, object>>();
            
            // 生成产品库存数据
            var products = new[]
            {
                new { Name = "智能手机", Category = "电子产品", Unit = "台" },
                new { Name = "笔记本电脑", Category = "电子产品", Unit = "台" },
                new { Name = "办公桌", Category = "办公家具", Unit = "张" },
                new { Name = "办公椅", Category = "办公家具", Unit = "把" },
                new { Name = "打印机", Category = "办公设备", Unit = "台" }
            };
            
            foreach (var product in products)
            {
                inventoryData.Add(new Dictionary<string, object>
                {
                    { "产品名称", product.Name },
                    { "产品类别", product.Category },
                    { "单位", product.Unit },
                    { "当前库存", new Random().Next(50, 500) },
                    { "安全库存", new Random().Next(20, 100) },
                    { "供应商", $"供应商{new Random().Next(1, 10)}" },
                    { "最后更新时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                });
            }
            
            Console.WriteLine($"成功生成库存报告数据");
            return inventoryData;
        }

        /// <summary>
        /// 生成销售趋势分析报告
        /// 这个方法演示了复杂的数据分析和多种导出格式的支持
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="analysisType">分析类型</param>
        /// <returns>趋势分析数据</returns>
        [Exportable(ExportType.Pdf, true, "exports/trends")]
        public List<Dictionary<string, object>> GenerateTrendAnalysis(int year, string analysisType)
        {
            Console.WriteLine($"正在生成{year}年{analysisType}趋势分析报告...");
            
            var trendData = new List<Dictionary<string, object>>();
            
            // 生成月度趋势数据
            for (int month = 1; month <= 12; month++)
            {
                trendData.Add(new Dictionary<string, object>
                {
                    { "年份", year },
                    { "月份", month },
                    { "月份名称", new[] { "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" }[month - 1] },
                    { "销售额", Math.Round(new Random().NextDouble() * 1000000 + 200000, 2) },
                    { "同比增长率", Math.Round((new Random().NextDouble() - 0.5) * 0.3, 4) }, // -15% 到 +15%
                    { "环比增长率", Math.Round((new Random().NextDouble() - 0.5) * 0.2, 4) }, // -10% 到 +10%
                    { "市场占有率", Math.Round(new Random().NextDouble() * 0.3 + 0.1, 3) }, // 10% 到 40%
                    { "客户满意度", Math.Round(new Random().NextDouble() * 0.2 + 0.7, 3) } // 70% 到 90%
                });
            }
            
            Console.WriteLine($"成功生成趋势分析数据");
            return trendData;
        }

        /// <summary>
        /// 演示方法：展示如何在业务代码中使用导出功能
        /// </summary>
        public void DemonstrateExportUsage()
        {
            Console.WriteLine("\n=== 销售报告服务导出功能演示 ===");
            Console.WriteLine("这个演示展示了如何在业务服务中集成导出功能");
            Console.WriteLine("通过特性标记，我们可以为不同的报告指定不同的默认导出格式");
            Console.WriteLine();

            // 1. 使用默认格式导出月度报告（Excel）
            Console.WriteLine("1. 导出月度销售报告（默认Excel格式）...");
            var monthlyData = GenerateMonthlyReport(2024, 1);
            var monthlyResult = this.ExportData("GenerateMonthlyReport", monthlyData, "monthly_sales_jan2024");
            Console.WriteLine($"导出结果: {monthlyResult.Result}, 文件: {monthlyResult.FilePath}");
            Console.WriteLine();

            // 2. 覆盖默认格式导出为CSV
            Console.WriteLine("2. 覆盖默认格式，导出为CSV格式...");
            var csvResult = this.ExportDataWithFormat("GenerateMonthlyReport", monthlyData, ExportType.Csv, "monthly_sales_jan2024_csv");
            Console.WriteLine($"导出结果: {csvResult.Result}, 文件: {csvResult.FilePath}");
            Console.WriteLine();

            // 3. 批量导出多种格式
            Console.WriteLine("3. 批量导出多种格式...");
            var formats = new List<ExportType> { ExportType.Excel, ExportType.Csv, ExportType.Pdf };
            var batchResults = this.ExportDataMultipleFormats("GenerateAnnualSummary", GenerateAnnualSummary(2024), formats, "annual_summary_2024");
            foreach (var result in batchResults)
            {
                Console.WriteLine($"格式: {result.ExportType}, 结果: {result.Result}, 文件: {result.FilePath}");
            }
            Console.WriteLine();

            // 4. 演示不允许覆盖的方法
            Console.WriteLine("4. 演示不允许覆盖默认格式的方法...");
            var inventoryData = GenerateInventoryReport();
            try
            {
                // 尝试覆盖默认格式（应该会失败）
                this.ExportDataWithFormat("GenerateInventoryReport", inventoryData, ExportType.Csv);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"预期的错误: {ex.Message}");
            }
            
            // 使用默认格式导出
            var inventoryResult = this.ExportData("GenerateInventoryReport", inventoryData);
            Console.WriteLine($"使用默认格式导出结果: {inventoryResult.Result}, 文件: {inventoryResult.FilePath}");
            Console.WriteLine();

            Console.WriteLine("=== 导出功能演示完成 ===");
            Console.WriteLine();
            Console.WriteLine("业务价值总结:");
            Console.WriteLine("• 通过特性标记实现声明式配置，业务代码更清晰");
            Console.WriteLine("• 工厂模式确保导出实现的可扩展性和可维护性");
            Console.WriteLine("• 扩展方法提供统一的导出接口，简化调用");
            Console.WriteLine("• 支持灵活的格式选择和批量导出，满足不同业务需求");
        }
    }
}
