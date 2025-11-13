using System;

namespace MokAbp.DependencyInjection.Attributes
{
    /// <summary>
    /// 作用域生命周期标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScopedDependencyAttribute : DependencyAttribute
    {
        public ScopedDependencyAttribute() : base(ServiceLifetime.Scoped)
        {
        }
    }
}
