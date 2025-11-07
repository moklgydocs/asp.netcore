/**
 * 第9课：错误处理与重试机制
 *
 * 学习目标：
 * 1. 理解 Axios 的错误类型
 * 2. 掌握完整的错误处理方案
 * 3. 实现智能重试机制
 */

import axios, { type AxiosError } from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. Axios 错误类型
// ===========================

/**
 * Axios 错误对象结构：
 * {
 *   message: string,      // 错误消息
 *   code: string,         // 错误代码（如 'ECONNABORTED'）
 *   config: {},           // 请求配置
 *   request: {},          // 请求对象
 *   response: {}          // 响应对象（如果有）
 * }
 *
 * 三种错误情况：
 * 1. 请求配置错误（发送前）
 * 2. 网络错误（无响应）
 * 3. HTTP 错误（有响应，状态码非 2xx）
 */

// ===========================
// 2. 基础错误处理
// ===========================

export async function basicErrorHandling() {
  try {
    const response = await axios.get(`${BASE_URL}/posts/99999`)
    console.log('✅ 请求成功：', response.data)
  } catch (error: any) {
    console.log('==========================================')
    console.log('❌ 错误处理示例')
    console.log('==========================================')

    if (axios.isAxiosError(error)) {
      // Axios 错误
      console.log('这是一个 Axios 错误')
      console.log('错误消息：', error.message)
      console.log('错误代码：', error.code)

      if (error.response) {
        // 服务器响应了错误状态码
        console.log('响应状态：', error.response.status)
        console.log('响应数据：', error.response.data)
        console.log('响应头：', error.response.headers)
      } else if (error.request) {
        // 请求已发出但没有收到响应
        console.log('请求对象：', error.request)
      } else {
        // 请求配置时发生错误
        console.log('配置错误：', error.message)
      }
    } else {
      // 非 Axios 错误
      console.log('其他错误：', error)
    }
  }
}

// ===========================
// 3. 错误分类处理
// ===========================

export class ErrorHandler {
  static handle(error: any) {
    if (!axios.isAxiosError(error)) {
      console.error('❌ 非 Axios 错误：', error)
      return
    }

    const axiosError = error as AxiosError

    // 1. 响应错误（服务器返回错误状态码）
    if (axiosError.response) {
      this.handleResponseError(axiosError)
    }
    // 2. 请求错误（请求已发送但无响应）
    else if (axiosError.request) {
      this.handleRequestError(axiosError)
    }
    // 3. 配置错误（请求配置问题）
    else {
      this.handleConfigError(axiosError)
    }
  }

  /**
   * 处理响应错误
   */
  static handleResponseError(error: AxiosError) {
    const { response } = error
    if (!response) return

    const { status, data } = response

    console.log(`❌ HTTP 错误 ${status}`)

    switch (status) {
      case 400:
        console.error('🔴 400 错误请求：', data)
        this.showMessage('请求参数错误')
        break

      case 401:
        console.error('🔴 401 未授权')
        this.showMessage('请先登录')
        this.redirectToLogin()
        break

      case 403:
        console.error('🔴 403 禁止访问')
        this.showMessage('您没有权限访问此资源')
        break

      case 404:
        console.error('🔴 404 资源不存在')
        this.showMessage('请求的资源不存在')
        break

      case 422:
        console.error('🔴 422 验证失败：', data)
        this.showValidationErrors(data)
        break

      case 429:
        console.error('🔴 429 请求过于频繁')
        this.showMessage('请求过于频繁，请稍后再试')
        break

      case 500:
        console.error('🔴 500 服务器内部错误')
        this.showMessage('服务器出错了，请稍后重试')
        break

      case 502:
        console.error('🔴 502 网关错误')
        this.showMessage('网关错误，请稍后重试')
        break

      case 503:
        console.error('🔴 503 服务不可用')
        this.showMessage('服务暂时不可用，请稍后重试')
        break

      case 504:
        console.error('🔴 504 网关超时')
        this.showMessage('请求超时，请稍后重试')
        break

      default:
        console.error(`🔴 ${status} 错误`)
        this.showMessage('请求失败，请稍后重试')
    }
  }

  /**
   * 处理请求错误（网络问题）
   */
  static handleRequestError(error: AxiosError) {
    console.error('❌ 请求错误：', error.message)

    if (error.code === 'ECONNABORTED') {
      console.error('⏱️ 请求超时')
      this.showMessage('请求超时，请检查网络连接')
    } else if (error.message === 'Network Error') {
      console.error('🌐 网络错误')
      this.showMessage('网络连接失败，请检查网络设置')
    } else {
      this.showMessage('请求失败，请稍后重试')
    }
  }

  /**
   * 处理配置错误
   */
  static handleConfigError(error: AxiosError) {
    console.error('❌ 配置错误：', error.message)
    this.showMessage('请求配置错误')
  }

  /**
   * 显示消息
   */
  static showMessage(message: string) {
    // 集成 UI 组件显示消息
    // ElMessage.error(message)
    console.log('💬 提示：', message)
  }

  /**
   * 显示验证错误
   */
  static showValidationErrors(data: any) {
    if (data.errors) {
      Object.keys(data.errors).forEach(field => {
        const messages = data.errors[field]
        console.error(`  ${field}: ${messages.join(', ')}`)
      })
    }
  }

  /**
   * 跳转登录
   */
  static redirectToLogin() {
    // 跳转到登录页
    // router.push('/login')
    console.log('🔐 跳转到登录页')
  }
}

// ===========================
// 4. 重试机制 - 基础版
// ===========================

export async function basicRetry(
  url: string,
  maxRetries: number = 3,
  delay: number = 1000
): Promise<any> {
  let lastError: any

  for (let i = 0; i <= maxRetries; i++) {
    try {
      console.log(`🔄 尝试请求 (${i + 1}/${maxRetries + 1})...`)

      const response = await axios.get(url)
      console.log('✅ 请求成功')
      return response.data
    } catch (error) {
      lastError = error

      if (i < maxRetries) {
        console.log(`❌ 请求失败，${delay}ms 后重试...`)
        await new Promise(resolve => setTimeout(resolve, delay))
      }
    }
  }

  console.error('❌ 重试次数已用尽')
  throw lastError
}

// ===========================
// 5. 重试机制 - 智能版
// ===========================

interface RetryConfig {
  maxRetries: number          // 最大重试次数
  retryDelay: number          // 重试延迟（毫秒）
  retryCondition?: (error: AxiosError) => boolean  // 重试条件
  onRetry?: (retryCount: number, error: AxiosError) => void  // 重试回调
  exponentialBackoff?: boolean  // 指数退避
}

export async function smartRetry(
  requestFn: () => Promise<any>,
  config: RetryConfig
): Promise<any> {
  const {
    maxRetries,
    retryDelay,
    retryCondition = () => true,
    onRetry,
    exponentialBackoff = false
  } = config

  let lastError: any

  for (let i = 0; i <= maxRetries; i++) {
    try {
      console.log(`🔄 尝试请求 (${i + 1}/${maxRetries + 1})...`)

      const result = await requestFn()
      console.log('✅ 请求成功')
      return result
    } catch (error: any) {
      lastError = error

      // 检查是否应该重试
      if (i < maxRetries && axios.isAxiosError(error) && retryCondition(error)) {
        // 计算延迟时间（指数退避）
        const delay = exponentialBackoff
          ? retryDelay * Math.pow(2, i)
          : retryDelay

        console.log(`❌ 请求失败，${delay}ms 后重试...`)

        // 触发重试回调
        onRetry?.(i + 1, error)

        // 延迟后重试
        await new Promise(resolve => setTimeout(resolve, delay))
      } else {
        break
      }
    }
  }

  console.error('❌ 重试次数已用尽或不满足重试条件')
  throw lastError
}

// ===========================
// 6. Axios 拦截器实现重试
// ===========================

export function setupRetryInterceptor(maxRetries: number = 3) {
  axios.interceptors.response.use(
    response => response,
    async error => {
      const config: any = error.config

      // 初始化重试计数
      if (!config.__retryCount) {
        config.__retryCount = 0
      }

      // 检查是否应该重试
      const shouldRetry =
        config.__retryCount < maxRetries &&
        isRetryableError(error)

      if (shouldRetry) {
        config.__retryCount += 1

        console.log(`🔄 重试请求 (${config.__retryCount}/${maxRetries})`)

        // 延迟后重试
        const delay = 1000 * config.__retryCount
        await new Promise(resolve => setTimeout(resolve, delay))

        // 重新发送请求
        return axios(config)
      }

      return Promise.reject(error)
    }
  )

  console.log('✅ 重试拦截器已安装')
}

/**
 * 判断错误是否可重试
 */
function isRetryableError(error: AxiosError): boolean {
  // 网络错误
  if (!error.response) {
    return true
  }

  // 特定状态码可重试
  const retryableStatusCodes = [408, 429, 500, 502, 503, 504]
  return retryableStatusCodes.includes(error.response.status)
}

// ===========================
// 7. 自定义错误类
// ===========================

export class ApiError extends Error {
  public code: number
  public status?: number
  public data?: any

  constructor(message: string, code: number, status?: number, data?: any) {
    super(message)
    this.name = 'ApiError'
    this.code = code
    this.status = status
    this.data = data
  }

  static fromAxiosError(error: AxiosError): ApiError {
    if (error.response) {
      return new ApiError(
        error.message,
        error.response.data?.code || error.response.status,
        error.response.status,
        error.response.data
      )
    }

    return new ApiError(error.message, -1)
  }
}

// ===========================
// 8. 错误日志记录
// ===========================

export class ErrorLogger {
  static errors: any[] = []

  /**
   * 记录错误
   */
  static log(error: any, context?: any) {
    const errorLog = {
      timestamp: new Date().toISOString(),
      message: error.message,
      stack: error.stack,
      context,
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      data: error.response?.data
    }

    this.errors.push(errorLog)
    console.log('📝 错误已记录：', errorLog)

    // 可以发送到服务器
    // this.sendToServer(errorLog)
  }

  /**
   * 发送到服务器
   */
  static async sendToServer(errorLog: any) {
    try {
      await axios.post('/api/errors', errorLog)
    } catch (error) {
      console.error('发送错误日志失败：', error)
    }
  }

  /**
   * 获取错误历史
   */
  static getErrors() {
    return this.errors
  }

  /**
   * 清空错误
   */
  static clear() {
    this.errors = []
  }
}

// ===========================
// 9. 实践示例
// ===========================

export async function practicalExamples() {
  console.log('==========================================')
  console.log('🎓 错误处理实践示例')
  console.log('==========================================')

  // 示例1：基础错误处理
  console.log('\n1️⃣ 基础错误处理')
  await basicErrorHandling()

  // 示例2：使用错误处理器
  console.log('\n2️⃣ 错误处理器示例')
  try {
    await axios.get(`${BASE_URL}/posts/99999`)
  } catch (error) {
    ErrorHandler.handle(error)
  }

  // 示例3：基础重试
  console.log('\n3️⃣ 基础重试示例')
  try {
    await basicRetry(`${BASE_URL}/posts/1`, 2, 500)
  } catch (error) {
    console.error('重试失败')
  }

  // 示例4：智能重试
  console.log('\n4️⃣ 智能重试示例')
  try {
    await smartRetry(
      () => axios.get(`${BASE_URL}/posts/1`),
      {
        maxRetries: 3,
        retryDelay: 500,
        exponentialBackoff: true,
        retryCondition: (error) => {
          // 只在特定情况下重试
          return error.response?.status === 500 || !error.response
        },
        onRetry: (count, error) => {
          console.log(`第 ${count} 次重试，错误：${error.message}`)
        }
      }
    )
  } catch (error) {
    console.error('智能重试失败')
  }
}

// ===========================
// 10. 实践任务
// ===========================

/**
 * 任务1：运行 practicalExamples()，观察不同的错误处理
 * 任务2：实现一个错误处理器，区分不同的错误类型
 * 任务3：实现一个重试机制，支持指数退避
 * 任务4：实现错误日志记录，并发送到服务器
 */

export function runLesson09() {
  console.log('==========================================')
  console.log('🎓 第9课：错误处理与重试机制')
  console.log('==========================================')

  practicalExamples()
}

// ===========================
// 11. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ 理解三种错误类型：配置错误、请求错误、响应错误
 * 2. ✅ 使用 axios.isAxiosError() 判断错误类型
 * 3. ✅ 根据状态码进行不同的错误处理
 * 4. ✅ 实现基础重试机制
 * 5. ✅ 实现智能重试（指数退避、重试条件）
 * 6. ✅ 使用拦截器实现全局重试
 * 7. ✅ 自定义错误类
 * 8. ✅ 错误日志记录
 *
 * 🎉 第二阶段完成！
 * 下一阶段预告：高级特性 - TypeScript、缓存、性能优化 📚
 */

export default {
  basicErrorHandling,
  ErrorHandler,
  basicRetry,
  smartRetry,
  setupRetryInterceptor,
  ApiError,
  ErrorLogger,
  practicalExamples,
  runLesson09
}
