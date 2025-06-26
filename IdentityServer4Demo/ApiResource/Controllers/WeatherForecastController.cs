using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiResource.Controllers
{
    /// <summary>
    /// 天气预报控制器
    /// 演示不同级别的API保护
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "冰冻", "寒冷", "凉爽", "温和", "温暖", "炎热", "酷热", "灼热", "焦灼", "炽热"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取天气预报（需要读取权限）
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "ReadAccess")]
        public IEnumerable<WeatherForecast> Get()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            var scopes = User.FindAll("scope").Select(c => c.Value).ToList();

            _logger.LogInformation("用户 {Username} 获取天气预报，拥有作用域: {Scopes}", 
                username, string.Join(", ", scopes));

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 获取指定日期的天气预报（需要读取权限）
        /// </summary>
        [HttpGet("{date}")]
        [Authorize(Policy = "ReadAccess")]
        public ActionResult<WeatherForecast> GetByDate(string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return BadRequest("日期格式无效");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            _logger.LogInformation("用户 {Username} 获取 {Date} 的天气预报", username, date);

            return new WeatherForecast
            {
                Date = parsedDate,
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
        }

        /// <summary>
        /// 创建天气预报（需要写入权限）
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "WriteAccess")]
        public ActionResult<WeatherForecast> Create([FromBody] CreateWeatherForecastRequest request)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            _logger.LogInformation("用户 {Username} 创建天气预报", username);

            var forecast = new WeatherForecast
            {
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            return CreatedAtAction(nameof(GetByDate), new { date = forecast.Date.ToString() }, forecast);
        }

        /// <summary>
        /// 更新天气预报（需要写入权限）
        /// </summary>
        [HttpPut("{date}")]
        [Authorize(Policy = "WriteAccess")]
        public ActionResult<WeatherForecast> Update(string date, [FromBody] UpdateWeatherForecastRequest request)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return BadRequest("日期格式无效");
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            _logger.LogInformation("用户 {Username} 更新 {Date} 的天气预报", username, date);

            var forecast = new WeatherForecast
            {
                Date = parsedDate,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            return forecast;
        }

        /// <summary>
        /// 删除天气预报（仅管理员可操作）
        /// </summary>
        [HttpDelete("{date}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(string date)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "未知角色";
            
            _logger.LogInformation("管理员 {Username} (角色: {Role}) 删除 {Date} 的天气预报", 
                username, role, date);

            return Ok(new { Message = $"已删除 {date} 的天气预报" });
        }
    }

    /// <summary>
    /// 天气预报模型
    /// </summary>
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string? Summary { get; set; }
    }

    /// <summary>
    /// 创建天气预报请求
    /// </summary>
    public class CreateWeatherForecastRequest
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新天气预报请求
    /// </summary>
    public class UpdateWeatherForecastRequest
    {
        public int TemperatureC { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}