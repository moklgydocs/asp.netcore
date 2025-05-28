using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IOnPostApplicationInitialization
    {

        // (可选) 应用程序初始化后
        void OnPostApplicationInitialization([NotNull] ApplicationInitializationContext context);
        Task OnPostApplicationInitializationAsync([NotNull] ApplicationInitializationContext context);
    }
}
