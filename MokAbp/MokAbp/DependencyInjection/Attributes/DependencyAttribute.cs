using System;

namespace MokAbp.DependencyInjection.Attributes
{
    /// <summary>
    /// 依赖注入标记特性，用于自动注册服务
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class DependencyAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; }
        
        /// <summary>
        /// 是否尝试注册（如果已存在则不注册）
        /// </summary>
        public bool TryRegister { get; set; }
        
        /// <summary>
        /// 是否替换已有的注册
        /// </summary>
        public bool ReplaceServices { get; set; }

        public DependencyAttribute()
        {
            Lifetime = ServiceLifetime.Transient;
            TryRegister = false;
            ReplaceServices = false;
        }

        public DependencyAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            TryRegister = false;
            ReplaceServices = false;
        }
    }
}
