using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomAspNetCore
{
    /// <summary>
    /// 简单HTTP服务器 - 模拟ASP.NET Core的Web服务器
    /// 核心职责：监听HTTP请求、创建HttpContext、执行中间件管道
    /// </summary>
    public class SimpleHttpServer
    {
        private readonly string _url;
        private readonly RequestDelegate _pipeline;
        private readonly Dictionary<string, RouteHandler> _routes;
        private readonly IServiceProvider _serviceProvider;
        private HttpListener _listener;
        
        public SimpleHttpServer(string url, RequestDelegate pipeline, 
            Dictionary<string, RouteHandler> routes, IServiceProvider serviceProvider)
        {
            _url = url;
            _pipeline = pipeline;
            _routes = routes;
            _serviceProvider = serviceProvider;
        }
        
        /// <summary>
        /// 启动HTTP服务器
        /// </summary>
        public async Task StartAsync()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url.EndsWith("/") ? _url : _url + "/");
            _listener.Start();
            
            // 持续监听请求
            while (true)
            {
                try
                {
                    var listenerContext = await _listener.GetContextAsync();
                    
                    // 不等待，允许并发处理多个请求
                    _ = Task.Run(() => ProcessRequestAsync(listenerContext));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"服务器错误: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 处理单个HTTP请求
        /// 核心流程：创建HttpContext -> 执行中间件管道 -> 处理路由 -> 返回响应
        /// </summary>
        private async Task ProcessRequestAsync(HttpListenerContext listenerContext)
        {
            try
            {
                // 创建自定义的HttpContext
                var context = await CreateHttpContextAsync(listenerContext);
                
                // 执行中间件管道
                await _pipeline(context);
                
                // 处理路由
                await HandleRoutingAsync(context);
                
                // 发送响应
                await SendResponseAsync(listenerContext, context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理请求时出错: {ex.Message}");
                
                // 发送500错误响应
                listenerContext.Response.StatusCode = 500;
                var errorBytes = Encoding.UTF8.GetBytes("Internal Server Error");
                await listenerContext.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                listenerContext.Response.Close();
            }
        }
        
        /// <summary>
        /// 从HttpListenerContext创建自定义HttpContext
        /// </summary>
        private async Task<HttpContext> CreateHttpContextAsync(HttpListenerContext listenerContext)
        {
            var context = new HttpContext();
            var request = listenerContext.Request;
            
            // 映射请求信息
            context.Request.Method = request.HttpMethod;
            context.Request.Path = request.Url?.AbsolutePath ?? "/";
            context.RequestServices = _serviceProvider;
            
            // 复制请求头
            foreach (string key in request.Headers.AllKeys)
            {
                context.Request.Headers[key] = request.Headers[key];
            }
            
            // 解析查询参数
            foreach (string key in request.QueryString.AllKeys ?? new string[0])
            {
                if (key != null)
                {
                    context.Request.Query[key] = request.QueryString[key];
                }
            }
            
            // 复制请求体
            if (request.HasEntityBody)
            {
                var bodyStream = new MemoryStream();
                await request.InputStream.CopyToAsync(bodyStream);
                bodyStream.Position = 0;
                context.Request.Body = bodyStream;
            }
            
            return context;
        }
        
        /// <summary>
        /// 处理路由匹配和执行
        /// </summary>
        private async Task HandleRoutingAsync(HttpContext context)
        {
            var routeKey = $"{context.Request.Method}:{context.Request.Path}";
            
            if (_routes.TryGetValue(routeKey, out var handler))
            {
                try
                {
                    var result = await handler.Handler(context);
                    await context.Response.WriteAsync(result);
                    Console.WriteLine($"[路由处理] 匹配路由: {routeKey}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[路由错误] {routeKey}: {ex.Message}");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("路由处理出错");
                }
            }
            else
            {
                // 404 Not Found
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"路由未找到: {routeKey}");
                Console.WriteLine($"[路由未找到] {routeKey}");
            }
        }
        
        /// <summary>
        /// 发送HTTP响应
        /// </summary>
        private async Task SendResponseAsync(HttpListenerContext listenerContext, HttpContext context)
        {
            var response = listenerContext.Response;
            
            // 设置响应状态码
            response.StatusCode = context.Response.StatusCode;
            
            // 设置响应头
            foreach (var header in context.Response.Headers)
            {
                try
                {
                    response.Headers[header.Key] = header.Value;
                }
                catch
                {
                    // 某些头可能不能直接设置，忽略错误
                }
            }
            
            // 设置内容类型
            response.ContentType = "text/plain; charset=utf-8";
            
            // 写入响应体
            var content = context.Response.GetContent();
            if (!string.IsNullOrEmpty(content))
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                response.ContentLength64 = bytes.Length;
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            }
            
            response.Close();
        }
        
        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            _listener?.Stop();
        }
    }
}