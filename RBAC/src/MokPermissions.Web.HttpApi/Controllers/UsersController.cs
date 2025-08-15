using Microsoft.AspNetCore.Mvc;
using MokPermissions.Domain;
using MokPermissions.Domain.Shared;

namespace MokPermissions.Web.HttpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IPermissionChecker _permissionChecker;

        public UsersController(IPermissionChecker permissionChecker)
        {
            _permissionChecker = permissionChecker;
        }

        [HttpGet]
        [PermissionAuthorize("UserManagement.View")]
        public IActionResult Get()
        {
            return Ok(new[] { "User1", "User2", "User3" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            // 手动检查权限
            if (!await _permissionChecker.IsGrantedAsync("UserManagement.View"))
            {
                return Forbid();
            }

            return Ok($"User{id}");
        }

        [HttpPost]
        [PermissionAuthorize("UserManagement.Create")]
        public IActionResult Post([FromBody] string value)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("UserManagement.Update")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("UserManagement.Delete")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
