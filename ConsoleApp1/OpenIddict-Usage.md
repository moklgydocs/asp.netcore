# OpenIddict 模块使用说明

## 项目启动顺序

1. **启动OpenIddict认证服务器**
   ```bash
   cd OpenIddictDemo/AuthServer
   dotnet run --urls="https://localhost:5002"
   ```

2. **启动API资源服务**
   ```bash
   cd OpenIddictDemo/ApiResource
   dotnet run --urls="https://localhost:6002"
   ```

3. **启动Web客户端（后续创建）**
   ```bash
   cd OpenIddictDemo/WebClient
   dotnet run --urls="https://localhost:7002"
   ```

## 测试账户

### 管理员账户
- **用户名**: superadmin
- **密码**: SuperAdmin123!
- **角色**: SuperAdmin, Admin
- **权限**: system.full, user.manage, client.manage, audit.view, settings.manage

### 普通管理员
- **用户名**: admin
- **密码**: Admin123!
- **角色**: Admin
- **权限**: user.manage, api.full, data.read, data.write

### 部门经理
- **用户名**: manager
- **密码**: Manager123!
- **角色**: Manager
- **权限**: team.manage, report.view, data.read, api.read

### 开发者
- **用户名**: developer
- **密码**: Developer123!
- **角色**: Developer
- **权限**: code.write, debug.access, api.test, data.read, data.write

### 普通用户
- **用户名**: user
- **密码**: User123!
- **角色**: User
- **权限**: data.read, profile.edit

## API端点测试

### 1. 客户端凭据流程
```bash
# 获取客户端访问令牌
curl -X POST "https://localhost:5002/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=service-client&client_secret=service-client-secret-key-2024&scope=api1"
```

### 2. 密码流程（仅用于测试）
```bash
# 获取用户访问令牌
curl -X POST "https://localhost:5002/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&client_id=dev-client&client_secret=dev-client-secret-key-2024&username=admin&password=Admin123!&scope=openid profile email api1 api1.read api1.write api1.users"
```

### 3. 调用受保护的API
```bash
# 获取产品列表
curl -X GET "https://localhost:6002/api/products" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"

# 创建产品（需要写入权限）
curl -X POST "https://localhost:6002/api/products" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "新产品",
    "description": "产品描述",
    "price": 199.99,
    "category": "测试分类",
    "stock": 100
  }'
```

### 4. 获取用户信息
```bash
curl -X GET "https://localhost:5002/connect/userinfo" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## OpenIddict vs Identity Server 4 对比

### 优势
1. **免费开源**: MIT许可证，无商业限制
2. **现代化**: 基于最新的.NET技术栈
3. **Entity Framework集成**: 原生支持EF Core
4. **性能优异**: 更好的性能和资源利用率
5. **活跃维护**: 社区活跃，持续更新

### 配置差异
1. **更简洁的配置**: 相比IS4，配置更直观
2. **内置数据存储**: 无需额外的存储配置包
3. **灵活的端点**: 可自定义端点行为
4. **更好的ASP.NET Core集成**: 原生集成体验

### 功能特性
1. **支持所有OAuth2/OIDC流程**: 
   - 授权码流程 (推荐)
   - 客户端凭据流程
   - 密码流程 (不推荐)
   - 隐式流程 (已弃用)
   - 混合流程

2. **安全特性**:
   - PKCE支持
   - 令牌加密
   - 令牌内省
   - 令牌撤销

3. **扩展性**:
   - 自定义声明
   - 自定义端点
   - 事件处理
   - 自定义存储

## 生产环境配置建议

### 1. 数据库配置
```csharp
// 使用SQL Server
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});
```

### 2. 证书配置
```csharp
// 生产环境使用真实证书
if (builder.Environment.IsProduction())
{
    options.AddEncryptionCertificate(encryptionCertificate)
           .AddSigningCertificate(signingCertificate);
}
```

### 3. 安全配置
```csharp
// 启用HTTPS要求
options.RequireHttpsMetadata = true;

// 配置CORS
options.AddCors(policy =>
{
    policy.WithOrigins("https://your-client-domain.com")
          .AllowAnyHeader()
          .AllowAnyMethod();
});
```

### 4. 监控和日志
```csharp
// 配置Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.File("logs/authserver-.txt", rollingInterval: RollingInterval.Day));
```

## 常见问题解决

### 1. CORS问题
确保在认证服务器和API资源中都正确配置了CORS策略。

### 2. 证书问题
开发环境使用`AddDevelopmentSigningCredential()`，生产环境必须使用真实证书。

### 3. 作用域权限
检查客户端配置的作用域是否与API资源要求的作用域匹配。

### 4. 令牌生命周期
根据业务需求合理设置访问令牌和刷新令牌的生命周期。