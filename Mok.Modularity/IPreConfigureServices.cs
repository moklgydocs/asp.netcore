using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IPreConfigureServices
    {
        // (可选) 在主要 ConfigureServices 执行前配置服务
        void PreConfigureServices([NotNull] ServiceConfigurationContext context);
        Task PreConfigureServicesAsync([NotNull] ServiceConfigurationContext context);
    }
}
