using System;

namespace MokAbp.DependencyInjection.Attributes
{
    /// <summary>
    /// 单例生命周期标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }
}
