using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限提供者接口，用于定义系统中的权限
    /// </summary>
    public interface IPermissionDefinitionProvider
    {
        /// <summary>
        /// 定义权限
        /// </summary>
        /// <param name="context">权限定义上下文</param>
        void Define(PermissionDefinitionContext context);
    }
}
