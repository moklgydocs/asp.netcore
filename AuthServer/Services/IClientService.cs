namespace AuthServer.Services
{
    /// <summary>
    /// OpenIddict客户端管理服务接口
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// 创建所有客户端应用程序
        /// </summary>
        Task CreateClientsAsync();

        /// <summary>
        /// 创建或更新客户端
        /// </summary>
        Task CreateOrUpdateClientAsync(string clientId, ClientDefinition definition);

        /// <summary>
        /// 删除客户端
        /// </summary>
        Task DeleteClientAsync(string clientId);

        /// <summary>
        /// 获取客户端信息
        /// </summary>
        Task<object?> GetClientAsync(string clientId);
    }

    /// <summary>
    /// 客户端定义
    /// </summary>
    public class ClientDefinition
    {
        public string ClientId { get; set; } = string.Empty;
        public string? ClientSecret { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string[] GrantTypes { get; set; } = Array.Empty<string>();
        public string[] Scopes { get; set; } = Array.Empty<string>();
        public string[] RedirectUris { get; set; } = Array.Empty<string>();
        public string[] PostLogoutRedirectUris { get; set; } = Array.Empty<string>();
        public bool RequireConsent { get; set; } = false;
        public bool RequirePkce { get; set; } = false;
        public TimeSpan? AccessTokenLifetime { get; set; }
        public TimeSpan? IdentityTokenLifetime { get; set; }
        public TimeSpan? RefreshTokenLifetime { get; set; }
    }
}