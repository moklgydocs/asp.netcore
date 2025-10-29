using EfCore.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EfCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PgSqlController : ControllerBase
    {
        private readonly IEFCorePractiseAppServices _practiseAppServices;
        public PgSqlController(IEFCorePractiseAppServices practiseAppServices)
        {
            _practiseAppServices = practiseAppServices;
        }
        public IActionResult Index()
        {
            return Ok();
        }
        [HttpGet("demo")]
        public async Task<IActionResult> SelectDemoData()
        {
            var data = await _practiseAppServices.PgSql_SelectDemo();
            return Ok(data);
        }
        [HttpGet("valid")]
        public async Task<IActionResult> Valid([Required] string name)
        {
            return Ok(name);
        }

    }
}
