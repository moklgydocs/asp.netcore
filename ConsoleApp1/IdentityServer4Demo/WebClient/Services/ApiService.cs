using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebClient.Services
{
    /// <summary>
    /// API服务实现
    /// 处理与受保护API的通信
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiService> _logger;
        private const string ApiBaseUrl = "https://localhost:6001/api";

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 获取访问令牌并设置到HTTP客户端
        /// </summary>
        private async Task SetAccessTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext不可用");
            }

            // 从认证属性中获取访问令牌
            var accessToken = await httpContext.GetTokenAsync("access_token");
            
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("访问令牌不可用");
            }

            // 设置Authorization头
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            _logger.LogDebug("设置访问令牌到HTTP客户端");
        }

        /// <summary>
        /// 获取天气预报
        /// </summary>
        public async Task<string> GetWeatherForecastAsync()
        {
            try
            {
                await SetAccessTokenAsync();
                
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/WeatherForecast");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("成功获取天气预报数据");
                    return content;
                }
                else
                {
                    var error = $"获取天气预报失败: {response.StatusCode} - {response.ReasonPhrase}";
                    _logger.LogWarning(error);
                    return $"错误: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取天气预报时发生异常");
                return $"异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public async Task<string> GetUsersAsync()
        {
            try
            {
                await SetAccessTokenAsync();
                
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Users");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("成功获取用户列表");
                    return content;
                }
                else
                {
                    var error = $"获取用户列表失败: {response.StatusCode} - {response.ReasonPhrase}";
                    _logger.LogWarning(error);
                    return $"错误: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户列表时发生异常");
                return $"异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        public async Task<string> CreateUserAsync(object user)
        {
            try
            {
                await SetAccessTokenAsync();
                
                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/Users", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("成功创建用户");
                    return responseContent;
                }
                else
                {
                    var error = $"创建用户失败: {response.StatusCode} - {response.ReasonPhrase}";
                    _logger.LogWarning(error);
                    return $"错误: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建用户时发生异常");
                return $"异常: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取当前用户的令牌信息
        /// </summary>
        public async Task<string> GetTokenInfoAsync()
        {
            try
            {
                await SetAccessTokenAsync();
                
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Users/me/token-info");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("成功获取令牌信息");
                    return content;
                }
                else
                {
                    var error = $"获取令牌信息失败: {response.StatusCode} - {response.ReasonPhrase}";
                    _logger.LogWarning(error);
                    return $"错误: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取令牌信息时发生异常");
                return $"异常: {ex.Message}";
            }
        }
    }
}