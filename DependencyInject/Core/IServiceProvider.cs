using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务解析的入口，用于运行时获取服务实例
    /// </summary>
    public interface IServiceProvider : IServiceScopeFactory
    {
        object GetService(Type serviceType);
    }
}
