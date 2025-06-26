# SSO单点登录系统使用说明

## 系统架构

### 核心组件
- **AuthServer** (端口5003): SSO认证中心
- **Portal** (端口7010): 企业门户网站
- **HRSystem** (端口7011): 人力资源系统
- **FinanceSystem** (端口7012): 财务系统
- **MobileAPI** (端口7013): 移动端API
- **AdminPanel** (端口7014): 管理后台

## 启动顺序

1. **启动SSO认证服务器**
   ```bash
   cd SSODemo/AuthServer
   dotnet run --urls="https://localhost:5003"
   ```

2. **启动企业门户**
   ```bash
   cd SSODemo/Portal
   dotnet run --urls="https://localhost:7010"
   ```

3. **启动其他业务系统**
   ```bash
   # HR系统
   cd SSODemo/HRSystem
   dotnet run --urls="https://localhost:7011"
   
   # 财务系统
   cd SSODemo/FinanceSystem
   dotnet run --urls="https://localhost:7012"
   
   # 管理后台
   cd SSODemo/AdminPanel
   dotnet run --urls="https://localhost:7014"
   ```

## 测试账户

### 超级管理员
- **用户名**: superadmin
- **密码**: SuperAdmin@2024!
- **权限**: 全系统访问权限

### 系统管理员
- **用户名**: admin
- **密码**: Admin@2024!
- **权限**: 用户管理、系统配置

### 人事经理
- **用户名**: hrmanager
- **密码**: HRManager@2024!
- **权限**: HR系统、员工管理

### 财务经理
- **用户名**: financemanager
- **密码**: FinanceManager@2024!
- **权限**: 财务系统、预算管理

### 普通员工
- **用户名**: employee1
- **密码**: Employee@2024!
- **权限**: 门户访问、个人信息

## SSO登录流程测试

### 1. 标准登录流程
1. 访问 `https://localhost:7010` (企业门户)
2. 系统自动重定向到SSO认证中心
3. 输入用户名密码登录
4. 登录成功后自动返回门户系统

### 2. 单点登录验证
1. 在门户系统登录后
2. 直接访问 `https://localhost:7011` (HR系统)
3. 应该无需再次登录，自动完成认证

### 3. 二维码登录（移动端）
1. 访问认证中心的二维码登录页面
2. 使用手机扫描二维码
3. 在手机上确认登录
4. PC端自动完成登录

### 4. 单点登出
1. 在任一系统点击登出
2. 所有已登录的系统都会自动登出

## API接口测试

### 获取访问令牌
```bash
# 密码流程（仅用于测试）
curl -X POST "https://localhost:5003/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&client_id=mobile-app&username=admin&password=Admin@2024!&scope=openid profile email roles portal hr finance user.read"

# 客户端凭据流程
curl -X POST "https://localhost:5003/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=service-client&client_secret=service-client-secret-2024-sso&scope=portal hr finance user.read"
```

### 获取用户信息
```bash
curl -X GET "https://localhost:5003/connect/userinfo" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 会话管理API
```bash
# 获取用户活跃会话
curl -X GET "https://localhost:5003/api/session/user/sessions" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"

# 结束特定会话
curl -X DELETE "https://localhost:5003/api/session/{sessionId}" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## 核心功能特性

### 1. 统一身份认证
- 所有系统共享同一套用户账户
- 支持多种认证方式（用户名密码、二维码、移动端）
- 集中的密码策略管理

### 2. 无缝单点登录
- 一次登录，全系统访问
- 自动令牌刷新
- 跨域认证支持

### 3. 会话管理
- 实时会话监控
- 并发会话控制
- 异常会话检测

### 4. 安全特性
- OAuth2/OpenID Connect标准
- PKCE防护（公共客户端）
- 令牌加密和签名
- 审计日志记录

### 5. 移动端支持
- 二维码登录
- 移动应用深度链接
- 离线访问支持

## 企业集成指南

### 1. 新系统接入
```csharp
// 在新系统中配置OpenID Connect
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "https://your-sso-server.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.ResponseType = OpenIdConnectResponseType.Code;
    // 配置所需的作用域
    options.Scope.Add("your-app-scope");
});
```

### 2. API保护
```csharp
// API资源配置
services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://your-sso-server.com";
        options.Audience = "your-api-resource";
    });
```

### 3. 权限控制
```csharp
// 基于角色的授权
[Authorize(Roles = "Admin,Manager")]
public IActionResult AdminPanel() { ... }

// 基于策略的授权
[Authorize(Policy = "RequireHRAccess")]
public IActionResult HRData() { ... }

// 基于作用域的授权
[Authorize(Policy = "RequireFinanceScope")]
public IActionResult FinanceReport() { ... }
```

## 部署和运维

### 1. 生产环境配置
- 使用真实SSL证书
- 配置Redis分布式缓存
- 设置数据库连接字符串
- 配置日志收集系统

### 2. 监控指标
- 登录成功率
- 令牌颁发量
- 系统响应时间
- 异常登录检测

### 3. 安全建议
- 定期更新密钥
- 监控审计日志
- 实施访问控制策略
- 定期安全评估

## 故障排除

### 常见问题
1. **重定向循环**: 检查客户端配置中的重定向URI
2. **令牌验证失败**: 验证时钟同步和证书配置
3. **跨域问题**: 确保CORS策略正确配置
4. **会话丢失**: 检查Redis连接和Cookie配置

### 日志查看
```bash
# 查看认证服务器日志
tail -f logs/sso-auth-*.txt

# 查看应用系统日志
tail -f logs/portal-*.txt
```