using System;
using System.Reflection;
using System.Linq;

namespace AssemblyDemo.Examples
{
    /// <summary>
    /// 程序集基础知识示例
    /// 演示如何获取和使用程序集信息
    /// </summary>
    public static class AssemblyBasics
    {
        /// <summary>
        /// 演示程序集基本信息
        /// 程序集是.NET应用程序的基本部署单元，包含类型定义和资源
        /// </summary>
        public static void DemonstrateAssemblyInfo()
        {
            Console.WriteLine("\n--- 程序集基本信息 ---");

            // 获取当前正在执行的程序集
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            
            // 程序集全名：包含名称、版本、文化和公钥标记
            Console.WriteLine($"程序集全名: {currentAssembly.FullName}");
            
            // 程序集位置（文件路径）
            Console.WriteLine($"程序集位置: {currentAssembly.Location}");
            
            // 程序集名称对象，包含详细信息
            AssemblyName assemblyName = currentAssembly.GetName();
            Console.WriteLine($"  - 名称: {assemblyName.Name}");
            Console.WriteLine($"  - 版本: {assemblyName.Version}");
            Console.WriteLine($"  - 文化: {(string.IsNullOrEmpty(assemblyName.CultureName) ? "中性" : assemblyName.CultureName)}");
            
            // 获取程序集的自定义特性
            var attributes = currentAssembly.GetCustomAttributes();
            Console.WriteLine($"\n程序集特性数量: {attributes.Count()}");
            foreach (var attr in attributes.Take(3)) // 只显示前3个
            {
                Console.WriteLine($"  - {attr.GetType().Name}");
            }

            // 获取程序集中定义的所有类型
            Type[] types = currentAssembly.GetTypes();
            Console.WriteLine($"\n程序集中定义的类型数量: {types.Length}");
            Console.WriteLine("类型列表:");
            foreach (var type in types.Take(5)) // 只显示前5个
            {
                Console.WriteLine($"  - {type.FullName} ({type.Namespace})");
            }
        }

        /// <summary>
        /// 探索元数据
        /// 元数据是描述程序集中类型和成员的信息
        /// </summary>
        public static void ExploreMetadata()
        {
            Console.WriteLine("\n--- 探索元数据 ---");

            // 使用示例类型探索元数据
            Type stringType = typeof(string);
            
            Console.WriteLine($"\n类型: {stringType.Name}");
            Console.WriteLine($"命名空间: {stringType.Namespace}");
            Console.WriteLine($"基类: {stringType.BaseType?.Name ?? "无"}");
            Console.WriteLine($"是否为值类型: {stringType.IsValueType}");
            Console.WriteLine($"是否为类: {stringType.IsClass}");
            Console.WriteLine($"是否为接口: {stringType.IsInterface}");
            Console.WriteLine($"是否为抽象类: {stringType.IsAbstract}");
            Console.WriteLine($"是否为密封类: {stringType.IsSealed}");

            // 获取方法元数据
            MethodInfo[] methods = stringType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Console.WriteLine($"\n公共方法数量: {methods.Length}");
            Console.WriteLine("示例方法:");
            foreach (var method in methods.Take(5))
            {
                var parameters = method.GetParameters();
                var paramStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {method.ReturnType.Name} {method.Name}({paramStr})");
            }

            // 获取属性元数据
            PropertyInfo[] properties = stringType.GetProperties();
            Console.WriteLine($"\n公共属性数量: {properties.Length}");
            foreach (var prop in properties)
            {
                Console.WriteLine($"  - {prop.PropertyType.Name} {prop.Name} (可读: {prop.CanRead}, 可写: {prop.CanWrite})");
            }
        }
    }
}
