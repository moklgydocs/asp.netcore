using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Data
{
    /// <summary>
    /// 权限初始化器接口
    /// </summary>
    public interface IPermissionInitializer
    {
        /// <summary>
        /// 初始化权限
        /// </summary>
        Task InitializeAsync();
    }
}
