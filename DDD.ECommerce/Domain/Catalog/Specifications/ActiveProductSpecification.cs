using System;
using System.Linq.Expressions;
using DDDCore.Specifications;

namespace DDD.ECommerce.Domain.Catalog.Specifications
{
    /// <summary>
    /// 活动产品规约
    /// </summary>
    public class ActiveProductSpecification : Specification<Product>
    {
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.IsActive;
        }
    }
}