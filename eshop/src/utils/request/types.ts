/**
 * Axios 请求工具 - 类型定义
 *
 * 定义所有相关的 TypeScript 类型
 */

import type { AxiosRequestConfig, AxiosResponse } from 'axios'

// ===========================
// 1. 通用响应接口
// ===========================

/**
 * 后端统一响应格式
 */
export interface ApiResponse<T = any> {
  code: number
  data: T
  message: string
  timestamp?: number
}

/**
 * 分页响应
 */
export interface PageResponse<T = any> {
  list: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

// ===========================
// 2. 请求配置扩展
// ===========================

/**
 * 自定义请求配置
 */
export interface RequestConfig extends AxiosRequestConfig {
  // 是否显示 loading
  showLoading?: boolean

  // 是否显示错误提示
  showError?: boolean

  // 是否需要 token
  needToken?: boolean

  // 重试次数
  retryCount?: number

  // 重试延迟（毫秒）
  retryDelay?: number

  // 是否使用缓存
  useCache?: boolean

  // 缓存时间（毫秒）
  cacheTime?: number

  // 自定义错误处理
  customErrorHandler?: (error: any) => void
}

// ===========================
// 3. 错误类型
// ===========================

/**
 * 自定义错误类
 */
export class ApiError extends Error {
  code: number
  response?: AxiosResponse

  constructor(message: string, code: number, response?: AxiosResponse) {
    super(message)
    this.name = 'ApiError'
    this.code = code
    this.response = response
  }
}

// ===========================
// 4. 拦截器配置
// ===========================

export interface InterceptorConfig {
  // 请求拦截器
  onRequest?: (config: RequestConfig) => RequestConfig | Promise<RequestConfig>
  onRequestError?: (error: any) => any

  // 响应拦截器
  onResponse?: (response: AxiosResponse) => AxiosResponse | Promise<AxiosResponse>
  onResponseError?: (error: any) => any
}

// ===========================
// 5. 请求方法类型
// ===========================

export type RequestMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH' | 'HEAD' | 'OPTIONS'

// ===========================
// 6. 缓存配置
// ===========================

export interface CacheConfig {
  enabled: boolean
  maxAge: number // 最大缓存时间（毫秒）
  maxSize: number // 最大缓存数量
}

// ===========================
// 7. 环境配置
// ===========================

export interface EnvConfig {
  baseURL: string
  timeout: number
  withCredentials: boolean
}

export type Environment = 'development' | 'test' | 'production'

// ===========================
// 8. 下载/上传进度回调
// ===========================

export interface ProgressCallback {
  (progressEvent: ProgressEvent): void
}

// ===========================
// 9. 请求取消
// ===========================

export interface CancelConfig {
  cancelToken?: any
  signal?: AbortSignal
}
