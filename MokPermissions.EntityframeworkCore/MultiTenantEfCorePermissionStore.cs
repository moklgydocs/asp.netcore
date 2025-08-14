using Microsoft.EntityFrameworkCore;
using MokPermissions.Domain.Shared;
using MokPermissions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 支持多租户的EF Core权限存储实现
    /// </summary>
    public class MultiTenantEfCorePermissionStore : IPermissionStore
    {
        private readonly PermissionManagementDbContext _dbContext;
        private readonly ICurrentTenant _currentTenant;

        public MultiTenantEfCorePermissionStore(
            PermissionManagementDbContext dbContext,
            ICurrentTenant currentTenant)
        {
            _dbContext = dbContext;
            _currentTenant = currentTenant;
        }

        public async Task<PermissionGrantStatus> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            var permissionGrant = await _dbContext.PermissionGrants
                .FirstOrDefaultAsync(p =>
                    p.Name == name &&
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    p.TenantId == _currentTenant.Id);

            if (permissionGrant == null)
            {
                return PermissionGrantStatus.Undefined;
            }

            return permissionGrant.IsGranted
                ? PermissionGrantStatus.Granted
                : PermissionGrantStatus.Prohibited;
        }

        public async Task<List<PermissionGrant>> GetAllAsync(string providerName, string providerKey)
        {
            return await _dbContext.PermissionGrants
                .Where(p =>
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    p.TenantId == _currentTenant.Id)
                .ToListAsync();
        }

        public async Task SaveAsync(string permissionName, string providerName, string providerKey, bool isGranted)
        {
            var permissionGrant = await _dbContext.PermissionGrants
                .FirstOrDefaultAsync(p =>
                    p.Name == permissionName &&
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    p.TenantId == _currentTenant.Id);

            if (permissionGrant == null)
            {
                permissionGrant = new PermissionGrant(
                    permissionName,
                    providerName,
                    providerKey,
                    _currentTenant.Id,
                    isGranted
                );

                await _dbContext.PermissionGrants.AddAsync(permissionGrant);
            }
            else
            {
                permissionGrant.IsGranted = isGranted;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string permissionName, string providerName, string providerKey)
        {
            var permissionGrant = await _dbContext.PermissionGrants
                .FirstOrDefaultAsync(p =>
                    p.Name == permissionName &&
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    p.TenantId == _currentTenant.Id);

            if (permissionGrant != null)
            {
                _dbContext.PermissionGrants.Remove(permissionGrant);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
