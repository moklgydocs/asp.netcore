using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore
{
    // 简化版的HttpResponse，对应ASP.NET Core中的HttpResponse
    public class SimpleHttpResponse
    {
        private readonly IHttpResponseFeature _feature;

        public SimpleHttpResponse(IHttpResponseFeature feature)
        {
            _feature = feature;
        }

        public int StatusCode
        {
            get => _feature.StatusCode;
            set => _feature.StatusCode = value;
        }

        public IDictionary<string, object> Headers => _feature.Headers;

        public Task WriteAsync(string content)
        {
            return _feature.WriteAsync(content);
        }
    }
}
