using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mok.Modularity
{
    public interface IMokModule 
    {  
        // 配置此模块的服务
        void ConfigureServices([NotNull]ServiceConfigurationContext context);
        Task ConfigureServicesAsync([NotNull] ServiceConfigurationContext context); 
    }
}
