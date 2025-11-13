using System;

namespace MokAbp.Modularity
{
    /// <summary>
    /// 模块依赖特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        public Type[] DependedTypes { get; }

        public DependsOnAttribute(params Type[] dependedTypes)
        {
            DependedTypes = dependedTypes ?? Array.Empty<Type>();
        }
    }
}
