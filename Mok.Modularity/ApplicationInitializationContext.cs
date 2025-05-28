using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting; // 添加此行以解决 IWebHostEnvironment 的引用问题  
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Mok.Modularity
{
    public class ApplicationInitializationContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public IApplicationBuilder ApplicationBuilder { get; set; }  

        public IHostingEnvironment Enviroment { get; set; }

        public ApplicationInitializationContext(IServiceProvider serviceProvider
            , IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment)
        {
            ServiceProvider = serviceProvider;
            ApplicationBuilder = applicationBuilder;
            Enviroment = hostingEnvironment;
        }


    }
}
