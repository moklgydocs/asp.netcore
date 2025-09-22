using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.Kestrel
{
    /// <summary>
    /// 服务器抽象，对应Asp.net core 的IServer
    /// </summary>
    public interface IServer : IDisposable
    {
        /// <summary>
        /// 启动服务器 并且传入应用程序处理器
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        Task StartAsync<TContext>(IHttpApplication<TContext> application,
            CancellationToken cancellationToken);

        /// <summary>
        /// 停止服务器
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 服务器特性集合
        /// </summary>
        IFeatureCollection Features { get; }
    }


}
