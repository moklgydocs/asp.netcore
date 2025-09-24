using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore
{
    /// <summary>
    ///  HTTP应用程序抽象，对应ASP.NET Core中的IHttpApplication<TContext>
    ///  在实际源码中位于Microsoft.AspNetCore.Hosting.Server.IHttpApplication
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IHttpApplication<TContext>
    {
        /// <summary>
        /// 创建上下文请求
        /// </summary>
        /// <param name="collectionFeatures"></param>
        /// <returns></returns>
        TContext CreateContext(IFeatureCollection collectionFeatures);

        /// <summary>
        ///  处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ProcessRequestAsync(TContext context);

        /// <summary>
        /// 释放上下文资源
        /// </summary>
        void DisposeContext(TContext context, Exception exception);

    }
    // 请求委托，对应ASP.NET Core中的RequestDelegate
    // 在实际源码中位于Microsoft.AspNetCore.Http.RequestDelegate
    public delegate Task RequestDelegate(SimpleHttpContext context);

}
