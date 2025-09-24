using SimpleAspNetCore.Kestrel;
using SimpleAspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleAspNetCore.Kestrel;
// 简化版Kestrel服务器，对应ASP.NET Core中的KestrelServer
// 在实际源码中位于Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServer
public class SimpleKestrelServer : IServer
{
    private readonly IPEndPoint _endPoint;
    private TcpListener _listener;
    private CancellationTokenSource _cancellationTokenSource;
    private IHttpApplication<SimpleHttpContext> _application;

    // 特性集合，包含服务器信息
    public IFeatureCollection Features { get; } = new FeatureCollection();

    public SimpleKestrelServer(IPEndPoint endPoint)
    {
        _endPoint = endPoint;
        Console.WriteLine($"[SimpleKestrel] 创建了服务器实例，监听端点: {endPoint}");
    }

    // 启动服务器并开始接受请求
    // 对应KestrelServer.StartAsync的实现
    public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
    {
        // 由于我们的简化实现，这里需要进行类型转换
        // 在实际ASP.NET Core中，使用泛型约束和接口保证类型安全
        _application = application as IHttpApplication<SimpleHttpContext>;
        if (_application == null)
        {
            throw new InvalidOperationException("Application context type mismatch");
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _listener = new TcpListener(_endPoint);
        _listener.Start();

        Console.WriteLine($"[SimpleKestrel] 服务器已启动，正在监听: {_endPoint}");

        // 在后台任务中接受连接请求
        _ = AcceptConnectionsAsync(_cancellationTokenSource.Token);

        await Task.CompletedTask;
    }

    // 停止服务器
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Cancel();
        _listener?.Stop();
        Console.WriteLine("[SimpleKestrel] 服务器已停止");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _listener?.Stop();
    }

    // 接受并处理连接请求
    // 在实际的KestrelServer中，这部分逻辑更加复杂，涉及到连接多路复用、线程池等
    private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 接受客户端连接
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("[SimpleKestrel] 接受了新的客户端连接");

                // 在新线程中处理请求，避免阻塞接受连接的线程
                _ = Task.Run(() => ProcessRequestAsync(client, cancellationToken));
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            Console.WriteLine($"[SimpleKestrel] 接受连接时发生错误: {ex.Message}");
        }
    }


    // SimpleKestrelServer.cs中的ProcessRequestAsync方法重新实现
    private async Task ProcessRequestAsync(TcpClient client, CancellationToken cancellationToken)
    {
        try
        {
            using (NetworkStream stream = client.GetStream())
            {
                // 读取原始数据
                byte[] buffer = new byte[8192];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    Console.WriteLine("[SimpleKestrel] 空连接");
                    return;
                }

                // 检查是否是HTTP请求（查找常见HTTP方法）
                string httpMethods = "GET|POST|PUT|DELETE|HEAD|OPTIONS|PATCH|CONNECT|TRACE";
                bool isHttpRequest = false;
                string requestData = string.Empty;

                // 尝试UTF8解码并检查HTTP方法
                try
                {
                    requestData = System.Text.Encoding.ASCII.GetString(buffer, 0, Math.Min(20, bytesRead));
                    isHttpRequest = httpMethods.Split('|').Any(method => requestData.StartsWith(method + " "));
                }
                catch
                {
                    isHttpRequest = false;
                }

                if (!isHttpRequest)
                {
                    // 如果不是HTTP请求，尝试检测是否是TLS握手（TLS记录以0x16开头）
                    if (bytesRead > 0 && buffer[0] == 0x16)
                    {
                        Console.WriteLine("[SimpleKestrel] 收到TLS/SSL握手请求，但我们的服务器不支持HTTPS");
                        string response = "HTTP/1.1 400 Bad Request\r\n" +
                                         "Content-Type: text/plain\r\n" +
                                         "Connection: close\r\n" +
                                         "Content-Length: 52\r\n" +
                                         "\r\n" +
                                         "This server only supports HTTP, not HTTPS (SSL/TLS)";
                        byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
                    else
                    {
                        Console.WriteLine("[SimpleKestrel] 收到非HTTP请求，输出前50个字节:");
                        DumpHexBytes(buffer, Math.Min(50, bytesRead));
                    }
                    return;
                }

                // 现在我们确定这是一个HTTP请求，解析它
                requestData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[SimpleKestrel] 收到HTTP请求：{bytesRead}字节");

                // 查找请求行结束位置
                int endOfRequestLine = requestData.IndexOf("\r\n");
                if (endOfRequestLine == -1)
                {
                    Console.WriteLine("[SimpleKestrel] 无效的HTTP请求：找不到请求行结束");
                    return;
                }

                // 解析请求行
                string requestLine = requestData.Substring(0, endOfRequestLine);
                Console.WriteLine($"[SimpleKestrel] 请求行: {requestLine}");

                string[] requestParts = requestLine.Split(' ');
                if (requestParts.Length < 3)
                {
                    Console.WriteLine("[SimpleKestrel] 无效的HTTP请求行格式");
                    return;
                }

                string method = requestParts[0];
                string path = requestParts[1];
                string httpVersion = requestParts[2];

                Console.WriteLine($"[SimpleKestrel] 解析HTTP请求: {method} {path} {httpVersion}");

                // 解析请求头
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                int headerSectionStart = endOfRequestLine + 2; // 跳过\r\n
                int headerSectionEnd = requestData.IndexOf("\r\n\r\n", headerSectionStart);

                if (headerSectionEnd == -1)
                {
                    Console.WriteLine("[SimpleKestrel] 无法找到请求头结束");
                    headerSectionEnd = requestData.Length; // 假设请求中没有请求体
                }
                else
                {
                    string headerSection = requestData.Substring(headerSectionStart, headerSectionEnd - headerSectionStart);
                    string[] headerLines = headerSection.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string headerLine in headerLines)
                    {
                        int colonIndex = headerLine.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            string key = headerLine.Substring(0, colonIndex).Trim();
                            string value = headerLine.Substring(colonIndex + 1).Trim();
                            headers[key] = value;
                        }
                    }
                }

                // 输出请求头
                Console.WriteLine("[SimpleKestrel] 请求头:");
                foreach (var header in headers)
                {
                    Console.WriteLine($"  {header.Key}: {header.Value}");
                }

                // 提取请求体
                string body = string.Empty;
                if (headerSectionEnd + 4 < requestData.Length)
                {
                    body = requestData.Substring(headerSectionEnd + 4);
                }

                // 创建并执行HTTP上下文
                var features = new FeatureCollection();

                // 添加请求特性
                var requestFeature = new HttpRequestFeature
                {
                    Method = method,
                    Path = path,
                    Body = body
                };
                foreach (var header in headers)
                {
                    requestFeature.Headers[header.Key] = header.Value;
                }
                features[typeof(IHttpRequestFeature)] = requestFeature;

                // 添加响应特性
                var responseFeature = new HttpResponseFeature(stream);
                features[typeof(IHttpResponseFeature)] = responseFeature;

                // 创建HTTP上下文并处理请求
                var context = _application.CreateContext(features);

                try
                {
                    Console.WriteLine($"[SimpleKestrel] 开始处理路径: {path}");
                    await _application.ProcessRequestAsync(context);
                    Console.WriteLine($"[SimpleKestrel] 完成处理路径: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SimpleKestrel] 处理请求时发生错误: {ex.Message}");
                    responseFeature.StatusCode = 500;
                    await responseFeature.WriteAsync("Internal Server Error: " + ex.Message);
                }
                finally
                {
                    _application.DisposeContext(context, null);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SimpleKestrel] 处理连接时发生错误: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    // 添加辅助方法用于显示十六进制数据
    private void DumpHexBytes(byte[] buffer, int length)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(buffer[i].ToString("X2") + " ");
            if ((i + 1) % 16 == 0) sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
    // 新增用于发送错误响应的辅助方法
    private async Task SendErrorResponseAsync(NetworkStream stream, int statusCode, string statusMessage)
    {
        string response = $"HTTP/1.1 {statusCode} {statusMessage}\r\n" +
                         "Content-Type: text/plain\r\n" +
                         $"Content-Length: {statusMessage.Length}\r\n" +
                         "\r\n" +
                         statusMessage;

        byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
    }
    // 添加这个辅助方法来验证HTTP方法
    private bool IsValidHttpMethod(string method)
    {
        return method.Equals("GET", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("DELETE", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("HEAD", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("TRACE", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("CONNECT", StringComparison.OrdinalIgnoreCase);
    }

    // HTTP请求特性实现
    private class HttpRequestFeature : IHttpRequestFeature
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>();
        public string Body { get; set; }
    }

    // SimpleKestrelServer.cs中的HttpResponseFeature类重新实现
    private class HttpResponseFeature : IHttpResponseFeature
    {
        private readonly NetworkStream _stream;
        private readonly List<(Func<object, Task> Callback, object State)> _onStartingCallbacks = new List<(Func<object, Task>, object)>();
        private readonly List<(Func<object, Task> Callback, object State)> _onCompletedCallbacks = new List<(Func<object, Task>, object)>();
        private bool _headersWritten = false;

        public HttpResponseFeature(NetworkStream stream)
        {
            _stream = stream;
        }

        public int StatusCode { get; set; } = 200;
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public void OnStarting(Func<object, Task> callback, object state)
        {
            _onStartingCallbacks.Add((callback, state));
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
            _onCompletedCallbacks.Add((callback, state));
        }

        public async Task WriteAsync(string content)
        {
            try
            {
                if (!_headersWritten)
                {
                    Console.WriteLine($"[HttpResponse] 准备发送状态码: {StatusCode}");

                    // 执行OnStarting回调
                    foreach (var (callback, state) in _onStartingCallbacks)
                    {
                        await callback(state);
                    }

                    // 准备响应
                    var responseBuilder = new System.Text.StringBuilder();

                    // 状态行
                    responseBuilder.AppendLine($"HTTP/1.1 {StatusCode} {GetStatusDescription(StatusCode)}");

                    // 设置默认头
                    if (!Headers.ContainsKey("Content-Type"))
                    {
                        Headers["Content-Type"] = "text/plain; charset=utf-8";
                    }

                    if (!Headers.ContainsKey("Connection"))
                    {
                        Headers["Connection"] = "close";  // 不使用Keep-Alive简化实现
                    }

                    // 设置内容长度
                    byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
                    Headers["Content-Length"] = contentBytes.Length.ToString();

                    // 添加所有响应头
                    foreach (var header in Headers)
                    {
                        responseBuilder.AppendLine($"{header.Key}: {header.Value}");
                    }

                    // 添加空行表示头部结束
                    responseBuilder.AppendLine();

                    // 转换为字节并发送
                    byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(responseBuilder.ToString());
                    await _stream.WriteAsync(headerBytes, 0, headerBytes.Length);

                    _headersWritten = true;
                    Console.WriteLine("[HttpResponse] 已发送响应头");
                }

                // 发送响应体
                byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(content);
                await _stream.WriteAsync(bodyBytes, 0, bodyBytes.Length);
                await _stream.FlushAsync();

                Console.WriteLine($"[HttpResponse] 已发送响应体: {content.Length} 字节");

                // 执行OnCompleted回调
                foreach (var (callback, state) in _onCompletedCallbacks)
                {
                    await callback(state);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HttpResponse] 发送响应时出错: {ex.Message}");
            }
        }

        private string GetStatusDescription(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                400 => "Bad Request",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => "Unknown"
            };
        }
    }
}