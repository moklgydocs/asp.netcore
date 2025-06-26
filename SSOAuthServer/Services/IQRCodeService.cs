namespace AuthServer.Services
{
    /// <summary>
    /// 二维码服务接口
    /// </summary>
    public interface IQRCodeService
    {
        /// <summary>
        /// 生成登录二维码
        /// </summary>
        Task<QRLoginResponse> GenerateLoginQRCodeAsync();

        /// <summary>
        /// 扫描确认登录
        /// </summary>
        Task<bool> ConfirmQRLoginAsync(string qrToken, string userId);

        /// <summary>
        /// 检查二维码登录状态
        /// </summary>
        Task<QRLoginStatus> CheckQRLoginStatusAsync(string qrToken);

        /// <summary>
        /// 取消二维码登录
        /// </summary>
        Task CancelQRLoginAsync(string qrToken);
    }

    /// <summary>
    /// 二维码登录响应
    /// </summary>
    public class QRLoginResponse
    {
        public string QRToken { get; set; } = string.Empty;
        public string QRCodeImage { get; set; } = string.Empty; // Base64编码的图片
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// 二维码登录状态
    /// </summary>
    public class QRLoginStatus
    {
        public QRLoginState State { get; set; }
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// 二维码登录状态枚举
    /// </summary>
    public enum QRLoginState
    {
        /// <summary>
        /// 等待扫描
        /// </summary>
        WaitingForScan = 0,

        /// <summary>
        /// 已扫描，等待确认
        /// </summary>
        Scanned = 1,

        /// <summary>
        /// 已确认，登录成功
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// 已过期
        /// </summary>
        Expired = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 4
    }
}