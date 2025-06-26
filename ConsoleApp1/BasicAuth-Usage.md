# 基础认证模块使用说明

## 测试账户
- **管理员**: admin / admin123
- **经理**: manager / manager123  
- **普通用户**: user / user123
- **未成年用户**: younguser / young123 (年龄16岁，用于测试年龄限制)

## API端点测试

### 1. Cookie认证登录
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123",
    "rememberMe": true
  }'
```

### 2. JWT认证登录
```bash
curl -X POST "https://localhost:7000/api/auth/login-jwt" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin", 
    "password": "admin123"
  }'
```

### 3. 获取当前用户信息
```bash
# Cookie认证
curl -X GET "https://localhost:7000/api/auth/me" \
  -H "Cookie: .AspNetCore.Cookies=你的cookie值"

# JWT认证
curl -X GET "https://localhost:7000/api/auth/me" \
  -H "Authorization: Bearer 你的JWT_TOKEN"
```

### 4. 测试授权策略
```bash
# 管理员专属内容
curl -X GET "https://localhost:7000/api/users/admin-only" \
  -H "Authorization: Bearer 你的JWT_TOKEN"

# 年龄限制内容（使用未成年用户测试）
curl -X GET "https://localhost:7000/api/users/adult-content" \
  -H "Authorization: Bearer 未成年用户的JWT_TOKEN"
```

## 核心概念解释

### 1. 认证方案
- **Cookie认证**: 适用于传统Web应用，状态保存在服务器端
- **JWT认证**: 适用于API和SPA，无状态认证

### 2. 授权策略类型
- **基于角色**: `[Authorize(Roles = "Admin")]`
- **基于策略**: `[Authorize(Policy = "AdminOnly")]`
- **基于声明**: 检查用户的特定声明值
- **自定义要求**: 实现复杂的授权逻辑

### 3. 声明(Claims)系统
- **NameIdentifier**: 用户唯一标识
- **Name**: 用户名
- **Role**: 用户角色
- **Email**: 邮箱
- **自定义声明**: 如权限、年龄等

## 最佳实践
1. 生产环境中使用HTTPS
2. JWT密钥要足够复杂且定期更换
3. 实现Token刷新机制
4. 记录安全相关的日志
5. 对敏感操作进行额外验证