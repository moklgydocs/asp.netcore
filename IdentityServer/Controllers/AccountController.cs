using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using Id4sIdentityServer.Services;
using Id4sIdentityServer.Models;
using IdentityServer4.Models;

namespace Id4sIdentityServer.Controllers
{
    /// <summary>
    /// 账户控制器
    /// 处理用户登录、登出等操作
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            IUserService userService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // 构建登录视图模型
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // 如果只有外部登录选项，直接重定向
                return RedirectToAction("Challenge", "External", new { scheme = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// 处理登录表单提交
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string? button)
        {
            // 检查是否点击了取消按钮
            if (button != "login")
            {
                // 用户点击了取消按钮
                var authcontext = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (authcontext != null)
                {
                    // 如果用户取消，发送访问被拒绝的结果
                    await _interaction.DenyAuthorizationAsync(authcontext, AuthorizationError.AccessDenied);

                    // 我们可以信任model.ReturnUrl，因为GetAuthorizationContextAsync返回了非null
                    if (authcontext.IsNativeClient())
                    {
                        // 原生客户端需要特殊处理
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl ?? "~/");
                }
                else
                {
                    // 由于我们没有有效的上下文，则重定向到主页
                    return Redirect("~/");
                }
            }

            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                // 验证用户凭据
                var user = await _userService.GetUserByUsernameAsync(model.Username);
                if (user != null && await _userService.ValidatePasswordAsync(user, model.Password) && user.IsActive)
                {
                    // 更新最后登录时间
                    await _userService.UpdateLastLoginAsync(user);

                    // 记录登录成功事件
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    // 设置认证属性
                    var props = new AuthenticationProperties
                    {
                        IsPersistent = AccountOptions.AllowRememberLogin && model.RememberLogin,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };

                    // 使用ASP.NET Core Identity进行登录
                    await _signInManager.SignInAsync(user, props);

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // 原生客户端需要特殊处理
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // 我们可以信任model.ReturnUrl，因为GetAuthorizationContextAsync返回了非null
                        return Redirect(model.ReturnUrl ?? "~/");
                    }

                    // 请求是本地的
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // 用户可能点击了恶意链接 - 应该记录日志
                        throw new Exception("无效的返回URL");
                    }
                }

                // 登录失败
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "无效的用户名或密码", clientId: context?.Client?.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // 出现错误，显示表单和错误
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// 登出页面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string? logoutId)
        {
            // 构建登出视图模型
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // 如果请求是经过身份验证的，则不显示登出提示
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// 处理登出操作
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // 构建已登出的视图模型
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity?.IsAuthenticated == true)
            {
                // 删除本地认证cookie
                await _signInManager.SignOutAsync();

                // 引发登出事件
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // 检查是否需要在联合网关上触发登出
            if (vm.TriggerExternalSignout)
            {
                // 构建要返回的URL，以便在登出联合提供商后
                // 用户将被重定向回来
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId }) ?? "~/";

                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        /// <summary>
        /// 访问被拒绝页面
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /*****************************************/
        /* 辅助方法 */
        /*****************************************/

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string? returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // 这是为了缩短代码
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string? logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity?.IsAuthenticated != true)
            {
                // 如果用户没有经过身份验证，则只显示已登出页面
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // 自动登出是安全的
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // 显示登出提示。这可以防止用户
            // 被另一个恶意网页自动登出。
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string? logoutId)
        {
            // 获取上下文信息（客户端名称、登出后重定向URI和iframe以进行联合登出）
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity?.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // 如果没有当前登出上下文，我们需要创建一个
                            // 这将捕获当前登录的用户和客户端的必要信息
                            // 以便我们可以在联合登出后进行最终登出
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        // 需要这些字段来处理外部身份验证
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            IUserService userService,
            IAuthenticationSchemeProvider schemeProvider,
            IClientStore clientStore,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _userService = userService;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _logger = logger;
        }
    }
}