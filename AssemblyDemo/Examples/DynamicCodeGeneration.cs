using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AssemblyDemo.Examples
{
    /// <summary>
    /// 动态代码生成示例
    /// 演示如何在运行时使用Reflection.Emit动态创建程序集、类型和方法
    /// </summary>
    public static class DynamicCodeGeneration
    {
        /// <summary>
        /// 生成动态程序集
        /// 在运行时创建新的程序集、类型和方法
        /// </summary>
        public static void GenerateDynamicAssembly()
        {
            Console.WriteLine("\n--- 动态代码生成演示 ---");

            // 创建动态程序集
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run); // Run表示只在内存中运行

            // 创建动态模块
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

            // 创建动态类型
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                "DynamicCalculator",
                TypeAttributes.Public | TypeAttributes.Class);

            Console.WriteLine("正在动态创建类型: DynamicCalculator");

            // 创建私有字段
            FieldBuilder nameField = typeBuilder.DefineField(
                "_name",
                typeof(string),
                FieldAttributes.Private);

            // 创建构造函数
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(string) });

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                      // 加载this
            ctorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!); // 调用基类构造函数
            ctorIL.Emit(OpCodes.Ldarg_0);                      // 加载this
            ctorIL.Emit(OpCodes.Ldarg_1);                      // 加载第一个参数
            ctorIL.Emit(OpCodes.Stfld, nameField);             // 设置字段值
            ctorIL.Emit(OpCodes.Ret);                          // 返回

            Console.WriteLine("  添加了构造函数");

            // 创建Name属性
            PropertyBuilder nameProperty = typeBuilder.DefineProperty(
                "Name",
                PropertyAttributes.HasDefault,
                typeof(string),
                null);

            // 创建属性的getter方法
            MethodBuilder getNameMethod = typeBuilder.DefineMethod(
                "get_Name",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(string),
                Type.EmptyTypes);

            ILGenerator getIL = getNameMethod.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);           // 加载this
            getIL.Emit(OpCodes.Ldfld, nameField);  // 加载字段
            getIL.Emit(OpCodes.Ret);               // 返回

            nameProperty.SetGetMethod(getNameMethod);
            Console.WriteLine("  添加了Name属性");

            // 创建Add方法
            MethodBuilder addMethod = typeBuilder.DefineMethod(
                "Add",
                MethodAttributes.Public,
                typeof(int),
                new Type[] { typeof(int), typeof(int) });

            ILGenerator addIL = addMethod.GetILGenerator();
            addIL.Emit(OpCodes.Ldarg_1);    // 加载第一个参数
            addIL.Emit(OpCodes.Ldarg_2);    // 加载第二个参数
            addIL.Emit(OpCodes.Add);        // 执行加法
            addIL.Emit(OpCodes.Ret);        // 返回结果

            Console.WriteLine("  添加了Add方法");

            // 创建Multiply方法
            MethodBuilder multiplyMethod = typeBuilder.DefineMethod(
                "Multiply",
                MethodAttributes.Public,
                typeof(int),
                new Type[] { typeof(int), typeof(int) });

            ILGenerator mulIL = multiplyMethod.GetILGenerator();
            mulIL.Emit(OpCodes.Ldarg_1);    // 加载第一个参数
            mulIL.Emit(OpCodes.Ldarg_2);    // 加载第二个参数
            mulIL.Emit(OpCodes.Mul);        // 执行乘法
            mulIL.Emit(OpCodes.Ret);        // 返回结果

            Console.WriteLine("  添加了Multiply方法");

            // 创建GetInfo方法（返回字符串）
            MethodBuilder getInfoMethod = typeBuilder.DefineMethod(
                "GetInfo",
                MethodAttributes.Public,
                typeof(string),
                Type.EmptyTypes);

            ILGenerator infoIL = getInfoMethod.GetILGenerator();
            infoIL.Emit(OpCodes.Ldstr, "这是一个动态生成的计算器类，名称为: ");
            infoIL.Emit(OpCodes.Ldarg_0);
            infoIL.Emit(OpCodes.Ldfld, nameField);
            infoIL.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) })!);
            infoIL.Emit(OpCodes.Ret);

            Console.WriteLine("  添加了GetInfo方法");

            // 完成类型创建
            Type? dynamicType = typeBuilder.CreateType();

            Console.WriteLine("\n动态类型创建完成！");

            if (dynamicType != null)
            {
                // 测试动态生成的类型
                Console.WriteLine("\n--- 测试动态生成的类型 ---");

                // 创建实例
                object? instance = Activator.CreateInstance(dynamicType, new object[] { "DynamicCalc-001" });

                if (instance != null)
                {
                    Console.WriteLine("成功创建实例");

                    // 获取并显示Name属性
                    PropertyInfo? nameProp = dynamicType.GetProperty("Name");
                    if (nameProp != null)
                    {
                        string? name = nameProp.GetValue(instance) as string;
                        Console.WriteLine($"Name属性值: {name}");
                    }

                    // 调用Add方法
                    MethodInfo? addMethodInfo = dynamicType.GetMethod("Add");
                    if (addMethodInfo != null)
                    {
                        int addResult = (int)(addMethodInfo.Invoke(instance, new object[] { 15, 27 }) ?? 0);
                        Console.WriteLine($"Add(15, 27) = {addResult}");
                    }

                    // 调用Multiply方法
                    MethodInfo? mulMethodInfo = dynamicType.GetMethod("Multiply");
                    if (mulMethodInfo != null)
                    {
                        int mulResult = (int)(mulMethodInfo.Invoke(instance, new object[] { 8, 9 }) ?? 0);
                        Console.WriteLine($"Multiply(8, 9) = {mulResult}");
                    }

                    // 调用GetInfo方法
                    MethodInfo? getInfoMethodInfo = dynamicType.GetMethod("GetInfo");
                    if (getInfoMethodInfo != null)
                    {
                        string? info = getInfoMethodInfo.Invoke(instance, null) as string;
                        Console.WriteLine($"GetInfo(): {info}");
                    }
                }
            }

            // 显示动态程序集的信息
            Console.WriteLine("\n--- 动态程序集信息 ---");
            Console.WriteLine($"程序集名称: {assemblyBuilder.FullName}");
            Console.WriteLine($"模块数量: {assemblyBuilder.GetModules().Length}");
            var types = assemblyBuilder.GetTypes();
            Console.WriteLine($"定义的类型数量: {types.Length}");
            foreach (var type in types)
            {
                Console.WriteLine($"  类型: {type.Name}");
                Console.WriteLine($"    方法数量: {type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length}");
                Console.WriteLine($"    属性数量: {type.GetProperties().Length}");
            }
        }
    }
}
