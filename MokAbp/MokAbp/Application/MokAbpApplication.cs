using Microsoft.Extensions.DependencyInjection;
using MokAbp.DependencyInjection;
using MokAbp.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MokAbp.Application
{
    /// <summary>
    /// MokAbp应用程序
    /// </summary>
    public class MokAbpApplication : IDisposable
    {
        public IServiceProvider? ServiceProvider { get; private set; }
        public IServiceCollection Services { get; }
        public List<ModuleDescriptor> Modules { get; }

        private bool _isInitialized;
        private bool _disposed;

        public MokAbpApplication(Type startupModuleType, IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            
            // 加载所有模块
            var moduleLoader = new ModuleLoader();
            Modules = moduleLoader.LoadModules(startupModuleType);

            // 配置服务
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var context = new ModuleContext(Services);

            // PreConfigureServices
            foreach (var module in Modules)
            {
                module.Instance.PreConfigureServices(context);
            }

            // ConfigureServices
            foreach (var module in Modules)
            {
                module.Instance.ConfigureServices(context);
            }

            // 注册约定服务
            RegisterConventionalServices();

            // PostConfigureServices
            foreach (var module in Modules)
            {
                module.Instance.PostConfigureServices(context);
            }
        }

        private void RegisterConventionalServices()
        {
            var assemblies = Modules.Select(m => m.Assembly).Distinct().ToArray();
            var registrationContext = new ServiceRegistrationContext(Services, assemblies);
            
            var conventionalRegistrar = new ConventionalRegistrar();
            conventionalRegistrar.RegisterServices(registrationContext);
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Application is already initialized");
            }

            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            var context = new ApplicationInitializationContext(ServiceProvider);
            
            // 调用实现了IApplicationInitialization的模块
            foreach (var module in Modules)
            {
                if (module.Instance is IApplicationInitialization initModule)
                {
                    initModule.OnApplicationInitialization(context);
                }
            }

            _isInitialized = true;
        }

        public void Shutdown()
        {
            if (!_isInitialized || ServiceProvider == null)
            {
                return;
            }

            var context = new ApplicationShutdownContext(ServiceProvider);
            
            // 反向调用关闭方法
            foreach (var module in Modules.AsEnumerable().Reverse())
            {
                if (module.Instance is IApplicationShutdown shutdownModule)
                {
                    shutdownModule.OnApplicationShutdown(context);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Shutdown();
            _disposed = true;
        }
    }
}
