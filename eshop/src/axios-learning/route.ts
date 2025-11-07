/**
 * Axios 学习系统 - 路由配置示例
 *
 * 将此路由添加到你的路由配置中
 */

// 如果你使用 Vue Router
export const axiosLearningRoute = {
  path: '/axios-learning',
  name: 'AxiosLearning',
  component: () => import('@/components/AxiosLearning.vue'),
  meta: {
    title: 'Axios 学习系统',
    icon: '🎓'
  }
}

// 使用示例：
// import { createRouter, createWebHistory } from 'vue-router'
// import { axiosLearningRoute } from './axios-learning-route'
//
// const routes = [
//   {
//     path: '/',
//     name: 'Home',
//     component: () => import('@/views/Home.vue')
//   },
//   axiosLearningRoute, // 添加学习路由
//   // ...其他路由
// ]
//
// const router = createRouter({
//   history: createWebHistory(),
//   routes
// })
//
// export default router
