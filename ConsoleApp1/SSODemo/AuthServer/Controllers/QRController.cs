using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Models;
using AuthServer.Services;

namespace AuthServer.Controllers
{
    /// <summary>
    /// 二维码登录控制器
    /// </summary>
    [Route("qr")]
    public class QRController : Controller
    {
        private readonly IQRCodeService _qrCodeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISessionService _sessionService;
        private readonly IAuditService _auditService;
        private readonly ILogger<QRController> _logger;

        public QRController(
            IQRCodeService qrCodeService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ISessionService sessionService,
            IAuditService auditService,
            ILogger<QRController> logger)
        {
            _qrCodeService = qrCodeService;
            _userManager = userManager;
            _signInManager = signInManager;
            _sessionService = sessionService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// 生成登录二维码
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQR()
        {
            try
            {
                var response = await _qrCodeService.GenerateLoginQRCodeAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成二维码失败");
                return StatusCode(500, new { message = "生成二维码失败" });
            }
        }

        /// <summary>
        /// 检查二维码登录状态
        /// </summary>
        [HttpGet("status/{qrToken}")]
        public async Task<IActionResult> CheckStatus(string qrToken)
        {
            try
            {
                var status = await _qrCodeService.CheckQRLoginStatusAsync(qrToken);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查二维码状态失败: QRToken={QRToken}", qrToken);
                return StatusCode(500, new { message = "检查状态失败" });
            }
        }

        /// <summary>
        /// 二维码扫描页面（移动端访问）
        /// </summary>
        [HttpGet("login/{qrToken}")]
        [Authorize]
        public async Task<IActionResult> ScanQR(string qrToken)
        {
            try
            {
                var status = await _qrCodeService.CheckQRLoginStatusAsync(qrToken);
                if (status.State != QRLoginState.WaitingForScan)
                {
                    return View("QRExpired");
                }

                var model = new QRScanViewModel
                {
                    QRToken = qrToken,
                    UserName = User.Identity?.Name ?? "未知用户"
                };

                return View("ScanQR", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理二维码扫描失败: QRToken={QRToken}", qrToken);
                return View("Error");
            }
        }

        /// <summary>
        /// 确认二维码登录
        /// </summary>
        [HttpPost("confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmQR([FromBody] ConfirmQRRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest(new { message = "用户未找到" });
                }

                var success = await _qrCodeService.ConfirmQRLoginAsync(request.QRToken, user.Id);
                if (success)
                {
                    // 记录审计日志
                    await _auditService.LogUserActionAsync(
                        user.Id,
                        "QRLoginConfirm",
                        $"用户确认二维码登录: {request.QRToken}",
                        HttpContext.Connection.RemoteIpAddress?.ToString());

                    return Ok(new { message = "确认成功" });
                }

                return BadRequest(new { message = "确认失败" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "确认二维码登录失败: QRToken={QRToken}", request.QRToken);
                return StatusCode(500, new { message = "确认失败" });
            }
        }

        /// <summary>
        /// 取消二维码登录
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelQR([FromBody] CancelQRRequest request)
        {
            try
            {
                await _qrCodeService.CancelQRLoginAsync(request.QRToken);
                return Ok(new { message = "取消成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消二维码登录失败: QRToken={QRToken}", request.QRToken);
                return StatusCode(500, new { message = "取消失败" });
            }
        }
    }

    /// <summary>
    /// 二维码扫描视图模型
    /// </summary>
    public class QRScanViewModel
    {
        public string QRToken { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 确认二维码请求
    /// </summary>
    public class ConfirmQRRequest
    {
        public string QRToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// 取消二维码请求
    /// </summary>
    public class CancelQRRequest
    {
        public string QRToken { get; set; } = string.Empty;
    }
}