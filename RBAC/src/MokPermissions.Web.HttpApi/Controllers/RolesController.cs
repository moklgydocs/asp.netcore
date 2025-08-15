using Microsoft.AspNetCore.Mvc;
using MokPermissions.Application.Contracts;
using MokPermissions.Domain.Shared;
using MokPermissions.Domain;

namespace MokPermissions.Web.HttpApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IPermissionChecker _permissionChecker;

        public RolesController(
            IRolePermissionService rolePermissionService,
            IPermissionChecker permissionChecker)
        {
            _rolePermissionService = rolePermissionService;
            _permissionChecker = permissionChecker;
        }

        [HttpGet("{id}/permissions")]
        [PermissionAuthorize("RoleManagement.View")]
        public async Task<IActionResult> GetPermissions(Guid id)
        {
            var permissions = await _rolePermissionService.GetPermissionsAsync(id);
            return Ok(permissions);
        }

        [HttpPost("{id}/permissions")]
        [PermissionAuthorize("RoleManagement.Update")]
        public async Task<IActionResult> SetPermissions(Guid id, [FromBody] List<string> permissionNames)
        {
            await _rolePermissionService.SetPermissionsAsync(id, permissionNames);
            return Ok();
        }
    }
}
