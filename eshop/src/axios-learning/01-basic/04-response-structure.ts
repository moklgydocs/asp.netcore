/**
 * 第4课：响应结构详解
 *
 * 学习目标：
 * 1. 理解 Axios 响应对象的结构
 * 2. 学习如何访问响应数据
 * 3. 掌握响应拦截和转换
 */

import axios, { type AxiosResponse } from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. 响应对象结构
// ===========================

/**
 * Axios 响应对象包含以下属性：
 * {
 *   data: {},           // 服务器返回的数据
 *   status: 200,        // HTTP 状态码
 *   statusText: 'OK',   // HTTP 状态文本
 *   headers: {},        // 响应头
 *   config: {},         // 请求配置
 *   request: {}         // 原始请求对象（XMLHttpRequest 或 Node.js http.ClientRequest）
 * }
 */

export async function exploreResponse() {
  const response = await axios.get(`${BASE_URL}/posts/1`)

  console.log('==========================================')
  console.log('📦 完整响应对象：')
  console.log('==========================================')

  // 1. data - 服务器返回的数据（最常用）
  console.log('1️⃣ response.data:', response.data)

  // 2. status - HTTP 状态码
  console.log('2️⃣ response.status:', response.status)

  // 3. statusText - HTTP 状态文本
  console.log('3️⃣ response.statusText:', response.statusText)

  // 4. headers - 响应头
  console.log('4️⃣ response.headers:', response.headers)

  // 5. config - 请求配置
  console.log('5️⃣ response.config:', {
    url: response.config.url,
    method: response.config.method,
    baseURL: response.config.baseURL
  })

  // 6. request - 原始请求对象（浏览器环境下是 XMLHttpRequest）
  console.log('6️⃣ response.request:', typeof response.request)

  return response
}

// ===========================
// 2. 访问响应数据
// ===========================

interface Post {
  userId: number
  id: number
  title: string
  body: string
}

export async function accessResponseData() {
  // 完整响应
  const response: AxiosResponse<Post> = await axios.get(`${BASE_URL}/posts/1`)

  // 最常见：只需要 data
  const post: Post = response.data
  console.log('📝 文章标题：', post.title)
  console.log('📝 文章内容：', post.body)

  // 检查状态码
  if (response.status === 200) {
    console.log('✅ 请求成功')
  }

  // 访问响应头
  const contentType = response.headers['content-type']
  console.log('📋 Content-Type:', contentType)

  return post
}

// ===========================
// 3. 解构响应对象
// ===========================

export async function destructureResponse() {
  // 解构获取需要的属性
  const { data, status, headers } = await axios.get(`${BASE_URL}/posts/1`)

  console.log('数据：', data)
  console.log('状态：', status)
  console.log('头部：', headers)

  // 只需要 data（最常见）
  const { data: post } = await axios.get<Post>(`${BASE_URL}/posts/1`)
  console.log('文章：', post.title)

  return data
}

// ===========================
// 4. 响应头详解
// ===========================

export async function exploreHeaders() {
  const response = await axios.get(`${BASE_URL}/posts/1`)

  console.log('==========================================')
  console.log('📨 响应头详解：')
  console.log('==========================================')

  const headers = response.headers

  // 常见响应头
  console.log('Content-Type:', headers['content-type'])
  console.log('Content-Length:', headers['content-length'])
  console.log('Date:', headers['date'])
  console.log('Server:', headers['server'])
  console.log('Cache-Control:', headers['cache-control'])
  console.log('ETag:', headers['etag'])

  // 自定义响应头（如果有）
  console.log('X-Custom-Header:', headers['x-custom-header'])

  return headers
}

// ===========================
// 5. 状态码处理
// ===========================

export async function handleStatusCodes() {
  try {
    const response = await axios.get(`${BASE_URL}/posts/1`)

    // 根据不同状态码处理
    switch (response.status) {
      case 200:
        console.log('✅ 200 OK - 请求成功')
        break
      case 201:
        console.log('✅ 201 Created - 资源已创建')
        break
      case 204:
        console.log('✅ 204 No Content - 成功但无内容')
        break
      default:
        console.log(`ℹ️ 状态码：${response.status}`)
    }

    // 检查成功状态（2xx）
    if (response.status >= 200 && response.status < 300) {
      console.log('✅ 成功响应')
    }

    return response.data
  } catch (error: any) {
    // 错误状态码会抛出异常
    if (error.response) {
      console.error('❌ 错误状态码：', error.response.status)
      console.error('❌ 错误数据：', error.response.data)
    }
  }
}

// ===========================
// 6. 响应类型转换
// ===========================

export async function responseTransformation() {
  // JSON 响应（默认）
  const jsonResponse = await axios.get(`${BASE_URL}/posts/1`)
  console.log('📄 JSON 类型：', typeof jsonResponse.data)

  // 文本响应
  const textResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'text'
  })
  console.log('📝 文本类型：', typeof textResponse.data)

  // 数组缓冲响应
  const bufferResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'arraybuffer'
  })
  console.log('💾 缓冲区类型：', bufferResponse.data instanceof ArrayBuffer)

  // Blob 响应（用于文件下载）
  const blobResponse = await axios.get(`${BASE_URL}/posts/1`, {
    responseType: 'blob'
  })
  console.log('🗂️ Blob 类型：', blobResponse.data instanceof Blob)

  return jsonResponse.data
}

// ===========================
// 7. 自定义响应转换
// ===========================

export async function customTransform() {
  const response = await axios.get(`${BASE_URL}/posts/1`, {
    // 转换响应数据（在传递给 then/catch 前）
    transformResponse: [
      function (data) {
        // 在默认转换之后执行
        const parsed = JSON.parse(data)

        // 添加自定义字段
        parsed.timestamp = new Date().toISOString()
        parsed.source = 'axios'

        // 转换字段名
        parsed.postTitle = parsed.title
        delete parsed.title

        console.log('🔄 响应已转换')
        return parsed
      }
    ]
  })

  console.log('转换后的数据：', response.data)
  return response.data
}

// ===========================
// 8. 响应拦截器（预览）
// ===========================

export function setupResponseInterceptor() {
  // 添加响应拦截器
  axios.interceptors.response.use(
    (response) => {
      // 2xx 状态码触发
      console.log('✅ 响应拦截器 - 成功')

      // 可以修改响应数据
      response.data.intercepted = true

      return response
    },
    (error) => {
      // 非 2xx 状态码触发
      console.error('❌ 响应拦截器 - 失败')

      return Promise.reject(error)
    }
  )

  console.log('🔧 响应拦截器已设置')
}

// ===========================
// 9. 流式响应处理
// ===========================

export async function streamResponse() {
  const response = await axios.get(`${BASE_URL}/posts`, {
    responseType: 'stream'
  })

  // 注意：stream 类型主要用于 Node.js 环境
  // 浏览器环境建议使用 Fetch API 或其他方式
  console.log('🌊 流式响应（Node.js）')

  return response.data
}

// ===========================
// 10. 完整示例 - 处理分页响应
// ===========================

interface PaginatedResponse<T> {
  data: T[]
  page: number
  totalPages: number
  totalItems: number
}

export async function handlePaginatedResponse() {
  const response = await axios.get(`${BASE_URL}/posts`, {
    params: {
      _page: 1,
      _limit: 10
    }
  })

  // 从响应头获取分页信息
  const totalItems = response.headers['x-total-count']
  const data = response.data
  const page = 1
  const limit = 10
  const totalPages = Math.ceil(parseInt(totalItems) / limit)

  const result: PaginatedResponse<Post> = {
    data,
    page,
    totalPages,
    totalItems: parseInt(totalItems)
  }

  console.log('📄 分页信息：')
  console.log(`   当前页：${result.page}`)
  console.log(`   总页数：${result.totalPages}`)
  console.log(`   总条数：${result.totalItems}`)
  console.log(`   当前页数据：${result.data.length} 条`)

  return result
}

// ===========================
// 11. 实践任务
// ===========================

/**
 * 任务1：运行 exploreResponse()，查看完整响应对象
 * 任务2：使用 destructureResponse() 练习解构
 * 任务3：实现一个函数，检查响应头中的 Content-Type
 * 任务4：尝试不同的 responseType，观察数据类型变化
 */

export function runLesson04() {
  console.log('==========================================')
  console.log('🎓 第4课：响应结构详解')
  console.log('==========================================')

  Promise.all([
    exploreResponse(),
    accessResponseData(),
    destructureResponse(),
    exploreHeaders(),
    handleStatusCodes(),
    responseTransformation(),
    customTransform(),
    handlePaginatedResponse()
  ])
    .then(() => {
      console.log('✨ 所有示例完成！')
    })
    .catch(error => {
      console.error('❌ 示例执行失败：', error)
    })
}

// ===========================
// 12. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ response.data - 服务器返回的数据（最常用）
 * 2. ✅ response.status - HTTP 状态码
 * 3. ✅ response.statusText - 状态文本
 * 4. ✅ response.headers - 响应头
 * 5. ✅ response.config - 请求配置
 * 6. ✅ 使用解构简化代码
 * 7. ✅ responseType 指定响应数据类型
 * 8. ✅ transformResponse 自定义转换
 *
 * 🎉 第一阶段完成！
 * 下一阶段预告：进阶使用 - 并发请求、取消请求、拦截器 📚
 */

export default {
  exploreResponse,
  accessResponseData,
  destructureResponse,
  exploreHeaders,
  handleStatusCodes,
  responseTransformation,
  customTransform,
  setupResponseInterceptor,
  streamResponse,
  handlePaginatedResponse,
  runLesson04
}
