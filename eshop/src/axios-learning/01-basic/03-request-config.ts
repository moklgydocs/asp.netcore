/**
 * 第3课：请求配置详解
 *
 * 学习目标：
 * 1. 掌握所有常用的请求配置选项
 * 2. 理解如何自定义请求行为
 * 3. 学习超时、重试等高级配置
 */

import axios, { type AxiosRequestConfig } from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. 基础配置选项
// ===========================

/**
 * Axios 请求配置接口
 * 这些是最常用的配置选项
 */

export async function basicConfig() {
  const config: AxiosRequestConfig = {
    // 请求的 URL
    url: '/posts/1',

    // 请求方法（默认 GET）
    method: 'GET',

    // 基础 URL，会自动添加到 url 前面
    baseURL: BASE_URL,

    // 请求头
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    },

    // URL 查询参数（用于 GET 请求）
    params: {
      userId: 1
    },

    // 请求体数据（用于 POST、PUT、PATCH）
    data: {
      title: 'foo',
      body: 'bar',
      userId: 1
    },

    // 请求超时时间（毫秒）
    timeout: 5000,

    // 响应数据类型
    responseType: 'json', // 'arraybuffer', 'blob', 'document', 'json', 'text', 'stream'

    // 响应编码
    responseEncoding: 'utf8'
  }

  const response = await axios(config)
  console.log('📋 基础配置请求结果：', response.data)
  return response.data
}

// ===========================
// 2. URL 参数配置
// ===========================

/**
 * params 会被序列化为 URL 查询字符串
 */

export async function paramsConfig() {
  // 对象形式
  const response1 = await axios.get(`${BASE_URL}/posts`, {
    params: {
      userId: 1,
      _limit: 5,
      _sort: 'id',
      _order: 'desc'
    }
  })
  console.log('🔍 查询参数（对象）：', response1.config.url)

  // 使用 URLSearchParams
  const params = new URLSearchParams()
  params.append('userId', '1')
  params.append('_limit', '5')

  const response2 = await axios.get(`${BASE_URL}/posts`, { params })
  console.log('🔍 查询参数（URLSearchParams）：', response2.config.url)

  return response1.data
}

// ===========================
// 3. 请求头配置
// ===========================

/**
 * 自定义请求头
 */

export async function headersConfig() {
  const response = await axios.post(`${BASE_URL}/posts`,
    {
      title: 'Test',
      body: 'Test body',
      userId: 1
    },
    {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer YOUR_TOKEN',
        'X-Custom-Header': 'custom-value',
        'Accept-Language': 'zh-CN,zh;q=0.9,en;q=0.8'
      }
    }
  )

  console.log('📨 请求头：', response.config.headers)
  return response.data
}

// ===========================
// 4. 超时配置
// ===========================

/**
 * 设置请求超时时间
 * 如果请求时间超过 timeout，请求会被中止
 */

export async function timeoutConfig() {
  try {
    // 设置 1 毫秒超时（肯定会超时）
    await axios.get(`${BASE_URL}/posts`, {
      timeout: 1 // 毫秒
    })
  } catch (error: any) {
    if (error.code === 'ECONNABORTED') {
      console.log('⏱️ 请求超时！')
    }
  }

  // 正常超时设置
  const response = await axios.get(`${BASE_URL}/posts`, {
    timeout: 5000 // 5秒
  })

  console.log('✅ 请求成功（5秒超时）')
  return response.data
}

// ===========================
// 5. 响应类型配置
// ===========================

/**
 * 指定服务器响应的数据类型
 */

export async function responseTypeConfig() {
  // JSON（默认）
  const jsonResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'json'
  })
  console.log('📄 JSON 响应：', typeof jsonResponse.data)

  // Text
  const textResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'text'
  })
  console.log('📝 文本响应：', typeof textResponse.data)

  // Blob（用于下载文件）
  const blobResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'blob'
  })
  console.log('💾 Blob 响应：', blobResponse.data instanceof Blob)

  return jsonResponse.data
}

// ===========================
// 6. 认证配置
// ===========================

/**
 * HTTP Basic Authentication
 */

export async function authConfig() {
  const response = await axios.get('https://httpbin.org/basic-auth/user/passwd', {
    auth: {
      username: 'user',
      password: 'passwd'
    }
  })

  console.log('🔐 Basic Auth 成功：', response.data)
  return response.data
}

// ===========================
// 7. 进度监控配置
// ===========================

/**
 * 监控上传和下载进度
 */

export async function progressConfig() {
  const response = await axios.post(`${BASE_URL}/posts`,
    {
      title: 'Test Upload',
      body: 'Test body',
      userId: 1
    },
    {
      // 上传进度
      onUploadProgress: (progressEvent) => {
        const percentCompleted = progressEvent.total
          ? Math.round((progressEvent.loaded * 100) / progressEvent.total)
          : 0
        console.log(`📤 上传进度：${percentCompleted}%`)
      },

      // 下载进度
      onDownloadProgress: (progressEvent) => {
        const percentCompleted = progressEvent.total
          ? Math.round((progressEvent.loaded * 100) / progressEvent.total)
          : 0
        console.log(`📥 下载进度：${percentCompleted}%`)
      }
    }
  )

  return response.data
}

// ===========================
// 8. 代理配置
// ===========================

/**
 * 配置代理服务器（主要用于开发环境）
 */

export async function proxyConfig() {
  const response = await axios.get(`${BASE_URL}/posts/1`, {
    proxy: {
      protocol: 'http',
      host: '127.0.0.1',
      port: 8080,
      auth: {
        username: 'proxyuser',
        password: 'proxypass'
      }
    }
  })

  console.log('🌐 通过代理请求成功')
  return response.data
}

// ===========================
// 9. 取消请求配置
// ===========================

/**
 * 使用 AbortController 取消请求
 */

export async function cancelConfig() {
  const controller = new AbortController()

  // 2秒后取消请求
  setTimeout(() => {
    controller.abort()
    console.log('🛑 请求已取消')
  }, 2000)

  try {
    const response = await axios.get(`${BASE_URL}/posts`, {
      signal: controller.signal
    })
    console.log('✅ 请求成功')
    return response.data
  } catch (error: any) {
    if (axios.isCancel(error)) {
      console.log('❌ 请求被取消：', error.message)
    } else {
      console.error('❌ 请求失败：', error.message)
    }
  }
}

// ===========================
// 10. 验证状态码配置
// ===========================

/**
 * 自定义哪些状态码被视为成功
 */

export async function validateStatusConfig() {
  try {
    // 默认情况下，2xx 被视为成功
    // 我们可以自定义这个行为
    const response = await axios.get(`${BASE_URL}/posts/999999`, {
      validateStatus: (status) => {
        // 200-299 或 404 都视为成功
        return (status >= 200 && status < 300) || status === 404
      }
    })

    if (response.status === 404) {
      console.log('✅ 404 被视为成功响应')
    }

    return response.data
  } catch (error) {
    console.error('❌ 请求失败')
  }
}

// ===========================
// 11. 完整配置示例
// ===========================

export async function fullConfig() {
  const config: AxiosRequestConfig = {
    // 基础设置
    url: '/posts',
    method: 'GET',
    baseURL: BASE_URL,

    // 请求数据
    params: { userId: 1 },
    data: {},

    // 请求头
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer token'
    },

    // 超时和重试
    timeout: 10000,

    // 响应处理
    responseType: 'json',
    responseEncoding: 'utf8',

    // 状态码验证
    validateStatus: (status) => status >= 200 && status < 300,

    // 进度监控
    onUploadProgress: (e) => console.log('Upload:', e.loaded),
    onDownloadProgress: (e) => console.log('Download:', e.loaded),

    // 其他
    maxRedirects: 5, // 最大重定向次数
    maxContentLength: 2000, // 响应体最大字节数
    maxBodyLength: 2000, // 请求体最大字节数

    // 跨域设置
    withCredentials: false, // 是否携带凭证（cookies）

    // 自定义参数序列化
    paramsSerializer: {
      serialize: (params) => {
        // 自定义序列化逻辑
        return Object.entries(params)
          .map(([key, value]) => `${key}=${value}`)
          .join('&')
      }
    }
  }

  const response = await axios(config)
  console.log('🎯 完整配置请求成功')
  return response.data
}

// ===========================
// 12. 实践任务
// ===========================

/**
 * 任务1：运行 basicConfig()，理解基础配置
 * 任务2：尝试修改 timeout，观察超时行为
 * 任务3：使用 progressConfig() 监控进度
 * 任务4：实现一个可取消的搜索请求
 */

export function runLesson03() {
  console.log('==========================================')
  console.log('🎓 第3课：请求配置详解')
  console.log('==========================================')

  // 依次运行各个示例
  Promise.all([
    basicConfig(),
    paramsConfig(),
    headersConfig(),
    timeoutConfig(),
    responseTypeConfig(),
    progressConfig(),
    validateStatusConfig(),
    fullConfig()
  ])
    .then(() => {
      console.log('✨ 所有配置示例完成！')
    })
    .catch(error => {
      console.error('❌ 示例执行失败：', error)
    })
}

// ===========================
// 13. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ baseURL + url 组合使用
 * 2. ✅ params 用于查询参数，data 用于请求体
 * 3. ✅ headers 自定义请求头
 * 4. ✅ timeout 设置超时时间
 * 5. ✅ responseType 指定响应数据类型
 * 6. ✅ onUploadProgress/onDownloadProgress 监控进度
 * 7. ✅ signal 用于取消请求
 * 8. ✅ validateStatus 自定义成功状态码
 *
 * 下一课预告：深入理解响应结构 📚
 */

export default {
  basicConfig,
  paramsConfig,
  headersConfig,
  timeoutConfig,
  responseTypeConfig,
  authConfig,
  progressConfig,
  proxyConfig,
  cancelConfig,
  validateStatusConfig,
  fullConfig,
  runLesson03
}
