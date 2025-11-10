using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AssemblyDemo.Models;

namespace AssemblyDemo.ORM
{
    /// <summary>
    /// 简易ORM框架核心类
    /// 使用反射实现对象与数据库表的映射
    /// </summary>
    public class SimpleORM
    {
        /// <summary>
        /// 获取表名
        /// 通过反射读取Table特性
        /// </summary>
        public static string GetTableName<T>() where T : class
        {
            Type type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            
            if (tableAttr != null)
            {
                return tableAttr.TableName;
            }
            
            // 如果没有Table特性，使用类名作为表名
            return type.Name;
        }

        /// <summary>
        /// 获取列映射信息
        /// 返回属性名和列名的映射
        /// </summary>
        public static Dictionary<PropertyInfo, ColumnAttribute> GetColumnMappings<T>() where T : class
        {
            Type type = typeof(T);
            var mappings = new Dictionary<PropertyInfo, ColumnAttribute>();

            foreach (var prop in type.GetProperties())
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    mappings[prop] = columnAttr;
                }
            }

            return mappings;
        }

        /// <summary>
        /// 获取主键属性
        /// </summary>
        public static PropertyInfo? GetPrimaryKeyProperty<T>() where T : class
        {
            var mappings = GetColumnMappings<T>();
            return mappings.FirstOrDefault(m => m.Value.IsPrimaryKey).Key;
        }

        /// <summary>
        /// 生成INSERT SQL语句
        /// 使用反射从实体对象生成SQL
        /// </summary>
        public static string GenerateInsertSQL<T>(T entity) where T : class
        {
            string tableName = GetTableName<T>();
            var mappings = GetColumnMappings<T>();

            var columns = new List<string>();
            var values = new List<string>();

            foreach (var mapping in mappings)
            {
                PropertyInfo prop = mapping.Key;
                ColumnAttribute column = mapping.Value;

                // 如果是自增主键，跳过
                if (column.IsPrimaryKey)
                {
                    continue;
                }

                columns.Add(column.ColumnName);
                
                object? value = prop.GetValue(entity);
                values.Add(FormatValue(value));
            }

            var sql = new StringBuilder();
            sql.Append($"INSERT INTO {tableName} ");
            sql.Append($"({string.Join(", ", columns)}) ");
            sql.Append($"VALUES ({string.Join(", ", values)})");

            return sql.ToString();
        }

        /// <summary>
        /// 生成SELECT SQL语句
        /// </summary>
        public static string GenerateSelectSQL<T>(object? id = null) where T : class
        {
            string tableName = GetTableName<T>();
            var mappings = GetColumnMappings<T>();
            var columns = mappings.Select(m => m.Value.ColumnName);

            var sql = new StringBuilder();
            sql.Append($"SELECT {string.Join(", ", columns)} FROM {tableName}");

            if (id != null)
            {
                var pkProp = GetPrimaryKeyProperty<T>();
                if (pkProp != null)
                {
                    var pkColumn = mappings[pkProp];
                    sql.Append($" WHERE {pkColumn.ColumnName} = {FormatValue(id)}");
                }
            }

            return sql.ToString();
        }

        /// <summary>
        /// 生成UPDATE SQL语句
        /// </summary>
        public static string GenerateUpdateSQL<T>(T entity) where T : class
        {
            string tableName = GetTableName<T>();
            var mappings = GetColumnMappings<T>();
            var pkProp = GetPrimaryKeyProperty<T>();

            if (pkProp == null)
            {
                throw new InvalidOperationException("未找到主键属性");
            }

            var setClauses = new List<string>();

            foreach (var mapping in mappings)
            {
                PropertyInfo prop = mapping.Key;
                ColumnAttribute column = mapping.Value;

                // 跳过主键
                if (column.IsPrimaryKey)
                {
                    continue;
                }

                object? value = prop.GetValue(entity);
                setClauses.Add($"{column.ColumnName} = {FormatValue(value)}");
            }

            var pkColumn = mappings[pkProp];
            object? pkValue = pkProp.GetValue(entity);

            var sql = new StringBuilder();
            sql.Append($"UPDATE {tableName} SET ");
            sql.Append(string.Join(", ", setClauses));
            sql.Append($" WHERE {pkColumn.ColumnName} = {FormatValue(pkValue)}");

            return sql.ToString();
        }

        /// <summary>
        /// 生成DELETE SQL语句
        /// </summary>
        public static string GenerateDeleteSQL<T>(object id) where T : class
        {
            string tableName = GetTableName<T>();
            var mappings = GetColumnMappings<T>();
            var pkProp = GetPrimaryKeyProperty<T>();

            if (pkProp == null)
            {
                throw new InvalidOperationException("未找到主键属性");
            }

            var pkColumn = mappings[pkProp];

            return $"DELETE FROM {tableName} WHERE {pkColumn.ColumnName} = {FormatValue(id)}";
        }

        /// <summary>
        /// 将对象转换为字典（用于显示）
        /// </summary>
        public static Dictionary<string, object?> EntityToDictionary<T>(T entity) where T : class
        {
            var result = new Dictionary<string, object?>();
            var mappings = GetColumnMappings<T>();

            foreach (var mapping in mappings)
            {
                PropertyInfo prop = mapping.Key;
                ColumnAttribute column = mapping.Value;
                object? value = prop.GetValue(entity);
                result[column.ColumnName] = value;
            }

            return result;
        }

        /// <summary>
        /// 格式化值为SQL字符串
        /// 处理不同类型的值
        /// </summary>
        private static string FormatValue(object? value)
        {
            if (value == null)
            {
                return "NULL";
            }

            Type type = value.GetType();

            if (type == typeof(string))
            {
                return $"'{value.ToString()?.Replace("'", "''")}'";
            }
            else if (type == typeof(DateTime))
            {
                DateTime dt = (DateTime)value;
                return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (type == typeof(bool))
            {
                return (bool)value ? "1" : "0";
            }
            else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            {
                return value.ToString() ?? "0";
            }
            else
            {
                return value.ToString() ?? "0";
            }
        }

        /// <summary>
        /// 生成创建表的SQL语句
        /// 基于实体类型生成DDL
        /// </summary>
        public static string GenerateCreateTableSQL<T>() where T : class
        {
            string tableName = GetTableName<T>();
            var mappings = GetColumnMappings<T>();

            var sql = new StringBuilder();
            sql.AppendLine($"CREATE TABLE {tableName} (");

            var columnDefinitions = new List<string>();

            foreach (var mapping in mappings)
            {
                PropertyInfo prop = mapping.Key;
                ColumnAttribute column = mapping.Value;

                string columnDef = $"    {column.ColumnName} {GetSQLType(prop.PropertyType)}";

                if (column.IsPrimaryKey)
                {
                    columnDef += " PRIMARY KEY";
                }

                columnDefinitions.Add(columnDef);
            }

            sql.AppendLine(string.Join(",\n", columnDefinitions));
            sql.Append(")");

            return sql.ToString();
        }

        /// <summary>
        /// 将C#类型映射为SQL类型
        /// </summary>
        private static string GetSQLType(Type type)
        {
            if (type == typeof(int))
                return "INTEGER";
            else if (type == typeof(long))
                return "BIGINT";
            else if (type == typeof(string))
                return "NVARCHAR(255)";
            else if (type == typeof(DateTime))
                return "DATETIME";
            else if (type == typeof(bool))
                return "BIT";
            else if (type == typeof(decimal))
                return "DECIMAL(18,2)";
            else if (type == typeof(double))
                return "FLOAT";
            else
                return "NVARCHAR(255)";
        }
    }
}
