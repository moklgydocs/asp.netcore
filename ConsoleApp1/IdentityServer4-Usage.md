# Identity Server 4 模块使用说明

## 项目启动顺序

1. **首先启动Identity Server**
   ```bash
   cd IdentityServer4Demo/IdentityServer
   dotnet run --urls="https://localhost:5001"
   ```

2. **启动API资源**
   ```bash
   cd IdentityServer4Demo/ApiResource  
   dotnet run --urls="https://localhost:6001"
   ```

3. **启动Web客户端**
   ```bash
   cd IdentityServer4Demo/WebClient
   dotnet run --urls="https://localhost:7001"
   ```

## 测试流程

### 1. Web客户端测试
- 访问 `https://localhost:7001`
- 点击登录，会重定向到Identity Server
- 使用测试账户登录（admin/Admin123!）
- 登录成功后返回Web客户端
- 测试各种API调用功能

### 2. 直接API测试
使用Postman或curl测试API：

```bash
# 1. 获取访问令牌（客户端凭据流程）
curl -X POST "https://localhost:5001/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=client-credentials-client&client_secret=client-credentials-secret&scope=api1"

# 2. 使用访问令牌调用API
curl -X GET "https://localhost:6001/api/WeatherForecast" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 3. 用户密码流程测试
```bash
# 获取用户访问令牌
curl -X POST "https://localhost:5001/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&client_id=password-client&client_secret=password-client-secret&username=admin&password=Admin123!&scope=openid profile api1 api1.users"
```

## 核心概念解释

### 1. OAuth2/OpenID Connect流程
- **授权码流程**: 最安全的流程，适用于Web应用
- **客户端凭据流程**: 用于服务器间通信
- **密码流程**: 不推荐，仅在特殊情况使用
- **混合流程**: 结合授权码和隐式流程

### 2. 令牌类型
- **访问令牌**: 用于访问API资源
- **身份令牌**: 包含用户身份信息
- **刷新令牌**: 用于获取新的访问令牌

### 3. 作用域(Scopes)
- **openid**: OpenID Connect必需的作用域
- **profile**: 用户档案信息
- **api1**: 基础API访问权限
- **api1.read**: API读取权限
- **api1.write**: API写入权限
- **api1.users**: 用户管理权限

### 4. 客户端类型
- **机密客户端**: 有客户端密钥，如Web应用
- **公共客户端**: 无客户端密钥，如SPA、移动应用

## 安全最佳实践

1. **生产环境配置**
   - 使用真实的SSL证书
   - 配置强密钥和密码
   - 启用HTTPS元数据验证
   - 使用真实的数据库存储

2. **令牌管理**
   - 设置合理的令牌生命周期
   - 实现令牌刷新机制
   - 安全存储刷新令牌

3. **客户端安全**
   - 验证重定向URI
   - 使用PKCE（针对公共客户端）
   - 实现适当的CORS策略

4. **监控和日志**
   - 记录所有认证事件
   - 监控失败的登录尝试
   - 实现安全审计

## 常见问题排查

1. **重定向循环**
   - 检查客户端配置中的重定向URI
   - 确保Identity Server和客户端的URL配置正确

2. **令牌验证失败**
   - 检查时钟同步
   - 验证颁发者和受众配置
   - 确保签名密钥正确

3. **作用域权限问题**
   - 检查客户端请求的作用域
   - 验证API资源配置
   - 确保用户有相应权限