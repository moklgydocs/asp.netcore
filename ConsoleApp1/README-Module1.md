# 模块1：ASP.NET Core 基础认证授权

本模块演示ASP.NET Core最基础的认证和授权机制，包括：
- Cookie认证
- 基于角色的授权
- 基于策略的授权
- JWT Token认证

## 项目结构
```
BasicAuth/
├── Controllers/
├── Models/
├── Services/
├── appsettings.json
├── Program.cs
└── BasicAuth.csproj
```

## 运行说明
1. 使用 `dotnet run` 启动项目
2. 访问 `/auth/login` 进行登录
3. 访问受保护的路由测试授权