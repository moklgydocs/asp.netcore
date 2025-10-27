using EfCore.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EfCore.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PgSqlController : Controller
    {
        private readonly IEFCorePractiseAppServices practiseAppServices;
        //public PgSqlController(pb) { }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("SelectDemoData")]
        public async Task<IActionResult> SelectDemoData()
        {
            var data = await practiseAppServices.PgSql_SelectDemo();
            return View(data);
        }

    }
}
