using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomAspNetCore
{
    /// <summary>
    /// HTTP上下文 - 封装HTTP请求和响应的核心对象
    /// 在ASP.NET Core中，这是处理Web请求的中心对象
    /// </summary>
    public class HttpContext
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public IServiceProvider RequestServices { get; set; }
        public Dictionary<object, object> Items { get; } = new();
        
        public HttpContext()
        {
            Request = new HttpRequest();
            Response = new HttpResponse();
        }
    }
    
    /// <summary>
    /// HTTP请求对象 - 封装客户端发送的HTTP请求信息
    /// </summary>
    public class HttpRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Headers { get; } = new();
        public Dictionary<string, string> Query { get; } = new();
        public Stream Body { get; set; }
        
        /// <summary>
        /// 读取请求体内容
        /// </summary>
        public async Task<string> ReadBodyAsync()
        {
            if (Body == null) return string.Empty;
            
            using var reader = new StreamReader(Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
    
    /// <summary>
    /// HTTP响应对象 - 封装发送给客户端的HTTP响应信息
    /// </summary>
    public class HttpResponse
    {
        public int StatusCode { get; set; } = 200;
        public Dictionary<string, string> Headers { get; } = new();
        public MemoryStream Body { get; } = new();
        
        /// <summary>
        /// 写入响应内容
        /// </summary>
        public async Task WriteAsync(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            await Body.WriteAsync(bytes, 0, bytes.Length);
        }
        
        /// <summary>
        /// 获取响应内容
        /// </summary>
        public string GetContent()
        {
            return Encoding.UTF8.GetString(Body.ToArray());
        }
    }
    
    /// <summary>
    /// 请求委托 - 中间件管道中的核心委托类型
    /// 代表处理HTTP请求的方法
    /// </summary>
    public delegate Task RequestDelegate(HttpContext context);
}