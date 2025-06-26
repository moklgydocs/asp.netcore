using Microsoft.Extensions.Caching.Distributed;
using QRCoder;
using System.Text.Json;

namespace AuthServer.Services
{
    /// <summary>
    /// 二维码服务实现
    /// </summary>
    public class QRCodeService : IQRCodeService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<QRCodeService> _logger;
        private const string QR_PREFIX = "qr_login:";
        private const int QR_EXPIRE_MINUTES = 5;

        public QRCodeService(IDistributedCache cache, ILogger<QRCodeService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 生成登录二维码
        /// </summary>
        public async Task<QRLoginResponse> GenerateLoginQRCodeAsync()
        {
            try
            {
                var qrToken = Guid.NewGuid().ToString("N");
                var expiresAt = DateTime.UtcNow.AddMinutes(QR_EXPIRE_MINUTES);

                var qrData = new QRLoginStatus
                {
                    State = QRLoginState.WaitingForScan,
                    ExpiresAt = expiresAt
                };

                // 存储二维码状态
                await _cache.SetStringAsync(
                    $"{QR_PREFIX}{qrToken}",
                    JsonSerializer.Serialize(qrData),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(QR_EXPIRE_MINUTES)
                    });

                // 生成二维码图片
                var qrCodeImage = GenerateQRCodeImage($"https://localhost:5003/qr/login/{qrToken}");

                _logger.LogInformation("生成登录二维码成功: QRToken={QRToken}", qrToken);

                return new QRLoginResponse
                {
                    QRToken = qrToken,
                    QRCodeImage = qrCodeImage,
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成登录二维码失败");
                throw;
            }
        }

        /// <summary>
        /// 扫描确认登录
        /// </summary>
        public async Task<bool> ConfirmQRLoginAsync(string qrToken, string userId)
        {
            try
            {
                var status = await CheckQRLoginStatusAsync(qrToken);
                if (status.State != QRLoginState.WaitingForScan && status.State != QRLoginState.Scanned)
                {
                    return false;
                }

                var qrData = new QRLoginStatus
                {
                    State = QRLoginState.Confirmed,
                    UserId = userId,
                    ExpiresAt = status.ExpiresAt
                };

                await _cache.SetStringAsync(
                    $"{QR_PREFIX}{qrToken}",
                    JsonSerializer.Serialize(qrData),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(QR_EXPIRE_MINUTES)
                    });

                _logger.LogInformation("二维码登录确认成功: QRToken={QRToken}, UserId={UserId}", qrToken, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "确认二维码登录失败: QRToken={QRToken}, UserId={UserId}", qrToken, userId);
                return false;
            }
        }

        /// <summary>
        /// 检查二维码登录状态
        /// </summary>
        public async Task<QRLoginStatus> CheckQRLoginStatusAsync(string qrToken)
        {
            try
            {
                var qrDataString = await _cache.GetStringAsync($"{QR_PREFIX}{qrToken}");
                if (string.IsNullOrEmpty(qrDataString))
                {
                    return new QRLoginStatus
                    {
                        State = QRLoginState.Expired,
                        Message = "二维码已过期"
                    };
                }

                var qrData = JsonSerializer.Deserialize<QRLoginStatus>(qrDataString);
                if (qrData == null)
                {
                    return new QRLoginStatus
                    {
                        State = QRLoginState.Expired,
                        Message = "二维码数据无效"
                    };
                }

                // 检查是否过期
                if (qrData.ExpiresAt.HasValue && qrData.ExpiresAt.Value < DateTime.UtcNow)
                {
                    await CancelQRLoginAsync(qrToken);
                    return new QRLoginStatus
                    {
                        State = QRLoginState.Expired,
                        Message = "二维码已过期"
                    };
                }

                return qrData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查二维码登录状态失败: QRToken={QRToken}", qrToken);
                return new QRLoginStatus
                {
                    State = QRLoginState.Expired,
                    Message = "检查状态时发生错误"
                };
            }
        }

        /// <summary>
        /// 取消二维码登录
        /// </summary>
        public async Task CancelQRLoginAsync(string qrToken)
        {
            try
            {
                await _cache.RemoveAsync($"{QR_PREFIX}{qrToken}");
                _logger.LogInformation("取消二维码登录成功: QRToken={QRToken}", qrToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消二维码登录失败: QRToken={QRToken}", qrToken);
            }
        }

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        private string GenerateQRCodeImage(string content)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                
                var qrCodeBytes = qrCode.GetGraphic(20);
                return Convert.ToBase64String(qrCodeBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成二维码图片失败: Content={Content}", content);
                throw;
            }
        }
    }
}