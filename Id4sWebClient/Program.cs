using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel;
using WebClient.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

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
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // Identity Server配置
    options.Authority = "https://localhost:5001";
    options.RequireHttpsMetadata = false; // 开发环境可设为false

    // 客户端配置
    options.ClientId = "web-client";
    options.ClientSecret = "web-client-secret";

    // 使用授权码流程
    options.ResponseType = OpenIdConnectResponseType.Code;

    // 请求的作用域
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("permissions");
    options.Scope.Add("api1");
    options.Scope.Add("api1.read");
    options.Scope.Add("api1.write");
    options.Scope.Add("api1.users");

    // 启用离线访问（刷新令牌）
    options.Scope.Add("offline_access");

    // 保存令牌到认证属性中
    options.SaveTokens = true;

    // 获取用户信息端点的声明
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
    options.ClaimActions.MapJsonKey("permission", "permission", "permission");

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
            // 令牌验证成功后的处理
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("用户 {User} 令牌验证成功", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // 认证失败后的处理
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "OpenID Connect认证失败");
            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            // 远程认证失败后的处理
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Failure, "远程认证失败");
            context.Response.Redirect("/Home/Error");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    };
});

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Admin", "Manager"));

    options.AddPolicy("UserManagement", policy =>
        policy.RequireClaim("permission", "user.manage"));
});

// 注册HTTP客户端
builder.Services.AddHttpClient();

// 注册API服务
builder.Services.AddScoped<IApiService, ApiService>();

// 配置访问令牌管理
builder.Services.AddAccessTokenManagement(options =>
{
    // 令牌管理选项
    options.User.RefreshBeforeExpiration = TimeSpan.FromMinutes(5);
})
.ConfigureBackchannelHttpClient();

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

// 认证和授权中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();