using System;
using System.Reflection;
using AssemblyDemo.Examples;
using AssemblyDemo.ORM;
using AssemblyDemo.Models;

namespace AssemblyDemo
{
    /// <summary>
    /// .NET 程序集深度学习示例程序
    /// 从基础到高级，逐步学习程序集、元数据、CLR头、PE头等知识
    /// 最终实现一个简易的ORM框架
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("  .NET 程序集深度学习示例程序");
            Console.WriteLine("====================================\n");

            // 第一部分：程序集和元数据基础
            Console.WriteLine("\n【第一部分：程序集和元数据基础】");
            AssemblyBasics.DemonstrateAssemblyInfo();
            AssemblyBasics.ExploreMetadata();

            // 第二部分：反射深入探索
            Console.WriteLine("\n【第二部分：反射深入探索】");
            ReflectionExamples.ExploreTypeInformation();
            ReflectionExamples.DynamicInvocation();

            // 第三部分：CLR和PE头探索
            Console.WriteLine("\n【第三部分：CLR和PE头探索】");
            PEHeaderExplorer.ExplorePEHeaders();

            // 第四部分：动态代码生成
            Console.WriteLine("\n【第四部分：动态代码生成】");
            DynamicCodeGeneration.GenerateDynamicAssembly();

            // 第五部分：简易ORM框架演示
            Console.WriteLine("\n【第五部分：简易ORM框架演示】");
            ORMDemo.RunORMDemo();

            Console.WriteLine("\n====================================");
            Console.WriteLine("  演示完成！");
            Console.WriteLine("====================================");
            
            // 仅在交互式控制台中等待用户输入
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("\n按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
