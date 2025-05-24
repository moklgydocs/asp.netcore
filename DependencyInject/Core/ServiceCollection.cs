using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务集合的具体实现，继承自 List&lt;ServiceDescriptor&gt; 并实现 IServiceCollection 接口。
    /// <para>
    /// 该类用于存储所有注册到依赖注入容器的服务描述符（ServiceDescriptor）。
    /// </para>
    /// <para>
    /// 设计说明：
    /// <list type="bullet">
    /// <item>1. 继承 List&lt;ServiceDescriptor&gt;，可直接使用 List 的所有方法（如 Add、Remove、索引访问等），便于操作服务集合。</item>
    /// <item>2. 实现 IServiceCollection 接口，保证与依赖注入扩展方法和容器的兼容性。</item>
    /// <item>3. 作为依赖注入容器的服务注册入口，所有服务注册操作（AddSingleton、AddScoped、AddTransient等）最终都操作该集合。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 为什么这样设计？
    /// <list type="bullet">
    /// <item>• 兼容主流 .NET 依赖注入生态（如 Microsoft.Extensions.DependencyInjection），方便迁移和扩展。</item>
    /// <item>• 简化服务注册和管理逻辑，利用 List 的高效实现。</item>
    /// <item>• 保持代码简洁，避免重复造轮子。</item>
    /// </list>
    /// </para>
    /// </summary>
    public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
        // 此类无需额外成员，所有功能由基类和接口提供。
        // 可根据需要扩展自定义方法或属性。
    }
}
