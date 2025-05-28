using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IOnApplicationInitialization
    {

        // 应用程序初始化
        void OnApplicationInitialization([NotNull] ApplicationInitializationContext context);
        Task OnApplicationInitializationAsync([NotNull] ApplicationInitializationContext context);

    }
}
