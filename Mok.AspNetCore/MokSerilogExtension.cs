using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;

namespace Mok.AspNetCore
{
    public static class MokSerilogExtension
    {

        /// <summary>
        /// 配置Serilog作为可选的日志提供程序
        /// </summary>
        public static ILoggerFactory CreateLoggerFactory(IServiceProvider serviceProvider, Action<LoggerConfiguration> configureLogger = null)
        {
            // 创建一个新的LoggerFactory
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                // 如果已经配置了Serilog，添加Serilog提供程序
                if (Log.Logger != null && Log.Logger.GetType().Name != "SilentLogger")
                {
                    // 使用现有的Serilog配置
                    builder.AddSerilog(dispose: false);
                }
                else if (configureLogger != null)
                {
                    // 使用提供的配置创建新的Serilog记录器
                    var loggerConfiguration = new LoggerConfiguration();
                    configureLogger(loggerConfiguration);
                    var logger = loggerConfiguration.CreateLogger();

                    // 添加新创建的Serilog记录器
                    builder.AddSerilog(logger, dispose: true);
                }

                // 确保使用默认的日志提供程序作为后备
                builder.AddConsole();
            });

            return loggerFactory;
        }

        /// <summary>
        /// 在IServiceCollection中注册Serilog服务
        /// </summary>
        public static IServiceCollection AddMokModuleSerilog(this IServiceCollection services, Action<LoggerConfiguration> configureLogger = null)
        {
            // 确保只有在提供了配置时才添加Serilog
            if (configureLogger != null)
            {
                // 创建Serilog配置
                var loggerConfiguration = new LoggerConfiguration();
                configureLogger(loggerConfiguration);

                // 配置全局Logger
                Log.Logger = loggerConfiguration.CreateLogger();

                // 将Serilog添加到.NET Core日志系统
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(dispose: false);
                });

                // 确保应用程序终止时清理日志资源
                AppDomain.CurrentDomain.ProcessExit += (sender, e) => Log.CloseAndFlush();
            }

            return services;
        }
    }
}
