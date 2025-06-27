using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using ApiResource.Data;
using ApiResource.Services;

var builder = WebApplication.CreateBuilder(args);

// 配置Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 配置Swagger支持OpenIddict认证
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "受保护的API资源", 
        Version = "v1",
        Description = "使用OpenIddict保护的API资源演示"
    });

    // 添加OAuth2认证配置
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://localhost:5002/connect/authorize"),
                TokenUrl = new Uri("https://localhost:5002/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api1", "基础API访问权限" },
                    { "api1.read", "API读取权限" },
                    { "api1.write", "API写入权限" },
                    { "api1.users", "用户管理权限" },
                    { "api1.admin", "管理员权限" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "api1" }
        }
    });

    // 包含XML注释
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 配置数据库（演示用）
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseInMemoryDatabase("ApiResourceDb"));

// 配置OpenIddict验证
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // 设置授权服务器地址
        options.SetIssuer("https://localhost:5002/");
        
        // 配置受众
        options.AddAudiences("api1");
        
        // 使用本地验证（连接到同一个授权服务器）
        //options.UseLocalServer();
        
        // 注册ASP.NET Core宿主
        options.UseAspNetCore();
        
        // 使用DataProtection进行令牌保护
        options.UseDataProtection();
    });

// 配置认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    // 默认策略：需要认证
    options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // 基于作用域的策略
    options.AddPolicy("RequireApiScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("scope", "api1"));

    options.AddPolicy("RequireReadScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.HasClaim("scope", "api1") ||
                  context.User.HasClaim("scope", "api1.read") ||
                  context.User.HasClaim("scope", "api1.admin")));

    options.AddPolicy("RequireWriteScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.HasClaim("scope", "api1.write") ||
                  context.User.HasClaim("scope", "api1.admin")));

    options.AddPolicy("RequireUserScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.HasClaim("scope", "api1.users") ||
                  context.User.HasClaim("scope", "api1.admin")));

    options.AddPolicy("RequireAdminScope", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("scope", "api1.admin"));

    // 基于角色的策略
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin"));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.IsInRole("Admin") ||
                  context.User.IsInRole("Manager")));

    // 基于权限的策略
    options.AddPolicy("UserManagement", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("permission", "user.manage"));
});

// 配置CORS
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

// 注册业务服务
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Resource v1");
        c.OAuthClientId("swagger-client");
        c.OAuthAppName("API Resource Swagger UI");
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

// 启用CORS
app.UseCors("AllowSpecificOrigins");

// 认证和授权
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 初始化示例数据
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    await SeedData.InitializeAsync(context);
}

app.Run();