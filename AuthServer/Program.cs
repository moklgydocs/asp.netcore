using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using AuthServer.Data;
using AuthServer.Models;
using AuthServer.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置Serilog日志
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 添加服务
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 配置Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 使用内存数据库进行演示（生产环境请使用SQL Server等）
    options.UseInMemoryDatabase("OpenIddictDb");
    
    // 集成OpenIddict Entity Framework stores
    options.UseOpenIddict();
});

// 配置ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // 密码策略
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    
    // 用户设置
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // 登录设置
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    
    // 锁定设置
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 配置OpenIddict
builder.Services.AddOpenIddict()
    // 注册OpenIddict核心服务
    .AddCore(options =>
    {
        // 配置Entity Framework stores
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    
    // 注册OpenIddict服务器服务
    .AddServer(options =>
    {
        // 启用授权、登出、令牌和用户信息端点
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetLogoutEndpointUris("/connect/logout")
               .SetTokenEndpointUris("/connect/token")
               .SetUserinfoEndpointUris("/connect/userinfo")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetRevocationEndpointUris("/connect/revoke");

        // 标记以下授权流程为有效
        options.AllowAuthorizationCodeFlow()    // 授权码流程
               .AllowClientCredentialsFlow()    // 客户端凭据流程
               .AllowRefreshTokenFlow()         // 刷新令牌流程
               .AllowPasswordFlow()             // 密码流程（不推荐，仅演示用）
               .AllowImplicitFlow();            // 隐式流程（不推荐，仅演示用）

        // 注册签名和加密凭据
        if (builder.Environment.IsDevelopment())
        {
            // 开发环境使用临时加密和签名凭据
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
        }
        else
        {
            // 生产环境应该使用真实的证书
            // options.AddEncryptionCertificate(certificate)
            //        .AddSigningCertificate(certificate);
        }

        // 注册作用域
        options.RegisterScopes(
            OpenIddictConstants.Scopes.OpenId,      // openid
            OpenIddictConstants.Scopes.Email,       // email
            OpenIddictConstants.Scopes.Profile,     // profile
            OpenIddictConstants.Scopes.Roles,       // roles
            "api1",                                 // 自定义API作用域
            "api1.read",                           // API读取作用域
            "api1.write",                          // API写入作用域
            "api1.users",                          // 用户管理作用域
            "api1.admin"                           // 管理员作用域
        );

        // 配置ASP.NET Core宿主
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()    // 启用授权端点透传
               .EnableLogoutEndpointPassthrough()           // 启用登出端点透传
               .EnableTokenEndpointPassthrough()            // 启用令牌端点透传
               .EnableUserinfoEndpointPassthrough()         // 启用用户信息端点透传
               .EnableStatusCodePagesIntegration();         // 启用状态码页面集成

        // 配置令牌格式
        options.SetAccessTokenLifetime(TimeSpan.FromHours(1))      // 访问令牌1小时
               .SetIdentityTokenLifetime(TimeSpan.FromMinutes(15))  // 身份令牌15分钟
               .SetRefreshTokenLifetime(TimeSpan.FromDays(30));     // 刷新令牌30天

        // 配置CORS
        options.SetIssuer(new Uri("https://localhost:5002"));
    })
    
    // 注册OpenIddict验证服务
    .AddValidation(options =>
    {
        // 导入服务器配置
        options.UseLocalServer();
        
        // 注册ASP.NET Core宿主
        options.UseAspNetCore();
    });

// 注册自定义服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();

// 配置认证
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// 配置授权
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Admin", "Manager"));
});

// 添加CORS支持
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7002", // Web客户端
            "https://localhost:7003", // SPA客户端
            "https://localhost:7004"  // 移动客户端
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// 添加Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOrigins");

// 认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 路由配置
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var clientService = services.GetRequiredService<IClientService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await SeedData.InitializeAsync(context, userManager, roleManager, clientService, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "数据库初始化时发生错误");
    }
}

app.Run();