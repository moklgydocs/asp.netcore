using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Entitys
{
    /// <summary>
    /// 当前租户信息实现
    /// </summary>
    public class CurrentTenant : ICurrentTenant
    {
        private readonly AsyncLocal<Tenant> _currentTenant = new AsyncLocal<Tenant>();

        public virtual Guid? Id => _currentTenant.Value?.TenantId;

        public virtual string Name => _currentTenant.Value?.TenantName;

        public virtual bool IsAvailable => Id.HasValue;

        public IDisposable Change(Guid? tenantId, string tenantName = null)
        {
            var previousTenant = _currentTenant.Value;
            _currentTenant.Value = new Tenant(tenantId, tenantName);

            return new DisposeAction(() =>
            {
                _currentTenant.Value = previousTenant;
            });
        }
    }
}
