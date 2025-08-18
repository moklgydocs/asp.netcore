using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Internal;
using Mok.Modularity;

namespace Mok.AspNetCore
{
    public static class MokApplicationBuilderExtensions
    {
        // 针对 .NET Standard 2.0 和 2.1  
        public static async Task<WebApplicationBuilder> AddApplicationAsync<TRootModule>(
            this WebApplicationBuilder webBuilder,
            ILoggerFactory loggerFactory = null)
            where TRootModule : MokModule // 约束根模块类型  
        {
            if (webBuilder == null)
            {
                throw new ArgumentNullException(nameof(webBuilder));
            }
            var moduleType = typeof(TRootModule);
            if (moduleType is null)
            {
                throw new ArgumentNullException(nameof(moduleType));
            }
            var assembliesSet = new HashSet<Assembly>(new AssemblyComparer());
            // 获取MokModule模块下所有程序集
            CollectAssembliesRecursively(moduleType.Assembly, assembliesSet);
            var assemblies = assembliesSet.ToArray();

            //var assemblies = new[] { typeof(TRootModule).Assembly };
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(x => !x.IsDynamic && !IsSystemAssembly(x)).ToArray();
            // 还可以添加加载尚未加载的程序集，根据命名约定查找
            //var loadedAssemblies = assemblies.Select(a => a.FullName).ToList();
            //var rootModuleAssemblyName = typeof(TRootModule).Assembly.GetName().Name; 

            // 获取或创建日志工厂
            if (loggerFactory == null)
            {
                loggerFactory = webBuilder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            }

            var application = await Application.CreateAsync(typeof(TRootModule), webBuilder.Services, loggerFactory, assemblies);
            // 返回 IServiceCollection，以便继续链式调用  
            return webBuilder;
        }

        private static void CollectAssembliesRecursively(Assembly assembly, HashSet<Assembly> assemblies)
        { 
            // 获取应用程序目录
            var binDirectory = Path.GetDirectoryName(assembly.Location);

            // 扫描目录中所有DLL
            foreach (var dllPath in Directory.GetFiles(binDirectory, "*.dll"))
            {
                try
                {
                    // 避免加载已经处理过的程序集
                    var assemblyName = AssemblyName.GetAssemblyName(dllPath);
                    if (assemblies.Any(a => a.GetName().Name == assemblyName.Name))
                    {
                        continue;
                    }

                    // 尝试加载程序集
                    var moduleAssembly = Assembly.Load(assemblyName);

                    // 检查是否包含MokModule子类
                    if (ContainsMokModuleTypes(moduleAssembly))
                    {
                        assemblies.Add(moduleAssembly);
                    }
                }
                catch (Exception ex)
                {
                    // 记录无法加载的程序集，但继续处理其他程序集
                    Console.WriteLine($"无法加载程序集 {dllPath}: {ex.Message}");
                }
            }
        }

        public static async Task InitializeApplicationAsync([NotNull] this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var application = app.ApplicationServices.GetRequiredService<IApplication>();

            // 设置应用程序停止时的回调
            applicationLifetime.ApplicationStopping.Register(async () =>
            {
                // 使用无等待模式避免可能的死锁
                try
                {
                    await application.ShutdownAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // 记录关闭时的异常，但不阻止进程退出
                    var logger = app.ApplicationServices.GetService<ILogger<Application>>();
                    logger?.LogError(ex, "Application shutdown error");
                }
            });

            // 应用程序完全停止后的资源释放
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                try
                {
                    (application as IDisposable)?.Dispose();
                }
                catch (Exception ex)
                {
                    var logger = app.ApplicationServices.GetService<ILogger<Application>>();
                    logger?.LogError(ex, "Application dispose error");
                }
            });
            // 初始化应用程序
            await application.InitializeApplicationAsync(app);
        }

        // 判断是否为系统程序集（避免扫描不必要的程序集）
        private static bool IsSystemAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            return name.StartsWith("System.") ||
                   name.StartsWith("Microsoft.") ||
                   name == "netstandard" ||
                   name == "mscorlib";
        }

        private static bool ContainsMokModule(Assembly assembly)
        {
            try
            {
                // 检查程序集中是否包含继承自MokModule的类型
                return assembly.GetTypes()
                    .Any(t => t.IsClass && !t.IsAbstract && typeof(MokModule).IsAssignableFrom(t));
            }
            catch
            {
                // 处理异常（例如，如果程序集无法被解析）
                return false;
            }
        }
        // 检查程序集是否包含MokModule子类
        private static bool ContainsMokModuleTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Any(t => t.IsClass && !t.IsAbstract && typeof(MokModule).IsAssignableFrom(t));
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 处理类型加载异常，检查成功加载的类型
                return ex.Types
                    .Where(t => t != null)
                    .Any(t => t.IsClass && !t.IsAbstract && typeof(MokModule).IsAssignableFrom(t));
            }
            catch
            {
                // 其他异常则认为此程序集没有相关类型
                return false;
            }
        }
        // 用于在HashSet中比较Assembly对象
        private class AssemblyComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                return x?.GetName().FullName == y?.GetName().FullName;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj?.GetName().FullName.GetHashCode() ?? 0;
            }
        }
    }
}
