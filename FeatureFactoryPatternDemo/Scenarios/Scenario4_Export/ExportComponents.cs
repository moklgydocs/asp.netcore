using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FeatureFactoryPatternDemo.Scenarios.Scenario4_Export
{
    /// <summary>
    /// 导出格式枚举 - 定义支持的导出格式
    /// </summary>
    public enum ExportType
    {
        Excel,    // Excel格式导出
        Csv,      // CSV格式导出
        Pdf       // PDF格式导出
    }

    /// <summary>
    /// 导出结果枚举 - 表示导出操作的结果状态
    /// </summary>
    public enum ExportResult
    {
        Success,      // 导出成功
        Failed,       // 导出失败
        FileExists    // 文件已存在（可选处理）
    }

    /// <summary>
    /// 导出信息结构 - 包含导出操作的详细信息
    /// </summary>
    public struct ExportInfo
    {
        public string FilePath { get; set; }
        public ExportType ExportType { get; set; }
        public DateTime ExportTime { get; set; }
        public ExportResult Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 导出接口（抽象产品）- 定义导出操作的核心方法
    /// 这是工厂模式中的"产品"接口，所有具体导出实现都必须实现此接口
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// 执行导出操作
        /// </summary>
        /// <param name="data">要导出的数据</param>
        /// <param name="filePath">导出文件路径</param>
        /// <returns>导出结果信息</returns>
        ExportInfo Export(List<Dictionary<string, object>> data, string filePath);
    }

    /// <summary>
    /// Excel导出实现（具体产品）- 负责将数据导出为Excel格式
    /// 在实际项目中，这里会使用EPPlus、NPOI等Excel操作库
    /// </summary>
    public class ExcelExporter : IExporter
    {
        public ExportInfo Export(List<Dictionary<string, object>> data, string filePath)
        {
            try
            {
                // 模拟Excel导出逻辑
                Console.WriteLine($"正在导出Excel文件到: {filePath}");
                
                // 检查目录是否存在，不存在则创建
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 模拟Excel文件生成（实际项目中会使用Excel库写入数据）
                File.WriteAllText(filePath, "Excel文件内容 - 模拟生成");
                
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Excel,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Excel,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Failed,
                    ErrorMessage = $"Excel导出失败: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// CSV导出实现（具体产品）- 负责将数据导出为CSV格式
    /// CSV格式简单易读，适合数据交换和导入其他系统
    /// </summary>
    public class CsvExporter : IExporter
    {
        public ExportInfo Export(List<Dictionary<string, object>> data, string filePath)
        {
            try
            {
                Console.WriteLine($"正在导出CSV文件到: {filePath}");
                
                // 检查目录是否存在，不存在则创建
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 生成CSV内容
                var csvContent = new List<string>();
                
                // 添加表头（如果数据不为空）
                if (data != null && data.Count > 0)
                {
                    var headers = new List<string>();
                    foreach (var key in data[0].Keys)
                    {
                        headers.Add($"\"{key}\""); // CSV字段用双引号包围
                    }
                    csvContent.Add(string.Join(",", headers));
                    
                    // 添加数据行
                    foreach (var row in data)
                    {
                        var values = new List<string>();
                        foreach (var key in data[0].Keys)
                        {
                            var value = row.ContainsKey(key) ? row[key]?.ToString() ?? "" : "";
                            values.Add($"\"{value}\"");
                        }
                        csvContent.Add(string.Join(",", values));
                    }
                }

                // 写入文件
                File.WriteAllLines(filePath, csvContent);
                
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Csv,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Csv,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Failed,
                    ErrorMessage = $"CSV导出失败: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// PDF导出实现（具体产品）- 负责将数据导出为PDF格式
    /// PDF格式适合打印和文档分发，保持格式一致性
    /// </summary>
    public class PdfExporter : IExporter
    {
        public ExportInfo Export(List<Dictionary<string, object>> data, string filePath)
        {
            try
            {
                Console.WriteLine($"正在导出PDF文件到: {filePath}");
                
                // 检查目录是否存在，不存在则创建
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 模拟PDF生成（实际项目中会使用iTextSharp、PdfSharp等PDF库）
                File.WriteAllText(filePath, "PDF文件内容 - 模拟生成");
                
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Pdf,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ExportInfo
                {
                    FilePath = filePath,
                    ExportType = ExportType.Pdf,
                    ExportTime = DateTime.Now,
                    Result = ExportResult.Failed,
                    ErrorMessage = $"PDF导出失败: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// 导出工厂接口（抽象工厂）- 定义工厂的核心方法
    /// 工厂模式中的"工厂"接口，负责创建具体的导出器实例
    /// </summary>
    public interface IExportFactory
    {
        /// <summary>
        /// 创建导出器实例
        /// </summary>
        /// <returns>具体的导出器实现</returns>
        IExporter CreateExporter();
    }

    /// <summary>
    /// Excel导出工厂（具体工厂）- 负责创建Excel导出器
    /// </summary>
    public class ExcelExportFactory : IExportFactory
    {
        public IExporter CreateExporter() => new ExcelExporter();
    }

    /// <summary>
    /// CSV导出工厂（具体工厂）- 负责创建CSV导出器
    /// </summary>
    public class CsvExportFactory : IExportFactory
    {
        public IExporter CreateExporter() => new CsvExporter();
    }

    /// <summary>
    /// PDF导出工厂（具体工厂）- 负责创建PDF导出器
    /// </summary>
    public class PdfExportFactory : IExportFactory
    {
        public IExporter CreateExporter() => new PdfExporter();
    }

    /// <summary>
    /// 导出管理器（工厂管理器）- 统一管理所有导出工厂
    /// 这是工厂模式的"管理器"，负责注册工厂并提供统一的获取入口
    /// 好处：
    /// 1. 集中管理所有导出工厂，避免直接依赖具体工厂类
    /// 2. 支持动态配置和扩展，新增导出格式只需注册新工厂
    /// 3. 提供统一的工厂获取接口，简化调用方代码
    /// </summary>
    public static class ExportManager
    {
        // 工厂注册表 - 使用字典存储不同导出类型的工厂创建函数
        // 使用Func<IExportFactory>而不是直接存储工厂实例，支持延迟初始化
        private static readonly Dictionary<ExportType, Func<IExportFactory>> _factoryCreators = new()
        {
            { ExportType.Excel, () => new ExcelExportFactory() },
            { ExportType.Csv, () => new CsvExportFactory() },
            { ExportType.Pdf, () => new PdfExportFactory() }
        };

        /// <summary>
        /// 获取指定类型的导出工厂
        /// </summary>
        /// <param name="exportType">导出类型</param>
        /// <returns>导出工厂实例</returns>
        /// <exception cref="NotSupportedException">当不支持的导出类型时抛出</exception>
        public static IExportFactory GetFactory(ExportType exportType)
        {
            if (_factoryCreators.TryGetValue(exportType, out var creator))
            {
                return creator();
            }
            throw new NotSupportedException($"不支持的导出类型: {exportType}");
        }

        /// <summary>
        /// 获取导出器实例（便捷方法）
        /// </summary>
        /// <param name="exportType">导出类型</param>
        /// <returns>导出器实例</returns>
        public static IExporter GetExporter(ExportType exportType)
        {
            return GetFactory(exportType).CreateExporter();
        }

        /// <summary>
        /// 检查是否支持指定的导出类型
        /// </summary>
        /// <param name="exportType">导出类型</param>
        /// <returns>是否支持</returns>
        public static bool IsSupported(ExportType exportType)
        {
            return _factoryCreators.ContainsKey(exportType);
        }

        /// <summary>
        /// 获取所有支持的导出类型
        /// </summary>
        /// <returns>支持的导出类型列表</returns>
        public static List<ExportType> GetSupportedTypes()
        {
            return new List<ExportType>(_factoryCreators.Keys);
        }
    }

    /// <summary>
    /// 导出特性（元数据标记）- 用于标记方法的默认导出格式
    /// 这是"特性+工厂模式+管理器+扩展方法"范式中的"特性"部分
    /// 好处：
    /// 1. 声明式编程：通过特性直接表达意图，无需在代码中硬编码导出格式
    /// 2. 配置集中化：导出格式配置集中在特性中，便于管理和修改
    /// 3. 解耦：业务逻辑与导出格式选择解耦，支持灵活配置
    /// 4. 可扩展：新增导出格式只需扩展枚举和工厂，无需修改业务代码
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExportableAttribute : Attribute
    {
        /// <summary>
        /// 默认导出格式
        /// </summary>
        public ExportType DefaultExportType { get; }

        /// <summary>
        /// 是否允许用户覆盖默认格式
        /// </summary>
        public bool AllowOverride { get; set; }

        /// <summary>
        /// 导出文件的默认目录
        /// </summary>
        public string DefaultDirectory { get; set; }

        /// <summary>
        /// 构造函数 - 只指定默认导出格式
        /// </summary>
        /// <param name="defaultExportType">默认导出格式</param>
        public ExportableAttribute(ExportType defaultExportType)
        {
            DefaultExportType = defaultExportType;
            AllowOverride = true; // 默认允许覆盖
            DefaultDirectory = "exports"; // 默认导出目录
        }

        /// <summary>
        /// 构造函数 - 支持完整配置
        /// </summary>
        /// <param name="defaultExportType">默认导出格式</param>
        /// <param name="allowOverride">是否允许覆盖</param>
        /// <param name="defaultDirectory">默认导出目录</param>
        public ExportableAttribute(ExportType defaultExportType, bool allowOverride = true, string defaultDirectory = "exports")
        {
            DefaultExportType = defaultExportType;
            AllowOverride = allowOverride;
            DefaultDirectory = defaultDirectory;
        }
    }

    /// <summary>
    /// 导出扩展方法 - 简化导出操作的使用
    /// 这是范式中的"扩展方法"部分，为对象添加导出能力
    /// 好处：
    /// 1. 语法糖：让调用方使用更简洁的语法
    /// 2. 封装复杂逻辑：隐藏特性读取、工厂获取、错误处理等细节
    /// 3. AOP思想：将导出逻辑与业务逻辑分离
    /// 4. 统一接口：为所有支持导出的对象提供统一的导出方法
    /// </summary>
    public static class ExportExtensions
    {
        /// <summary>
        /// 使用特性标记的默认格式导出数据
        /// </summary>
        /// <param name="service">服务对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="exportData">要导出的数据</param>
        /// <param name="fileName">导出文件名（可选）</param>
        /// <returns>导出结果信息</returns>
        public static ExportInfo ExportData(this object service, string methodName, List<Dictionary<string, object>> exportData, string fileName = null)
        {
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"方法 {methodName} 在类型 {service.GetType().Name} 中不存在");
            }

            // 读取导出特性
            var exportAttr = method.GetCustomAttribute<ExportableAttribute>();
            if (exportAttr == null)
            {
                throw new InvalidOperationException($"方法 {methodName} 未标记 [Exportable] 特性");
            }

            // 生成文件路径
            var filePath = GenerateFilePath(exportAttr.DefaultDirectory, fileName ?? $"{methodName}_export", exportAttr.DefaultExportType);

            // 获取导出器并执行导出
            var exporter = ExportManager.GetExporter(exportAttr.DefaultExportType);
            return exporter.Export(exportData, filePath);
        }

        /// <summary>
        /// 使用指定格式导出数据（允许覆盖默认设置）
        /// </summary>
        /// <param name="service">服务对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="exportData">要导出的数据</param>
        /// <param name="exportType">指定的导出格式</param>
        /// <param name="fileName">导出文件名（可选）</param>
        /// <returns>导出结果信息</returns>
        public static ExportInfo ExportDataWithFormat(this object service, string methodName, List<Dictionary<string, object>> exportData, ExportType exportType, string fileName = null)
        {
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"方法 {methodName} 在类型 {service.GetType().Name} 中不存在");
            }

            // 检查方法是否有导出特性
            var exportAttr = method.GetCustomAttribute<ExportableAttribute>();
            if (exportAttr == null)
            {
                throw new InvalidOperationException($"方法 {methodName} 未标记 [Exportable] 特性");
            }

            // 检查是否允许格式覆盖
            if (!exportAttr.AllowOverride)
            {
                throw new InvalidOperationException($"方法 {methodName} 不允许覆盖默认导出格式");
            }

            // 检查指定格式是否支持
            if (!ExportManager.IsSupported(exportType))
            {
                throw new NotSupportedException($"不支持的导出格式: {exportType}");
            }

            // 生成文件路径
            var filePath = GenerateFilePath(exportAttr.DefaultDirectory, fileName ?? $"{methodName}_export", exportType);

            // 获取导出器并执行导出
            var exporter = ExportManager.GetExporter(exportType);
            return exporter.Export(exportData, filePath);
        }

        /// <summary>
        /// 批量导出数据到多种格式
        /// </summary>
        /// <param name="service">服务对象</param>
        /// <param name="methodName">方法名</param>
        /// <param name="exportData">要导出的数据</param>
        /// <param name="exportTypes">要导出的格式列表</param>
        /// <param name="fileName">导出文件名（可选）</param>
        /// <returns>批量导出结果列表</returns>
        public static List<ExportInfo> ExportDataMultipleFormats(this object service, string methodName, List<Dictionary<string, object>> exportData, List<ExportType> exportTypes, string fileName = null)
        {
            var method = service.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"方法 {methodName} 在类型 {service.GetType().Name} 中不存在");
            }

            var exportAttr = method.GetCustomAttribute<ExportableAttribute>();
            if (exportAttr == null)
            {
                throw new InvalidOperationException($"方法 {methodName} 未标记 [Exportable] 特性");
            }

            var results = new List<ExportInfo>();

            foreach (var exportType in exportTypes)
            {
                if (!ExportManager.IsSupported(exportType))
                {
                    results.Add(new ExportInfo
                    {
                        ExportType = exportType,
                        ExportTime = DateTime.Now,
                        Result = ExportResult.Failed,
                        ErrorMessage = $"不支持的导出格式: {exportType}"
                    });
                    continue;
                }

                var filePath = GenerateFilePath(exportAttr.DefaultDirectory, fileName ?? $"{methodName}_export", exportType);
                var exporter = ExportManager.GetExporter(exportType);
                var result = exporter.Export(exportData, filePath);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// 生成导出文件路径
        /// </summary>
        /// <param name="directory">导出目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="exportType">导出格式</param>
        /// <returns>完整文件路径</returns>
        private static string GenerateFilePath(string directory, string fileName, ExportType exportType)
        {
            // 确保目录存在
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 根据导出格式生成文件扩展名
            var extension = exportType switch
            {
                ExportType.Excel => ".xlsx",
                ExportType.Csv => ".csv",
                ExportType.Pdf => ".pdf",
                _ => ".txt"
            };

            // 添加时间戳避免文件名冲突
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(directory, $"{fileName}_{timestamp}{extension}");
        }
    }
}
