using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Mok.Modularity
{
    public class ApplicationInitializationContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public IApplicationBuilder ApplicationBuilder { get; set; }

        //public Microsoft.AspNetCore.Hosting.IHostingEnvironment Enviroment { get; set; }// 专为 Web 应用程序设计的托管基础架构
        /// <summary>
        /// 通用应用程序托管框架
        /// <para>ASP.NET Core 3.0+ 应用</para>
        /// <para>后台服务、工作者服务 (Worker Services)</para>
        /// <para>Windows 服务</para>
        /// <para>需要在单个应用中托管多种服务类型</para>
        /// <para>需要使用通用托管功能的任何应用类型</para>
        /// <para>现代 .NET 应用开发</para>
        /// </summary>
        public IHostEnvironment Enviroment { get; set; }

        public ApplicationInitializationContext(IServiceProvider serviceProvider
            , IApplicationBuilder applicationBuilder, IHostEnvironment hostingEnvironment)
        {
            ServiceProvider = serviceProvider;
            ApplicationBuilder = applicationBuilder;
            Enviroment = hostingEnvironment;
        }


    }
}
