using System;

namespace DDDCore.Domain
{
    /// <summary>
    /// 实体基类 - DDD中的核心构建块之一
    /// 实体由其唯一标识定义，而非其属性
    /// </summary>
    /// <typeparam name="TId">实体ID的类型</typeparam>
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        /// <summary>
        /// 实体的唯一标识符
        /// </summary>
        public TId Id { get; protected set; }

        /// <summary>
        /// 重写Equals方法，以便通过ID进行实体比较
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            var other = (Entity<TId>)obj;
            return Id.Equals(other.Id);
        }

        /// <summary>
        /// 重写GetHashCode方法，以支持基于ID的哈希计算
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// 重载相等运算符
        /// </summary>
        public static bool operator ==(Entity<TId> left, Entity<TId> right)
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
        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !(left == right);
        }
    }
}