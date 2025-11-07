/**
 * 使用示例 - 在 Vue 组件中使用封装的 API
 */

import { ref, onMounted } from 'vue'
import api from '@/api'
import type { Post, User } from '@/api'

// ===========================
// 示例1：基础使用
// ===========================

export function useBasicExample() {
  const user = ref<User | null>(null)
  const loading = ref(false)

  const login = async () => {
    try {
      loading.value = true
      const result = await api.user.login({
        username: 'admin',
        password: '123456'
      })

      console.log('登录成功：', result)
      user.value = result.user

      // Token 已自动存储，后续请求会自动携带
    } catch (error) {
      console.error('登录失败：', error)
    } finally {
      loading.value = false
    }
  }

  return {
    user,
    loading,
    login
  }
}

// ===========================
// 示例2：列表数据加载
// ===========================

export function usePostList() {
  const posts = ref<Post[]>([])
  const total = ref(0)
  const loading = ref(false)
  const page = ref(1)
  const pageSize = ref(10)

  const loadPosts = async () => {
    try {
      loading.value = true

      const result = await api.post.getPostList({
        page: page.value,
        pageSize: pageSize.value,
        sortBy: 'createTime',
        order: 'desc'
      })

      posts.value = result.list
      total.value = result.total

      console.log('文章列表加载成功')
    } catch (error) {
      console.error('加载失败：', error)
    } finally {
      loading.value = false
    }
  }

  // 分页变化
  const handlePageChange = (newPage: number) => {
    page.value = newPage
    loadPosts()
  }

  // 初始化加载
  onMounted(() => {
    loadPosts()
  })

  return {
    posts,
    total,
    loading,
    page,
    pageSize,
    loadPosts,
    handlePageChange
  }
}

// ===========================
// 示例3：文件上传
// ===========================

export function useFileUpload() {
  const uploading = ref(false)
  const uploadProgress = ref(0)

  const uploadAvatar = async (file: File) => {
    try {
      uploading.value = true

      const result = await api.user.uploadAvatar(file, (percent) => {
        uploadProgress.value = percent
        console.log(`上传进度：${percent}%`)
      })

      console.log('头像上传成功：', result.url)
      return result.url
    } catch (error) {
      console.error('上传失败：', error)
      throw error
    } finally {
      uploading.value = false
      uploadProgress.value = 0
    }
  }

  return {
    uploading,
    uploadProgress,
    uploadAvatar
  }
}

// ===========================
// 示例4：并发请求
// ===========================

export function useDashboard() {
  const statistics = ref(null)
  const categories = ref([])
  const recentPosts = ref([])
  const loading = ref(false)

  const loadDashboard = async () => {
    try {
      loading.value = true

      // 并发请求多个接口
      const [stats, cats, posts] = await Promise.all([
        api.stats.getStatistics(),
        api.category.getCategoryList(),
        api.post.getPostList({ page: 1, pageSize: 5 })
      ])

      statistics.value = stats
      categories.value = cats
      recentPosts.value = posts.list

      console.log('仪表盘数据加载完成')
    } catch (error) {
      console.error('加载失败：', error)
    } finally {
      loading.value = false
    }
  }

  onMounted(() => {
    loadDashboard()
  })

  return {
    statistics,
    categories,
    recentPosts,
    loading,
    loadDashboard
  }
}

// ===========================
// 示例5：自定义错误处理
// ===========================

export function useCustomErrorHandling() {
  const createPost = async (title: string, content: string) => {
    try {
      const result = await api.post.createPost({
        title,
        content,
        author: 'Current User',
        authorId: 1,
        categoryId: 1,
        tags: [],
        viewCount: 0,
        likeCount: 0,
        status: 'draft'
      })

      console.log('文章创建成功：', result)
      return result
    } catch (error: any) {
      // 自定义错误处理
      if (error.message.includes('标题已存在')) {
        console.error('文章标题重复，请修改')
      } else if (error.message.includes('内容过短')) {
        console.error('文章内容太短，请补充')
      } else {
        console.error('创建失败：', error.message)
      }
      throw error
    }
  }

  return {
    createPost
  }
}

// ===========================
// 示例6：轮询请求
// ===========================

export function usePolling() {
  let timer: number | null = null

  const startPolling = (interval: number = 5000) => {
    const poll = async () => {
      try {
        // 轮询接口不显示 loading
        const stats = await api.stats.getStatistics()
        console.log('轮询数据：', stats)
      } catch (error) {
        console.error('轮询失败：', error)
      }
    }

    // 立即执行一次
    poll()

    // 定时轮询
    timer = window.setInterval(poll, interval)
  }

  const stopPolling = () => {
    if (timer) {
      clearInterval(timer)
      timer = null
      console.log('轮询已停止')
    }
  }

  return {
    startPolling,
    stopPolling
  }
}

// ===========================
// 示例7：搜索防抖
// ===========================

export function useSearch() {
  const keyword = ref('')
  const results = ref<Post[]>([])
  const searching = ref(false)
  let searchTimer: number | null = null

  const search = async (kw: string) => {
    try {
      searching.value = true

      const result = await api.post.getPostList({
        keyword: kw,
        page: 1,
        pageSize: 20
      })

      results.value = result.list
      console.log('搜索结果：', result.total, '条')
    } catch (error) {
      console.error('搜索失败：', error)
    } finally {
      searching.value = false
    }
  }

  // 防抖搜索
  const debouncedSearch = (kw: string) => {
    if (searchTimer) {
      clearTimeout(searchTimer)
    }

    searchTimer = window.setTimeout(() => {
      search(kw)
    }, 500) // 500ms 防抖
  }

  // 监听关键词变化
  const handleKeywordChange = (kw: string) => {
    keyword.value = kw
    if (kw.trim()) {
      debouncedSearch(kw)
    } else {
      results.value = []
    }
  }

  return {
    keyword,
    results,
    searching,
    handleKeywordChange
  }
}

// ===========================
// 导出所有示例
// ===========================

export default {
  useBasicExample,
  usePostList,
  useFileUpload,
  useDashboard,
  useCustomErrorHandling,
  usePolling,
  useSearch
}
