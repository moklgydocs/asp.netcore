using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    /// <summary>
    /// 主页控制器
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 首页
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// 隐私政策页面
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }
    }
}