/**
 * 完整实战项目 - 博客管理系统
 *
 * 这是一个完整的示例，展示如何在实际项目中使用封装的请求工具
 */

import { ref, reactive, onMounted, onUnmounted } from 'vue'
import api from '@/api'
import type { Post, User, Comment } from '@/api'

// ===========================
// 1. 用户认证模块
// ===========================

export function useAuth() {
  const user = ref<User | null>(null)
  const isLoggedIn = ref(false)
  const loading = ref(false)

  // 登录
  const login = async (username: string, password: string) => {
    try {
      loading.value = true

      const result = await api.user.login({
        username,
        password
      })

      user.value = result.user
      isLoggedIn.value = true

      console.log('✅ 登录成功')
      return true
    } catch (error) {
      console.error('❌ 登录失败：', error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 登出
  const logout = async () => {
    try {
      await api.user.logout()
      user.value = null
      isLoggedIn.value = false

      console.log('✅ 登出成功')
    } catch (error) {
      console.error('❌ 登出失败：', error)
    }
  }

  // 获取用户信息
  const getUserInfo = async () => {
    try {
      user.value = await api.user.getUserInfo()
      isLoggedIn.value = true
    } catch (error) {
      console.error('❌ 获取用户信息失败：', error)
    }
  }

  // 初始化时获取用户信息
  onMounted(() => {
    const token = localStorage.getItem('access_token')
    if (token) {
      getUserInfo()
    }
  })

  return {
    user,
    isLoggedIn,
    loading,
    login,
    logout,
    getUserInfo
  }
}

// ===========================
// 2. 文章列表模块
// ===========================

export function usePostList() {
  const posts = ref<Post[]>([])
  const total = ref(0)
  const loading = ref(false)

  // 分页参数
  const pagination = reactive({
    page: 1,
    pageSize: 10,
    totalPages: 0
  })

  // 搜索参数
  const filters = reactive({
    keyword: '',
    categoryId: undefined as number | undefined,
    status: undefined as string | undefined,
    sortBy: 'createTime' as 'createTime' | 'viewCount' | 'likeCount',
    order: 'desc' as 'asc' | 'desc'
  })

  // 加载文章列表
  const loadPosts = async () => {
    try {
      loading.value = true

      const result = await api.post.getPostList({
        page: pagination.page,
        pageSize: pagination.pageSize,
        ...filters
      })

      posts.value = result.list
      total.value = result.total
      pagination.totalPages = result.totalPages

      console.log(`✅ 加载了 ${result.list.length} 篇文章`)
    } catch (error) {
      console.error('❌ 加载文章失败：', error)
    } finally {
      loading.value = false
    }
  }

  // 分页变化
  const handlePageChange = (page: number) => {
    pagination.page = page
    loadPosts()
  }

  // 搜索
  let searchTimer: number | null = null
  const handleSearch = (keyword: string) => {
    // 防抖搜索
    if (searchTimer) {
      clearTimeout(searchTimer)
    }

    searchTimer = window.setTimeout(() => {
      filters.keyword = keyword
      pagination.page = 1
      loadPosts()
    }, 500)
  }

  // 筛选
  const handleFilter = (key: string, value: any) => {
    filters[key as keyof typeof filters] = value
    pagination.page = 1
    loadPosts()
  }

  // 排序
  const handleSort = (sortBy: typeof filters.sortBy, order: typeof filters.order) => {
    filters.sortBy = sortBy
    filters.order = order
    loadPosts()
  }

  // 刷新
  const refresh = () => {
    pagination.page = 1
    loadPosts()
  }

  // 初始化
  onMounted(() => {
    loadPosts()
  })

  return {
    posts,
    total,
    loading,
    pagination,
    filters,
    loadPosts,
    handlePageChange,
    handleSearch,
    handleFilter,
    handleSort,
    refresh
  }
}

// ===========================
// 3. 文章详情模块
// ===========================

export function usePostDetail(postId: number) {
  const post = ref<Post | null>(null)
  const comments = ref<Comment[]>([])
  const loading = ref(false)
  const commentsLoading = ref(false)

  // 加载文章详情
  const loadPost = async () => {
    try {
      loading.value = true

      // 并发加载文章和评论
      const [postData, commentsData] = await Promise.all([
        api.post.getPostDetail(postId),
        api.comment.getCommentList(postId)
      ])

      post.value = postData
      comments.value = commentsData.list

      console.log('✅ 文章详情加载成功')
    } catch (error) {
      console.error('❌ 加载文章详情失败：', error)
    } finally {
      loading.value = false
    }
  }

  // 点赞文章
  const likePost = async () => {
    if (!post.value) return

    try {
      await api.post.likePost(post.value.id)
      post.value.likeCount++
      console.log('✅ 点赞成功')
    } catch (error) {
      console.error('❌ 点赞失败：', error)
    }
  }

  // 添加评论
  const addComment = async (content: string, parentId?: number) => {
    try {
      const newComment = await api.comment.createComment({
        postId,
        content,
        parentId
      })

      comments.value.unshift(newComment)
      console.log('✅ 评论成功')
      return true
    } catch (error) {
      console.error('❌ 评论失败：', error)
      return false
    }
  }

  // 删除评论
  const deleteComment = async (commentId: number) => {
    try {
      await api.comment.deleteComment(commentId)
      comments.value = comments.value.filter(c => c.id !== commentId)
      console.log('✅ 删除评论成功')
      return true
    } catch (error) {
      console.error('❌ 删除评论失败：', error)
      return false
    }
  }

  // 初始化
  onMounted(() => {
    loadPost()
  })

  return {
    post,
    comments,
    loading,
    commentsLoading,
    loadPost,
    likePost,
    addComment,
    deleteComment
  }
}

// ===========================
// 4. 文章编辑模块
// ===========================

export function usePostEditor(postId?: number) {
  const formData = reactive({
    title: '',
    content: '',
    categoryId: undefined as number | undefined,
    tags: [] as string[],
    coverImage: '',
    status: 'draft' as 'draft' | 'published' | 'archived'
  })

  const saving = ref(false)
  const uploading = ref(false)
  const uploadProgress = ref(0)

  // 加载文章（编辑模式）
  const loadPost = async () => {
    if (!postId) return

    try {
      const post = await api.post.getPostDetail(postId)

      // 填充表单
      formData.title = post.title
      formData.content = post.content
      formData.categoryId = post.categoryId
      formData.tags = post.tags
      formData.coverImage = post.coverImage || ''
      formData.status = post.status

      console.log('✅ 文章数据加载成功')
    } catch (error) {
      console.error('❌ 加载文章失败：', error)
    }
  }

  // 上传封面
  const uploadCover = async (file: File) => {
    try {
      uploading.value = true

      const result = await api.file.uploadFile(file, (percent) => {
        uploadProgress.value = percent
      })

      formData.coverImage = result.url
      console.log('✅ 封面上传成功')
      return true
    } catch (error) {
      console.error('❌ 上传失败：', error)
      return false
    } finally {
      uploading.value = false
      uploadProgress.value = 0
    }
  }

  // 保存文章
  const save = async () => {
    try {
      saving.value = true

      if (postId) {
        // 更新
        await api.post.updatePost(postId, formData)
        console.log('✅ 文章更新成功')
      } else {
        // 创建
        await api.post.createPost({
          ...formData,
          author: 'Current User',
          authorId: 1,
          viewCount: 0,
          likeCount: 0
        })
        console.log('✅ 文章创建成功')
      }

      return true
    } catch (error) {
      console.error('❌ 保存失败：', error)
      return false
    } finally {
      saving.value = false
    }
  }

  // 发布文章
  const publish = async () => {
    formData.status = 'published'
    return save()
  }

  // 初始化
  onMounted(() => {
    if (postId) {
      loadPost()
    }
  })

  return {
    formData,
    saving,
    uploading,
    uploadProgress,
    uploadCover,
    save,
    publish
  }
}

// ===========================
// 5. 仪表盘模块
// ===========================

export function useDashboard() {
  const statistics = ref<any>(null)
  const trends = ref<any>(null)
  const recentPosts = ref<Post[]>([])
  const loading = ref(false)

  // 轮询定时器
  let pollTimer: number | null = null

  // 加载仪表盘数据
  const loadDashboard = async () => {
    try {
      loading.value = true

      // 并发加载多个数据
      const [stats, trendsData, posts] = await Promise.all([
        api.stats.getStatistics(),
        api.stats.getTrends(7),
        api.post.getPostList({ page: 1, pageSize: 5, sortBy: 'createTime', order: 'desc' })
      ])

      statistics.value = stats
      trends.value = trendsData
      recentPosts.value = posts.list

      console.log('✅ 仪表盘数据加载成功')
    } catch (error) {
      console.error('❌ 加载仪表盘失败：', error)
    } finally {
      loading.value = false
    }
  }

  // 开始轮询
  const startPolling = (interval: number = 30000) => {
    loadDashboard()

    pollTimer = window.setInterval(() => {
      loadDashboard()
    }, interval)
  }

  // 停止轮询
  const stopPolling = () => {
    if (pollTimer) {
      clearInterval(pollTimer)
      pollTimer = null
    }
  }

  // 刷新
  const refresh = () => {
    loadDashboard()
  }

  // 初始化
  onMounted(() => {
    startPolling()
  })

  // 清理
  onUnmounted(() => {
    stopPolling()
  })

  return {
    statistics,
    trends,
    recentPosts,
    loading,
    refresh,
    startPolling,
    stopPolling
  }
}

// ===========================
// 6. 批量操作模块
// ===========================

export function useBatchOperations() {
  const selectedIds = ref<number[]>([])
  const operating = ref(false)

  // 选择/取消选择
  const toggleSelect = (id: number) => {
    const index = selectedIds.value.indexOf(id)
    if (index > -1) {
      selectedIds.value.splice(index, 1)
    } else {
      selectedIds.value.push(id)
    }
  }

  // 全选
  const selectAll = (ids: number[]) => {
    selectedIds.value = [...ids]
  }

  // 清空选择
  const clearSelection = () => {
    selectedIds.value = []
  }

  // 批量删除
  const batchDelete = async () => {
    if (selectedIds.value.length === 0) {
      console.warn('⚠️ 请先选择要删除的项')
      return false
    }

    try {
      operating.value = true

      await api.post.batchDelete(selectedIds.value)

      console.log(`✅ 成功删除 ${selectedIds.value.length} 项`)
      clearSelection()
      return true
    } catch (error) {
      console.error('❌ 批量删除失败：', error)
      return false
    } finally {
      operating.value = false
    }
  }

  return {
    selectedIds,
    operating,
    toggleSelect,
    selectAll,
    clearSelection,
    batchDelete
  }
}

// ===========================
// 7. 导出所有模块
// ===========================

export default {
  useAuth,
  usePostList,
  usePostDetail,
  usePostEditor,
  useDashboard,
  useBatchOperations
}
