/**
 * 第2课：HTTP 请求方法详解
 *
 * 学习目标：
 * 1. 掌握 GET、POST、PUT、DELETE、PATCH 请求
 * 2. 理解不同请求方法的使用场景
 * 3. 学习如何传递参数和数据
 */

import axios from 'axios'

// 测试 API 基础 URL
const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. GET 请求 - 获取数据
// ===========================

/**
 * GET 请求用于从服务器获取数据
 * 特点：
 * - 参数通过 URL 查询字符串传递
 * - 不应该修改服务器数据（幂等操作）
 * - 可以被缓存
 */

// 示例1：获取单个资源
export async function getPost(id: number) {
  const response = await axios.get(`${BASE_URL}/posts/${id}`)
  console.log('📖 获取文章：', response.data)
  return response.data
}

// 示例2：获取列表（带查询参数）
export async function getPosts(userId?: number) {
  // 方式1：直接拼接 URL
  // const response = await axios.get(`${BASE_URL}/posts?userId=${userId}`)

  // 方式2：使用 params 配置（推荐）
  const response = await axios.get(`${BASE_URL}/posts`, {
    params: {
      userId: userId,
      _limit: 5 // 限制返回数量
    }
  })

  console.log('📚 获取文章列表：', response.data)
  return response.data
}

// ===========================
// 2. POST 请求 - 创建数据
// ===========================

/**
 * POST 请求用于向服务器提交数据，通常用于创建新资源
 * 特点：
 * - 数据通过请求体（body）传递
 * - 非幂等操作（多次执行会创建多个资源）
 * - 不会被缓存
 */

interface Post {
  title: string
  body: string
  userId: number
  id?: number
}

// 示例：创建新文章
export async function createPost(post: Omit<Post, 'id'>) {
  const response = await axios.post(`${BASE_URL}/posts`, post)
  console.log('✅ 创建成功：', response.data)
  return response.data
}

// 示例：创建新文章（带请求头）
export async function createPostWithHeaders(post: Omit<Post, 'id'>) {
  const response = await axios.post(`${BASE_URL}/posts`, post, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer your-token-here'
    }
  })

  console.log('✅ 创建成功（带请求头）：', response.data)
  return response.data
}

// ===========================
// 3. PUT 请求 - 完整更新
// ===========================

/**
 * PUT 请求用于完整更新资源
 * 特点：
 * - 需要提供完整的资源数据
 * - 幂等操作（多次执行结果相同）
 * - 如果资源不存在，可能会创建
 */

export async function updatePost(id: number, post: Post) {
  const response = await axios.put(`${BASE_URL}/posts/${id}`, post)
  console.log('🔄 完整更新成功：', response.data)
  return response.data
}

// ===========================
// 4. PATCH 请求 - 部分更新
// ===========================

/**
 * PATCH 请求用于部分更新资源
 * 特点：
 * - 只需要提供要更新的字段
 * - 幂等操作
 * - 更灵活，节省带宽
 */

export async function patchPost(id: number, updates: Partial<Post>) {
  const response = await axios.patch(`${BASE_URL}/posts/${id}`, updates)
  console.log('🔧 部分更新成功：', response.data)
  return response.data
}

// ===========================
// 5. DELETE 请求 - 删除数据
// ===========================

/**
 * DELETE 请求用于删除资源
 * 特点：
 * - 幂等操作
 * - 通常不需要请求体
 * - 返回状态码通常为 204 No Content 或 200 OK
 */

export async function deletePost(id: number) {
  const response = await axios.delete(`${BASE_URL}/posts/${id}`)
  console.log('🗑️ 删除成功：', response.status)
  return response.data
}

// ===========================
// 6. HEAD 请求 - 获取元数据
// ===========================

/**
 * HEAD 请求类似 GET，但只返回响应头，不返回响应体
 * 用途：检查资源是否存在、获取资源元信息
 */

export async function checkPostExists(id: number) {
  try {
    const response = await axios.head(`${BASE_URL}/posts/${id}`)
    console.log('✅ 资源存在，状态码：', response.status)
    console.log('📋 响应头：', response.headers)
    return true
  } catch (error) {
    console.log('❌ 资源不存在')
    return false
  }
}

// ===========================
// 7. OPTIONS 请求 - 获取支持的方法
// ===========================

/**
 * OPTIONS 请求用于获取服务器支持的 HTTP 方法
 * 主要用于 CORS 预检请求
 */

export async function getOptions() {
  const response = await axios.options(`${BASE_URL}/posts`)
  console.log('🔍 支持的方法：', response.headers['allow'])
  return response.headers
}

// ===========================
// 8. 请求方法对比
// ===========================

/**
 * 方法对比表：
 *
 * | 方法   | 用途       | 幂等性 | 安全性 | 可缓存 | 请求体 |
 * |--------|-----------|--------|--------|--------|--------|
 * | GET    | 获取资源   | ✅     | ✅     | ✅     | ❌     |
 * | POST   | 创建资源   | ❌     | ❌     | ❌     | ✅     |
 * | PUT    | 完整更新   | ✅     | ❌     | ❌     | ✅     |
 * | PATCH  | 部分更新   | ✅     | ❌     | ❌     | ✅     |
 * | DELETE | 删除资源   | ✅     | ❌     | ❌     | ❌     |
 * | HEAD   | 获取元数据 | ✅     | ✅     | ✅     | ❌     |
 * | OPTIONS| 获取选项   | ✅     | ✅     | ❌     | ❌     |
 *
 * 幂等性：多次执行结果相同
 * 安全性：不修改服务器数据
 */

// ===========================
// 9. 综合示例 - CRUD 操作
// ===========================

export async function crudExample() {
  console.log('==========================================')
  console.log('🎓 CRUD 操作演示')
  console.log('==========================================')

  try {
    // 1. Create - 创建
    console.log('\n1️⃣ 创建文章...')
    const newPost = await createPost({
      title: '学习 Axios',
      body: '这是一篇关于 Axios 的文章',
      userId: 1
    })

    // 2. Read - 读取
    console.log('\n2️⃣ 读取文章...')
    await getPost(1)
    await getPosts(1)

    // 3. Update - 更新
    console.log('\n3️⃣ 更新文章...')
    await updatePost(1, {
      id: 1,
      title: '学习 Axios（已更新）',
      body: '这是更新后的内容',
      userId: 1
    })

    // 3.5 Patch - 部分更新
    console.log('\n3️⃣.5️⃣ 部分更新文章...')
    await patchPost(1, {
      title: '学习 Axios（部分更新）'
    })

    // 4. Delete - 删除
    console.log('\n4️⃣ 删除文章...')
    await deletePost(1)

    console.log('\n✨ CRUD 操作演示完成！')
  } catch (error) {
    console.error('❌ 操作失败：', error)
  }
}

// ===========================
// 10. 实践任务
// ===========================

/**
 * 任务1：运行 crudExample()，观察控制台输出
 * 任务2：修改 createPost，创建自己的文章
 * 任务3：尝试获取不存在的资源，观察错误处理
 * 任务4：使用 checkPostExists 检查资源是否存在
 */

// 测试函数
export function runLesson02() {
  console.log('==========================================')
  console.log('🎓 第2课：HTTP 请求方法详解')
  console.log('==========================================')

  crudExample()
}

// ===========================
// 11. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ GET - 获取数据，参数用 params
 * 2. ✅ POST - 创建数据，数据用 request body
 * 3. ✅ PUT - 完整更新，需要完整数据
 * 4. ✅ PATCH - 部分更新，只需更新字段
 * 5. ✅ DELETE - 删除数据
 * 6. ✅ 理解幂等性和安全性的区别
 *
 * 下一课预告：深入学习请求配置选项 📚
 */

export default {
  getPost,
  getPosts,
  createPost,
  createPostWithHeaders,
  updatePost,
  patchPost,
  deletePost,
  checkPostExists,
  getOptions,
  crudExample,
  runLesson02
}
