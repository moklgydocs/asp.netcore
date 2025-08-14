using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MokPermissions.Application.Contracts.Dtos;
using MokPermissions.Domain;
using MokPermissions.Domain.Shared;

namespace MokPermissions.Web.HttpApi.Controllers
{
    /// <summary>
    /// 权限管理控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionManager _permissionManager;
        private readonly PermissionDefinitionManager _permissionDefinitionManager;
        private readonly IPermissionChecker _permissionChecker;

        public PermissionsController(
            IPermissionManager permissionManager,
            PermissionDefinitionManager permissionDefinitionManager,
            IPermissionChecker permissionChecker)
        {
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionChecker = permissionChecker;
        }

        /// <summary>
        /// 获取所有权限组
        /// </summary>
        /// <returns>权限组列表</returns>
        [HttpGet("groups")]
        [Authorize("Permission.PermissionManagement")]
        public async Task<ActionResult<List<PermissionGroupDto>>> GetGroupsAsync()
        {
            var groups = _permissionDefinitionManager.GetGroups();
            var result = new List<PermissionGroupDto>();

            foreach (var group in groups)
            {
                var groupDto = new PermissionGroupDto
                {
                    Name = group.Name,
                    DisplayName = group.DisplayName,
                    Permissions = new List<PermissionDto>()
                };

                foreach (var permission in group.Permissions)
                {
                    await AddPermissionToDtoRecursively(permission, null, groupDto.Permissions);
                }

                result.Add(groupDto);
            }

            return result;
        }

        /// <summary>
        /// 获取指定提供者的权限
        /// </summary>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        /// <returns>权限组列表</returns>
        [HttpGet]
        [Authorize("Permission.PermissionManagement")]
        public async Task<ActionResult<List<PermissionGroupDto>>> GetAsync(
            [FromQuery] string providerName,
            [FromQuery] string providerKey)
        {
            var groups = _permissionDefinitionManager.GetGroups();
            var grantedPermissions = await _permissionManager.GetAllAsync(providerName, providerKey);
            var result = new List<PermissionGroupDto>();

            foreach (var group in groups)
            {
                var groupDto = new PermissionGroupDto
                {
                    Name = group.Name,
                    DisplayName = group.DisplayName,
                    Permissions = new List<PermissionDto>()
                };

                foreach (var permission in group.Permissions)
                {
                    await AddPermissionToDtoRecursively(
                        permission,
                        grantedPermissions,
                        groupDto.Permissions);
                }

                result.Add(groupDto);
            }

            return result;
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="providerName">提供者名称</param>
        /// <param name="providerKey">提供者键</param>
        /// <param name="request">更新请求</param>
        [HttpPut]
        [Authorize("Permission.PermissionManagement")]
        public async Task<ActionResult> UpdateAsync(
            [FromQuery] string providerName,
            [FromQuery] string providerKey,
            [FromBody] UpdatePermissionRequest request)
        {
            if (request.IsGranted)
            {
                await _permissionManager.GrantAsync(request.Name, providerName, providerKey);
            }
            else
            {
                await _permissionManager.ProhibitAsync(request.Name, providerName, providerKey);
            }

            return Ok();
        }

        private async Task AddPermissionToDtoRecursively(
            PermissionDefinition permission,
            List<PermissionGrant> grantedPermissions,
            List<PermissionDto> permissions,
            string parentName = null)
        {
            var isGranted = false;
            var isProhibited = false;

            if (grantedPermissions != null)
            {
                var grantedPermission = grantedPermissions
                    .FirstOrDefault(p => p.Name == permission.Name);

                if (grantedPermission != null)
                {
                    isGranted = grantedPermission.IsGranted;
                    isProhibited = !grantedPermission.IsGranted;
                }
            }
            else
            {
                isGranted = await _permissionChecker.IsGrantedAsync(permission.Name);
            }

            var permissionDto = new PermissionDto
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName,
                ParentName = parentName,
                IsGranted = isGranted,
                IsProhibited = isProhibited
            };

            permissions.Add(permissionDto);

            foreach (var child in permission.Children)
            {
                await AddPermissionToDtoRecursively(
                    child,
                    grantedPermissions,
                    permissions,
                    permission.Name);
            }
        }
    }

}
