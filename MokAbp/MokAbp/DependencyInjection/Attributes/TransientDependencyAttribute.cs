using System;

namespace MokAbp.DependencyInjection.Attributes
{
    /// <summary>
    /// 瞬时生命周期标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute() : base(ServiceLifetime.Transient)
        {
        }
    }
}
