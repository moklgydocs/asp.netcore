using MokPermissions.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain
{
    /// <summary>
    /// 权限检查器实现
    /// </summary>
    public class PermissionChecker : IPermissionChecker
    {
        private readonly IPermissionStore _permissionStore;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;
        private readonly ICurrentUser _currentUser;

        public PermissionChecker(
            IPermissionStore permissionStore,
            PermissionDefinitionManager permissionDefinitionManager,
            ICurrentUser currentUser)
        {
            _permissionStore = permissionStore;
            _permissionDefinitionManager = permissionDefinitionManager;
            _currentUser = currentUser;
        }

        public async Task<bool> IsGrantedAsync(string name)
        {
            return await IsGrantedAsync(_currentUser.Principal, name);
        }

        public async Task<bool> IsGrantedAsync(ClaimsPrincipal user, string name)
        {
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            var permissionDefinition = _permissionDefinitionManager.GetPermission(name);

            // 如果默认授予该权限，则直接返回 true
            if (permissionDefinition.IsGrantedByDefault)
            {
                return true;
            }
            //  首先检查用户级别的权限
            var userID = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userID))
            {
                var userResult = await _permissionStore.IsGrantedAsync(name, "U", userID);
                if (userResult != PermissionGrantStatus.Undefined)
                {
                    return userResult == PermissionGrantStatus.Granted;
                }
            }

            // 其次检查角色级别的权限
            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            foreach (var roleName in roles)
            {
                var roleResult = await _permissionStore.IsGrantedAsync(name, "R", roleName);
                if (roleResult == PermissionGrantStatus.Granted)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<List<PermissionCheckResult>> IsGrantedAsync(string[] names)
        {
            return await IsGrantedAsync(_currentUser.Principal, names);
        }

        public async Task<List<PermissionCheckResult>> IsGrantedAsync(ClaimsPrincipal user, string[] names)
        {
            var result = new List<PermissionCheckResult>();
            foreach (var name in names)
            {
                var isGranted = await IsGrantedAsync(user, name);
                result.Add(new PermissionCheckResult(name, isGranted));
            }
            return result;
        }
    }
}
