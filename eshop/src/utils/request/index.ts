/**
 * Axios 请求工具 - 主文件
 *
 * 封装完整的请求功能
 */

import axios, { type AxiosInstance, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios'
import type { RequestConfig, ApiResponse } from './types'
import { defaultConfig, tokenConfig, statusCodeConfig, whitelistConfig } from './config'

// ===========================
// 1. 创建 Axios 实例
// ===========================

class HttpRequest {
  private instance: AxiosInstance
  private pendingRequests: Map<string, AbortController> = new Map()
  private requestCache: Map<string, { data: any; timestamp: number }> = new Map()

  constructor() {
    // 创建实例
    this.instance = axios.create({
      baseURL: defaultConfig.baseURL,
      timeout: defaultConfig.timeout,
      withCredentials: defaultConfig.withCredentials,
      headers: defaultConfig.headers
    })

    // 设置拦截器
    this.setupInterceptors()
  }

  // ===========================
  // 2. 设置拦截器
  // ===========================

  private setupInterceptors() {
    // 请求拦截器
    this.instance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        return this.handleRequest(config as RequestConfig)
      },
      (error) => {
        return this.handleRequestError(error)
      }
    )

    // 响应拦截器
    this.instance.interceptors.response.use(
      (response: AxiosResponse) => {
        return this.handleResponse(response)
      },
      (error) => {
        return this.handleResponseError(error)
      }
    )
  }

  // ===========================
  // 3. 请求拦截处理
  // ===========================

  private handleRequest(config: RequestConfig): RequestConfig {
    // 添加 Token
    if (this.needToken(config)) {
      const token = this.getToken()
      if (token) {
        config.headers = config.headers || {}
        config.headers[tokenConfig.tokenHeaderKey] = `${tokenConfig.tokenPrefix}${token}`
      }
    }

    // 显示 Loading
    if (config.showLoading !== false && !this.isInWhitelist(config.url, whitelistConfig.noLoadingUrls)) {
      this.showLoading()
    }

    // 处理请求取消
    this.handleRequestCancel(config)

    // 添加时间戳防止缓存
    if (!config.useCache) {
      config.params = {
        ...config.params,
        _t: Date.now()
      }
    }

    console.log('📤 请求发送：', config.method?.toUpperCase(), config.url)

    return config
  }

  private handleRequestError(error: any) {
    console.error('❌ 请求错误：', error)
    this.hideLoading()
    return Promise.reject(error)
  }

  // ===========================
  // 4. 响应拦截处理
  // ===========================

  private handleResponse(response: AxiosResponse): any {
    this.hideLoading()

    const config = response.config as RequestConfig

    // 移除pending请求
    this.removePendingRequest(config)

    console.log('📥 响应成功：', response.status, response.config.url)

    // 如果是下载文件，直接返回
    if (response.config.responseType === 'blob') {
      return response
    }

    // 处理业务响应
    const data = response.data as ApiResponse

    // 判断业务状态码
    if (statusCodeConfig.successCodes.includes(data.code)) {
      // 成功：返回数据
      return data.data
    } else {
      // 失败：显示错误信息
      const errorMsg = data.message || '请求失败'
      if (config.showError !== false) {
        this.showError(errorMsg)
      }
      return Promise.reject(new Error(errorMsg))
    }
  }

  private async handleResponseError(error: any): Promise<any> {
    this.hideLoading()

    const config = error.config as RequestConfig

    // 移除pending请求
    this.removePendingRequest(config)

    // 请求被取消
    if (axios.isCancel(error)) {
      console.log('🛑 请求已取消：', error.message)
      return Promise.reject(error)
    }

    // 处理HTTP错误状态码
    if (error.response) {
      const { status, data } = error.response
      console.error('❌ HTTP错误：', status, error.config.url)

      const errorMsg = this.getErrorMessage(status, data)

      // 特殊状态码处理
      switch (status) {
        case 401:
          // Token过期，尝试刷新
          return this.handleTokenExpired(error)

        case 403:
          // 无权限
          this.handleForbidden()
          break

        case 500:
        case 502:
        case 503:
        case 504:
          // 服务器错误
          this.handleServerError(status)
          break
      }

      if (config.showError !== false) {
        this.showError(errorMsg)
      }

      return Promise.reject(new Error(errorMsg))
    }

    // 网络错误
    if (error.message.includes('Network Error')) {
      console.error('❌ 网络错误')
      if (config.showError !== false) {
        this.showError('网络连接失败，请检查您的网络设置')
      }
      return Promise.reject(new Error('网络错误'))
    }

    // 超时错误
    if (error.code === 'ECONNABORTED') {
      console.error('❌ 请求超时')

      // 重试机制
      if (config.retryCount && config.retryCount > 0) {
        return this.retryRequest(config)
      }

      if (config.showError !== false) {
        this.showError('请求超时，请稍后重试')
      }
      return Promise.reject(new Error('请求超时'))
    }

    return Promise.reject(error)
  }

  // ===========================
  // 5. Token 处理
  // ===========================

  private needToken(config: RequestConfig): boolean {
    if (config.needToken === false) {
      return false
    }

    return !this.isInWhitelist(config.url, whitelistConfig.noTokenUrls)
  }

  private getToken(): string | null {
    return localStorage.getItem(tokenConfig.accessTokenKey)
  }

  private setToken(token: string) {
    localStorage.setItem(tokenConfig.accessTokenKey, token)
  }

  private async handleTokenExpired(error: any): Promise<any> {
    const refreshToken = localStorage.getItem(tokenConfig.refreshTokenKey)

    if (!refreshToken) {
      this.redirectToLogin()
      return Promise.reject(error)
    }

    try {
      // 刷新 Token
      const response = await axios.post(tokenConfig.refreshTokenUrl, {
        refreshToken
      })

      const newToken = response.data.data.accessToken
      this.setToken(newToken)

      // 重试原请求
      const config = error.config as RequestConfig
      config.headers = config.headers || {}
      config.headers[tokenConfig.tokenHeaderKey] = `${tokenConfig.tokenPrefix}${newToken}`

      return this.instance(config)
    } catch (refreshError) {
      // 刷新失败，跳转登录
      this.redirectToLogin()
      return Promise.reject(refreshError)
    }
  }

  private redirectToLogin() {
    localStorage.clear()
    window.location.href = '/login'
  }

  // ===========================
  // 6. 请求取消
  // ===========================

  private handleRequestCancel(config: RequestConfig) {
    const requestKey = this.getRequestKey(config)

    // 检查是否有相同的请求正在进行
    if (this.pendingRequests.has(requestKey)) {
      // 取消之前的请求
      const controller = this.pendingRequests.get(requestKey)!
      controller.abort()
      this.pendingRequests.delete(requestKey)
    }

    // 创建新的 AbortController
    const controller = new AbortController()
    config.signal = controller.signal
    this.pendingRequests.set(requestKey, controller)
  }

  private removePendingRequest(config: RequestConfig) {
    const requestKey = this.getRequestKey(config)
    this.pendingRequests.delete(requestKey)
  }

  private getRequestKey(config: RequestConfig): string {
    return `${config.method}:${config.url}:${JSON.stringify(config.params)}:${JSON.stringify(config.data)}`
  }

  // ===========================
  // 7. 请求重试
  // ===========================

  private async retryRequest(config: RequestConfig): Promise<any> {
    const retryCount = config.retryCount || 0
    const retryDelay = config.retryDelay || 1000

    console.log(`🔄 重试请求 (${retryCount} 次剩余)`)

    // 延迟后重试
    await new Promise(resolve => setTimeout(resolve, retryDelay))

    // 减少重试次数
    config.retryCount = retryCount - 1

    return this.instance(config)
  }

  // ===========================
  // 8. UI 交互
  // ===========================

  private showLoading() {
    // 显示 loading（可以集成 Element Plus 的 loading）
    // ElLoading.service()
    console.log('🔄 Loading...')
  }

  private hideLoading() {
    // 隐藏 loading
    // loadingInstance?.close()
    console.log('✅ Loading 关闭')
  }

  private showError(message: string) {
    // 显示错误提示（可以集成 Element Plus 的 message）
    // ElMessage.error(message)
    console.error('❌', message)
  }

  private handleForbidden() {
    // 处理403错误
    // 可以跳转到无权限页面
    console.error('🚫 无权限访问')
  }

  private handleServerError(status: number) {
    // 处理服务器错误
    console.error('💥 服务器错误：', status)
  }

  // ===========================
  // 9. 工具方法
  // ===========================

  private isInWhitelist(url: string | undefined, whitelist: string[]): boolean {
    if (!url) return false
    return whitelist.some(item => url.includes(item))
  }

  private getErrorMessage(status: number, data: any): string {
    // 优先使用后端返回的错误信息
    if (data && data.message) {
      return data.message
    }

    // 使用预定义的错误信息
    return statusCodeConfig.specialCodes[status as keyof typeof statusCodeConfig.specialCodes] || `请求失败 (${status})`
  }

  // ===========================
  // 10. 公开方法
  // ===========================

  public request<T = any>(config: RequestConfig): Promise<T> {
    return this.instance.request(config)
  }

  public get<T = any>(url: string, config?: RequestConfig): Promise<T> {
    return this.instance.get(url, config)
  }

  public post<T = any>(url: string, data?: any, config?: RequestConfig): Promise<T> {
    return this.instance.post(url, data, config)
  }

  public put<T = any>(url: string, data?: any, config?: RequestConfig): Promise<T> {
    return this.instance.put(url, data, config)
  }

  public delete<T = any>(url: string, config?: RequestConfig): Promise<T> {
    return this.instance.delete(url, config)
  }

  public patch<T = any>(url: string, data?: any, config?: RequestConfig): Promise<T> {
    return this.instance.patch(url, data, config)
  }

  // 下载文件
  public download(url: string, filename?: string, config?: RequestConfig): Promise<void> {
    return this.instance.get(url, {
      ...config,
      responseType: 'blob'
    }).then((response) => {
      const blob = new Blob([response.data])
      const downloadUrl = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = downloadUrl
      link.download = filename || 'download'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(downloadUrl)
    })
  }

  // 上传文件
  public upload<T = any>(
    url: string,
    file: File,
    onProgress?: (percent: number) => void,
    config?: RequestConfig
  ): Promise<T> {
    const formData = new FormData()
    formData.append('file', file)

    return this.instance.post(url, formData, {
      ...config,
      headers: {
        'Content-Type': 'multipart/form-data'
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(percent)
        }
      }
    })
  }

  // 取消所有请求
  public cancelAllRequests() {
    this.pendingRequests.forEach((controller) => {
      controller.abort()
    })
    this.pendingRequests.clear()
    console.log('🛑 所有请求已取消')
  }
}

// ===========================
// 11. 导出实例
// ===========================

const request = new HttpRequest()

export default request

// 导出便捷方法
export const { get, post, put, delete: del, patch, download, upload } = request
