using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario4_Export
{
    /// <summary>
    /// 场景4演示程序 - 数据导出策略
    /// 这个演示展示了如何使用"特性+工厂模式+管理器+扩展方法"范式实现灵活的数据导出功能
    /// </summary>
    public static class Scenario4Demo
    {
        /// <summary>
        /// 运行场景4的完整演示
        /// </summary>
        public static void RunDemo()
        {
            Console.WriteLine("=== 场景4：数据导出策略演示 ===");
            Console.WriteLine();
            Console.WriteLine("这个场景展示了如何使用特性标记、工厂模式、管理器和扩展方法");
            Console.WriteLine("实现灵活的数据导出功能，支持Excel、CSV、PDF等多种格式。");
            Console.WriteLine();

            // 创建销售报告服务实例
            var salesService = new SalesReportService();

            // 1. 演示基本导出功能
            Console.WriteLine("1. 基本导出功能演示：");
            Console.WriteLine("   - 生成月度销售报告并导出为默认格式（Excel）");
            Console.WriteLine();

            var monthlyData = salesService.GenerateMonthlyReport(2024, 1);
            var monthlyResult = salesService.ExportData("GenerateMonthlyReport", monthlyData, "monthly_sales_jan2024");
            Console.WriteLine($"   导出结果: {monthlyResult.Result}");
            Console.WriteLine($"   文件路径: {monthlyResult.FilePath}");
            Console.WriteLine($"   导出格式: {monthlyResult.ExportType}");
            Console.WriteLine();

            // 2. 演示格式覆盖功能
            Console.WriteLine("2. 格式覆盖功能演示：");
            Console.WriteLine("   - 使用指定格式导出（覆盖默认设置）");
            Console.WriteLine();

            var csvResult = salesService.ExportDataWithFormat("GenerateMonthlyReport", monthlyData, ExportType.Csv, "monthly_sales_jan2024_csv");
            Console.WriteLine($"   导出结果: {csvResult.Result}");
            Console.WriteLine($"   文件路径: {csvResult.FilePath}");
            Console.WriteLine($"   导出格式: {csvResult.ExportType}");
            Console.WriteLine();

            // 3. 演示批量导出功能
            Console.WriteLine("3. 批量导出功能演示：");
            Console.WriteLine("   - 将同一数据导出为多种格式");
            Console.WriteLine();

            var annualData = salesService.GenerateAnnualSummary(2024);
            var formats = new List<ExportType> { ExportType.Excel, ExportType.Csv, ExportType.Pdf };
            var batchResults = salesService.ExportDataMultipleFormats("GenerateAnnualSummary", annualData, formats, "annual_summary_2024");

            Console.WriteLine("   批量导出结果：");
            foreach (var result in batchResults)
            {
                Console.WriteLine($"   - 格式: {result.ExportType}, 结果: {result.Result}");
                if (result.Result == ExportResult.Success)
                {
                    Console.WriteLine($"     文件: {result.FilePath}");
                }
                else
                {
                    Console.WriteLine($"     错误: {result.ErrorMessage}");
                }
            }
            Console.WriteLine();

            // 4. 演示导出管理器功能
            Console.WriteLine("4. 导出管理器功能演示：");
            Console.WriteLine("   - 展示管理器的工厂注册和类型支持功能");
            Console.WriteLine();

            var supportedTypes = ExportManager.GetSupportedTypes();
            Console.WriteLine("   支持的导出格式：");
            foreach (var type in supportedTypes)
            {
                Console.WriteLine($"   - {type}");
            }

            Console.WriteLine();
            Console.WriteLine("   测试格式支持检查：");
            Console.WriteLine($"   - Excel支持: {ExportManager.IsSupported(ExportType.Excel)}");
            Console.WriteLine($"   - CSV支持: {ExportManager.IsSupported(ExportType.Csv)}");
            Console.WriteLine($"   - PDF支持: {ExportManager.IsSupported(ExportType.Pdf)}");
            Console.WriteLine($"   - JSON支持: {ExportManager.IsSupported((ExportType)999)}"); // 测试不支持的格式
            Console.WriteLine();

            // 5. 演示不同业务方法的导出配置
            Console.WriteLine("5. 不同业务方法的导出配置演示：");
            Console.WriteLine("   - 展示不同方法可以有不同的默认导出配置");
            Console.WriteLine();

            // 导出客户排行榜（CSV格式）
            var rankingData = salesService.GenerateCustomerRanking(10);
            var rankingResult = salesService.ExportData("GenerateCustomerRanking", rankingData, "top10_customers");
            Console.WriteLine($"   客户排行榜导出：{rankingResult.ExportType}格式，结果: {rankingResult.Result}");

            // 导出库存报告（Excel格式，不允许覆盖）
            var inventoryData = salesService.GenerateInventoryReport();
            var inventoryResult = salesService.ExportData("GenerateInventoryReport", inventoryData);
            Console.WriteLine($"   库存报告导出：{inventoryResult.ExportType}格式，结果: {inventoryResult.Result}");

            // 尝试覆盖不允许覆盖的方法
            Console.WriteLine();
            Console.WriteLine("   测试不允许覆盖的方法：");
            try
            {
                salesService.ExportDataWithFormat("GenerateInventoryReport", inventoryData, ExportType.Csv);
                Console.WriteLine("   ❌ 错误：应该抛出异常但没有");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"   ✅ 正确：{ex.Message}");
            }
            Console.WriteLine();

            // 6. 业务价值总结
            Console.WriteLine("6. 业务价值总结：");
            Console.WriteLine();
            Console.WriteLine("   ✅ 声明式配置：通过特性直接表达导出意图，代码更清晰");
            Console.WriteLine("   ✅ 实现解耦：业务逻辑与导出实现分离，便于维护");
            Console.WriteLine("   ✅ 灵活扩展：新增导出格式只需添加工厂，无需修改业务代码");
            Console.WriteLine("   ✅ 统一接口：所有导出操作都通过扩展方法提供一致API");
            Console.WriteLine("   ✅ 配置集中：导出格式、目录等配置集中在特性中管理");
            Console.WriteLine("   ✅ 安全控制：支持禁止格式覆盖，确保重要报告的一致性");
            Console.WriteLine();

            // 7. 实际应用场景
            Console.WriteLine("7. 实际应用场景：");
            Console.WriteLine();
            Console.WriteLine("   📊 企业报表系统：");
            Console.WriteLine("      - 财务报表：默认Excel，支持CSV导入");
            Console.WriteLine("      - 销售报告：默认PDF，适合打印分享");
            Console.WriteLine("      - 数据分析：默认CSV，便于数据处理");
            Console.WriteLine();
            Console.WriteLine("   🏭 生产管理系统：");
            Console.WriteLine("      - 库存报告：固定Excel格式，确保一致性");
            Console.WriteLine("      - 质量报告：支持多种格式导出");
            Console.WriteLine("      - 生产计划：默认PDF，便于分发");
            Console.WriteLine();
            Console.WriteLine("   📈 商业智能系统：");
            Console.WriteLine("      - 趋势分析：默认Excel，支持图表制作");
            Console.WriteLine("      - 客户分析：默认CSV，便于数据挖掘");
            Console.WriteLine("      - 绩效报告：默认PDF，适合汇报展示");
            Console.WriteLine();

            Console.WriteLine("=== 场景4演示完成 ===");
            Console.WriteLine();
            Console.WriteLine("按任意键继续...");
            Console.ReadKey();
        }

        /// <summary>
        /// 运行销售报告服务的完整演示
        /// </summary>
        public static void RunSalesServiceDemo()
        {
            Console.WriteLine("=== 销售报告服务完整演示 ===");
            Console.WriteLine();

            var salesService = new SalesReportService();
            
            // 调用演示方法
            salesService.DemonstrateExportUsage();

            Console.WriteLine("按任意键返回...");
            Console.ReadKey();
        }

        /// <summary>
        /// 运行场景演示的入口方法（供Program.cs调用）
        /// </summary>
        public static void Run()
        {
            RunDemo();
        }
    }
}
