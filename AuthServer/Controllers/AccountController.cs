using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using AuthServer.Models;
using AuthServer.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthServer.Controllers
{
    /// <summary>
    /// 账户控制器
    /// 处理用户登录、登出和注册等操作
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // 如果用户已经登录，直接重定向
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        /// <summary>
        /// 处理登录表单提交
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 查找用户
                var user = await _userService.GetUserByUsernameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "用户名或密码错误");
                    return View(model);
                }

                // 验证密码
                var result = await _signInManager.PasswordSignInAsync(
                    user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // 更新最后登录时间
                    await _userService.UpdateLastLoginAsync(user);

                    _logger.LogInformation("用户 {Username} 登录成功", user.UserName);

                    // 重定向到返回URL或主页
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("用户 {Username} 账户已锁定", model.Username);
                    ModelState.AddModelError(string.Empty, "账户已锁定，请稍后再试");
                }
                else if (result.IsNotAllowed)
                {
                    _logger.LogWarning("用户 {Username} 账户未激活", model.Username);
                    ModelState.AddModelError(string.Empty, "账户未激活");
                }
                else
                {
                    _logger.LogWarning("用户 {Username} 登录失败", model.Username);
                    ModelState.AddModelError(string.Empty, "用户名或密码错误");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户 {Username} 登录时发生错误", model.Username);
                ModelState.AddModelError(string.Empty, "登录时发生错误，请稍后再试");
            }

            return View(model);
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            var username = User.Identity?.Name;

            await _signInManager.SignOutAsync();

            _logger.LogInformation("用户 {Username} 登出成功", username);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 访问被拒绝页面
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 注册页面
        /// </summary>
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            var model = new RegisterViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        /// <summary>
        /// 处理注册表单提交
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    DisplayName = model.DisplayName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = false // 实际项目中需要邮箱验证
                };

                var (success, errors) = await _userService.CreateUserAsync(user, model.Password);

                if (success)
                {
                    // 分配默认角色
                    await _userManager.AddToRoleAsync(user, "User");

                    _logger.LogInformation("用户 {Username} 注册成功", user.UserName);

                    // 自动登录
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户 {Username} 注册时发生错误", model.Username);
                ModelState.AddModelError(string.Empty, "注册时发生错误，请稍后再试");
            }

            return View(model);
        }

        /// <summary>
        /// 用户资料页面
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Username = user.UserName!,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Department = user.Department,
                JobTitle = user.JobTitle,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.DisplayName = model.DisplayName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Department = model.Department;
                user.JobTitle = model.JobTitle;
                user.PhoneNumber = model.PhoneNumber;

                var (success, errors) = await _userService.UpdateUserAsync(user);

                if (success)
                {
                    _logger.LogInformation("用户 {Username} 更新资料成功", user.UserName);
                    TempData["Success"] = "资料更新成功";
                }
                else
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新用户资料时发生错误");
                ModelState.AddModelError(string.Empty, "更新资料时发生错误，请稍后再试");
            }

            return View(model);
        }
    }

    /// <summary>
    /// 登录视图模型
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "用户名")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// 注册视图模型
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
        [Display(Name = "用户名")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [Display(Name = "邮箱")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(100, ErrorMessage = "密码长度必须至少{2}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "显示名称")]
        public string? DisplayName { get; set; }

        [Display(Name = "名字")]
        public string? FirstName { get; set; }

        [Display(Name = "姓氏")]
        public string? LastName { get; set; }

        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// 用户资料视图模型
    /// </summary>
    public class ProfileViewModel
    {
        [Display(Name = "用户名")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "邮箱")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "显示名称")]
        public string? DisplayName { get; set; }

        [Display(Name = "名字")]
        public string? FirstName { get; set; }

        [Display(Name = "姓氏")]
        public string? LastName { get; set; }

        [Display(Name = "部门")]
        public string? Department { get; set; }

        [Display(Name = "职位")]
        public string? JobTitle { get; set; }

        [Phone]
        [Display(Name = "电话号码")]
        public string? PhoneNumber { get; set; }
    }
}