using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    /// <summary>
    /// Identity Server 4 配置类
    /// 定义身份资源、API资源、API作用域和客户端
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 身份资源配置
        /// 定义用户身份信息的标准声明
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                // 标准OpenID Connect身份资源
                new IdentityResources.OpenId(),      // subject id
                new IdentityResources.Profile(),     // 用户档案信息
                new IdentityResources.Email(),       // 邮箱
                new IdentityResources.Phone(),       // 电话
                new IdentityResources.Address(),     // 地址
                
                // 自定义身份资源
                new IdentityResource(
                    name: "roles",
                    displayName: "用户角色",
                    claimTypes: new[] { "role" }
                ),
                new IdentityResource(
                    name: "permissions",
                    displayName: "用户权限", 
                    claimTypes: new[] { "permission" }
                )
            };

        /// <summary>
        /// API作用域配置
        /// 定义API访问的细粒度权限
        /// </summary>
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                // 基础API作用域
                new ApiScope("api1", "基础API访问权限")
                {
                    Description = "允许访问基础API功能",
                    UserClaims = { "role", "permission" }
                },
                
                // 用户管理API作用域
                new ApiScope("api1.users", "用户管理API")
                {
                    Description = "允许管理用户相关操作",
                    UserClaims = { "role", "permission" }
                },
                
                // 数据读取作用域
                new ApiScope("api1.read", "数据读取权限")
                {
                    Description = "允许读取数据",
                    UserClaims = { "role" }
                },
                
                // 数据写入作用域
                new ApiScope("api1.write", "数据写入权限")
                {
                    Description = "允许写入和修改数据",
                    UserClaims = { "role", "permission" }
                },
                
                // 完整访问权限
                new ApiScope("api1.full", "完整API访问权限")
                {
                    Description = "允许完整的API访问权限",
                    UserClaims = { "role", "permission", "email" }
                }
            };

        /// <summary>
        /// API资源配置
        /// 定义受保护的API资源
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("api1", "演示API资源")
                {
                    Description = "用于演示的API资源",
                    
                    // 关联的作用域
                    Scopes = { "api1", "api1.users", "api1.read", "api1.write", "api1.full" },
                    
                    // 包含在访问令牌中的用户声明
                    UserClaims = 
                    {
                        "role",
                        "permission", 
                        "email",
                        "name",
                        "given_name",
                        "family_name"
                    },
                    
                    // API密钥（用于API资源验证）
                    ApiSecrets = { new Secret("api1-secret".Sha256()) }
                }
            };

        /// <summary>
        /// 客户端配置
        /// 定义可以访问身份服务器的客户端应用程序
        /// </summary>
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // 1. 客户端凭据流程（Client Credentials Flow）
                // 适用于服务器到服务器的通信，没有用户上下文
                new Client
                {
                    ClientId = "client-credentials-client",
                    ClientName = "客户端凭据流程客户端",
                    Description = "用于服务器间通信的客户端",
                    
                    // 授权类型：客户端凭据
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    
                    // 客户端密钥
                    ClientSecrets = { new Secret("client-credentials-secret".Sha256()) },
                    
                    // 允许访问的作用域
                    AllowedScopes = { "api1", "api1.read" },
                    
                    // 访问令牌生命周期（秒）
                    AccessTokenLifetime = 3600,
                    
                    // 是否允许离线访问（刷新令牌）
                    AllowOfflineAccess = false
                },

                // 2. 授权码流程（Authorization Code Flow）
                // 适用于Web应用程序
                new Client
                {
                    ClientId = "web-client",
                    ClientName = "Web应用程序客户端",
                    Description = "传统Web应用程序客户端",
                    
                    // 授权类型：授权码
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    // 客户端密钥
                    ClientSecrets = { new Secret("web-client-secret".Sha256()) },
                    
                    // 重定向URI（授权后重定向）
                    RedirectUris = { "https://localhost:7001/signin-oidc" },
                    
                    // 登出后重定向URI
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },
                    
                    // 允许访问的作用域
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "permissions",
                        "api1",
                        "api1.users",
                        "api1.read",
                        "api1.write"
                    },
                    
                    // 是否允许离线访问（刷新令牌）
                    AllowOfflineAccess = true,
                    
                    // 访问令牌生命周期
                    AccessTokenLifetime = 3600,
                    
                    // 身份令牌生命周期
                    IdentityTokenLifetime = 300,
                    
                    // 刷新令牌设置
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 86400, // 24小时
                    
                    // 是否需要同意页面
                    RequireConsent = false,
                    
                    // 是否包含JWT ID
                    AlwaysIncludeUserClaimsInIdToken = true
                },

                // 3. PKCE授权码流程（适用于SPA和移动应用）
                new Client
                {
                    ClientId = "spa-client",
                    ClientName = "单页应用程序客户端",
                    Description = "JavaScript SPA应用程序",
                    
                    // 授权类型：带PKCE的授权码
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    // 不需要客户端密钥（公共客户端）
                    RequireClientSecret = false,
                    
                    // 需要PKCE
                    RequirePkce = true,
                    
                    // 重定向URI
                    RedirectUris = { "https://localhost:7002/callback" },
                    
                    // 登出重定向URI
                    PostLogoutRedirectUris = { "https://localhost:7002/" },
                    
                    // 允许的CORS源
                    AllowedCorsOrigins = { "https://localhost:7002" },
                    
                    // 允许访问的作用域
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api1",
                        "api1.read"
                    },
                    
                    // 访问令牌类型
                    AccessTokenType = AccessTokenType.Jwt,
                    
                    // 令牌生命周期
                    AccessTokenLifetime = 3600,
                    IdentityTokenLifetime = 300,
                    
                    // 是否允许离线访问
                    AllowOfflineAccess = true,
                    
                    // 不需要同意页面
                    RequireConsent = false
                },

                // 4. 资源所有者密码流程（Resource Owner Password Credentials）
                // 注意：此流程不推荐使用，仅在特殊情况下使用
                new Client
                {
                    ClientId = "password-client",
                    ClientName = "密码流程客户端",
                    Description = "资源所有者密码凭据流程（不推荐）",
                    
                    // 授权类型：资源所有者密码
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    
                    // 客户端密钥
                    ClientSecrets = { new Secret("password-client-secret".Sha256()) },
                    
                    // 允许访问的作用域
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        "api1.read",
                        "api1.write"
                    },
                    
                    // 是否允许离线访问
                    AllowOfflineAccess = true,
                    
                    // 访问令牌生命周期
                    AccessTokenLifetime = 3600,
                    
                    // 刷新令牌设置
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 86400
                },

                // 5. 混合流程（Hybrid Flow）
                new Client
                {
                    ClientId = "hybrid-client",
                    ClientName = "混合流程客户端",
                    Description = "混合流程客户端（授权码+隐式）",
                    
                    // 授权类型：混合流程
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    
                    // 客户端密钥
                    ClientSecrets = { new Secret("hybrid-client-secret".Sha256()) },
                    
                    // 重定向URI
                    RedirectUris = { "https://localhost:7003/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7003/signout-callback-oidc" },
                    
                    // 允许访问的作用域
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "permissions",
                        "api1.full"
                    },
                    
                    // 响应类型
                    AllowedResponseTypes = { "code id_token" },
                    
                    // 是否允许离线访问
                    AllowOfflineAccess = true,
                    
                    // 不需要同意页面
                    RequireConsent = false,
                    
                    // 访问令牌生命周期
                    AccessTokenLifetime = 3600,
                    IdentityTokenLifetime = 300
                }
            };
    }
}