# 模块3：OpenIddict 集成

本模块演示如何使用OpenIddict作为现代化的OpenID Connect服务器替代方案，包括：
- OpenIddict 认证服务器搭建
- Entity Framework集成
- 多种OAuth2/OpenID Connect流程
- API资源保护
- 客户端应用集成

## 项目结构
```
OpenIddictDemo/
├── AuthServer/             # OpenIddict认证服务器
├── ApiResource/           # 受保护的API资源
├── WebClient/             # Web客户端应用
├── SpaClient/             # SPA客户端（React/Vue等）
└── MobileClient/          # 移动客户端示例
```

## OpenIddict优势
- 免费开源，MIT许可证
- 原生支持Entity Framework
- 现代化的设计和API
- 优秀的性能和安全性
- 活跃的社区支持