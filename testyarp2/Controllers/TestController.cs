using Microsoft.AspNetCore.Mvc;

namespace testyarp2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            var response = new
            {
                Message = "Response from testyarp2",
                Service = "testyarp2",
                Port = "5226",
                Timestamp = DateTime.Now
            };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult SayHello()
        {

            return Ok("Hello world");
        }
    }
}
