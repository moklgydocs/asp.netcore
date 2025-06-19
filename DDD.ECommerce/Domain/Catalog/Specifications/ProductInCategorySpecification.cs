using System;
using System.Linq.Expressions;
using DDDCore.Specifications;

namespace DDD.ECommerce.Domain.Catalog.Specifications
{
    /// <summary>
    /// 指定类别的产品规约
    /// </summary>
    public class ProductInCategorySpecification : Specification<Product>
    {
        private readonly CategoryType _categoryType;
        
        public ProductInCategorySpecification(CategoryType categoryType)
        {
            _categoryType = categoryType;
        }
        
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.Category.Type == _categoryType;
        }
    }
}