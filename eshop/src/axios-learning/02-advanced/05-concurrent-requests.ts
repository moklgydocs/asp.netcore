/**
 * 第5课：并发请求处理
 *
 * 学习目标：
 * 1. 掌握多个请求的并发处理
 * 2. 理解 Promise.all、Promise.race 等方法
 * 3. 学习请求依赖和串行处理
 */

import axios from 'axios'

const BASE_URL = 'https://jsonplaceholder.typicode.com'

// ===========================
// 1. 使用 Promise.all 并发请求
// ===========================

/**
 * Promise.all 会等待所有请求完成
 * 如果任何一个请求失败，整个操作失败
 */

export async function parallelRequests() {
  console.log('🚀 开始并发请求...')

  const startTime = Date.now()

  try {
    // 同时发送多个请求
    const [posts, users, comments] = await Promise.all([
      axios.get(`${BASE_URL}/posts?_limit=5`),
      axios.get(`${BASE_URL}/users?_limit=5`),
      axios.get(`${BASE_URL}/comments?_limit=5`)
    ])

    const endTime = Date.now()

    console.log('✅ 所有请求完成')
    console.log(`⏱️ 耗时：${endTime - startTime}ms`)
    console.log('📚 文章数：', posts.data.length)
    console.log('👥 用户数：', users.data.length)
    console.log('💬 评论数：', comments.data.length)

    return {
      posts: posts.data,
      users: users.data,
      comments: comments.data
    }
  } catch (error) {
    console.error('❌ 请求失败：', error)
    throw error
  }
}

// ===========================
// 2. 使用 Promise.allSettled
// ===========================

/**
 * Promise.allSettled 等待所有请求完成（无论成功或失败）
 * 不会因为某个请求失败而中断
 */

export async function allSettledRequests() {
  console.log('🚀 开始 allSettled 请求...')

  const results = await Promise.allSettled([
    axios.get(`${BASE_URL}/posts/1`),
    axios.get(`${BASE_URL}/posts/99999`), // 这个会失败
    axios.get(`${BASE_URL}/users/1`)
  ])

  results.forEach((result, index) => {
    if (result.status === 'fulfilled') {
      console.log(`✅ 请求 ${index + 1} 成功：`, result.value.data)
    } else {
      console.log(`❌ 请求 ${index + 1} 失败：`, result.reason.message)
    }
  })

  // 只获取成功的结果
  const successResults = results
    .filter(r => r.status === 'fulfilled')
    .map(r => (r as PromiseFulfilledResult<any>).value.data)

  console.log('✅ 成功的结果数：', successResults.length)

  return successResults
}

// ===========================
// 3. 使用 Promise.race
// ===========================

/**
 * Promise.race 返回最快完成的请求
 * 常用于超时控制和多源加载
 */

export async function raceRequests() {
  console.log('🏁 开始竞速请求...')

  try {
    const result = await Promise.race([
      axios.get(`${BASE_URL}/posts/1`),
      axios.get(`${BASE_URL}/users/1`),
      axios.get(`${BASE_URL}/comments/1`)
    ])

    console.log('🏆 最快的请求完成：', result.config.url)
    console.log('📦 数据：', result.data)

    return result.data
  } catch (error) {
    console.error('❌ 最快的请求失败了')
    throw error
  }
}

// ===========================
// 4. 请求超时控制（使用 race）
// ===========================

/**
 * 使用 Promise.race 实现更灵活的超时控制
 */

function timeoutPromise(ms: number): Promise<never> {
  return new Promise((_, reject) => {
    setTimeout(() => {
      reject(new Error(`请求超时（${ms}ms）`))
    }, ms)
  })
}

export async function requestWithTimeout(url: string, timeout: number = 5000) {
  console.log(`⏱️ 发送请求（${timeout}ms 超时）...`)

  try {
    const result = await Promise.race([
      axios.get(url),
      timeoutPromise(timeout)
    ])

    console.log('✅ 请求在超时前完成')
    return result.data
  } catch (error: any) {
    console.error('❌ 请求失败：', error.message)
    throw error
  }
}

// ===========================
// 5. 串行请求（依赖关系）
// ===========================

/**
 * 当后续请求依赖前面请求的结果时，需要串行处理
 */

export async function sequentialRequests() {
  console.log('🔄 开始串行请求...')

  try {
    // 1. 获取第一篇文章
    const postResponse = await axios.get(`${BASE_URL}/posts/1`)
    const post = postResponse.data
    console.log('✅ 1. 获取文章：', post.title)

    // 2. 根据文章的 userId 获取作者信息
    const userResponse = await axios.get(`${BASE_URL}/users/${post.userId}`)
    const user = userResponse.data
    console.log('✅ 2. 获取作者：', user.name)

    // 3. 获取文章的评论
    const commentsResponse = await axios.get(`${BASE_URL}/posts/${post.id}/comments`)
    const comments = commentsResponse.data
    console.log('✅ 3. 获取评论：', comments.length, '条')

    // 组合结果
    return {
      post,
      author: user,
      comments
    }
  } catch (error) {
    console.error('❌ 串行请求失败：', error)
    throw error
  }
}

// ===========================
// 6. 批量请求（分组处理）
// ===========================

/**
 * 一次性请求多个资源
 */

export async function batchRequests(ids: number[]) {
  console.log(`🔢 批量请求 ${ids.length} 个资源...`)

  const requests = ids.map(id =>
    axios.get(`${BASE_URL}/posts/${id}`)
  )

  try {
    const responses = await Promise.all(requests)
    const posts = responses.map(r => r.data)

    console.log('✅ 批量请求完成')
    posts.forEach(post => {
      console.log(`   - ${post.id}: ${post.title}`)
    })

    return posts
  } catch (error) {
    console.error('❌ 批量请求失败')
    throw error
  }
}

// ===========================
// 7. 限制并发数量
// ===========================

/**
 * 控制同时进行的请求数量，避免过载
 */

export async function limitedConcurrency(urls: string[], limit: number = 3) {
  console.log(`🎯 限制并发数：${limit}`)

  const results: any[] = []
  const executing: Promise<any>[] = []

  for (const url of urls) {
    const promise = axios.get(url).then(res => {
      console.log(`✅ 完成：${url}`)
      return res.data
    })

    results.push(promise)

    if (limit <= urls.length) {
      const e = promise.then(() => {
        executing.splice(executing.indexOf(e), 1)
      })
      executing.push(e)

      if (executing.length >= limit) {
        await Promise.race(executing)
      }
    }
  }

  return Promise.all(results)
}

// ===========================
// 8. 请求重试（并发）
// ===========================

/**
 * 并发发送请求，失败后重试
 */

export async function retryParallelRequests(maxRetries: number = 3) {
  console.log(`🔄 并发请求（最多重试 ${maxRetries} 次）...`)

  const requests = [
    `${BASE_URL}/posts/1`,
    `${BASE_URL}/posts/2`,
    `${BASE_URL}/posts/3`
  ]

  const retryRequest = async (url: string, retries: number = 0): Promise<any> => {
    try {
      const response = await axios.get(url)
      return response.data
    } catch (error) {
      if (retries < maxRetries) {
        console.log(`⚠️ 重试 ${url} (第 ${retries + 1} 次)`)
        await new Promise(resolve => setTimeout(resolve, 1000 * (retries + 1)))
        return retryRequest(url, retries + 1)
      }
      throw error
    }
  }

  const results = await Promise.all(requests.map(url => retryRequest(url)))
  console.log('✅ 所有请求完成（含重试）')

  return results
}

// ===========================
// 9. 瀑布流请求
// ===========================

/**
 * 逐个发送请求，每个请求完成后立即处理
 */

export async function waterfallRequests(ids: number[]) {
  console.log('🌊 瀑布流请求...')

  const results = []

  for (const id of ids) {
    const response = await axios.get(`${BASE_URL}/posts/${id}`)
    console.log(`✅ 获取文章 ${id}`)
    results.push(response.data)

    // 可以在这里立即处理每个结果
    // 例如：更新 UI、存储到缓存等
  }

  console.log('✅ 瀑布流请求完成')
  return results
}

// ===========================
// 10. 综合示例 - 加载页面数据
// ===========================

export async function loadPageData() {
  console.log('==========================================')
  console.log('📄 加载页面数据（综合示例）')
  console.log('==========================================')

  try {
    // 并发加载不相关的数据
    const [postsRes, usersRes] = await Promise.all([
      axios.get(`${BASE_URL}/posts?_limit=5`),
      axios.get(`${BASE_URL}/users?_limit=5`)
    ])

    console.log('✅ 基础数据加载完成')

    // 为每篇文章加载评论（限制并发）
    const posts = postsRes.data
    const commentUrls = posts.map((post: any) =>
      `${BASE_URL}/posts/${post.id}/comments`
    )

    const commentsData = await limitedConcurrency(commentUrls, 2)

    // 组合数据
    const postsWithComments = posts.map((post: any, index: number) => ({
      ...post,
      comments: commentsData[index]
    }))

    console.log('✅ 页面数据加载完成')
    console.log(`   - ${posts.length} 篇文章`)
    console.log(`   - ${usersRes.data.length} 个用户`)
    console.log(`   - 每篇文章都有评论数据`)

    return {
      posts: postsWithComments,
      users: usersRes.data
    }
  } catch (error) {
    console.error('❌ 页面数据加载失败')
    throw error
  }
}

// ===========================
// 11. 实践任务
// ===========================

/**
 * 任务1：运行 parallelRequests()，观察并发效果
 * 任务2：对比 parallelRequests() 和 waterfallRequests() 的耗时
 * 任务3：使用 allSettledRequests() 处理部分失败的情况
 * 任务4：实现一个函数，同时加载用户及其所有文章
 */

export function runLesson05() {
  console.log('==========================================')
  console.log('🎓 第5课：并发请求处理')
  console.log('==========================================')

  // 示例：并发 vs 串行对比
  const testConcurrency = async () => {
    console.log('\n📊 性能对比：\n')

    // 并发
    const start1 = Date.now()
    await batchRequests([1, 2, 3, 4, 5])
    console.log(`并发耗时：${Date.now() - start1}ms\n`)

    // 串行
    const start2 = Date.now()
    await waterfallRequests([1, 2, 3, 4, 5])
    console.log(`串行耗时：${Date.now() - start2}ms\n`)
  }

  testConcurrency()
}

// ===========================
// 12. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ Promise.all - 并发请求，全部成功才成功
 * 2. ✅ Promise.allSettled - 并发请求，获取所有结果
 * 3. ✅ Promise.race - 竞速请求，最快的获胜
 * 4. ✅ 串行请求 - 有依赖关系的请求
 * 5. ✅ 批量请求 - 同时请求多个资源
 * 6. ✅ 限制并发 - 避免过载
 * 7. ✅ 请求重试 - 提高成功率
 *
 * 下一课预告：请求取消机制 📚
 */

export default {
  parallelRequests,
  allSettledRequests,
  raceRequests,
  requestWithTimeout,
  sequentialRequests,
  batchRequests,
  limitedConcurrency,
  retryParallelRequests,
  waterfallRequests,
  loadPageData,
  runLesson05
}
