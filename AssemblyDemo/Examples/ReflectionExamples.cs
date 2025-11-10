using System;
using System.Reflection;
using System.Linq;

namespace AssemblyDemo.Examples
{
    /// <summary>
    /// 反射深入探索示例
    /// 反射允许在运行时检查和操作类型、方法、属性等
    /// </summary>
    public static class ReflectionExamples
    {
        /// <summary>
        /// 探索类型信息
        /// 使用反射深入了解类型的结构
        /// </summary>
        public static void ExploreTypeInformation()
        {
            Console.WriteLine("\n--- 类型信息探索 ---");

            // 创建一个示例类用于反射
            Type exampleType = typeof(ExampleClass);

            Console.WriteLine($"类型名称: {exampleType.Name}");
            Console.WriteLine($"完全限定名: {exampleType.FullName}");

            // 探索字段（包括私有字段）
            Console.WriteLine("\n字段信息:");
            var fields = exampleType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                string accessModifier = field.IsPublic ? "public" : field.IsPrivate ? "private" : "protected";
                string staticModifier = field.IsStatic ? "static" : "";
                Console.WriteLine($"  {accessModifier} {staticModifier} {field.FieldType.Name} {field.Name}");
            }

            // 探索属性
            Console.WriteLine("\n属性信息:");
            var properties = exampleType.GetProperties();
            foreach (var prop in properties)
            {
                var getter = prop.GetGetMethod();
                var setter = prop.GetSetMethod();
                Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name}");
                Console.WriteLine($"    Getter: {(getter != null ? "有" : "无")}, Setter: {(setter != null ? "有" : "无")}");
            }

            // 探索方法
            Console.WriteLine("\n方法信息:");
            var methods = exampleType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods.Where(m => !m.IsSpecialName)) // 排除属性的get/set方法
            {
                var parameters = method.GetParameters();
                var paramStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({paramStr})");
            }

            // 探索构造函数
            Console.WriteLine("\n构造函数信息:");
            var constructors = exampleType.GetConstructors();
            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                var paramStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {exampleType.Name}({paramStr})");
            }
        }

        /// <summary>
        /// 演示动态调用
        /// 使用反射在运行时创建对象和调用方法
        /// </summary>
        public static void DynamicInvocation()
        {
            Console.WriteLine("\n--- 动态调用演示 ---");

            // 使用反射创建对象实例
            Type exampleType = typeof(ExampleClass);
            
            // 方式1: 使用Activator.CreateInstance
            object? instance1 = Activator.CreateInstance(exampleType);
            Console.WriteLine($"使用Activator创建实例: {instance1 != null}");

            // 方式2: 使用构造函数信息
            ConstructorInfo? constructor = exampleType.GetConstructor(new Type[] { typeof(string), typeof(int) });
            object? instance2 = constructor?.Invoke(new object[] { "测试", 42 });
            Console.WriteLine($"使用构造函数创建实例: {instance2 != null}");

            if (instance2 != null)
            {
                // 使用反射设置属性值
                PropertyInfo? nameProp = exampleType.GetProperty("Name");
                nameProp?.SetValue(instance2, "反射设置的名称");
                Console.WriteLine($"设置Name属性后的值: {nameProp?.GetValue(instance2)}");

                // 使用反射调用方法
                MethodInfo? greetMethod = exampleType.GetMethod("Greet");
                if (greetMethod != null)
                {
                    string? result = greetMethod.Invoke(instance2, null) as string;
                    Console.WriteLine($"调用Greet方法结果: {result}");
                }

                // 调用带参数的方法
                MethodInfo? addMethod = exampleType.GetMethod("Add");
                if (addMethod != null)
                {
                    int result = (int)(addMethod.Invoke(instance2, new object[] { 10, 20 }) ?? 0);
                    Console.WriteLine($"调用Add(10, 20)结果: {result}");
                }

                // 访问私有字段
                FieldInfo? privateField = exampleType.GetField("_privateValue", BindingFlags.NonPublic | BindingFlags.Instance);
                if (privateField != null)
                {
                    Console.WriteLine($"私有字段_privateValue的值: {privateField.GetValue(instance2)}");
                    privateField.SetValue(instance2, 100);
                    Console.WriteLine($"修改后的私有字段值: {privateField.GetValue(instance2)}");
                }
            }
        }
    }

    /// <summary>
    /// 用于反射演示的示例类
    /// </summary>
    public class ExampleClass
    {
        // 私有字段
        private int _privateValue;
        
        // 公共字段
        public string PublicField = "公共字段";

        // 静态字段
        public static int StaticField = 999;

        // 自动属性
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        // 只读属性
        public DateTime CreatedTime { get; } = DateTime.Now;

        // 无参构造函数
        public ExampleClass()
        {
            _privateValue = 0;
        }

        // 带参数的构造函数
        public ExampleClass(string name, int age)
        {
            Name = name;
            Age = age;
            _privateValue = age;
        }

        // 公共方法
        public string Greet()
        {
            return $"你好，我是 {Name}，年龄 {Age}";
        }

        // 带参数和返回值的方法
        public int Add(int a, int b)
        {
            return a + b;
        }

        // 私有方法
        private void PrivateMethod()
        {
            Console.WriteLine("这是一个私有方法");
        }
    }
}
