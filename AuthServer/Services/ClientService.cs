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
                    await _applicationManager.UpdateAsync(existingClient, new OpenIddictApplicationDescriptor
                    {
                        ClientId = definition.ClientId,
                        ClientSecret = definition.ClientSecret,
                        DisplayName = definition.DisplayName,
                        Permissions = GetPermissions(definition.GrantTypes, definition.Scopes).ToHashSet(),
                        RedirectUris = definition.RedirectUris.Select(uri => new Uri(uri)).ToHashSet(),
                        PostLogoutRedirectUris = definition.PostLogoutRedirectUris.Select(uri => new Uri(uri)).ToHashSet(),
                        Requirements = GetRequirements(definition.RequireConsent, definition.RequirePkce).ToHashSet()
                    });

                    _logger.LogInformation("更新客户端成功: {ClientId}", clientId);
                }
                else
                {
                    // 创建新客户端
                    await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = definition.ClientId,
                        ClientSecret = definition.ClientSecret,
                        DisplayName = definition.DisplayName,
                        Permissions = GetPermissions(definition.GrantTypes, definition.Scopes).ToHashSet(),
                        RedirectUris = definition.RedirectUris.Select(uri => new Uri(uri)).ToHashSet(),
                        PostLogoutRedirectUris = definition.PostLogoutRedirectUris.Select(uri => new Uri(uri)).ToHashSet(),
                        Requirements = GetRequirements(definition.RequireConsent, definition.RequirePkce).ToHashSet()
                    });

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

                // 2. 公共SPA客户端（带PKCE的授权码流程）
                new ClientDefinition
                {
                    ClientId = "spa-client",
                    ClientSecret = null, // 公共客户端不需要密钥
                    DisplayName = "单页应用程序客户端",
                    Description = "JavaScript SPA应用程序（React/Vue/Angular）",
                    GrantTypes = new[] { GrantTypes.AuthorizationCode, GrantTypes.RefreshToken },
                    Scopes = new[] { 
                        Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles,
                        "api1", "api1.read" 
                    },
                    RedirectUris = new[] { 
                        "https://localhost:7003/callback",
                        "https://localhost:7003/silent-renew" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "https://localhost:7003/",
                        "https://localhost:7003/logout-callback" 
                    },
                    RequireConsent = false,
                    RequirePkce = true // SPA必须使用PKCE
                },

                // 3. 移动应用客户端
                new ClientDefinition
                {
                    ClientId = "mobile-client",
                    ClientSecret = null, // 移动应用是公共客户端
                    DisplayName = "移动应用程序客户端",
                    Description = "iOS/Android移动应用程序",
                    GrantTypes = new[] { GrantTypes.AuthorizationCode, GrantTypes.RefreshToken },
                    Scopes = new[] { 
                        Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles,
                        "api1", "api1.read" 
                    },
                    RedirectUris = new[] { 
                        "com.company.mobileapp://callback",
                        "https://localhost:7004/callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "com.company.mobileapp://logout",
                        "https://localhost:7004/" 
                    },
                    RequireConsent = false,
                    RequirePkce = true // 移动应用必须使用PKCE
                },

                // 4. 客户端凭据客户端（服务器到服务器）
                new ClientDefinition
                {
                    ClientId = "service-client",
                    ClientSecret = "service-client-secret-key-2024",
                    DisplayName = "服务客户端",
                    Description = "后台服务或API之间的通信",
                    GrantTypes = new[] { GrantTypes.ClientCredentials },
                    Scopes = new[] { "api1", "api1.read", "api1.write" },
                    RedirectUris = Array.Empty<string>(),
                    PostLogoutRedirectUris = Array.Empty<string>(),
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 5. 桌面应用客户端
                new ClientDefinition
                {
                    ClientId = "desktop-client",
                    ClientSecret = null, // 桌面应用是公共客户端
                    DisplayName = "桌面应用程序客户端",
                    Description = "Windows/macOS/Linux桌面应用程序",
                    GrantTypes = new[] { GrantTypes.AuthorizationCode, GrantTypes.RefreshToken },
                    Scopes = new[] { 
                        Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles,
                        "api1", "api1.read", "api1.write" 
                    },
                    RedirectUris = new[] { 
                        "http://localhost:8080/callback",
                        "com.company.desktopapp://callback" 
                    },
                    PostLogoutRedirectUris = new[] { 
                        "http://localhost:8080/",
                        "com.company.desktopapp://logout" 
                    },
                    RequireConsent = false,
                    RequirePkce = true // 桌面应用建议使用PKCE
                },

                // 6. 开发/测试客户端（密码流程 - 仅用于开发和测试）
                new ClientDefinition
                {
                    ClientId = "dev-client",
                    ClientSecret = "dev-client-secret-key-2024",
                    DisplayName = "开发测试客户端",
                    Description = "仅用于开发和测试环境的客户端",
                    GrantTypes = new[] { GrantTypes.Password, GrantTypes.RefreshToken },
                    Scopes = new[] { 
                        Scopes.OpenId, Scopes.Email, Scopes.Profile, Scopes.Roles,
                        "api1", "api1.read", "api1.write", "api1.users", "api1.admin" 
                    },
                    RedirectUris = Array.Empty<string>(),
                    PostLogoutRedirectUris = Array.Empty<string>(),
                    RequireConsent = false,
                    RequirePkce = false
                },

                // 7. API文档客户端（Swagger UI）
                new ClientDefinition
                {
                    ClientId = "swagger-client",
                    ClientSecret = null,
                    DisplayName = "API文档客户端",
                    Description = "Swagger UI用于API文档和测试",
                    GrantTypes = new[] { GrantTypes.AuthorizationCode },
                    Scopes = new[] { 
                        Scopes.OpenId, Scopes.Profile,
                        "api1", "api1.read", "api1.write" 
                    },
                    RedirectUris = new[] { 
                        "https://localhost:6002/swagger/oauth2-redirect.html",
                        "https://localhost:6003/swagger/oauth2-redirect.html" 
                    },
                    PostLogoutRedirectUris = Array.Empty<string>(),
                    RequireConsent = false,
                    RequirePkce = true
                }
            };
        }

        /// <summary>
        /// 根据授权类型和作用域获取权限列表
        /// </summary>
        private static IEnumerable<string> GetPermissions(string[] grantTypes, string[] scopes)
        {
            var permissions = new List<string>();

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
            var requirements = new List<string>();

            if (requireConsent)
            {
                requirements.Add(Requirements.Features.ConsentRequired);
            }

            if (requirePkce)
            {
                requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
            }

            return requirements;
        }
    }
}