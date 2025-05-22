using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务注册的容器，用于配置阶段
    /// </summary>
    public interface IServiceCollection : IList<ServiceDescriptor>
    {
    }
}
