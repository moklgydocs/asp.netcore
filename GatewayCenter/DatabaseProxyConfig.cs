using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace GatewayCenter
{
    public class DatabaseProxyConfig : IProxyConfig
    {
        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }

        public DatabaseProxyConfig(
        IReadOnlyList<RouteConfig> routes,
        IReadOnlyList<ClusterConfig> clusters,
        CancellationToken changeToken)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(changeToken);
        }
    }
}
