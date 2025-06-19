using System;
using System.Linq.Expressions;

namespace DDDCore.Specifications
{
    /// <summary>
    /// 规约模式抽象基类
    /// 规约模式将查询条件封装为对象，便于复用和组合
    /// </summary>
    /// <typeparam name="T">规约适用的实体类型</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// 获取表示规约的表达式树
        /// </summary>
        /// <returns>规约的表达式树</returns>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// 检查实体是否满足规约
        /// </summary>
        /// <param name="entity">要检查的实体</param>
        /// <returns>如果满足规约则为true，否则为false</returns>
        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        /// <summary>
        /// 组合两个规约为"与"关系
        /// </summary>
        /// <param name="specification">要组合的规约</param>
        /// <returns>组合后的新规约</returns>
        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        /// <summary>
        /// 组合两个规约为"或"关系
        /// </summary>
        /// <param name="specification">要组合的规约</param>
        /// <returns>组合后的新规约</returns>
        public Specification<T> Or(Specification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        /// <summary>
        /// 创建规约的否定
        /// </summary>
        /// <returns>表示原规约否定的新规约</returns>
        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }
    }

    /// <summary>
    /// 表示两个规约"与"组合的规约
    /// </summary>
    internal class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();
            
            var parameter = Expression.Parameter(typeof(T));
            
            var leftVisitor = new ReplaceParameterVisitor(leftExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(leftExpression.Body);
            
            var rightVisitor = new ReplaceParameterVisitor(rightExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(rightExpression.Body);
            
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

    /// <summary>
    /// 表示两个规约"或"组合的规约
    /// </summary>
    internal class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public OrSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> leftExpression = _left.ToExpression();
            Expression<Func<T, bool>> rightExpression = _right.ToExpression();
            
            var parameter = Expression.Parameter(typeof(T));
            
            var leftVisitor = new ReplaceParameterVisitor(leftExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(leftExpression.Body);
            
            var rightVisitor = new ReplaceParameterVisitor(rightExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(rightExpression.Body);
            
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }
    }

    /// <summary>
    /// 表示规约的否定
    /// </summary>
    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            Expression<Func<T, bool>> expression = _specification.ToExpression();
            
            var notExpression = Expression.Not(expression.Body);
            
            return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters[0]);
        }
    }

    /// <summary>
    /// 用于替换表达式中参数的访问者
    /// </summary>
    internal class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
        }
    }
}