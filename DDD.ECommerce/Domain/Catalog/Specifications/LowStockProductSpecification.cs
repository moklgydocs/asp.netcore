using System;
using System.Linq.Expressions;
using DDDCore.Specifications;

namespace DDD.ECommerce.Domain.Catalog.Specifications
{
    /// <summary>
    /// 低库存产品规约
    /// </summary>
    public class LowStockProductSpecification : Specification<Product>
    {
        private readonly int _threshold;
        
        public LowStockProductSpecification(int threshold)
        {
            _threshold = threshold;
        }
        
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.StockQuantity <= _threshold && product.StockQuantity > 0;
        }
    }
}