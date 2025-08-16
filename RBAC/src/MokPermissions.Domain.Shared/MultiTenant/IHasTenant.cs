using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared.MultiTenant
{
    public interface IHasTenant
    {
        public Guid? TenantId { get; set; }
    }
}
