using Microsoft.EntityFrameworkCore;
using MokPermissions.Application.Contracts;
using MokPermissions.Domain;
using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 基于EF Core的权限存储实现
    /// </summary>
    public class EfCorePermissionStore : IPermissionStore
    {
        private readonly PermissionManagementDbContext _dbContext;

        public EfCorePermissionStore(PermissionManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PermissionGrantStatus> IsGrantedAsync(string name, string providerName, string providerKey)
        {
            var permissionGrant = await _dbContext.PermissionGrants
                .FirstOrDefaultAsync(p =>
                    p.Name == name &&
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey);

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
                .Where(p => p.ProviderName == providerName && p.ProviderKey == providerKey)
                .ToListAsync();
        }

        public async Task SaveAsync(string permissionName, string providerName, string providerKey, bool isGranted)
        {
            var permissionGrant = await _dbContext.PermissionGrants
                .FirstOrDefaultAsync(p =>
                    p.Name == permissionName &&
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey);

            if (permissionGrant == null)
            {
                permissionGrant = new PermissionGrant(
                    permissionName,
                    providerName,
                    providerKey,
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
                    p.ProviderKey == providerKey);

            if (permissionGrant != null)
            {
                _dbContext.PermissionGrants.Remove(permissionGrant);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
