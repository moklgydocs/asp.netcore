using SimpleAspNetCore.Kestrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore
{
    /// <summary>
    ///  应用构建器，对应ASP.NET Core中的WebApplicationBuilder
    /// 在实际源码中位于Microsoft.AspNetCore.Builder.WebApplicationBuilder
    /// </summary>
    public class SimpleWebApplicationBuilder
    {
        // 服务集合，简化版的DI容器
        public Dictionary<Type, Func<object>> Services { get; } = new Dictionary<Type, Func<object>>();

        public SimpleWebApplicationOptions Options { get; } = new SimpleWebApplicationOptions();

        // 创建默认构建器
        public static SimpleWebApplicationBuilder CreateBuilder()
        {
            var builder = new SimpleWebApplicationBuilder();

            // 注册默认服务

            builder.Services[typeof(IServer)] = () => new SimpleKestrelServer(
                    new System.Net.IPEndPoint(IPAddress.Loopback, 5000)
                    );
            Console.WriteLine("[Builder] 创建应用构建器并注册默认服务");

            return builder;
        }

        public SimpleWebApplication Build()
        {
            Console.WriteLine("[Builder] 构建应用程序");

            return new SimpleWebApplication(this);
        }

        public SimpleWebApplicationBuilder ConfigureKestrel(Action<SimpleWebApplicationOptions> configure)
        {
            configure(Options);
            return this;
        }
    }
}
