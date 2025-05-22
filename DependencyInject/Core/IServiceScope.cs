using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    // 服务作用域：管理作用域内服务的生命周期
    public interface IServiceScope:IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
    // 服务作用域工厂：创建新的服务作用域
    public interface IServiceScopeFactory
    {
        IServiceScope CreateScope();
    }
}
