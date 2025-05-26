using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Mok.Modularity
{
    public class MokModule:IMokModule
    {
        protected internal bool SkipAutoServiceRegistration { get; protected set; }

        protected internal ServiceConfigurationContext ServiceConfigurationContext
        {
            get
            {
                if (_serviceConfigurationContext == null)
                {
                    throw new Exception($"{nameof(ServiceConfigurationContext)} is only available in the {nameof(ConfigureServices)}, {nameof(PreConfigureServices)} and {nameof(PostConfigureServices)} methods.");
                }

                return _serviceConfigurationContext;
            }
            internal set => _serviceConfigurationContext = value;
        }

        private ServiceConfigurationContext _serviceConfigurationContext;
        // 同步方法的默认空实现
        public virtual void PreConfigureServices(ServiceConfigurationContext context) { }

        public virtual void ConfigureServices(ServiceConfigurationContext context) { }
        public virtual void PostConfigureServices(ServiceConfigurationContext context) { }
        public virtual void OnPreApplicationInitialization(ApplicationInitializationContext context) { }
        public virtual void OnApplicationInitialization(ApplicationInitializationContext context) { }
        public virtual void OnPostApplicationInitialization(ApplicationInitializationContext context) { }
        public virtual void OnApplicationShutdown(ApplicationShutdownContext context) { }


        // 异步方法的默认 Task.CompletedTask 实现
        public virtual async Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigureServices(context);
            await Task.CompletedTask;
        }
        public virtual async Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            ConfigureServices(context);
            await Task.CompletedTask;
        } // 实际模块通常会重写此方法
        public virtual async Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PostConfigureServices(context);
            await Task.CompletedTask;
        }
        public virtual async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnPreApplicationInitialization(context);
            await Task.CompletedTask;
        }
        public virtual async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnApplicationInitialization(context);
            await Task.CompletedTask;
        }
        public virtual async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnPostApplicationInitialization(context);
            await Task.CompletedTask;
        }
        public virtual Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
        {
            OnApplicationShutdown(context);
            return Task.CompletedTask;
        }


        protected void Configure<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(configureOptions);
        }

        protected void Configure<TOptions>(string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(name, configureOptions);
        }

        //protected void Configure<TOptions>(IConfiguration configuration)
        //    where TOptions : class
        //{
        //    ServiceConfigurationContext.Services.Configure<TOptions>(configuration);
        //}

        //protected void Configure<TOptions>(IConfiguration configuration, Action<BinderOptions> configureBinder)
        //    where TOptions : class
        //{
        //    ServiceConfigurationContext.Services.Configure<TOptions>(configuration, configureBinder);
        //}

        //protected void Configure<TOptions>(string name, IConfiguration configuration)
        //    where TOptions : class
        //{
        //    ServiceConfigurationContext.Services.Configure<TOptions>(name, configuration);
        //}

        //protected void PreConfigure<TOptions>(Action<TOptions> configureOptions)
        //    where TOptions : class
        //{
        //    ServiceConfigurationContext.Services.PreConfigure(configureOptions);
        //}

        protected void PostConfigure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.PostConfigure(configureOptions);
        }

        protected void PostConfigureAll<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.PostConfigureAll(configureOptions);
        }
    }
}
