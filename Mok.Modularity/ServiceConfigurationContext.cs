using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Modularity
{
    /// <summary>
    /// 模块配置上下文
    /// </summary>
    public class ServiceConfigurationContext
    {
        public IServiceCollection Services { get; set; }

        //public IConfiguration Configuration { get; set; }

        public ServiceConfigurationContext(IServiceCollection services/*,IConfiguration configuration*/)
        {
            Services = services;
            //Configuration = configuration;
        }   
    }
}
