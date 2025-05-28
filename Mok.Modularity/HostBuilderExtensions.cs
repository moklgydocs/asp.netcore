using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Mok.Modularity
{
    public static class HostBuilderExtensions
    {
        public static async Task<IHostBuilder> AddApplicationAsync<TModule>(this IHostBuilder builder) where TModule : MokModule
        {
            // 获取当前模块的类型
            var moduleType = typeof(TModule);

            // 获取模块所在程序集
            var assembly = moduleType.Assembly;

            // 加载模块并注册到服务容器中
            builder.ConfigureServices((hostContext, services) =>
            {
                // 加载模块
                var moduleLoader = new ModuleLoader(services);
                moduleLoader.LoadModulesAsync(new[] { assembly }).Wait(); // 同步加载模块
            });

            // 返回 builder 以便链式调用
            return await Task.FromResult(builder);
        }

        public static async Task<IWebHostBuilder> AddApplicationAsync<TModule>(this IWebHostBuilder builder) where TModule : MokModule
        {
            // 获取当前模块的类型
            var moduleType = typeof(TModule);

            // 获取模块所在程序集
            var assembly = moduleType.Assembly;

            // 加载模块并注册到服务容器中
            builder.ConfigureServices((hostContext, services) =>
            {
                // 加载模块
                var moduleLoader = new ModuleLoader(services);
                moduleLoader.LoadModulesAsync(new[] { assembly }).Wait(); // 同步加载模块
            });

            // 返回 builder 以便链式调用
            return await Task.FromResult(builder);
        } 

    }
}
