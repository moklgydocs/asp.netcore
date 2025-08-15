using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.Data
{
    /// <summary>
    /// 权限数据迁移接口
    /// </summary>
    public interface IPermissionDataMigrator
    {
        /// <summary>
        /// 迁移权限数据
        /// </summary>
        Task MigrateAsync();
    }
}
