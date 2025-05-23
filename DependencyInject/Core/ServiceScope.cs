using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务作用域实现类（ServiceScope）
    /// <para>用于管理依赖注入容器的作用域生命周期。</para>
    /// <para>每个ServiceScope都持有一个独立的DIContainer实例，负责管理当前作用域内的服务实例。</para>
    /// <para>实现IServiceScope接口，支持作用域服务的释放。</para>
    /// </summary>
    public class ServiceScope : IServiceScope
    {
        // 当前作用域对应的DIContainer实例（每个作用域有独立的作用域服务缓存）
        private readonly DIContainer _container;

        /// <summary>
        /// 构造函数，创建新的服务作用域
        /// </summary>
        /// <param name="container">当前作用域的DIContainer实例</param>
        public ServiceScope(DIContainer container)
        {
            // 保存容器引用
            _container = container;
            // ServiceProvider属性指向当前作用域的容器
            ServiceProvider = container;
        }

        /// <summary>
        /// 获取当前作用域的服务提供者（IServiceProvider）
        /// <para>用于解析当前作用域内的服务实例。</para>
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 释放当前作用域及其管理的服务实例
        /// <para>调用DIContainer.Dispose，释放所有作用域服务。</para>
        /// </summary>
        public void Dispose()
        {
            // 释放作用域容器及其服务
            _container.Dispose();
        }
    }
}
