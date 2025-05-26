using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IApplication:IDisposable
    {
        Type RootModuleType { get; }
        IServiceProvider ServiceProvider { get; }

        Task InitializeApplicationAsync();
        Task ShutdownAsync();
    }
}
