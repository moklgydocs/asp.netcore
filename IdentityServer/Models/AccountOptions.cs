namespace Id4sIdentityServer.Models
{
    /// <summary>
    /// 账户选项配置
    /// </summary>
    public class AccountOptions
    {
        /// <summary>
        /// 是否允许本地登录
        /// </summary>
        public static bool AllowLocalLogin = true;

        /// <summary>
        /// 是否允许记住登录
        /// </summary>
        public static bool AllowRememberLogin = true;

        /// <summary>
        /// 记住登录的持续时间
        /// </summary>
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// 是否显示登出提示
        /// </summary>
        public static bool ShowLogoutPrompt = true;

        /// <summary>
        /// 登出后是否自动重定向
        /// </summary>
        public static bool AutomaticRedirectAfterSignOut = false;

        /// <summary>
        /// 无效凭据错误消息
        /// </summary>
        public static string InvalidCredentialsErrorMessage = "用户名或密码错误";
    }
}