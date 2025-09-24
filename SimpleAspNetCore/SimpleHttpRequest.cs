using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore
{
    // 简化版的HttpRequest，对应ASP.NET Core中的HttpRequest
    public class SimpleHttpRequest
    {
        private readonly IHttpRequestFeature _feature;

        public SimpleHttpRequest(IHttpRequestFeature feature)
        {
            _feature = feature;
        }

        public string Method => _feature.Method;

        public string Path => _feature.Path;

        public IDictionary<string, object> Headers => _feature.Headers;

        public string Body => _feature.Body;
    }
}
