using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Core.Domain
{
    /// <summary>
    /// 值对象基类 - DDD中的核心概念
    /// 值对象没有标识，通过所有属性值来判断相等性
    /// 值对象是不可变的，任何修改都应该返回新的实例
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        /// <summary>
        /// 获取用于相等性比较的原子值
        /// 子类必须实现此方法，返回所有用于比较的属性值
        /// </summary>
        /// <returns>用于比较的属性值集合</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// 值对象相等性比较
        /// </summary>
        public bool Equals(ValueObject other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValueObject);
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// 复制当前值对象 - 用于创建副本
        /// </summary>
        protected T Copy<T>() where T : ValueObject
        {
            return (T)MemberwiseClone();
        }
    }
}