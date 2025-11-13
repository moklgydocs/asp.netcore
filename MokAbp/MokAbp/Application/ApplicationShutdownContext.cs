using System;

namespace MokAbp.Application
{
    /// <summary>
    /// 应用程序关闭上下文
    /// </summary>
    public class ApplicationShutdownContext
    {
        public IServiceProvider ServiceProvider { get; }

        public ApplicationShutdownContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
