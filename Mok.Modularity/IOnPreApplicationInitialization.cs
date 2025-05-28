using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IOnPreApplicationInitialization
    {
        Task OnPreApplicationInitializationAsync([NotNull] ApplicationInitializationContext context);

        // (可选) 应用程序初始化前
        void OnPreApplicationInitialization([NotNull] ApplicationInitializationContext context);
    
    }
}
