using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore
{
    // 简化版的HostingApplication实现
    // 对应ASP.NET Core中的HostingApplication
    // 在实际源码中位于Microsoft.AspNetCore.Hosting.HostingApplication
    internal class SimpleHostingApplication : IHttpApplication<SimpleHttpContext>
    {
        private readonly SimpleWebApplication _application;

        public SimpleHostingApplication(SimpleWebApplication application)
        {
            _application = application;
        }

        // 创建HTTP上下文
        // 对应HostingApplication.CreateContext
        public SimpleHttpContext CreateContext(IFeatureCollection contextFeatures)
        {
            // 创建HTTP上下文实例
            return new SimpleHttpContext(contextFeatures);
        }

        // 处理请求
        // 对应HostingApplication.ProcessRequestAsync
        public Task ProcessRequestAsync(SimpleHttpContext context)
        {
            // 执行应用程序的中间件管道
            return _application.ExecuteAsync(context);
        }

        // 释放上下文资源
        // 对应HostingApplication.DisposeContext
        public void DisposeContext(SimpleHttpContext context, Exception exception)
        {
            // 在简化版实现中，我们不需要特殊的资源释放逻辑
        }
    }
}
