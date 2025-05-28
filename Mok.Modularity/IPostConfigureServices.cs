using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IPostConfigureServices
    { 
        // (可选) 在主要 ConfigureServices 执行后配置服务
        void PostConfigureServices([NotNull] ServiceConfigurationContext context);
        Task PostConfigureServicesAsync([NotNull] ServiceConfigurationContext context);
    }
}
