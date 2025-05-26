using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Mok.Modularity
{
    public class ModuleLoader : IDisposable
    {

        private readonly List<MokModule> Modules = new List<MokModule>();

        private readonly IServiceCollection Services;

        private readonly ILogger<ModuleLoader> Logger; 

        public IConfiguration Configuration { get; set; }
        public ModuleLoader(
            IServiceCollection services, 
            ILoggerFactory loggerFactory = null)
        { 
            Services = services ?? throw new ArgumentNullException(nameof(services));// 服务集合不能为空
            Logger = loggerFactory?.CreateLogger<ModuleLoader>() ?? throw new ArgumentNullException(nameof(loggerFactory));// 日志工厂不能为空
        }

        public async Task LoadModulesAsync(Assembly[] assembliesToScan)
        {
            // 1. 开始加载模块
            Logger.LogInformation("starting load modules...");

            // 1. 发现模块类型
            var moduleTypes = DiscoverModuleTypes(assembliesToScan);
            Logger?.LogInformation("发现 {Count} 个模块类型。", moduleTypes.Count);
            // 2. 根据依赖关系进行拓扑排序模块
            var sortedModuleTypes = SortModulesTopologically(moduleTypes);
            Logger.LogInformation("拓扑排序完成。");

            // 3. 实例化模块
            foreach (var moduleType in sortedModuleTypes)
            {
                try
                {
                    // 假设模块有无参构造函数，或者可以通过某种方式从一个临时的、非常基础的 DI 容器解析（如果模块构造自身需要依赖）
                    // 为简单起见，这里使用 Activator.CreateInstance
                    var moduleInstance = (MokModule)Activator.CreateInstance(moduleType);
                    Modules.Add(moduleInstance);
                    Logger?.LogDebug("已实例化模块: {ModuleName}", moduleType.FullName);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "实例化模块 {ModuleName} 失败。", moduleType.FullName);
                    throw; // 或者进行更优雅的错误处理
                }
            }

            // 4. 配置服务
            var serviceContext = new ServiceConfigurationContext(Services /*,Configuration*/);
            Logger?.LogInformation("开始配置模块服务...");

            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 PreConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                await module.PreConfigureServicesAsync(serviceContext);
            }
            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 ConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                await module.ConfigureServicesAsync(serviceContext); // 模块在此注册其服务
            }
            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 PostConfigureServicesAsync: {ModuleName}", module.GetType().FullName);
                await module.PostConfigureServicesAsync(serviceContext);
            }
            Logger?.LogInformation("模块服务配置完成。");
        }
        private List<Type> DiscoverModuleTypes(Assembly[] assembliesToScan)
        {
            var discoveredModuleTypes = new List<Type>();
            if (assembliesToScan == null || assembliesToScan.Length == 0)
            {
                Logger?.LogWarning("未提供用于扫描的程序集");
                return discoveredModuleTypes;
            }

            foreach (var assembly in assembliesToScan)
            {
                try
                {
                    discoveredModuleTypes.AddRange(
                        assembly.GetTypes().Where(t =>
                        typeof(MokModule).IsAssignableFrom(t)
                        && !t.IsInterface && !t.IsAbstract));
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Logger.LogError(ex, "扫描程序集{AssemblyName} 时发生类型加载错误", assembly.FullName);
                    foreach (var loadException in ex.LoaderExceptions)
                    {
                        Logger?.LogError(loadException, "具体的加载器异常");
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "扫描程序集 {AssemblyName} 时发生未知错误。", assembly.FullName);
                }
            }
            return discoveredModuleTypes.Distinct().ToList();
        }

        private List<Type> SortModulesTopologically(List<Type> moduleTypes)
        {
            // 这是一个拓扑排序的简化实现 (Kahn's algorithm 概念)
            // 生产环境中建议使用更健壮的实现或库
            var sortedList = new List<Type>();
            var inDegree = moduleTypes.ToDictionary(m => m, m => 0);
            var adj = moduleTypes.ToDictionary(m => m, m => new List<Type>()); // 邻接表，adj[U] 存储所有 U -> V 中的 V

            var moduleTypeSet = new HashSet<Type>(moduleTypes); // 用于快速查找

            foreach (var moduleType in moduleTypes)
            {
                var dependsOnAttrs = moduleType.GetCustomAttributes<DependsOnAttribute>(true);
                foreach (var attr in dependsOnAttrs)
                {
                    foreach (var depType in attr.DependedModuleTypes)
                    {
                        if (moduleTypeSet.Contains(depType)) // 确保依赖项是已发现的模块之一
                        {
                            // 如果 moduleType 依赖于 depType，则存在一条从 depType 到 moduleType 的边
                            // 即 depType 必须在 moduleType 之前处理
                            adj[depType].Add(moduleType);
                            inDegree[moduleType]++;
                        }
                        else
                        {
                            Logger?.LogWarning("模块 {Module} 依赖于未找到的模块 {Dependency}。此依赖将被忽略。", moduleType.FullName, depType.FullName);
                        }
                    }
                }
            }

            var queue = new Queue<Type>(moduleTypes.Where(m => inDegree[m] == 0));

            while (queue.Any())
            {
                var u = queue.Dequeue();
                sortedList.Add(u);

                foreach (var v in adj[u]) // 对于 u 的所有后续节点 v
                {
                    inDegree[v]--;
                    if (inDegree[v] == 0)
                    {
                        queue.Enqueue(v);
                    }
                }
            }

            if (sortedList.Count != moduleTypes.Count)
            {
                // 找出图中剩余的节点（即入度不为0的节点），它们构成了循环
                var missingModules = moduleTypes.Except(sortedList).Select(t => t.FullName);
                var errorMessage = $"模块依赖中存在循环或缺失依赖。未能排序的模块: {string.Join(", ", missingModules)}";
                Logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return sortedList;
        }

        public async Task InitializeModulesAsync(IServiceProvider serviceProvider, IApplicationBuilder appBuilder, IHostingEnvironment env)
        {
            Logger?.LogInformation("开始初始化模块...");
            var appContext = new ApplicationInitializationContext(serviceProvider, appBuilder, env);

            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 OnPreApplicationInitializationAsync: {ModuleName}", module.GetType().FullName);
                await module.OnPreApplicationInitializationAsync(appContext);
            }
            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 OnApplicationInitializationAsync: {ModuleName}", module.GetType().FullName);
                await module.OnApplicationInitializationAsync(appContext);
            }
            foreach (var module in Modules)
            {
                Logger?.LogDebug("执行 OnPostApplicationInitializationAsync: {ModuleName}", module.GetType().FullName);
                await module.OnPostApplicationInitializationAsync(appContext);
            }
            Logger?.LogInformation("模块初始化完成。");
        }

        public async Task ShutdownModulesAsync(IServiceProvider serviceProvider)
        {
            Logger?.LogInformation("开始关闭模块...");
            var shutdownContext = new ApplicationShutdownContext(serviceProvider);

            // 按初始化顺序的逆序关闭
            for (int i = Modules.Count - 1; i >= 0; i--)
            {
                var module = Modules[i];
                Logger?.LogDebug("执行 OnApplicationShutdownAsync: {ModuleName}", module.GetType().FullName);
                try
                {
                    await module.OnApplicationShutdownAsync(shutdownContext);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "关闭模块 {ModuleName} 时发生错误。", module.GetType().FullName);
                    // 根据需要决定是否继续关闭其他模块
                }
            }
            Logger?.LogInformation("模块关闭完成。");
        }
        public void Dispose()
        {
            // 如果模块实现了 IDisposable，可以在这里处理它们的 Dispose
            foreach (var module in Modules.OfType<IDisposable>())
            {
                module.Dispose();
            }
            Modules.Clear();
            GC.SuppressFinalize(this);
        }
    }


}
