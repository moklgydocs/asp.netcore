using System.Collections.Generic;
using System.Linq;

namespace DDDCore.Domain
{
    /// <summary>
    /// 值对象基类 - DDD中的核心构建块
    /// 值对象是通过其所有属性值定义的不可变对象，没有身份标识
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// 子类必须实现此方法以提供用于相等比较的组件
        /// </summary>
        /// <returns>用于对比的属性值集合</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// 重写Equals方法，通过比较所有属性值确定相等性
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// 重写GetHashCode方法，以支持基于所有属性值的哈希计算
        /// </summary>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// 重载相等运算符
        /// </summary>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// 重载不等运算符
        /// </summary>
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 创建值对象的副本
        /// </summary>
        /// <returns>当前值对象的深拷贝</returns>
        public abstract ValueObject Clone();
    }
}