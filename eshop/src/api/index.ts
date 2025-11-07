/**
 * API 接口封装示例
 *
 * 演示如何使用封装的请求工具定义 API
 */

import request from '@/utils/request'
import type { PageResponse } from '@/utils/request/types'

// ===========================
// 1. 用户相关 API
// ===========================

export interface User {
  id: number
  username: string
  email: string
  avatar?: string
  role: string
  createTime: string
}

export interface LoginParams {
  username: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: User
}

export const userApi = {
  // 登录
  login(data: LoginParams) {
    return request.post<LoginResponse>('/auth/login', data, {
      needToken: false, // 登录接口不需要 token
      showLoading: true
    })
  },

  // 退出登录
  logout() {
    return request.post('/auth/logout')
  },

  // 获取用户信息
  getUserInfo() {
    return request.get<User>('/user/info')
  },

  // 更新用户信息
  updateUser(data: Partial<User>) {
    return request.put<User>('/user/update', data)
  },

  // 修改密码
  changePassword(oldPassword: string, newPassword: string) {
    return request.post('/user/password', {
      oldPassword,
      newPassword
    })
  },

  // 上传头像
  uploadAvatar(file: File, onProgress?: (percent: number) => void) {
    return request.upload<{ url: string }>('/user/avatar', file, onProgress)
  }
}

// ===========================
// 2. 文章相关 API
// ===========================

export interface Post {
  id: number
  title: string
  content: string
  author: string
  authorId: number
  categoryId: number
  tags: string[]
  coverImage?: string
  viewCount: number
  likeCount: number
  status: 'draft' | 'published' | 'archived'
  createTime: string
  updateTime: string
}

export interface PostQueryParams {
  page?: number
  pageSize?: number
  keyword?: string
  categoryId?: number
  authorId?: number
  status?: string
  sortBy?: 'createTime' | 'viewCount' | 'likeCount'
  order?: 'asc' | 'desc'
}

export const postApi = {
  // 获取文章列表
  getPostList(params: PostQueryParams) {
    return request.get<PageResponse<Post>>('/posts', { params })
  },

  // 获取文章详情
  getPostDetail(id: number) {
    return request.get<Post>(`/posts/${id}`)
  },

  // 创建文章
  createPost(data: Omit<Post, 'id' | 'createTime' | 'updateTime'>) {
    return request.post<Post>('/posts', data)
  },

  // 更新文章
  updatePost(id: number, data: Partial<Post>) {
    return request.put<Post>(`/posts/${id}`, data)
  },

  // 删除文章
  deletePost(id: number) {
    return request.delete(`/posts/${id}`)
  },

  // 批量删除
  batchDelete(ids: number[]) {
    return request.post('/posts/batch-delete', { ids })
  },

  // 点赞
  likePost(id: number) {
    return request.post(`/posts/${id}/like`)
  },

  // 取消点赞
  unlikePost(id: number) {
    return request.delete(`/posts/${id}/like`)
  }
}

// ===========================
// 3. 分类相关 API
// ===========================

export interface Category {
  id: number
  name: string
  slug: string
  description?: string
  parentId?: number
  children?: Category[]
}

export const categoryApi = {
  // 获取分类树
  getCategoryTree() {
    return request.get<Category[]>('/categories/tree', {
      useCache: true, // 使用缓存
      cacheTime: 10 * 60 * 1000 // 10分钟
    })
  },

  // 获取分类列表
  getCategoryList() {
    return request.get<Category[]>('/categories')
  },

  // 创建分类
  createCategory(data: Omit<Category, 'id'>) {
    return request.post<Category>('/categories', data)
  },

  // 更新分类
  updateCategory(id: number, data: Partial<Category>) {
    return request.put<Category>(`/categories/${id}`, data)
  },

  // 删除分类
  deleteCategory(id: number) {
    return request.delete(`/categories/${id}`)
  }
}

// ===========================
// 4. 评论相关 API
// ===========================

export interface Comment {
  id: number
  postId: number
  userId: number
  userName: string
  userAvatar?: string
  content: string
  parentId?: number
  replyTo?: number
  replyToName?: string
  likeCount: number
  replies?: Comment[]
  createTime: string
}

export const commentApi = {
  // 获取评论列表
  getCommentList(postId: number, params?: { page?: number; pageSize?: number }) {
    return request.get<PageResponse<Comment>>(`/posts/${postId}/comments`, {
      params
    })
  },

  // 创建评论
  createComment(data: {
    postId: number
    content: string
    parentId?: number
    replyTo?: number
  }) {
    return request.post<Comment>('/comments', data)
  },

  // 删除评论
  deleteComment(id: number) {
    return request.delete(`/comments/${id}`)
  },

  // 点赞评论
  likeComment(id: number) {
    return request.post(`/comments/${id}/like`)
  }
}

// ===========================
// 5. 文件相关 API
// ===========================

export interface UploadResult {
  url: string
  filename: string
  size: number
  mimeType: string
}

export const fileApi = {
  // 上传单个文件
  uploadFile(file: File, onProgress?: (percent: number) => void) {
    return request.upload<UploadResult>('/upload', file, onProgress)
  },

  // 上传多个文件
  async uploadMultiple(files: File[], onProgress?: (percent: number) => void) {
    const uploadPromises = files.map(file =>
      request.upload<UploadResult>('/upload', file, onProgress)
    )
    return Promise.all(uploadPromises)
  },

  // 下载文件
  downloadFile(url: string, filename: string) {
    return request.download(url, filename)
  },

  // 导出数据
  exportData(type: 'excel' | 'csv' | 'pdf', params?: any) {
    return request.get(`/export/${type}`, {
      params,
      responseType: 'blob'
    })
  }
}

// ===========================
// 6. 统计相关 API
// ===========================

export interface Statistics {
  totalPosts: number
  totalUsers: number
  totalViews: number
  totalLikes: number
  todayPosts: number
  todayUsers: number
  todayViews: number
}

export const statsApi = {
  // 获取统计数据
  getStatistics() {
    return request.get<Statistics>('/stats/dashboard', {
      showLoading: false // 不显示 loading
    })
  },

  // 获取趋势数据
  getTrends(days: number = 7) {
    return request.get('/stats/trends', {
      params: { days }
    })
  }
}

// ===========================
// 7. 导出所有 API
// ===========================

export default {
  user: userApi,
  post: postApi,
  category: categoryApi,
  comment: commentApi,
  file: fileApi,
  stats: statsApi
}
