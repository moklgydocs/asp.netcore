# 模块2：Identity Server 4 集成

本模块演示如何集成Identity Server 4作为独立的认证授权服务器，包括：
- Identity Server 4 认证服务器搭建
- API资源保护
- 客户端应用集成
- 多种OAuth2/OpenID Connect流程

## 项目结构
```
IdentityServer4Demo/
├── IdentityServer/          # 认证服务器
├── ApiResource/            # 受保护的API资源
├── WebClient/              # Web客户端应用
└── ConsoleClient/          # 控制台客户端
```

## 注意事项
Identity Server 4已于2022年停止免费使用，本示例仅用于学习目的。
生产环境建议使用OpenIddict或Duende Identity Server。