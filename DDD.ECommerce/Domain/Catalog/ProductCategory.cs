using System;
using System.Collections.Generic;
using DDDCore.Domain;

namespace DDD.ECommerce.Domain.Catalog
{
    /// <summary>
    /// 产品类别枚举
    /// </summary>
    public enum CategoryType
    {
        Electronics,
        Clothing,
        Books,
        HomeAndGarden,
        Sports,
        Toys,
        Beauty,
        Food,
        Other
    }

    /// <summary>
    /// 产品类别值对象
    /// </summary>
    public class ProductCategory : ValueObject
    {
        public CategoryType Type { get; }
        public string SubCategory { get; }
        
        private ProductCategory() { }

        /// <summary>
        /// 创建产品类别值对象
        /// </summary>
        /// <param name="type">主类别</param>
        /// <param name="subCategory">子类别(可选)</param>
        public static ProductCategory Create(CategoryType type, string subCategory = null)
        {
            // 验证子类别长度
            if (subCategory != null && subCategory.Length > 50)
                throw new ArgumentException("Subcategory name cannot exceed 50 characters.", nameof(subCategory));
                
            return new ProductCategory 
            { 
                Type = type, 
                SubCategory = subCategory?.Trim() 
            };
        }
        
        /// <summary>
        /// 实现ValueObject的抽象方法，提供用于比较的值
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return SubCategory ?? string.Empty;
        }

        /// <summary>
        /// 实现克隆方法
        /// </summary>
        public override ValueObject Clone()
        {
            return Create(Type, SubCategory);
        }
        
        /// <summary>
        /// 提供字符串表示形式
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(SubCategory))
                return Type.ToString();
                
            return $"{Type} / {SubCategory}";
        }
    }
}