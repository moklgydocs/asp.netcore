using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Modularity
{
    [AttributeUsage(AttributeTargets.Class,  AllowMultiple = true)]
    public class DependsOnAttribute: Attribute
    {
        public Type[] DependedModuleTypes { get; }
        public DependsOnAttribute(params Type[] dependsOn)
        {
            DependedModuleTypes = dependsOn;
        }
        public virtual Type[] GetDependedTypes()
        {
            return DependedModuleTypes;
        }
    }
}
