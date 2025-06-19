using System;
using System.Linq.Expressions;

namespace DDD.Core.Domain
{
    /// <summary>
    /// 规约模式基类 - 用于封装复杂的业务查询逻辑
    /// 规约可以组合，重用，便于测试
    /// </summary>
    /// <typeparam name="T">规约适用的实体类型</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// 规约表达式
        /// </summary>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// 检查实体是否满足规约
        /// </summary>
        public bool IsSatisfiedBy(T entity)
        {
            var predicate = ToExpression().Compile();
            return predicate(entity);
        }

        /// <summary>
        /// 与操作 - 组合两个规约
        /// </summary>
        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        /// <summary>
        /// 或操作 - 组合两个规约
        /// </summary>
        public Specification<T> Or(Specification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        /// <summary>
        /// 非操作 - 规约取反
        /// </summary>
        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        // 操作符重载
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            return specification.ToExpression();
        }

        public static bool operator true(Specification<T> specification)
        {
            return false;
        }

        public static bool operator false(Specification<T> specification)
        {
            return false;
        }

        public static Specification<T> operator &(Specification<T> left, Specification<T> right)
        {
            return left.And(right);
        }

        public static Specification<T> operator |(Specification<T> left, Specification<T> right)
        {
            return left.Or(right);
        }

        public static Specification<T> operator !(Specification<T> specification)
        {
            return specification.Not();
        }
    }

    // 组合规约实现
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
            var leftExpression = _left.ToExpression();
            var rightExpression = _right.ToExpression();

            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(leftExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(rightExpression.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

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
            var leftExpression = _left.ToExpression();
            var rightExpression = _right.ToExpression();

            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(leftExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);
            var right = rightVisitor.Visit(rightExpression.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left, right), parameter);
        }
    }

    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expression = _specification.ToExpression();
            var parameter = expression.Parameters[0];
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

    // 表达式访问者 - 用于参数替换
    internal class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}