using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Mok.Modularity
{
    /// <summary>
    /// 模块加载器 - 负责模块的发现、排序、初始化和关闭
    /// </summary>
    public class ModuleLoader : IDisposable, IAsyncDisposable
    {
        private readonly List<MokModule> _modules = new List<MokModule>();
        private readonly IServiceCollection _services;
        private readonly ILogger<ModuleLoader> _logger;
        private bool _isDisposed;

        // 使用线程安全的集合缓存模块类型信息，避免重复扫描
        private static readonly ConcurrentDictionary<Assembly, IReadOnlyList<Type>> _assemblyModuleTypesCache =
            new ConcurrentDictionary<Assembly, IReadOnlyList<Type>>();

        /// <summary>
        /// 应用程序配置
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="loggerFactory">日志工厂</param>
        public ModuleLoader(
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            // 注意这里的日志工厂可以为空，应该提供默认的日志记录机制
            if (loggerFactory == null)
            {
                // 使用NullLogger作为默认值，而不是抛出异常
                _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ModuleLoader>.Instance;
            }
            else
            {
                try
                {
                    _logger = loggerFactory.CreateLogger<ModuleLoader>();
                }
                catch (ObjectDisposedException)
                {
                    // LoggerFactory已被释放，使用NullLogger
                    _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ModuleLoader>.Instance;
                }
            }
        }

        /// <summary>
        /// 加载模块并配置服务
        /// </summary>
        /// <param name="assembliesToScan">要扫描的程序集</param>
        public async Task LoadModulesAsync(Assembly[] assembliesToScan)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                sw.Start();
                _logger.LogInformation("开始加载模块...");

                // 1. 发现模块类型
                var moduleTypes = DiscoverModuleTypes(assembliesToScan);
                _logger.LogInformation("发现 {Count} 个模块类型", moduleTypes.Count);
                 
                // 2. 根据依赖关系进行拓扑排序模块
                var sortedModuleTypes = SortModulesTopologically(moduleTypes);
                _logger.LogInformation("模块拓扑排序完成");

                // 3. 实例化模块
                // 提前预热模块工厂，编译表达式树   
                ModuleFactory.InstantiateModules(moduleTypes, _modules);

                // 4. 配置服务
                await ConfigureModuleServicesAsync();

                // 5. 注册模块到服务容器
                RegisterModulesToServices();

                _logger.LogInformation("模块加载和服务配置完成");
                sw.Stop();
                _logger.LogInformation($"模块加载时长： {sw.Elapsed}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载模块过程中发生错误");
                throw;
            }
        }

        /// <summary>
        /// 注册所有模块到服务容器
        /// </summary>
        private void RegisterModulesToServices()
        {
            // 注册模块加载器本身
            _services.AddSingleton<ModuleLoader>(this);

            // 注册所有模块实例
            foreach (var module in _modules)
            {
                var moduleType = module.GetType();

                // 注册具体模块类型
                _services.AddSingleton(moduleType, module);

                // 注册为MokModule基类
                _services.AddSingleton<MokModule>(module);

                // 预先获取所有接口，避免重复调用GetInterfaces()
                var interfaces = moduleType.GetInterfaces();

                // 使用Span进行高效遍历，减少内存分配
                foreach (var interfaceType in interfaces)
                {
                    // 过滤不需要注册的系统接口
                    if (interfaceType == typeof(IDisposable) ||
                        interfaceType == typeof(IAsyncDisposable) ||
                        interfaceType.Namespace?.StartsWith("System") == true)
                    {
                        continue;
                    }

                    _services.AddSingleton(interfaceType, module);
                    _logger.LogDebug("已注册接口 {Interface} 到模块 {Module}",
                        interfaceType.Name, moduleType.Name);
                }
            }
        }

        /// <summary>
        /// 实例化模块 - 高性能版本
        /// </summary>
        private void InstantiateModules(List<Type> moduleTypes)
        {
            // 预分配容量以避免动态扩容
            _modules.Capacity = Math.Max(_modules.Capacity, _modules.Count + moduleTypes.Count);

            foreach (var moduleType in moduleTypes)
            {
                try
                {
                    // 使用高性能工厂创建模块实例
                    var moduleInstance = ModuleFactory.CreateModule(moduleType);
                    _modules.Add(moduleInstance);
                    _logger.LogDebug("已实例化模块: {ModuleName}", moduleType.FullName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "实例化模块 {ModuleName} 失败", moduleType.FullName);
                    throw;
                }
            }
        }

        /// <summary>
        /// 配置模块服务
        /// </summary>
        private async Task ConfigureModuleServicesAsync()
        {
            var serviceContext = new ServiceConfigurationContext(_services);
            _logger.LogInformation("开始配置模块服务...");

            // 1. PreConfigureServices阶段
            foreach (var module in _modules)
            {
                _logger.LogDebug("执行 PreConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                module.ServiceConfigurationContext = serviceContext; // 设置上下文
                await module.PreConfigureServicesAsync(serviceContext);
            }

            // 2. ConfigureServices阶段
            foreach (var module in _modules)
            {
                _logger.LogDebug("执行 ConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                await module.ConfigureServicesAsync(serviceContext);
            }

            // 3. PostConfigureServices阶段
            foreach (var module in _modules)
            {
                _logger.LogDebug("执行 PostConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                await module.PostConfigureServicesAsync(serviceContext);
                module.ServiceConfigurationContext = null; // 清理上下文
            }

            _logger.LogInformation("模块服务配置完成");
        }

        /// <summary>
        /// 发现模块类型 - 使用缓存提高性能
        /// </summary>
        private List<Type> DiscoverModuleTypes(Assembly[] assembliesToScan)
        {
            var discoveredModuleTypes = new List<Type>();

            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                _logger.LogWarning("未提供用于扫描的程序集");
                return discoveredModuleTypes;
            }

            // 预分配足够大的容量以避免扩容
            discoveredModuleTypes.Capacity = assembliesToScan.Length * 5; // 假设每个程序集平均有5个模块

            foreach (var assembly in assembliesToScan)
            {
                try
                {
                    // 使用缓存避免重复扫描程序集
                    var moduleTypes = _assemblyModuleTypesCache.GetOrAdd(assembly, asm =>
                    {
                        try
                        {
                            return asm.GetTypes()
                                .Where(t => typeof(MokModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                .ToArray();
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            _logger.LogError(ex, "扫描程序集 {AssemblyName} 时发生类型加载错误", asm.FullName);
                            foreach (var loadException in ex.LoaderExceptions.Where(e => e != null))
                            {
                                _logger.LogError(loadException, "加载器异常");
                            }
                            return Array.Empty<Type>();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "扫描程序集 {AssemblyName} 时发生错误", asm.FullName);
                            return Array.Empty<Type>();
                        }
                    });

                    discoveredModuleTypes.AddRange(moduleTypes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "获取程序集 {AssemblyName} 的模块类型时发生错误", assembly.FullName);
                }
            }

            // 使用HashSet去重，然后转回List
            return discoveredModuleTypes.Distinct().ToList();
        }

        /// <summary>
        /// 拓扑排序模块 - 优化版本
        /// </summary>
        private List<Type> SortModulesTopologically(List<Type> moduleTypes)
        {
            //if (moduleTypes.Count <= 1)
            //{
            //    return moduleTypes;
            //}

            var sortedList = new List<Type>(moduleTypes.Count);
            var inDegree = new Dictionary<Type, int>(moduleTypes.Count);
            var adj = new Dictionary<Type, List<Type>>(moduleTypes.Count);
            var moduleTypeSet = new HashSet<Type>(moduleTypes);

            // 初始化数据结构
            foreach (var moduleType in moduleTypes)
            {
                inDegree[moduleType] = 0;
                adj[moduleType] = new List<Type>();
            }

            // 构建依赖图
            foreach (var moduleType in moduleTypes)
            {
                var dependsOnAttrs = moduleType.GetCustomAttributes<DependsOnAttribute>(true).ToArray();

                foreach (var attr in dependsOnAttrs)
                {
                    foreach (var depType in attr.DependedModuleTypes)
                    {
                        if (moduleTypeSet.Contains(depType))
                        {
                            adj[depType].Add(moduleType);
                            inDegree[moduleType]++;
                        }
                        else
                        {
                            _logger.LogWarning("模块 {Module} 依赖于未找到的模块 {Dependency}",
                                moduleType.FullName, depType.FullName);
                        }
                    }
                }
            }

            // Kahn算法执行拓扑排序 - 使用Queue<T>而不是LINQ以提高性能
            var queue = new Queue<Type>();

            // 初始化队列
            foreach (var moduleType in moduleTypes)
            {
                if (inDegree[moduleType] == 0)
                {
                    queue.Enqueue(moduleType);
                }
            }

            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                sortedList.Add(u);

                foreach (var v in adj[u])
                {
                    inDegree[v]--;
                    if (inDegree[v] == 0)
                    {
                        queue.Enqueue(v);
                    }
                }
            }

            // 检测循环依赖
            if (sortedList.Count != moduleTypes.Count)
            {
                var missingModules = new List<string>();
                foreach (var moduleType in moduleTypes)
                {
                    if (!sortedList.Contains(moduleType))
                    {
                        missingModules.Add(moduleType.FullName);
                    }
                }

                var errorMessage = $"模块依赖中存在循环依赖。未能排序的模块: {string.Join(", ", missingModules)}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return sortedList;
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public async Task InitializeModulesAsync(
            IServiceProvider serviceProvider,
            IApplicationBuilder appBuilder,
            IHostingEnvironment env)
        {
            ThrowIfDisposed();

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            try
            {
                _logger.LogInformation("开始初始化模块...");
                var appContext = new ApplicationInitializationContext(serviceProvider, appBuilder, env);

                // 1. 预初始化阶段
                foreach (var module in _modules)
                {
                    _logger.LogDebug("执行 OnPreApplicationInitializationAsync: {ModuleName}",
                        module.GetType().FullName);
                    await module.OnPreApplicationInitializationAsync(appContext);
                }

                // 2. 主初始化阶段
                foreach (var module in _modules)
                {
                    _logger.LogDebug("执行 OnApplicationInitializationAsync: {ModuleName}",
                        module.GetType().FullName);
                    await module.OnApplicationInitializationAsync(appContext);
                }

                // 3. 后初始化阶段
                foreach (var module in _modules)
                {
                    _logger.LogDebug("执行 OnPostApplicationInitializationAsync: {ModuleName}",
                        module.GetType().FullName);
                    await module.OnPostApplicationInitializationAsync(appContext);
                }

                _logger.LogInformation("模块初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块初始化过程中发生错误");
                throw;
            }
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        public async Task ShutdownModulesAsync(IServiceProvider serviceProvider)
        {
            if (_isDisposed)
                return;

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            try
            {
                _logger.LogInformation("开始关闭模块...");
                var shutdownContext = new ApplicationShutdownContext(serviceProvider);

                // 按初始化顺序的逆序关闭
                for (int i = _modules.Count - 1; i >= 0; i--)
                {
                    var module = _modules[i];
                    _logger.LogDebug("执行 OnApplicationShutdownAsync: {ModuleName}",
                        module.GetType().FullName);

                    try
                    {
                        await module.OnApplicationShutdownAsync(shutdownContext);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "关闭模块 {ModuleName} 时发生错误",
                            module.GetType().FullName);
                        // 继续关闭其他模块
                    }
                }

                _logger.LogInformation("模块关闭完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "模块关闭过程中发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取模块实例
        /// </summary>
        public IReadOnlyList<MokModule> GetModules() => _modules.AsReadOnly();

        /// <summary>
        /// 检查是否已释放
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ModuleLoader));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // 释放模块资源
                foreach (var module in _modules.OfType<IDisposable>())
                {
                    try
                    {
                        module.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "释放模块 {ModuleName} 资源时发生错误",
                            module.GetType().FullName);
                    }
                }

                _modules.Clear();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 异步释放资源
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // 分离异步和同步可释放模块，避免重复处理
                var asyncDisposables = _modules.OfType<IAsyncDisposable>().ToArray();
                var disposables = _modules.OfType<IDisposable>()
                    .Where(m => !(m is IAsyncDisposable))
                    .ToArray();

                // 异步释放模块资源
                foreach (var module in asyncDisposables)
                {
                    try
                    {
                        await module.DisposeAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "异步释放模块 {ModuleName} 资源时发生错误",
                            module.GetType().FullName);
                    }
                }

                // 同步释放其他模块
                foreach (var module in disposables)
                {
                    try
                    {
                        module.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "释放模块 {ModuleName} 资源时发生错误",
                            module.GetType().FullName);
                    }
                }

                _modules.Clear();
            }
        }
    }
}