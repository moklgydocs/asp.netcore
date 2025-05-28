using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Mok.Modularity
{
    public class MokModule : IMokModule,
        IOnPreApplicationInitialization,
        IOnApplicationInitialization,
        IOnPostApplicationInitialization,
        IOnApplicationShutdown,
        IPreConfigureServices,
        IPostConfigureServices
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
        public virtual Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigureServices(context);
            return Task.CompletedTask;
        }
        public virtual Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            ConfigureServices(context);
            return Task.CompletedTask;
        } // 实际模块通常会重写此方法
        public virtual Task PostConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PostConfigureServices(context);
            return Task.CompletedTask;
        }
        public virtual Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnPreApplicationInitialization(context);
            return Task.CompletedTask;
        }
        public virtual Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnApplicationInitialization(context);
            return Task.CompletedTask;
        }
        public virtual Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            OnPostApplicationInitialization(context);
            return Task.CompletedTask;
        }
        public virtual Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
        {
            OnApplicationShutdown(context);
            return Task.CompletedTask;
        }

        public static void IsMokModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsClass && !typeInfo.IsAbstract && !typeInfo.IsGenericType && typeof(MokModule).GetTypeInfo().IsAssignableFrom(type))
            {
                return;
            }
            else
            {
                throw new ArgumentException($"Type '{type.FullName}' is not a valid MokModule. It must be a non-abstract class that inherits from MokModule.");
            }
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
