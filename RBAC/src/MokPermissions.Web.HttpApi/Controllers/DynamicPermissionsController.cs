using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MokPermissions.Domain.Entitys;
using MokPermissions.Domain.Shared;

namespace MokPermissions.Web.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Permission.MokPermission")]
    public class DynamicPermissionsController : ControllerBase
    {
        private readonly IDynamicPermissionStore _dynamicPermissionStore;

        public DynamicPermissionsController(IDynamicPermissionStore dynamicPermissionStore)
        {
            _dynamicPermissionStore = dynamicPermissionStore;
        }

        [HttpGet]
        public async Task<ActionResult<List<DynamicPermissionRecord>>> GetAsync()
        {
            return await _dynamicPermissionStore.GetPermissionsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] DynamicPermissionRecord record)
        {
            await _dynamicPermissionStore.SavePermissionAsync(record);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] DynamicPermissionRecord record)
        {
            await _dynamicPermissionStore.SavePermissionAsync(record);
            return Ok();
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult> DeleteAsync(string name)
        {
            await _dynamicPermissionStore.DeletePermissionAsync(name);
            return Ok();
        }
    }
}
