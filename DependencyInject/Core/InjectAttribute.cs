using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    // 属性注入特性
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
