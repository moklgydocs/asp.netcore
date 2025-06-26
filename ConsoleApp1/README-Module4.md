# 模块4：单点登录(SSO)实现

本模块演示如何使用OpenIddict实现企业级单点登录系统，包括：
- 多个应用系统的统一认证
- 会话管理和单点登出
- 跨域认证支持
- 移动端和Web端统一登录
- 第三方系统集成

## 项目结构
```
SSODemo/
├── AuthServer/            # SSO认证中心
├── Portal/               # 企业门户网站
├── HRSystem/             # 人力资源系统
├── FinanceSystem/        # 财务系统
├── MobileAPI/            # 移动端API
└── AdminPanel/           # 管理后台
```

## SSO架构特点
- 统一用户身份管理
- 无缝应用切换体验
- 集中的会话控制
- 安全的跨域认证
- 支持多种客户端类型