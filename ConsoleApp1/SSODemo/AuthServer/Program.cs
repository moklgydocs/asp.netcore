using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Serilog;
using AuthServer.Data;
using AuthServer.Models;
using AuthServer.Services;

var builder = WebApplication.CreateBuilder(args);

// 配置Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 添加服务
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 配置数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsDevelopment())
{
    // 开发环境使用内存数据库
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseInMemoryDatabase("SSOAuthDb");
        options.UseOpenIddict();
    });
}
else
{
    // 生产环境使用SQL Server
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
        options.UseOpenIddict();
    });
}

// 配置Redis缓存（用于会话管理）
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "SSOAuthServer";
    });
}
else
{
    builder.Services.AddMemoryCache();
}

// 配置ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // 密码策略
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    
    // 用户策略
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // 登录策略
    options.SignIn.RequireConfirmedEmail = false; // 生产环境应设为true
    options.SignIn.RequireConfirmedPhoneNumber = false;
    
    // 锁定策略
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 配置Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8小时过期
    options.SlidingExpiration = true;
    options.Cookie.Name = "SSOAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 配置OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
               
        // 配置缓存
        if (!builder.Environment.IsDevelopment())
        {
            options.UseDistributedCache();
        }
    })
    .AddServer(options =>
    {
        // 配置端点
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetLogoutEndpointUris("/connect/logout")
               .SetTokenEndpointUris("/connect/token")
               .SetUserinfoEndpointUris("/connect/userinfo")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetRevocationEndpointUris("/connect/revoke")
               .SetConfigurationEndpointUris("/.well-known/openid_configuration")
               .SetCryptographyEndpointUris("/.well-known/jwks");

        // 配置流程
        options.AllowAuthorizationCodeFlow()
               .AllowClientCredentialsFlow()
               .AllowRefreshTokenFlow()
               .AllowPasswordFlow(); // 仅用于移动端

        // 配置加密
        if (builder.Environment.IsDevelopment())
        {
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
        }
        else
        {
            // 生产环境配置真实证书
            // options.AddEncryptionCertificate(...)
            //        .AddSigningCertificate(...);
        }

        // 配置作用域
        options.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles,
            // 系统特定作用域
            "portal",           // 企业门户
            "hr",              // 人力资源系统
            "finance",         // 财务系统
            "mobile",          // 移动端
            "admin",           // 管理后台
            // 功能作用域
            "user.read",       // 用户信息读取
            "user.write",      // 用户信息写入
            "system.admin",    // 系统管理
            "offline_access"   // 离线访问
        );

        // 配置令牌
        options.SetAccessTokenLifetime(TimeSpan.FromHours(2))      // 访问令牌2小时
               .SetIdentityTokenLifetime(TimeSpan.FromMinutes(15))  // 身份令牌15分钟
               .SetRefreshTokenLifetime(TimeSpan.FromDays(30));     // 刷新令牌30天

        // 配置ASP.NET Core集成
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough()
               .EnableStatusCodePagesIntegration();

        // 配置发行者
        options.SetIssuer(new Uri(builder.Configuration["OpenIddict:Issuer"] ?? "https://localhost:5003"));
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// 配置授权
builder.Services.AddAuthorization(options =>
{
    // 系统管理员策略
    options.AddPolicy("SystemAdmin", policy =>
        policy.RequireRole("SystemAdmin", "SuperAdmin"));

    // 用户管理策略
    options.AddPolicy("UserAdmin", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("SystemAdmin") ||
            context.User.IsInRole("UserAdmin") ||
            context.User.HasClaim("permission", "user.manage")));

    // 应用管理策略
    options.AddPolicy("AppAdmin", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("SystemAdmin") ||
            context.User.HasClaim("permission", "app.manage")));
});

// 配置CORS（支持多个应用系统）
builder.Services.AddCors(options =>
{
    options.AddPolicy("SSOPolicy", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7010", // Portal
            "https://localhost:7011", // HR System
            "https://localhost:7012", // Finance System
            "https://localhost:7013", // Mobile API
            "https://localhost:7014"  // Admin Panel
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetPreflightMaxAge(TimeSpan.FromMinutes(30));
    });
});

// 注册业务服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();

// 配置健康检查
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// 配置HTTP管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("SSOPolicy");

// 认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 健康检查
app.MapHealthChecks("/health");

// 路由配置
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 初始化数据
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var clientService = services.GetRequiredService<IClientService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await SeedData.InitializeAsync(context, userManager, roleManager, clientService, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "初始化数据时发生错误");
    }
}

app.Run();