using System.ComponentModel.DataAnnotations;

namespace Id4sIdentityServer.Models
{
    /// <summary>
    /// 登录输入模型
    /// </summary>
    public class LoginInputModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "用户名")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "记住我")]
        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// 登录视图模型
    /// </summary>
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }

    /// <summary>
    /// 外部身份提供者
    /// </summary>
    public class ExternalProvider
    {
        public string? DisplayName { get; set; }
        public string AuthenticationScheme { get; set; } = string.Empty;
    }

    /// <summary>
    /// 登出输入模型
    /// </summary>
    public class LogoutInputModel
    {
        public string? LogoutId { get; set; }
    }

    /// <summary>
    /// 登出视图模型
    /// </summary>
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }

    /// <summary>
    /// 已登出视图模型
    /// </summary>
    public class LoggedOutViewModel
    {
        public string? PostLogoutRedirectUri { get; set; }
        public string? ClientName { get; set; }
        public string? SignOutIframeUrl { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; }

        public string? LogoutId { get; set; }
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string? ExternalAuthenticationScheme { get; set; }
    }

    /// <summary>
    /// 错误视图模型
    /// </summary>
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage? Error { get; set; }
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public class ErrorMessage
    {
        public string Error { get; set; } = string.Empty;
        public string? ErrorDescription { get; set; }
        public string? RequestId { get; set; }
        //
        // 摘要:
        //     The display mode passed from the authorization request.
        //
        // 值:
        //     The display mode.
        public string DisplayMode { get; set; }

        //
        // 摘要:
        //     The UI locales passed from the authorization request.
        //
        // 值:
        //     The UI locales.
        public string UiLocales { get; set; }   
        //
        // 摘要:
        //     The redirect URI.
        public string RedirectUri { get; set; }

        //
        // 摘要:
        //     The response mode.
        public string ResponseMode { get; set; }

        //
        // 摘要:
        //     The client id making the request (if available).
        public string ClientId { get; set; }
    }
}