using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MokAbp.Modularity
{
    /// <summary>
    /// 模块加载器，负责加载模块及其依赖
    /// </summary>
    public class ModuleLoader
    {
        public List<ModuleDescriptor> LoadModules(Type startupModuleType)
        {
            var allModules = new List<ModuleDescriptor>();
            var visitedTypes = new HashSet<Type>();

            LoadModuleRecursive(startupModuleType, allModules, visitedTypes);

            // 拓扑排序，确保依赖项先加载
            return SortModules(allModules);
        }

        private void LoadModuleRecursive(Type moduleType, List<ModuleDescriptor> modules, HashSet<Type> visitedTypes)
        {
            if (visitedTypes.Contains(moduleType))
            {
                return;
            }

            visitedTypes.Add(moduleType);

            // 验证模块类型
            ValidateModuleType(moduleType);

            // 创建模块实例
            var instance = (IMokAbpModule)Activator.CreateInstance(moduleType)!;
            var assembly = moduleType.Assembly;
            var descriptor = new ModuleDescriptor(moduleType, instance, assembly);

            // 加载依赖模块
            var dependsOnAttributes = moduleType.GetCustomAttributes<DependsOnAttribute>();
            foreach (var dependsOn in dependsOnAttributes)
            {
                foreach (var dependedType in dependsOn.DependedTypes)
                {
                    LoadModuleRecursive(dependedType, modules, visitedTypes);
                    var dependedDescriptor = modules.First(m => m.ModuleType == dependedType);
                    descriptor.Dependencies.Add(dependedDescriptor);
                }
            }

            modules.Add(descriptor);
        }

        private void ValidateModuleType(Type moduleType)
        {
            if (!typeof(IMokAbpModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException($"Type {moduleType.FullName} must implement {nameof(IMokAbpModule)}");
            }

            if (moduleType.IsAbstract)
            {
                throw new ArgumentException($"Type {moduleType.FullName} cannot be abstract");
            }

            if (moduleType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"Type {moduleType.FullName} must have a parameterless constructor");
            }
        }

        private List<ModuleDescriptor> SortModules(List<ModuleDescriptor> modules)
        {
            // 拓扑排序算法
            var sorted = new List<ModuleDescriptor>();
            var visited = new HashSet<ModuleDescriptor>();
            var visiting = new HashSet<ModuleDescriptor>();

            foreach (var module in modules)
            {
                Visit(module, sorted, visited, visiting);
            }

            return sorted;
        }

        private void Visit(ModuleDescriptor module, List<ModuleDescriptor> sorted, 
            HashSet<ModuleDescriptor> visited, HashSet<ModuleDescriptor> visiting)
        {
            if (visited.Contains(module))
            {
                return;
            }

            if (visiting.Contains(module))
            {
                throw new InvalidOperationException($"Circular dependency detected for module {module.ModuleType.FullName}");
            }

            visiting.Add(module);

            foreach (var dependency in module.Dependencies)
            {
                Visit(dependency, sorted, visited, visiting);
            }

            visiting.Remove(module);
            visited.Add(module);
            sorted.Add(module);
        }
    }
}
