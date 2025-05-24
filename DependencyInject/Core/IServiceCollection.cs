using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    /// <summary>
    /// 服务注册集合接口，定义依赖注入容器在配置阶段用于注册服务的集合结构。
    /// <para>
    /// 该接口继承自 <see cref="IList{T}"/>，其中 T 为 <see cref="ServiceDescriptor"/>。
    /// 这意味着它具备所有列表操作能力（如 Add、Remove、索引访问等），
    /// 并且可以灵活扩展以支持自定义的服务注册逻辑。
    /// </para>
    /// <para>
    /// 设计目的：
    /// <list type="bullet">
    /// <item>1. 作为依赖注入容器服务注册的标准入口，所有服务注册操作都基于该接口。</item>
    /// <item>2. 兼容主流 .NET 依赖注入生态（如 Microsoft.Extensions.DependencyInjection），便于迁移和扩展。</item>
    /// <item>3. 通过接口隔离具体实现，便于单元测试和自定义扩展。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 好处：
    /// <list type="bullet">
    /// <item>• 统一服务注册方式，提升代码一致性和可维护性。</item>
    /// <item>• 支持链式注册和批量操作，提升开发效率。</item>
    /// <item>• 便于后续扩展自定义方法（如条件注册、批量注册等）。</item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IServiceCollection : IList<ServiceDescriptor>
    {
        // 该接口本身不定义新成员，所有操作由 IList<ServiceDescriptor> 提供。
        // 可根据需要扩展自定义服务注册方法。
    }
}
