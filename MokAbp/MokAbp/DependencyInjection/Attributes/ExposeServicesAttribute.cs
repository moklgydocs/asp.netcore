using System;

namespace MokAbp.DependencyInjection.Attributes
{
    /// <summary>
    /// 指定要公开的服务类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExposeServicesAttribute : Attribute
    {
        public Type[] ServiceTypes { get; }
        
        /// <summary>
        /// 是否包含默认服务（即实现类本身）
        /// </summary>
        public bool IncludeDefaults { get; set; }
        
        /// <summary>
        /// 是否包含实现的所有接口
        /// </summary>
        public bool IncludeInterfaces { get; set; }

        public ExposeServicesAttribute(params Type[] serviceTypes)
        {
            ServiceTypes = serviceTypes ?? Array.Empty<Type>();
            IncludeDefaults = true;
            IncludeInterfaces = false;
        }
    }
}
