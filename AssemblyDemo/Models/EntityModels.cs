using System;

namespace AssemblyDemo.Models
{
    /// <summary>
    /// 表特性 - 用于标记实体类对应的数据库表名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }

    /// <summary>
    /// 列特性 - 用于标记属性对应的数据库列名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; }
        public bool IsPrimaryKey { get; set; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// 示例实体类 - 用户
    /// </summary>
    [Table("Users")]
    public class User
    {
        [Column("Id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column("UserName")]
        public string UserName { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Age")]
        public int Age { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; }

        public override string ToString()
        {
            return $"User[Id={Id}, UserName={UserName}, Email={Email}, Age={Age}, Created={CreatedTime:yyyy-MM-dd}]";
        }
    }

    /// <summary>
    /// 示例实体类 - 产品
    /// </summary>
    [Table("Products")]
    public class Product
    {
        [Column("ProductId", IsPrimaryKey = true)]
        public int ProductId { get; set; }

        [Column("ProductName")]
        public string ProductName { get; set; } = string.Empty;

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Stock")]
        public int Stock { get; set; }

        public override string ToString()
        {
            return $"Product[Id={ProductId}, Name={ProductName}, Price={Price:C}, Stock={Stock}]";
        }
    }
}
