/**
 * 第1课：Axios 简介与安装
 *
 * 学习目标：
 * 1. 了解 Axios 是什么
 * 2. Axios 的特点和优势
 * 3. 如何安装和引入 Axios
 */

// ===========================
// 1. Axios 是什么？
// ===========================

/**
 * Axios 是一个基于 Promise 的 HTTP 客户端，用于浏览器和 Node.js
 *
 * 主要特点：
 * - 从浏览器创建 XMLHttpRequests
 * - 从 Node.js 创建 HTTP 请求
 * - 支持 Promise API
 * - 拦截请求和响应
 * - 转换请求和响应数据
 * - 取消请求
 * - 自动转换 JSON 数据
 * - 客户端支持防御 XSRF
 */

// ===========================
// 2. 为什么选择 Axios？
// ===========================

/**
 * 对比原生 Fetch API 的优势：
 *
 * ✅ 自动转换 JSON（Fetch 需要手动调用 .json()）
 * ✅ 请求和响应拦截器（Fetch 需要手动封装）
 * ✅ 支持请求取消（Fetch 的 AbortController 较复杂）
 * ✅ 超时设置更简单
 * ✅ 浏览器兼容性更好（支持 IE11+）
 * ✅ 错误处理更友好
 * ✅ 进度监控（上传/下载）
 */

// ===========================
// 3. 安装 Axios
// ===========================

/**
 * 使用 npm：
 * npm install axios
 *
 * 使用 yarn：
 * yarn add axios
 *
 * 使用 pnpm：
 * pnpm add axios
 *
 * 本项目已经在 package.json 中安装了 axios ^1.13.2
 */

// ===========================
// 4. 引入 Axios
// ===========================

// 方式1：ES6 模块导入（推荐）
import axios from 'axios'

// 方式2：CommonJS 导入
// const axios = require('axios')

// 方式3：CDN 引入（在 HTML 中）
// <script src="https://cdn.jsdelivr.net/npm/axios/dist/axios.min.js"></script>

// ===========================
// 5. 第一个 Axios 请求
// ===========================

/**
 * 发送一个简单的 GET 请求
 */
const  firstRequest=async ()=> {
  try {
    // 发送 GET 请求到测试 API
    const response = await axios.get('https://jsonplaceholder.typicode.com/posts/1')

    console.log('✅ 请求成功！')
    console.log('响应数据：', response.data)
    console.log('响应状态：', response.status)
    console.log('响应状态文本：', response.statusText)

    return response.data
  } catch (error) {
    console.error('❌ 请求失败：', error)
    throw error
  }
}

// ===========================
// 6. Axios 核心概念预览
// ===========================

/**
 * 在后续课程中，我们将深入学习：
 *
 * 📌 请求方法：GET, POST, PUT, DELETE, PATCH 等
 * 📌 请求配置：headers, params, data, timeout 等
 * 📌 响应结构：data, status, headers, config 等
 * 📌 拦截器：请求拦截、响应拦截
 * 📌 实例创建：axios.create() 自定义配置
 * 📌 错误处理：try-catch、拦截器、重试机制
 * 📌 取消请求：AbortController
 * 📌 并发请求：Promise.all、Promise.race
 */

// ===========================
// 7. 实践任务
// ===========================

/**
 * 任务1：运行 firstRequest 函数，查看控制台输出
 * 任务2：修改请求的 URL，尝试获取不同的数据
 * 任务3：查看浏览器开发者工具的网络面板，观察请求详情
 */

// 测试函数（可以在控制台或组件中调用）
export function runLesson01() {
  console.log('==========================================')
  console.log('🎓 第1课：Axios 简介与安装')
  console.log('==========================================')

  firstRequest()
    .then(data => {
      console.log('✨ 学习完成！获取到的数据：', data)
    })
    .catch(error => {
      console.error('💥 学习过程中遇到错误：', error.message)
    })
}

// ===========================
// 8. 知识点总结
// ===========================

/**
 * 本课重点：
 * 1. ✅ Axios 是基于 Promise 的 HTTP 客户端
 * 2. ✅ 相比 Fetch API 有更多开箱即用的功能
 * 3. ✅ 使用 import axios from 'axios' 引入
 * 4. ✅ 最基本的用法：axios.get(url)
 * 5. ✅ 使用 async/await 处理异步请求
 *
 * 下一课预告：学习各种 HTTP 请求方法 📚
 */

export default {
  firstRequest,
  runLesson01
}
