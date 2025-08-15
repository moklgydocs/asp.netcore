using Microsoft.EntityFrameworkCore;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 支持批量操作的EF Core权限存储实现
    /// </summary>
    public class BatchEfCorePermissionStore : EfCorePermissionStore, IBatchPermissionStore
    {
        private readonly MokPermissionDbContext _dbContext;

        public BatchEfCorePermissionStore(MokPermissionDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BatchSaveAsync(IEnumerable<string> permissionNames, string providerName, string providerKey, bool isGranted)
        {
            // 获取现有权限
            var existingPermissions = await _dbContext.PermissionGrants
                .Where(p =>
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    permissionNames.Contains(p.Name))
                .ToListAsync();

            // 计算需要添加的权限
            var existingPermissionNames = existingPermissions.Select(p => p.Name).ToHashSet();
            var permissionsToAdd = permissionNames
                .Where(name => !existingPermissionNames.Contains(name))
                .Select(name => new PermissionGrant(name, providerName, providerKey, null, isGranted))
                .ToList();

            // 更新现有权限
            foreach (var permission in existingPermissions)
            {
                permission.IsGranted = isGranted;
            }

            // 添加新权限
            if (permissionsToAdd.Any())
            {
                await _dbContext.PermissionGrants.AddRangeAsync(permissionsToAdd);
            }

            // 保存更改
            await _dbContext.SaveChangesAsync();
        }

        public async Task BatchDeleteAsync(IEnumerable<string> permissionNames, string providerName, string providerKey)
        {
            // 获取需要删除的权限
            var permissionsToDelete = await _dbContext.PermissionGrants
                .Where(p =>
                    p.ProviderName == providerName &&
                    p.ProviderKey == providerKey &&
                    permissionNames.Contains(p.Name))
                .ToListAsync();

            // 删除权限
            if (permissionsToDelete.Any())
            {
                _dbContext.PermissionGrants.RemoveRange(permissionsToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
