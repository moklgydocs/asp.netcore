using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IOnApplicationShutdown
    {
        // 应用程序关闭
        void OnApplicationShutdown([NotNull] ApplicationShutdownContext context); 

        Task OnApplicationShutdownAsync([NotNull] ApplicationShutdownContext context);
    }
}
