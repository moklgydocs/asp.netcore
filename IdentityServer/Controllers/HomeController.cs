using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Mapster;

namespace IdentityServer.Controllers
{
    /// <summary>
    /// 主页控制器
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment,
            ILogger<HomeController> logger)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// 主页
        /// </summary>
        public IActionResult Index()
        {
            if (_environment.IsDevelopment())
            {
                // 仅在开发环境显示
                return View();
            }

            _logger.LogInformation("主页被访问，当前环境：{Environment}", _environment.EnvironmentName);
            return NotFound();
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // 从Identity Server检索错误详细信息
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                message.Adapt(vm.Error);
                //vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // 仅在开发环境显示敏感信息
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }

    /// <summary>
    /// 安全头部属性
    /// 添加安全相关的HTTP头部
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                // 添加安全头部
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "DENY");
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';");
                }

                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Referrer-Policy", "no-referrer");
                }
            }
        }
    }
}