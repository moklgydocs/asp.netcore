using System;
using System.Collections.Generic;
using System.Reflection;

namespace MokAbp.Modularity
{
    /// <summary>
    /// 模块描述符
    /// </summary>
    public class ModuleDescriptor
    {
        public Type ModuleType { get; }
        public IMokAbpModule Instance { get; }
        public Assembly Assembly { get; }
        public List<ModuleDescriptor> Dependencies { get; }

        public ModuleDescriptor(Type moduleType, IMokAbpModule instance, Assembly assembly)
        {
            ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            Dependencies = new List<ModuleDescriptor>();
        }
    }
}
