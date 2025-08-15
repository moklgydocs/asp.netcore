using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Manager
{
    /// <summary>
    /// 发布事件的权限管理服务实现
    /// </summary>
    public class EventPublishingPermissionManager : IPermissionManager
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICurrentTenant _currentTenant;

        public EventPublishingPermissionManager(
            IPermissionManager permissionManager,
            IEventPublisher eventPublisher,
            ICurrentTenant currentTenant)
        {
            _permissionManager = permissionManager;
            _eventPublisher = eventPublisher;
            _currentTenant = currentTenant;
        }

        public async Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            return await _permissionManager.GetAllAsync(providerName, providerKey);
        }

        public async Task GrantAsync(string permissionName, string providerName, string providerKey)
        {
            await _permissionManager.GrantAsync(permissionName, providerName, providerKey);

            await _eventPublisher.PublishAsync(new PermissionGrantedEventData
            {
                PermissionName = permissionName,
                ProviderName = providerName,
                ProviderKey = providerKey,
                TenantId = _currentTenant.Id
            });
        }

        public async Task ProhibitAsync(string permissionName, string providerName, string providerKey)
        {
            await _permissionManager.ProhibitAsync(permissionName, providerName, providerKey);

            await _eventPublisher.PublishAsync(new PermissionProhibitedEventData
            {
                PermissionName = permissionName,
                ProviderName = providerName,
                ProviderKey = providerKey,
                TenantId = _currentTenant.Id
            });
        }

        public async Task RevokeAsync(string permissionName, string providerName, string providerKey)
        {
            await _permissionManager.RevokeAsync(permissionName, providerName, providerKey);

            await _eventPublisher.PublishAsync(new PermissionRevokedEventData
            {
                PermissionName = permissionName,
                ProviderName = providerName,
                ProviderKey = providerKey,
                TenantId = _currentTenant.Id
            });
        }
    }
}
