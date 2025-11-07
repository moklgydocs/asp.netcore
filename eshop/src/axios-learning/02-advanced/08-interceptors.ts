/**
 * 第8课：请求和响应拦截器
 *
 * 学习目标：
 * 1. 理解拦截器的工作原理
 * 2. 掌握请求拦截器的使用
 * 3. 掌握响应拦截器的使用
 * 4. 学习拦截器的实际应用场景
 */

import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosResponse } from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. 拦截器基础概念
// ===========================

/**
 * 拦截器（Interceptors）：
 * - 在请求或响应被 then 或 catch 处理前拦截它们
 * - 可以修改请求配置或响应数据
 * - 常用于：添加 token、统一错误处理、日志记录等
 *
 * 两种拦截器：
 * 1. 请求拦截器：在请求发送前执行
 * 2. 响应拦截器：在响应返回后执行
 */

// ===========================
// 2. 请求拦截器
// ===========================

/**
 * 请求拦截器 - 在请求发送前修改配置
 */

export function setupRequestInterceptor() {
  // 添加请求拦截器
  const requestInterceptor = axios.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      // 在发送请求之前做些什么
      console.log('📤 请求拦截器 - 请求发送前')
      console.log('   URL:', config.url)
      console.log('   Method:', config.method)

      // 示例1：添加时间戳
      if (config.params) {
        config.params._t = Date.now()
      } else {
        config.params = { _t: Date.now() }
      }

      // 示例2：添加认证 token
      const token = localStorage.getItem('token')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }

      // 示例3：设置通用请求头
      config.headers['X-Requested-With'] = 'XMLHttpRequest'

      return config
    },
    (error) => {
      // 对请求错误做些什么
      console.error('❌ 请求拦截器 - 请求错误')
      return Promise.reject(error)
    }
  )

  console.log('✅ 请求拦截器已安装，ID:', requestInterceptor)
  return requestInterceptor
}

// ===========================
// 3. 响应拦截器
// ===========================

/**
 * 响应拦截器 - 在响应返回后处理数据
 */

export function setupResponseInterceptor() {
  // 添加响应拦截器
  const responseInterceptor = axios.interceptors.response.use(
    (response: AxiosResponse) => {
      // 2xx 范围内的状态码都会触发该函数
      console.log('📥 响应拦截器 - 响应成功')
      console.log('   Status:', response.status)
      console.log('   Data:', response.data)

      // 示例1：统一处理响应数据格式
      const data = response.data

      // 假设后端返回格式：{ code: 0, data: {...}, message: 'success' }
      // if (data.code === 0) {
      //   return data.data // 只返回实际数据
      // }

      // 示例2：记录响应时间
      const requestTime = response.config.headers['X-Request-Time']
      if (requestTime) {
        const duration = Date.now() - Number(requestTime)
        console.log(`   ⏱️ 请求耗时: ${duration}ms`)
      }

      return response
    },
    (error) => {
      // 超出 2xx 范围的状态码都会触发该函数
      console.error('❌ 响应拦截器 - 响应错误')

      if (error.response) {
        // 请求成功发出且服务器也响应了状态码，但状态代码超出了 2xx 的范围
        console.error('   Status:', error.response.status)
        console.error('   Data:', error.response.data)

        // 统一处理错误
        switch (error.response.status) {
          case 401:
            console.error('   🔒 未授权，请重新登录')
            // 跳转到登录页
            // router.push('/login')
            break
          case 403:
            console.error('   🚫 拒绝访问')
            break
          case 404:
            console.error('   🔍 请求的资源不存在')
            break
          case 500:
            console.error('   💥 服务器内部错误')
            break
          default:
            console.error('   ❓ 其他错误')
        }
      } else if (error.request) {
        // 请求已经成功发起，但没有收到响应
        console.error('   📡 无响应')
      } else {
        // 发送请求时出了点问题
        console.error('   ⚠️ 请求配置错误:', error.message)
      }

      return Promise.reject(error)
    }
  )

  console.log('✅ 响应拦截器已安装，ID:', responseInterceptor)
  return responseInterceptor
}

// ===========================
// 4. 移除拦截器
// ===========================

export function removeInterceptor(interceptorId: number, type: 'request' | 'response') {
  if (type === 'request') {
    axios.interceptors.request.eject(interceptorId)
    console.log('🗑️ 请求拦截器已移除')
  } else {
    axios.interceptors.response.eject(interceptorId)
    console.log('🗑️ 响应拦截器已移除')
  }
}

// ===========================
// 5. 多个拦截器
// ===========================

/**
 * 可以添加多个拦截器
 * 执行顺序：
 * - 请求拦截器：后添加的先执行（栈结构）
 * - 响应拦截器：先添加的先执行（队列结构）
 */

export function setupMultipleInterceptors() {
  // 请求拦截器1
  axios.interceptors.request.use(config => {
    console.log('📤 请求拦截器1')
    config.headers['X-Interceptor'] = '1'
    return config
  })

  // 请求拦截器2
  axios.interceptors.request.use(config => {
    console.log('📤 请求拦截器2')
    config.headers['X-Interceptor'] = '2'
    return config
  })

  // 响应拦截器1
  axios.interceptors.response.use(response => {
    console.log('📥 响应拦截器1')
    return response
  })

  // 响应拦截器2
  axios.interceptors.response.use(response => {
    console.log('📥 响应拦截器2')
    return response
  })

  console.log('✅ 多个拦截器已安装')
  console.log('   执行顺序：请求2 → 请求1 → 发送请求 → 响应1 → 响应2')
}

// ===========================
// 6. 实例级拦截器
// ===========================

/**
 * 为特定的 axios 实例添加拦截器
 */

export function createInstanceWithInterceptors() {
  // 创建实例
  const instance = axios.create({
    baseURL: BASE_URL,
    timeout: 5000
  })

  // 为实例添加请求拦截器
  instance.interceptors.request.use(
    config => {
      console.log('📤 实例请求拦截器')
      config.headers['X-Instance'] = 'custom-instance'
      return config
    }
  )

  // 为实例添加响应拦截器
  instance.interceptors.response.use(
    response => {
      console.log('📥 实例响应拦截器')
      return response.data // 直接返回 data
    }
  )

  console.log('✅ 自定义实例拦截器已设置')
  return instance
}

// ===========================
// 7. 常见应用场景
// ===========================

/**
 * 场景1：Token 认证
 */
export function setupTokenInterceptor() {
  axios.interceptors.request.use(config => {
    const token = localStorage.getItem('access_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  })

  axios.interceptors.response.use(
    response => response,
    async error => {
      if (error.response?.status === 401) {
        // Token 过期，尝试刷新
        const refreshToken = localStorage.getItem('refresh_token')
        if (refreshToken) {
          try {
            // 刷新 token
            const res = await axios.post('/auth/refresh', { refreshToken })
            localStorage.setItem('access_token', res.data.accessToken)

            // 重试原请求
            error.config.headers.Authorization = `Bearer ${res.data.accessToken}`
            return axios(error.config)
          } catch (refreshError) {
            // 刷新失败，跳转登录
            localStorage.clear()
            window.location.href = '/login'
          }
        }
      }
      return Promise.reject(error)
    }
  )
}

/**
 * 场景2：Loading 状态管理
 */
let loadingCount = 0

export function setupLoadingInterceptor() {
  axios.interceptors.request.use(config => {
    loadingCount++
    showLoading()
    return config
  })

  axios.interceptors.response.use(
    response => {
      loadingCount--
      if (loadingCount === 0) {
        hideLoading()
      }
      return response
    },
    error => {
      loadingCount--
      if (loadingCount === 0) {
        hideLoading()
      }
      return Promise.reject(error)
    }
  )
}

function showLoading() {
  console.log('🔄 显示 Loading...')
  // 显示 loading 组件
}

function hideLoading() {
  console.log('✅ 隐藏 Loading')
  // 隐藏 loading 组件
}

/**
 * 场景3：请求日志记录
 */
export function setupLogInterceptor() {
  axios.interceptors.request.use(config => {
    const timestamp = Date.now()
    config.headers['X-Request-Time'] = timestamp

    console.log('📊 请求日志：', {
      time: new Date(timestamp).toISOString(),
      method: config.method?.toUpperCase(),
      url: config.url,
      params: config.params,
      data: config.data
    })

    return config
  })

  axios.interceptors.response.use(
    response => {
      const requestTime = Number(response.config.headers['X-Request-Time'])
      const duration = Date.now() - requestTime

      console.log('📊 响应日志：', {
        url: response.config.url,
        status: response.status,
        duration: `${duration}ms`,
        data: response.data
      })

      return response
    }
  )
}

/**
 * 场景4：数据转换
 */
export function setupDataTransformInterceptor() {
  axios.interceptors.response.use(response => {
    // 统一的后端响应格式：{ code, data, message }
    const { code, data, message } = response.data

    if (code === 0 || code === 200) {
      // 成功：直接返回数据
      response.data = data
      return response
    } else {
      // 失败：转换为错误
      return Promise.reject(new Error(message || '请求失败'))
    }
  })
}

// ===========================
// 8. 条件拦截
// ===========================

/**
 * 根据条件决定是否执行拦截逻辑
 */
export function setupConditionalInterceptor() {
  axios.interceptors.request.use(config => {
    // 只对特定 URL 添加 token
    if (config.url?.includes('/api/')) {
      const token = localStorage.getItem('token')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
    }

    // 对上传请求使用不同的超时时间
    if (config.url?.includes('/upload')) {
      config.timeout = 60000 // 60秒
    }

    return config
  })
}

// ===========================
// 9. 测试示例
// ===========================

export async function testInterceptors() {
  console.log('==========================================')
  console.log('🧪 测试拦截器')
  console.log('==========================================')

  // 设置拦截器
  setupRequestInterceptor()
  setupResponseInterceptor()

  try {
    // 发送请求
    const response = await axios.get(`${BASE_URL}/posts/1`)
    console.log('✅ 请求成功，数据：', response.data)
  } catch (error) {
    console.error('❌ 请求失败：', error)
  }
}

// ===========================
// 10. 实践任务
// ===========================

/**
 * 任务1：运行 testInterceptors()，观察拦截器执行
 * 任务2：创建一个拦截器，为所有请求添加自定义头
 * 任务3：创建一个拦截器，统一处理 404 错误
 * 任务4：实现 Token 自动刷新机制
 */

export function runLesson08() {
  console.log('==========================================')
  console.log('🎓 第8课：请求和响应拦截器')
  console.log('==========================================')

  testInterceptors()
}

// ===========================
// 11. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ 请求拦截器：修改请求配置
 * 2. ✅ 响应拦截器：处理响应数据
 * 3. ✅ 多个拦截器的执行顺序
 * 4. ✅ 实例级拦截器 vs 全局拦截器
 * 5. ✅ 常见应用：Token、Loading、日志
 * 6. ✅ 条件拦截：根据情况执行不同逻辑
 * 7. ✅ eject() 移除拦截器
 *
 * 🎉 拦截器是 Axios 最强大的功能之一！
 *
 * 下一课预告：错误处理与重试机制 📚
 */

export default {
  setupRequestInterceptor,
  setupResponseInterceptor,
  removeInterceptor,
  setupMultipleInterceptors,
  createInstanceWithInterceptors,
  setupTokenInterceptor,
  setupLoadingInterceptor,
  setupLogInterceptor,
  setupDataTransformInterceptor,
  setupConditionalInterceptor,
  testInterceptors,
  runLesson08
}
