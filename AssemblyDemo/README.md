# AssemblyDemo - .NET 程序集深度学习项目

## 项目简介

本项目是一个完整的.NET程序集学习示例，从基础到高级，逐步深入学习.NET程序集、元数据、CLR头、PE头等核心概念，并最终实现一个简易的ORM（对象关系映射）框架。

## 学习内容

### 第一部分：程序集和元数据基础

**位置**: `Examples/AssemblyBasics.cs`

学习内容：
- 什么是程序集（Assembly）
- 如何获取程序集信息
- 程序集的结构和组成
- 元数据（Metadata）的概念
- 如何读取和分析类型元数据

关键概念：
- `Assembly.GetExecutingAssembly()` - 获取当前程序集
- `AssemblyName` - 程序集名称信息
- `Type.GetTypes()` - 获取程序集中的所有类型
- 反射读取类型的属性、方法、字段等元数据

### 第二部分：反射深入探索

**位置**: `Examples/ReflectionExamples.cs`

学习内容：
- 反射（Reflection）的工作原理
- 使用反射检查类型信息
- 动态创建对象实例
- 动态调用方法
- 访问和修改私有成员

关键概念：
- `Type.GetProperties()` - 获取属性
- `Type.GetMethods()` - 获取方法
- `Type.GetFields()` - 获取字段
- `Activator.CreateInstance()` - 动态创建实例
- `MethodInfo.Invoke()` - 动态调用方法
- `BindingFlags` - 控制成员搜索范围

### 第三部分：CLR和PE头探索

**位置**: `Examples/PEHeaderExplorer.cs`

学习内容：
- PE（Portable Executable）文件格式
- COFF头（Common Object File Format）
- PE头的结构和内容
- CLR头（.NET特定）
- 节（Section）的概念
- 元数据表

关键概念：
- `PEReader` - PE文件读取器
- `CoffHeader` - COFF头信息
- `PEHeader` - PE头信息
- `CorHeader` - CLR头信息
- `MetadataReader` - 元数据读取器
- 节头、入口点、基地址等

### 第四部分：动态代码生成

**位置**: `Examples/DynamicCodeGeneration.cs`

学习内容：
- Reflection.Emit的使用
- 运行时动态创建程序集
- 动态创建类型和成员
- IL（中间语言）代码生成
- OpCodes的使用

关键概念：
- `AssemblyBuilder` - 程序集构建器
- `ModuleBuilder` - 模块构建器
- `TypeBuilder` - 类型构建器
- `MethodBuilder` - 方法构建器
- `ILGenerator` - IL代码生成器
- `OpCodes` - IL操作码

### 第五部分：简易ORM框架实现

**位置**: `ORM/SimpleORM.cs` 和 `ORM/ORMDemo.cs`

学习内容：
- ORM的基本概念
- 使用特性（Attribute）进行映射
- 通过反射实现对象到关系的映射
- SQL语句的动态生成
- CRUD操作的实现

关键概念：
- 自定义特性：`TableAttribute`、`ColumnAttribute`
- 实体类到数据库表的映射
- SQL生成：INSERT, SELECT, UPDATE, DELETE
- DDL生成：CREATE TABLE
- 类型映射：C#类型到SQL类型

## 项目结构

```
AssemblyDemo/
├── Program.cs                          # 主程序入口
├── Examples/                           # 示例代码目录
│   ├── AssemblyBasics.cs              # 程序集基础
│   ├── ReflectionExamples.cs          # 反射示例
│   ├── PEHeaderExplorer.cs            # PE头探索
│   └── DynamicCodeGeneration.cs       # 动态代码生成
├── ORM/                                # ORM框架目录
│   ├── SimpleORM.cs                   # ORM核心实现
│   └── ORMDemo.cs                     # ORM演示
└── Models/                             # 模型类目录
    └── EntityModels.cs                # 实体模型和特性定义
```

## 如何运行

### 前提条件
- .NET 8.0 SDK 或更高版本

### 运行步骤

1. 进入项目目录：
```bash
cd demo
```

2. 构建项目：
```bash
dotnet build
```

3. 运行项目：
```bash
dotnet run --project AssemblyDemo
```

或者直接：
```bash
cd AssemblyDemo
dotnet run
```

## 运行结果说明

程序运行后会按顺序执行以下演示：

1. **程序集和元数据基础** - 显示当前程序集的详细信息，包括名称、版本、包含的类型等
2. **反射深入探索** - 演示如何使用反射检查类型、创建对象、调用方法
3. **CLR和PE头探索** - 深入分析PE文件格式，显示COFF头、PE头、CLR头、节信息等
4. **动态代码生成** - 在运行时动态创建一个计算器类，并测试其功能
5. **简易ORM框架演示** - 展示如何使用ORM框架生成各种SQL语句，实现对象关系映射

## 核心技术要点

### 反射（Reflection）
- 程序在运行时检查和操作自身结构的能力
- 可以获取类型信息、创建实例、调用方法、访问字段等
- 性能开销较大，应谨慎使用

### 元数据（Metadata）
- 描述程序集、类型、成员的数据
- 存储在程序集中
- 通过反射API访问

### PE格式
- Windows可执行文件的标准格式
- 包含COFF头、PE头、节、CLR头等
- .NET程序集是特殊的PE文件，包含额外的CLR元数据

### IL（中间语言）
- .NET的中间表示形式
- 介于源代码和机器码之间
- 由JIT编译器在运行时编译为机器码

### ORM（对象关系映射）
- 在对象模型和关系数据库之间建立映射
- 使用反射自动处理对象和数据库的转换
- 简化数据访问代码

## 扩展学习建议

1. **深入学习IL** - 使用ILDasm工具查看编译后的IL代码
2. **性能优化** - 学习如何缓存反射结果，使用表达式树等技术
3. **完整的ORM** - 研究Entity Framework Core的实现
4. **代码生成** - 学习T4模板、Source Generator等技术
5. **高级反射** - 学习动态方法、委托、表达式树等

## 注意事项

1. 本项目仅用于学习目的，ORM框架是简化实现
2. 生产环境中应使用成熟的ORM框架如Entity Framework Core
3. 反射操作有性能开销，在性能敏感场景应考虑其他方案
4. PE头分析依赖于System.Reflection.Metadata包

## 参考资料

- [.NET反射官方文档](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection)
- [PE格式规范](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format)
- [ECMA-335 CLI规范](https://www.ecma-international.org/publications-and-standards/standards/ecma-335/)
- [System.Reflection.Metadata文档](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata)

## 许可证

本项目采用MIT许可证，可自由使用和修改。
