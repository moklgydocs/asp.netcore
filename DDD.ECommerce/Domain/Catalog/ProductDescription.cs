using System;
using System.Collections.Generic;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 产品描述值对象
    /// 封装产品描述的业务规则
    /// </summary>
    public class ProductDescription : ValueObject
    {
        // 描述长度最大值
        private const int MaxLength = 2000;
        
        public string Value { get; }
        
        private ProductDescription() { }

        /// <summary>
        /// 创建产品描述值对象
        /// </summary>
        /// <param name="description">产品描述文本</param>
        /// <exception cref="ArgumentException">如果描述不符合业务规则</exception>
        public static ProductDescription Create(string description)
        {
            // 如果为null，创建空描述
            if (description == null)
                return new ProductDescription { Value = string.Empty };
                
            // 验证描述长度
            if (description.Length > MaxLength)
                throw new ArgumentException($"Product description cannot be longer than {MaxLength} characters.", nameof(description));
                
            // 创建并返回实例
            return new ProductDescription { Value = description.Trim() };
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