using Microsoft.AspNetCore.Mvc;

namespace testyarp1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public IActionResult Index()
        {
            var response = new
            {
                Message = "Response from testyarp1",
                Service = "testyarp1",
                Port = "5085",
                Timestamp = DateTime.Now
            };
            return Ok(response);
        }
    }
}
