using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BasicAuth.Models;
using BasicAuth.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置数据库上下文（使用内存数据库用于演示）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("BasicAuthDb"));

// 注册自定义服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// JWT配置
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.ASCII.GetBytes(secretKey!);

// 配置认证服务
builder.Services.AddAuthentication(options =>
{
    // 默认认证方案设置为Cookie认证
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    // Cookie认证配置
    options.LoginPath = "/auth/login";           // 登录页面路径
    options.LogoutPath = "/auth/logout";         // 登出页面路径
    options.AccessDeniedPath = "/auth/forbidden"; // 访问被拒绝页面
    options.ExpireTimeSpan = TimeSpan.FromHours(24); // Cookie过期时间
    options.SlidingExpiration = true;            // 启用滑动过期
    options.Cookie.HttpOnly = true;              // 防止XSS攻击
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTPS设置
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // JWT认证配置
    options.RequireHttpsMetadata = false; // 开发环境可设为false
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // 消除时钟偏差
    };
});

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    // 基于角色的策略
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // 基于声明的策略
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim("permission", "user.manage"));

    // 复合策略：需要特定角色AND特定声明
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("Admin")
              .RequireClaim("permission", "system.admin"));

    // 基于自定义要求的策略
    options.AddPolicy("MinimumAge", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));
});

// 注册自定义授权处理器
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, MinimumAgeHandler>();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 认证中间件必须在授权中间件之前
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.Initialize(context);
}

app.Run();