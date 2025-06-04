using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace Mok.Modularity;
public static class ModuleFactory
{
    // 使用延迟初始化
    private static readonly ConcurrentDictionary<Type, Func<MokModule>> _factoryCache =
        new ConcurrentDictionary<Type, Func<MokModule>>();

    // 使用延迟初始化
    private static readonly ConcurrentDictionary<Type, Lazy<Func<MokModule>>> _lazyFactoryCache =
        new ConcurrentDictionary<Type, Lazy<Func<MokModule>>>();

    // 当模块数量低于此阈值时使用Activator
    private const int EXPRESSION_TREES_THRESHOLD = 10;

    public static MokModule CreateModule(Type moduleType, bool preferActivator = false)
    {
        if (moduleType == null)
            throw new ArgumentNullException(nameof(moduleType));

        if (!typeof(MokModule).IsAssignableFrom(moduleType))
            throw new ArgumentException($"类型 {moduleType.FullName} 不是有效的模块类型");

        // 如果明确指定使用Activator或模块数量少，直接使用Activator
        if (preferActivator)
        {
            return (MokModule)Activator.CreateInstance(moduleType);
        }

        // 获取或创建工厂委托
        var factory = _factoryCache.GetOrAdd(moduleType, CreateFactory);
        return factory();
    }

    private static Func<MokModule> CreateFactory(Type moduleType)
    {
        // 简单版本 - 仅包装Activator以获得缓存优势
        return () => (MokModule)Activator.CreateInstance(moduleType);
    }

    // 在ModuleLoader中使用的适配方法
    public static void InstantiateModules(List<Type> moduleTypes, List<MokModule> targetList)
    {
        // 根据模块数量选择策略
        bool useActivator = moduleTypes.Count < EXPRESSION_TREES_THRESHOLD;

        foreach (var moduleType in moduleTypes)
        {
            var module = CreateModule(moduleType, useActivator);
            targetList.Add(module);
        }
    }

    public static MokModule CreateLazyModule(Type moduleType)
    {
        if (moduleType == null)
            throw new ArgumentNullException(nameof(moduleType));

        if (!typeof(MokModule).IsAssignableFrom(moduleType))
            throw new ArgumentException($"类型 {moduleType.FullName} 不是有效的模块类型");

        // 使用Lazy<T>延迟初始化工厂
        var lazyFactory = _lazyFactoryCache.GetOrAdd(
            moduleType,
            t => new Lazy<Func<MokModule>>(() =>
            {
                // 如果只有少量模块，直接使用Activator可能更高效
                return () => (MokModule)Activator.CreateInstance(t);
            })
        );

        return lazyFactory.Value();
    }
}