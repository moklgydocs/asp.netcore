using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Enums
{
    /// <summary>
    /// 生命周期
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        ///  单例：整个应用程序生命周期内只创建一次
        /// </summary>
        Singleton,    
        /// <summary>
        ///  作用域：在同一个作用域内只创建一次
        /// </summary>
        Scoped,      
        /// <summary>
        /// 临时：每次请求都创建新实例
        /// </summary>
        Transient     
    }
}
