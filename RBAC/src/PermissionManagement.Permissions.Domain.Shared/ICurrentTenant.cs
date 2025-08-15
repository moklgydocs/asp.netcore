using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 当前租户信息接口
    /// </summary>
    public interface ICurrentTenant
    {
        /// <summary>
        /// 当前租户ID
        /// </summary>
        Guid? Id { get; }

        /// <summary>
        /// 当前租户名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 是否有当前租户
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// 切换当前租户
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="tenantName">租户名称</param>
        /// <returns>用于恢复先前租户的上下文对象</returns>
        IDisposable Change(Guid? tenantId, string tenantName = null);
    }

}
