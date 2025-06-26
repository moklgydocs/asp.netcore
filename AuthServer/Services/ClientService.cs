using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthServer.Services
{
    /// <summary>
    /// OpenIddict客户端管理服务实现
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly ILogger<ClientService> _logger;

        public ClientService(
            IOpenIddictApplicationManager applicationManager,
            ILogger<ClientService> logger)
        {
            _applicationManager = applicationManager;
            _logger = logger;
        }

        /// <summary>
        /// 创建所有客户端应用程序
        /// </summary>
        public async Task CreateClientsAsync()
        {
            var clients = GetClientDefinitions();

            foreach (var client in clients)
            {
                await CreateOrUpdateClientAsync(client.ClientId, client);
            }
        }

        /// <summary>
        /// 创建或更新客户端
        /// </summary>
        public async Task CreateOrUpdateClientAsync(string clientId, ClientDefinition definition)
        {
            try
            {
                var existingClient = await _applicationManager.FindByClientIdAsync(clientId);

                if (existingClient != null)
                {
                    // 更新现有客户端
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = definition.ClientId,
                        ClientSecret = definition.ClientSecret,
                        DisplayName = definition.DisplayName
                    };

                    // 添加权限
                    foreach (var permission in GetPermissions(definition.GrantTypes, definition.Scopes))
                    {
                        descriptor.Permissions.Add(permission);
                    }

                    // 添加重定向URI
                    foreach (var uri in definition.RedirectUris)
                    {
                        descriptor.RedirectUris.Add(new Uri(uri));
                    }

                    // 添加登出后重定向URI
                    foreach (var uri in definition.PostLogoutRedirectUris)
                    {
                        descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
                    }

                    // 添加要求
                    foreach (var requirement in GetRequirements(definition.RequireConsent, definition.RequirePkce))
                    {
                        descriptor.Requirements.Add(requirement);
                    }

                    await _applicationManager.UpdateAsync(existingClient, descriptor);

                    _logger.LogInformation("更新客户端成功: {ClientId}", clientId);
                }
                else
                {
                    // 创建新客户端
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = definition.ClientId,
                        ClientSecret = definition.ClientSecret,
                        DisplayName = definition.DisplayName
                    };

                    // 添加权限
                    foreach (var permission in GetPermissions(definition.GrantTypes, definition.Scopes))
                    {
                        descriptor.Permissions.Add(permission);
                    }

                    // 添加重定向URI
                    foreach (var uri in definition.RedirectUris)
                    {
                        descriptor.RedirectUris.Add(new Uri(uri));
                    }

                    // 添加登出后重定向URI
                    foreach (var uri in definition.PostLogoutRedirectUris)
                    {
                        descriptor.PostLogoutRedirectUris.Add(new Uri(uri));
                    }

                    // 添加要求
                    foreach (var requirement in GetRequirements(definition.RequireConsent, definition.RequirePkce))
                    {
                        descriptor.Requirements.Add(requirement);
                    }

                    await _applicationManager.CreateAsync(descriptor);

                    _logger.LogInformation("创建客户端成功: {ClientId}", clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建或更新客户端失败: {ClientId}", clientId);
                throw;
            }
        }

        /// <summary>
        /// 删除客户端
        /// </summary>
        public async Task DeleteClientAsync(string clientId)
        {
            try
            {
                var client = await _applicationManager.FindByClientIdAsync(clientId);
                if (client != null)
                {
                    await _applicationManager.DeleteAsync(client);
                    _logger.LogInformation("删除客户端成功: {ClientId}", clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除客户端失败: {ClientId}", clientId);
                throw;
            }
        }

        /// <summary>
        /// 获取客户端信息
        /// </summary>
        public async Task<object?> GetClientAsync(string clientId)
        {
            try
            {
                var client = await _applicationManager.FindByClientIdAsync(clientId);
                if (client != null)
                {
                    return new
                    {
                        ClientId = await _applicationManager.GetClientIdAsync(client),
                        DisplayName = await _applicationManager.GetDisplayNameAsync(client),
                        Permissions = await _applicationManager.GetPermissionsAsync(client),
                        RedirectUris = await _applicationManager.GetRedirectUrisAsync(client),
                        PostLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(client)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取客户端信息失败: {ClientId}", clientId);
                throw;
            }
        }

        /// <summary>
        /// 获取客户端定义列表
        /// </summary>
        private static ClientDefinition[] GetClientDefinitions()
        {
            // 保持不变的代码
            return new[]
            {
                // 1. 机密Web客户端（授权码流程）
                new ClientDefinition
                {
                    ClientId = "web-client",
                    ClientSecret = "web-client-secret-key-2024",
                    DisplayName = "Web应用程序客户端",
                    Description = "传统的服务器端Web应用程序",
                    GrantTypes = new[] { GrantTypes.AuthorizationCode, GrantTypes.RefreshToken },
                    Scopes = new[] {
                        Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles,
                        "api1", "api1.read", "api1.write", "api1.users"
                    },
                    RedirectUris = new[] {
                        "https://localhost:7002/signin-oidc",
                        "https://localhost:7002/callback"
                    },
                    PostLogoutRedirectUris = new[] {
                        "https://localhost:7002/signout-callback-oidc",
                        "https://localhost:7002/"
                    },
                    RequireConsent = false,
                    RequirePkce = false
                },
                
                // 其余客户端定义保持不变...
            };
        }

        /// <summary>
        /// 根据授权类型和作用域获取权限列表
        /// </summary>
        private static HashSet<string> GetPermissions(string[] grantTypes, string[] scopes)
        {
            // 保持不变的代码
            var permissions = new HashSet<string>();

            // 端点权限
            permissions.Add(Permissions.Endpoints.Authorization);
            permissions.Add(Permissions.Endpoints.Token);
            permissions.Add(Permissions.Endpoints.Revocation);
            permissions.Add(Permissions.Endpoints.Introspection);

            // 授权类型权限
            foreach (var grantType in grantTypes)
            {
                switch (grantType)
                {
                    case GrantTypes.AuthorizationCode:
                        permissions.Add(Permissions.GrantTypes.AuthorizationCode);
                        break;
                    case GrantTypes.ClientCredentials:
                        permissions.Add(Permissions.GrantTypes.ClientCredentials);
                        break;
                    case GrantTypes.Password:
                        permissions.Add(Permissions.GrantTypes.Password);
                        break;
                    case GrantTypes.RefreshToken:
                        permissions.Add(Permissions.GrantTypes.RefreshToken);
                        break;
                    case GrantTypes.Implicit:
                        permissions.Add(Permissions.GrantTypes.Implicit);
                        break;
                }
            }

            // 作用域权限
            foreach (var scope in scopes)
            {
                permissions.Add(Permissions.Prefixes.Scope + scope);
            }

            // 响应类型权限
            if (grantTypes.Contains(GrantTypes.AuthorizationCode) || grantTypes.Contains(GrantTypes.Implicit))
            {
                permissions.Add(Permissions.ResponseTypes.Code);
            }

            if (grantTypes.Contains(GrantTypes.Implicit))
            {
                permissions.Add(Permissions.ResponseTypes.IdToken);
                permissions.Add(Permissions.ResponseTypes.Token);
            }

            return permissions;
        }

        /// <summary>
        /// 根据配置获取要求列表
        /// </summary>
        private static IEnumerable<string> GetRequirements(bool requireConsent, bool requirePkce)
        {
            // 保持不变的代码
            var requirements = new List<string>();

            if (requireConsent)
            {
                requirements.Add(Errors.ConsentRequired);
            }

            if (requirePkce)
            {
                requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
            }

            return requirements;
        }
    }
}
