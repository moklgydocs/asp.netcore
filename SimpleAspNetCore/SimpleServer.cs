using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore
{
    public class SimpleServer
    {
    }

    // 特性集合，用于再不同组件间传递HTTP请求的信息
    public interface IFeatureCollection : IDictionary<Type, object> { }

    // 简化版特性集合实现
    public class FeatureCollection : Dictionary<Type, object>, IFeatureCollection { }

    // HTTP请求特性，包含请求信息
    public interface IHttpRequestFeature
    {
        string Method { get; set; }

        string Path { get; set; }

        IDictionary<string, object> Headers { get;}

        string Body { get; set; }
    }
    // HTTP响应特性，包含响应信息
    public interface IHttpResponseFeature
    {
        int StatusCode { get; set; }

        IDictionary<string, object> Headers { get; }

        void OnStarting(Func<object, Task> callback, object state);

        void OnCompleted(Func<object, Task> callback, object state);

        Task WriteAsync(string content);
    }
}
