using System;
using System.Collections.Generic;
using AssemblyDemo.Models;

namespace AssemblyDemo.ORM
{
    /// <summary>
    /// ORM框架演示类
    /// 展示如何使用简易ORM框架进行CRUD操作
    /// </summary>
    public static class ORMDemo
    {
        /// <summary>
        /// 运行ORM演示
        /// </summary>
        public static void RunORMDemo()
        {
            Console.WriteLine("\n--- 简易ORM框架演示 ---");
            Console.WriteLine("本演示展示如何使用反射实现对象关系映射");

            // 1. 创建表结构
            DemonstrateCreateTable();

            // 2. 插入数据
            DemonstrateInsert();

            // 3. 查询数据
            DemonstrateSelect();

            // 4. 更新数据
            DemonstrateUpdate();

            // 5. 删除数据
            DemonstrateDelete();

            // 6. 展示元数据分析
            DemonstrateMetadataAnalysis();
        }

        /// <summary>
        /// 演示创建表
        /// </summary>
        private static void DemonstrateCreateTable()
        {
            Console.WriteLine("\n【1. 创建表SQL生成】");
            
            // 为User实体生成创建表SQL
            string userTableSQL = SimpleORM.GenerateCreateTableSQL<User>();
            Console.WriteLine("\nUser表创建SQL:");
            Console.WriteLine(userTableSQL);

            // 为Product实体生成创建表SQL
            string productTableSQL = SimpleORM.GenerateCreateTableSQL<Product>();
            Console.WriteLine("\nProduct表创建SQL:");
            Console.WriteLine(productTableSQL);
        }

        /// <summary>
        /// 演示插入操作
        /// </summary>
        private static void DemonstrateInsert()
        {
            Console.WriteLine("\n【2. INSERT语句生成】");

            // 创建User对象
            var user = new User
            {
                Id = 1,
                UserName = "张三",
                Email = "zhangsan@example.com",
                Age = 28,
                CreatedTime = DateTime.Now
            };

            // 生成INSERT SQL
            string insertSQL = SimpleORM.GenerateInsertSQL(user);
            Console.WriteLine("\n插入用户的SQL:");
            Console.WriteLine(insertSQL);

            // 创建Product对象
            var product = new Product
            {
                ProductId = 101,
                ProductName = "笔记本电脑",
                Price = 5999.99m,
                Stock = 50
            };

            // 生成INSERT SQL
            string productInsertSQL = SimpleORM.GenerateInsertSQL(product);
            Console.WriteLine("\n插入产品的SQL:");
            Console.WriteLine(productInsertSQL);

            // 显示实体详情
            Console.WriteLine("\n实体对象详情:");
            Console.WriteLine($"  {user}");
            Console.WriteLine($"  {product}");
        }

        /// <summary>
        /// 演示查询操作
        /// </summary>
        private static void DemonstrateSelect()
        {
            Console.WriteLine("\n【3. SELECT语句生成】");

            // 查询所有用户
            string selectAllSQL = SimpleORM.GenerateSelectSQL<User>();
            Console.WriteLine("\n查询所有用户:");
            Console.WriteLine(selectAllSQL);

            // 按ID查询特定用户
            string selectByIdSQL = SimpleORM.GenerateSelectSQL<User>(1);
            Console.WriteLine("\n查询ID为1的用户:");
            Console.WriteLine(selectByIdSQL);

            // 查询所有产品
            string selectAllProductsSQL = SimpleORM.GenerateSelectSQL<Product>();
            Console.WriteLine("\n查询所有产品:");
            Console.WriteLine(selectAllProductsSQL);

            // 按ID查询特定产品
            string selectProductByIdSQL = SimpleORM.GenerateSelectSQL<Product>(101);
            Console.WriteLine("\n查询ID为101的产品:");
            Console.WriteLine(selectProductByIdSQL);
        }

        /// <summary>
        /// 演示更新操作
        /// </summary>
        private static void DemonstrateUpdate()
        {
            Console.WriteLine("\n【4. UPDATE语句生成】");

            // 创建要更新的User对象
            var user = new User
            {
                Id = 1,
                UserName = "张三（已更新）",
                Email = "zhangsan_new@example.com",
                Age = 29,
                CreatedTime = DateTime.Now
            };

            // 生成UPDATE SQL
            string updateSQL = SimpleORM.GenerateUpdateSQL(user);
            Console.WriteLine("\n更新用户信息:");
            Console.WriteLine(updateSQL);

            // 创建要更新的Product对象
            var product = new Product
            {
                ProductId = 101,
                ProductName = "笔记本电脑 Pro",
                Price = 6999.99m,
                Stock = 45
            };

            // 生成UPDATE SQL
            string productUpdateSQL = SimpleORM.GenerateUpdateSQL(product);
            Console.WriteLine("\n更新产品信息:");
            Console.WriteLine(productUpdateSQL);
        }

        /// <summary>
        /// 演示删除操作
        /// </summary>
        private static void DemonstrateDelete()
        {
            Console.WriteLine("\n【5. DELETE语句生成】");

            // 生成DELETE SQL
            string deleteUserSQL = SimpleORM.GenerateDeleteSQL<User>(1);
            Console.WriteLine("\n删除ID为1的用户:");
            Console.WriteLine(deleteUserSQL);

            // 生成DELETE SQL
            string deleteProductSQL = SimpleORM.GenerateDeleteSQL<Product>(101);
            Console.WriteLine("\n删除ID为101的产品:");
            Console.WriteLine(deleteProductSQL);
        }

        /// <summary>
        /// 演示元数据分析
        /// 展示ORM框架如何通过反射获取实体信息
        /// </summary>
        private static void DemonstrateMetadataAnalysis()
        {
            Console.WriteLine("\n【6. 元数据分析】");
            Console.WriteLine("ORM框架通过反射分析实体类的元数据\n");

            // 分析User实体
            Console.WriteLine("User实体分析:");
            AnalyzeEntity<User>();

            // 分析Product实体
            Console.WriteLine("\nProduct实体分析:");
            AnalyzeEntity<Product>();
        }

        /// <summary>
        /// 分析实体元数据
        /// </summary>
        private static void AnalyzeEntity<T>() where T : class
        {
            // 获取表名
            string tableName = SimpleORM.GetTableName<T>();
            Console.WriteLine($"  表名: {tableName}");

            // 获取列映射
            var mappings = SimpleORM.GetColumnMappings<T>();
            Console.WriteLine($"  列数量: {mappings.Count}");

            // 获取主键
            var pkProp = SimpleORM.GetPrimaryKeyProperty<T>();
            Console.WriteLine($"  主键: {pkProp?.Name ?? "未找到"}");

            // 显示所有列
            Console.WriteLine("  列信息:");
            foreach (var mapping in mappings)
            {
                string pkMarker = mapping.Value.IsPrimaryKey ? " [主键]" : "";
                Console.WriteLine($"    - {mapping.Key.Name} ({mapping.Key.PropertyType.Name}) -> {mapping.Value.ColumnName}{pkMarker}");
            }

            // 创建示例实例并转换为字典
            object? instance = Activator.CreateInstance(typeof(T));
            if (instance != null)
            {
                var dict = SimpleORM.EntityToDictionary((T)instance);
                Console.WriteLine("  默认值字典:");
                foreach (var kvp in dict)
                {
                    Console.WriteLine($"    {kvp.Key}: {kvp.Value ?? "NULL"}");
                }
            }
        }
    }
}
