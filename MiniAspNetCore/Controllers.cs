using System;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// 控制器基类 - 提供控制器的基础功能
    /// 模拟ASP.NET Core MVC中的Controller基类
    /// </summary>
    public abstract class ControllerBase
    {
        protected ILogger Logger { get; }
        
        protected ControllerBase(ILogger logger)
        {
            Logger = logger;
        }
        
        /// <summary>
        /// 返回字符串结果
        /// </summary>
        protected string Ok(string content)
        {
            return content;
        }
        
        /// <summary>
        /// 返回JSON结果（简化版）
        /// </summary>
        protected string Json(object data)
        {
            // 简化的JSON序列化
            return $"{{\"data\": \"{data}\"}}";
        }
    }
    
    /// <summary>
    /// 首页控制器 - 演示MVC控制器的使用
    /// </summary>
    public class HomeController : ControllerBase
    {
        public HomeController(ILogger logger) : base(logger)
        {
        }
        
        /// <summary>
        /// 首页动作方法
        /// </summary>
        public async Task<string> Index()
        {
            Logger.Log("[控制器] HomeController.Index 被调用");
            
            // 模拟异步操作
            await Task.Delay(100);
            
            return Ok("欢迎来到自定义 ASP.NET Core 框架！\n" +
                     "这是一个完整的框架核心功能演示，包括：\n" +
                     "✓ 依赖注入容器\n" +
                     "✓ 中间件管道\n" +
                     "✓ 路由系统\n" +
                     "✓ MVC控制器\n" +
                     "✓ HTTP请求处理");
        }
        
        /// <summary>
        /// 创建数据的动作方法
        /// </summary>
        public async Task<string> Create(HttpContext context)
        {
            Logger.Log("[控制器] HomeController.Create 被调用");
            
            // 读取请求体
            var body = await context.Request.ReadBodyAsync();
            
            // 模拟数据处理
            await Task.Delay(50);
            
            return Json(new { 
                Message = "数据创建成功", 
                Data = body.Length > 0 ? body : "无数据",
                Timestamp = DateTime.Now 
            });
        }
    }
}