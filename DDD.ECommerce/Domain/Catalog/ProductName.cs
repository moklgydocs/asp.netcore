using System;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 产品名称值对象
    /// 封装产品名称的业务规则
    /// </summary>
    public class ProductName : ValueObject
    {
        // 名称长度范围
        private const int MinLength = 3;
        private const int MaxLength = 100;
        
        // 公开值的只读属性
        public string Value { get; }
        
        // 防止外部直接实例化
        private ProductName() { }

        /// <summary>
        /// 创建产品名称值对象
        /// </summary>
        /// <param name="name">产品名称</param>
        /// <exception cref="ArgumentException">如果名称不符合业务规则</exception>
        public static ProductName Create(string name)
        {
            // 验证名称是否为空
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));
                
            // 验证名称长度
            if (name.Length < MinLength)
                throw new ArgumentException($"Product name must be at least {MinLength} characters long.", nameof(name));
                
            if (name.Length > MaxLength)
                throw new ArgumentException($"Product name cannot be longer than {MaxLength} characters.", nameof(name));
                
            // 创建并返回实例
            return new ProductName { Value = name.Trim() };
        }

        /// <summary>
        /// 实现ValueObject的抽象方法，提供用于比较的值
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        public override ValueObject Clone()
        {
            return Create(Value);
        }
        
        /// <summary>
        /// 提供字符串表示形式
        /// </summary>
        public override string ToString()
        {
            return Value;
        }
    }
}