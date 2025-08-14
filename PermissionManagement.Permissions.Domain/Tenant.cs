using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    public class Tenant
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string TenantName { get; }

        public string Password { get; }

        public Tenant(Guid? tenantId, string tenantName = null, string password = null)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            Password = password;
        }
    }
}
