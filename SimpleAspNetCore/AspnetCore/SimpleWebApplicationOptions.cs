using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAspNetCore.AspnetCore
{
    public class SimpleWebApplicationOptions
    {
        public IPEndPoint EndPoint { get; set; } = new IPEndPoint(IPAddress.Loopback, 5000);
    }
}
