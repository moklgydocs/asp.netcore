using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 配置Swagger以支持JWT Bearer认证
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "受保护的API资源", Version = "v1" });
    
    // 添加JWT Bearer认证配置
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// 配置Identity Server 4身份验证
builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        // Identity Server的基地址
        options.Authority = "https://localhost:5001";
        
        // API资源名称（必须与Identity Server中配置的ApiResource名称匹配）
        options.ApiName = "api1";
        
        // 是否要求HTTPS（开发环境可以设为false）
        options.RequireHttpsMetadata = false;
        
        // 是否启用缓存
        options.EnableCaching = true;
        options.CacheDuration = TimeSpan.FromMinutes(10);
        
        // 声明映射
        options.ClaimTypeMap.Clear();
        options.ClaimTypeMap.Add("role", "role");
        options.ClaimTypeMap.Add("permission", "permission");
        
        // JWT选项配置
        options.JwtValidationClockSkew = TimeSpan.FromMinutes(5);
        
        // 保存令牌以便在控制器中访问
        options.SaveToken = true;
    });

// 配置授权策略
builder.Services.AddAuthorization(options =>
{
    // 默认策略：需要认证
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // 基于作用域的策略
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });

    // 读取权限策略
    options.AddPolicy("ReadAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("scope", "api1") ||
            context.User.HasClaim("scope", "api1.read") ||
            context.User.HasClaim("scope", "api1.full"));
    });

    // 写入权限策略
    options.AddPolicy("WriteAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("scope", "api1.write") ||
            context.User.HasClaim("scope", "api1.full"));
    });

    // 用户管理权限策略
    options.AddPolicy("UserManagement", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
            context.User.HasClaim("scope", "api1.users") ||
            context.User.HasClaim("scope", "api1.full"));
    });

    // 管理员角色策略
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });

    // 权限声明策略
    options.AddPolicy("UserManagePermission", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("permission", "user.manage");
    });
});

// 添加CORS支持
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7001", // Web客户端
            "https://localhost:7002", // SPA客户端
            "https://localhost:7003"  // 混合客户端
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// 添加HTTP客户端服务
builder.Services.AddHttpClient();

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
    });
}

app.UseHttpsRedirection();

// 启用CORS
app.UseCors("AllowSpecificOrigins");

// 认证中间件必须在授权之前
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();