/**
 * 第6课：请求取消机制
 *
 * 学习目标：
 * 1. 理解为什么需要取消请求
 * 2. 掌握 AbortController 的使用
 * 3. 学习实际应用场景
 */

import axios from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. 为什么需要取消请求？
// ===========================

/**
 * 常见场景：
 * 1. 用户快速切换页面，之前的请求已无意义
 * 2. 搜索框输入时，只需要最新的搜索结果
 * 3. 文件上传时用户点击取消
 * 4. 组件卸载时取消未完成的请求，避免内存泄漏
 * 5. 防止重复提交（取消之前的提交请求）
 */

// ===========================
// 2. 使用 AbortController
// ===========================

/**
 * AbortController 是现代浏览器的标准 API
 * Axios 从 v0.22.0 开始支持 AbortController
 */

export async function basicCancelExample() {
  // 创建 AbortController
  const controller = new AbortController()

  console.log('🚀 开始请求...')

  // 1秒后取消请求
  setTimeout(() => {
    controller.abort()
    console.log('🛑 请求已取消')
  }, 1000)

  try {
    const response = await axios.get(`${BASE_URL}/posts`, {
      signal: controller.signal
    })
    console.log('✅ 请求成功')
    return response.data
  } catch (error: any) {
    if (axios.isCancel(error)) {
      console.log('❌ 请求被取消')
    } else {
      console.error('❌ 请求失败：', error.message)
    }
  }
}

// ===========================
// 3. 取消单个请求
// ===========================

export class CancellableRequest {
  private controller: AbortController | null = null

  async fetchData(url: string) {
    // 如果有进行中的请求，先取消
    if (this.controller) {
      this.controller.abort()
    }

    // 创建新的 controller
    this.controller = new AbortController()

    try {
      const response = await axios.get(url, {
        signal: this.controller.signal
      })
      console.log('✅ 数据获取成功')
      return response.data
    } catch (error: any) {
      if (axios.isCancel(error)) {
        console.log('🛑 请求被取消')
      } else {
        throw error
      }
    } finally {
      this.controller = null
    }
  }

  cancel() {
    if (this.controller) {
      this.controller.abort()
      this.controller = null
      console.log('🛑 手动取消请求')
    }
  }
}

// ===========================
// 4. 搜索防抖 + 请求取消
// ===========================

export class SearchWithCancel {
  private controller: AbortController | null = null
  private timer: number | null = null

  async search(keyword: string, delay: number = 300) {
    // 清除之前的定时器
    if (this.timer) {
      clearTimeout(this.timer)
    }

    // 取消之前的请求
    if (this.controller) {
      this.controller.abort()
    }

    // 防抖
    return new Promise((resolve, reject) => {
      this.timer = window.setTimeout(async () => {
        this.controller = new AbortController()

        try {
          const response = await axios.get(`${BASE_URL}/posts`, {
            params: { q: keyword },
            signal: this.controller.signal
          })

          console.log(`🔍 搜索 "${keyword}" 完成`)
          resolve(response.data)
        } catch (error: any) {
          if (axios.isCancel(error)) {
            console.log(`🛑 搜索 "${keyword}" 被取消`)
            resolve([]) // 返回空结果
          } else {
            reject(error)
          }
        }
      }, delay)
    })
  }
}

// ===========================
// 5. 管理多个请求
// ===========================

export class RequestManager {
  private requests: Map<string, AbortController> = new Map()

  /**
   * 发起请求并注册
   */
  async request<T = any>(key: string, url: string, config?: any): Promise<T> {
    // 如果该 key 的请求已存在，先取消
    this.cancel(key)

    // 创建新的 controller
    const controller = new AbortController()
    this.requests.set(key, controller)

    try {
      const response = await axios.get(url, {
        ...config,
        signal: controller.signal
      })

      console.log(`✅ 请求 [${key}] 完成`)
      return response.data
    } catch (error: any) {
      if (axios.isCancel(error)) {
        console.log(`🛑 请求 [${key}] 被取消`)
      }
      throw error
    } finally {
      this.requests.delete(key)
    }
  }

  /**
   * 取消指定请求
   */
  cancel(key: string) {
    const controller = this.requests.get(key)
    if (controller) {
      controller.abort()
      this.requests.delete(key)
      console.log(`🛑 取消请求 [${key}]`)
    }
  }

  /**
   * 取消所有请求
   */
  cancelAll() {
    this.requests.forEach((controller, key) => {
      controller.abort()
      console.log(`🛑 取消请求 [${key}]`)
    })
    this.requests.clear()
  }

  /**
   * 获取进行中的请求数量
   */
  getPendingCount(): number {
    return this.requests.size
  }
}

// ===========================
// 6. Vue 组件中使用（Composition API）
// ===========================

export function useRequest() {
  let controller: AbortController | null = null

  const fetchData = async (url: string) => {
    // 取消之前的请求
    if (controller) {
      controller.abort()
    }

    controller = new AbortController()

    try {
      const response = await axios.get(url, {
        signal: controller.signal
      })
      return response.data
    } catch (error: any) {
      if (!axios.isCancel(error)) {
        throw error
      }
    }
  }

  // 组件卸载时取消请求
  const cleanup = () => {
    if (controller) {
      controller.abort()
      controller = null
    }
  }

  // 在 Vue 组件中使用：
  // onUnmounted(cleanup)

  return {
    fetchData,
    cleanup
  }
}

// ===========================
// 7. 文件上传取消
// ===========================

export class FileUploader {
  private controller: AbortController | null = null
  private uploadProgress: number = 0

  async upload(file: File, url: string, onProgress?: (percent: number) => void) {
    this.controller = new AbortController()
    this.uploadProgress = 0

    const formData = new FormData()
    formData.append('file', file)

    try {
      const response = await axios.post(url, formData, {
        signal: this.controller.signal,
        onUploadProgress: (progressEvent) => {
          if (progressEvent.total) {
            this.uploadProgress = Math.round(
              (progressEvent.loaded * 100) / progressEvent.total
            )
            onProgress?.(this.uploadProgress)
            console.log(`📤 上传进度：${this.uploadProgress}%`)
          }
        }
      })

      console.log('✅ 上传成功')
      return response.data
    } catch (error: any) {
      if (axios.isCancel(error)) {
        console.log('🛑 上传已取消')
      } else {
        throw error
      }
    } finally {
      this.controller = null
    }
  }

  cancel() {
    if (this.controller) {
      this.controller.abort()
      console.log('🛑 取消上传')
    }
  }

  getProgress(): number {
    return this.uploadProgress
  }
}

// ===========================
// 8. 超时控制（使用取消机制）
// ===========================

export async function requestWithTimeout(url: string, timeout: number = 5000) {
  const controller = new AbortController()

  // 设置超时
  const timeoutId = setTimeout(() => {
    controller.abort()
  }, timeout)

  try {
    const response = await axios.get(url, {
      signal: controller.signal
    })

    clearTimeout(timeoutId)
    console.log('✅ 请求在超时前完成')
    return response.data
  } catch (error: any) {
    clearTimeout(timeoutId)

    if (axios.isCancel(error)) {
      console.error('❌ 请求超时')
      throw new Error('请求超时')
    }
    throw error
  }
}

// ===========================
// 9. 防止重复提交
// ===========================

export class FormSubmitter {
  private isSubmitting: boolean = false
  private controller: AbortController | null = null

  async submit(url: string, data: any) {
    // 防止重复提交
    if (this.isSubmitting) {
      console.warn('⚠️ 表单正在提交中，请勿重复提交')
      return
    }

    this.isSubmitting = true
    this.controller = new AbortController()

    try {
      const response = await axios.post(url, data, {
        signal: this.controller.signal
      })

      console.log('✅ 提交成功')
      return response.data
    } catch (error: any) {
      if (axios.isCancel(error)) {
        console.log('🛑 提交已取消')
      } else {
        throw error
      }
    } finally {
      this.isSubmitting = false
      this.controller = null
    }
  }

  cancel() {
    if (this.controller) {
      this.controller.abort()
    }
  }
}

// ===========================
// 10. 页面切换时取消请求
// ===========================

export class PageRequestManager {
  private pageControllers: Map<string, AbortController[]> = new Map()

  /**
   * 注册页面请求
   */
  registerRequest(pageName: string, controller: AbortController) {
    const controllers = this.pageControllers.get(pageName) || []
    controllers.push(controller)
    this.pageControllers.set(pageName, controllers)
  }

  /**
   * 发起页面请求
   */
  async pageRequest(pageName: string, url: string, config?: any) {
    const controller = new AbortController()
    this.registerRequest(pageName, controller)

    try {
      const response = await axios.get(url, {
        ...config,
        signal: controller.signal
      })
      return response.data
    } catch (error: any) {
      if (axios.isCancel(error)) {
        console.log(`🛑 页面 [${pageName}] 的请求被取消`)
      }
      throw error
    }
  }

  /**
   * 页面卸载时取消所有请求
   */
  cancelPageRequests(pageName: string) {
    const controllers = this.pageControllers.get(pageName)
    if (controllers) {
      controllers.forEach(controller => controller.abort())
      this.pageControllers.delete(pageName)
      console.log(`🛑 取消页面 [${pageName}] 的所有请求`)
    }
  }
}

// ===========================
// 11. 实践示例
// ===========================

export async function practicalExamples() {
  console.log('==========================================')
  console.log('🎓 请求取消实践示例')
  console.log('==========================================')

  // 示例1：基础取消
  console.log('\n1️⃣ 基础取消示例')
  await basicCancelExample()

  // 示例2：搜索防抖
  console.log('\n2️⃣ 搜索防抖示例')
  const searcher = new SearchWithCancel()
  searcher.search('vue')
  searcher.search('react')
  searcher.search('angular') // 只有这个会执行

  await new Promise(resolve => setTimeout(resolve, 500))

  // 示例3：请求管理
  console.log('\n3️⃣ 请求管理示例')
  const manager = new RequestManager()

  manager.request('posts', `${BASE_URL}/posts/1`)
  manager.request('users', `${BASE_URL}/users/1`)

  console.log('进行中的请求数：', manager.getPendingCount())

  await new Promise(resolve => setTimeout(resolve, 100))
  manager.cancelAll()
}

// ===========================
// 12. 实践任务
// ===========================

/**
 * 任务1：运行 practicalExamples()，观察取消效果
 * 任务2：实现一个搜索组件，支持防抖和取消
 * 任务3：实现一个文件上传组件，支持取消上传
 * 任务4：在 Vue 组件中使用 useRequest，确保组件卸载时取消请求
 */

export function runLesson06() {
  console.log('==========================================')
  console.log('🎓 第6课：请求取消机制')
  console.log('==========================================')

  practicalExamples()
}

// ===========================
// 13. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ AbortController 是取消请求的标准方式
 * 2. ✅ signal 属性传递给 Axios 配置
 * 3. ✅ axios.isCancel() 判断是否为取消错误
 * 4. ✅ 搜索防抖 + 请求取消
 * 5. ✅ 管理多个请求的取消
 * 6. ✅ 组件卸载时取消请求（避免内存泄漏）
 * 7. ✅ 防止重复提交
 * 8. ✅ 文件上传取消
 *
 * 下一课预告：创建实例与默认配置 📚
 */

export default {
  basicCancelExample,
  CancellableRequest,
  SearchWithCancel,
  RequestManager,
  useRequest,
  FileUploader,
  requestWithTimeout,
  FormSubmitter,
  PageRequestManager,
  practicalExamples,
  runLesson06
}
