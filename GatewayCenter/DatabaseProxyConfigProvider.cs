using Microsoft.EntityFrameworkCore;
using Yarp.ReverseProxy.Configuration;

namespace GatewayCenter
{
    public class DatabaseProxyConfigProvider : IProxyConfigProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DatabaseProxyConfigProvider> _logger;
        private DatabaseProxyConfig _currentConfig;
        private CancellationTokenSource _changeTokenSource = new();

        public DatabaseProxyConfigProvider(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<DatabaseProxyConfigProvider> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _currentConfig = LoadConfig().GetAwaiter().GetResult(); // 初始加载配置
        }

        // 加载最新配置（核心方法）
        private async Task<DatabaseProxyConfig> LoadConfig()
        {
            try
            {
                // 创建新的作用域来获取 DbContext
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<YarpConfigDbContext>();

                // 从数据库查询路由、集群和目标节点
                var routes = await dbContext.Routes
                    .Select(r => new RouteConfig
                    {
                        RouteId = r.RouteId,
                        ClusterId = r.ClusterId,
                        Match = new RouteMatch
                        {
                            Path = r.MatchPath,
                            Hosts = r.MatchHosts.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()
                        }
                        // 可扩展：添加转换（Transforms）、元数据（Metadata）等
                    })
                    .ToListAsync();

                var clusters = await dbContext.Clusters
                    .GroupJoin(
                        dbContext.Destinations,
                        cluster => cluster.ClusterId,
                        dest => dest.ClusterId,
                        (cluster, destinations) => new ClusterConfig
                        {
                            ClusterId = cluster.ClusterId,
                            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
                            Destinations = destinations.ToDictionary(
                                d => d.DestinationId,
                                d => new DestinationConfig { Address = d.Address }
                            )
                            // 可扩展：健康检查、会话亲和性等配置
                        })
                    .ToListAsync();

                return new DatabaseProxyConfig(routes, clusters, _changeTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load YARP config from database");
                throw; // 加载失败时抛出异常，阻止应用启动
            }
        }

        // 提供当前配置
        public IProxyConfig GetConfig() => _currentConfig;

        // 手动触发配置更新（可定时调用或通过数据库事件触发）
        public async Task RefreshConfigAsync()
        {
            try
            {
                var newConfig = await LoadConfig();
                // 创建新的变更令牌
                var newChangeTokenSource = new CancellationTokenSource();
                // 更新当前配置
                _currentConfig = new DatabaseProxyConfig(
                    newConfig.Routes,
                    newConfig.Clusters,
                    newChangeTokenSource.Token);
                // 取消旧令牌，通知YARP配置已变更
                _changeTokenSource.Cancel();
                _changeTokenSource.Dispose();
                // 替换为新的令牌源
                _changeTokenSource = newChangeTokenSource;
                _logger.LogInformation("YARP config refreshed from database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh YARP config");
            }
        }
    }
}
