using System;

namespace MokAbp.Application
{
    /// <summary>
    /// 应用程序初始化上下文
    /// </summary>
    public class ApplicationInitializationContext
    {
        public IServiceProvider ServiceProvider { get; }

        public ApplicationInitializationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
