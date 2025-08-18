using Microsoft.EntityFrameworkCore;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Shared.MultiTenant;
using MokPermissions.Domain.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// EF Core动态权限存储实现
    /// </summary>
    public class EfCoreDynamicPermissionStore : IDynamicPermissionStore
    {
        private readonly MokPermissionDbContext _dbContext;
        private readonly ICurrentTenant _currentTenant;

        public EfCoreDynamicPermissionStore(
            MokPermissionDbContext dbContext,
            ICurrentTenant currentTenant)
        {
            _dbContext = dbContext;
            _currentTenant = currentTenant;
        }

        public async Task<List<DynamicPermissionRecord>> GetPermissionsAsync()
        {
            var permissions = await _dbContext.Set<DynamicPermission>()
                .Where(p => p.TenantId == _currentTenant.Id)
                .ToListAsync();

            return permissions.Select(p => new DynamicPermissionRecord
            {
                Name = p.Name,
                DisplayName = p.DisplayName,
                ParentName = p.ParentName,
                IsGrantedByDefault = p.IsGrantedByDefault,
                Description = p.Description,
                GroupName = p.GroupName
            }).ToList();
        }

        public async Task SavePermissionAsync(DynamicPermissionRecord record)
        {
            var permission = await _dbContext.Set<DynamicPermission>()
                .FirstOrDefaultAsync(p => p.Name == record.Name && p.TenantId == _currentTenant.Id);

            if (permission == null)
            {
                permission = new DynamicPermission
                {
                    Name = record.Name,
                    TenantId = _currentTenant.Id
                };

                await _dbContext.Set<DynamicPermission>().AddAsync(permission);
            }

            permission.DisplayName = record.DisplayName;
            permission.ParentName = record.ParentName;
            permission.IsGrantedByDefault = record.IsGrantedByDefault;
            permission.Description = record.Description;
            permission.GroupName = record.GroupName;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(string name)
        {
            var permission = await _dbContext.Set<DynamicPermission>()
                .FirstOrDefaultAsync(p => p.Name == name && p.TenantId == _currentTenant.Id);

            if (permission != null)
            {
                _dbContext.Set<DynamicPermission>().Remove(permission);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
