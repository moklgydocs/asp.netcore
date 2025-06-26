using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel;
using Portal.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 添加服务
builder.Services.AddControllersWithViews();

// 配置认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "Portal.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Home/AccessDenied";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // SSO认证服务器配置
    options.Authority = "https://localhost:5003";
    options.RequireHttpsMetadata = false; // 开发环境设置
    
    // 客户端配置
    options.ClientId = "portal-web";
    options.ClientSecret = "portal-web-secret-2024-sso";
    
    // 使用授权码流程
    options.ResponseType = OpenIdConnectResponseType.Code;
    
    // 请求的作用域
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("portal");
    options.Scope.Add("user.read");
    options.Scope.Add("offline_access");
    
    // 保存令牌
    options.SaveTokens = true;
    
    // 获取用户信息
    options.GetClaimsFromUserInfoEndpoint = true;
    
    // 声明映射
    options.ClaimActions.DeleteClaim("amr");
    options.ClaimActions.DeleteClaim("s_hash");
    options.ClaimActions.DeleteClaim("sid");
    options.ClaimActions.DeleteClaim("idp");
    options.ClaimActions.DeleteClaim("aud");
    options.ClaimActions.DeleteClaim("azp");
    options.ClaimActions.DeleteClaim("at_hash");
    
    // 映射自定义声明
    options.ClaimActions.MapJsonKey("role", "role", "role");
    options.ClaimActions.MapJsonKey("department", "department", "department");
    options.ClaimActions.MapJsonKey("position", "position", "position");
    
    // 令牌验证参数
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = JwtClaimTypes.Name,
        RoleClaimType = JwtClaimTypes.Role,
    };
    
    // 事件处理
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("用户 {User} 令牌验证成功", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "OpenID Connect认证失败");
            return Task.CompletedTask;
        },
        OnRemoteSignOut = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("收到远程登出信号");
            return Task.CompletedTask;
        }
    };
});

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    // 默认策略：需要认证
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // 员工策略
    options.AddPolicy("Employee", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.IsInRole("Employee") ||
                  context.User.IsInRole("HRStaff") ||
                  context.User.IsInRole("HRManager") ||
                  context.User.IsInRole("FinanceStaff") ||
                  context.User.IsInRole("FinanceManager") ||
                  context.User.IsInRole("SystemAdmin") ||
                  context.User.IsInRole("SuperAdmin")));

    // 管理员策略
    options.AddPolicy("Manager", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.IsInRole("HRManager") ||
                  context.User.IsInRole("FinanceManager") ||
                  context.User.IsInRole("SystemAdmin") ||
                  context.User.IsInRole("SuperAdmin")));
});

// 注册服务
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// 配置访问令牌管理
builder.Services.AddAccessTokenManagement(options =>
{
    options.RefreshBeforeExpiration = TimeSpan.FromMinutes(5);
})
.ConfigureBackchannelHttpClient()
.AddTransientHttpErrorPolicy(policy => 
    policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

var app = builder.Build();

// 配置HTTP请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 认证和授权
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();