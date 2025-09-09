namespace GatewayCenter
{
    // 路由配置模型
    public class YarpRoute
    {
        public int Id { get; set; }
        public string RouteId { get; set; } // 路由唯一标识
        public string ClusterId { get; set; } // 关联的集群ID
        public string MatchPath { get; set; } // 路径匹配规则（如 "/api/{**catch-all}"）
        public string MatchHosts { get; set; } // 主机匹配（逗号分隔，如 "api.example.com,www.example.com"）
                                               // 可扩展其他字段：查询参数、请求方法等
    }

    // 集群配置模型
    public class YarpCluster
    {
        public int Id { get; set; }
        public string ClusterId { get; set; } // 集群唯一标识
        public string LoadBalancingPolicy { get; set; } // 负载均衡策略（如 "RoundRobin"）
    }

    // 集群目标节点模型（后端服务地址）
    public class YarpDestination
    {
        public int Id { get; set; }
        public string ClusterId { get; set; } // 关联的集群ID
        public string DestinationId { get; set; } // 目标节点唯一标识
        public string Address { get; set; } // 后端服务地址（如 "https://localhost:5001"）
    }
}
