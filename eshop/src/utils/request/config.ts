/**
 * Axios 请求工具 - 配置文件
 *
 * 管理不同环境的配置
 */

import type { Environment, EnvConfig } from './types'

// ===========================
// 1. 环境配置
// ===========================

const envConfigs: Record<Environment, EnvConfig> = {
  // 开发环境
  development: {
    baseURL: '/api', // 开发环境使用代理
    timeout: 30000,
    withCredentials: true
  },

  // 测试环境
  test: {
    baseURL: 'https://test-api.example.com',
    timeout: 30000,
    withCredentials: true
  },

  // 生产环境
  production: {
    baseURL: 'https://api.example.com',
    timeout: 30000,
    withCredentials: true
  }
}

// ===========================
// 2. 获取当前环境
// ===========================

function getCurrentEnv(): Environment {
  // Vite 环境变量
  const mode = import.meta.env.MODE

  if (mode === 'production') {
    return 'production'
  } else if (mode === 'test') {
    return 'test'
  } else {
    return 'development'
  }
}

// ===========================
// 3. 导出当前环境配置
// ===========================

export const currentEnv = getCurrentEnv()
export const config = envConfigs[currentEnv]

// ===========================
// 4. 请求默认配置
// ===========================

export const defaultConfig = {
  // 基础配置
  ...config,

  // 请求头
  headers: {
    'Content-Type': 'application/json;charset=UTF-8'
  },

  // 自定义配置
  showLoading: true,
  showError: true,
  needToken: true,
  retryCount: 3,
  retryDelay: 1000,
  useCache: false,
  cacheTime: 5 * 60 * 1000 // 5分钟
}

// ===========================
// 5. Token 配置
// ===========================

export const tokenConfig = {
  // Token 存储的 key
  accessTokenKey: 'access_token',
  refreshTokenKey: 'refresh_token',

  // Token 请求头名称
  tokenHeaderKey: 'Authorization',

  // Token 前缀
  tokenPrefix: 'Bearer ',

  // 刷新 Token 的 API
  refreshTokenUrl: '/auth/refresh'
}

// ===========================
// 6. 状态码配置
// ===========================

export const statusCodeConfig = {
  // 成功状态码
  successCodes: [0, 200, 201],

  // 特殊处理的状态码
  specialCodes: {
    401: '未授权，请重新登录',
    403: '拒绝访问',
    404: '请求的资源不存在',
    500: '服务器内部错误',
    502: '网关错误',
    503: '服务不可用',
    504: '网关超时'
  }
}

// ===========================
// 7. 缓存配置
// ===========================

export const cacheConfig = {
  enabled: true,
  maxAge: 5 * 60 * 1000, // 5分钟
  maxSize: 100 // 最多缓存100个请求
}

// ===========================
// 8. 白名单配置
// ===========================

export const whitelistConfig = {
  // 不需要 token 的接口
  noTokenUrls: [
    '/auth/login',
    '/auth/register',
    '/auth/captcha',
    '/public/'
  ],

  // 不需要显示 loading 的接口
  noLoadingUrls: [
    '/polling/', // 轮询接口
    '/heartbeat' // 心跳接口
  ]
}

// ===========================
// 9. 导出所有配置
// ===========================

export default {
  currentEnv,
  config,
  defaultConfig,
  tokenConfig,
  statusCodeConfig,
  cacheConfig,
  whitelistConfig
}
