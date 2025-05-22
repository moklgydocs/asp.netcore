using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInject.Enums;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务描述 
    /// <para>描述一个服务注册，包含服务类型、如何创建服务及其生命周期</para>
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public object Instance { get; internal set; }

        public Func<IServiceProvider, object> Factory { get;  }

        /// <summary>
        /// 定义服务的生命周期，影响实例的创建和缓存策略
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }
    }
}
