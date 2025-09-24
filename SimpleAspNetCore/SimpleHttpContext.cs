using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore
{
    // 简化版的HttpContext，对应ASP.NET Core中的HttpContext
    public class SimpleHttpContext
    {
        public SimpleHttpRequest Request { get; }

        public SimpleHttpResponse Response { get; }

        public IFeatureCollection Features { get; }

        public SimpleHttpContext(IFeatureCollection features)
        {
            Features = features;

            var requestReature = (IHttpRequestFeature)features[typeof(IHttpRequestFeature)];
            var responseReature = (IHttpResponseFeature)features[typeof(IHttpResponseFeature)];

            Request = new SimpleHttpRequest(requestReature);
            Response = new SimpleHttpResponse(responseReature);
        }
    }
}
