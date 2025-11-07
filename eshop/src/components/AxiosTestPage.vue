<template>
  <div class="test-page">
    <h1>🧪 Axios 学习系统测试页面</h1>

    <div class="button-group">
      <el-button type="primary" @click="runLesson1" size="large">
        运行第1课：Axios 简介
      </el-button>

      <el-button type="success" @click="runLesson2" size="large">
        运行第2课：HTTP 方法
      </el-button>

      <el-button type="info" @click="runLesson8" size="large">
        运行第8课：拦截器
      </el-button>

      <el-button type="warning" @click="showProgress" size="large">
        查看学习进度
      </el-button>
    </div>

    <div class="test-section">
      <h2>测试封装的请求工具</h2>

      <el-space>
        <el-button @click="testGet">测试 GET 请求</el-button>
        <el-button @click="testPost">测试 POST 请求</el-button>
        <el-button @click="testError">测试错误处理</el-button>
        <el-button @click="testCancel">测试请求取消</el-button>
      </el-space>
    </div>

    <div class="console-output">
      <h3>📝 控制台输出</h3>
      <div class="console-content">
        <p>请打开浏览器控制台查看详细输出</p>
        <p>或者点击上面的按钮运行测试</p>
      </div>
    </div>

    <div class="quick-start">
      <h2>💡 快速开始</h2>
      <el-card>
        <template #header>
          <div class="card-header">
            <span>在控制台中运行</span>
          </div>
        </template>
        <pre><code>// 导入学习模块
import axiosLearning from '@/axios-learning/quick-start'

// 开始学习
axiosLearning.start()

// 运行第1课
axiosLearning.runLesson(1)

// 查看进度
axiosLearning.showProgress()

// 运行下一课
axiosLearning.nextLesson()</code></pre>
      </el-card>
    </div>

    <div class="full-learning-system">
      <h2>🎓 完整学习系统</h2>
      <el-button type="primary" size="large" @click="openLearningSystem">
        打开可视化学习页面
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ElMessage } from 'element-plus'
import axiosLearning from '@/axios-learning/quick-start'
import request from '@/utils/request'

// 运行课程
const runLesson1 = () => {
  console.clear()
  axiosLearning.runLesson(1)
  ElMessage.success('第1课已运行，请查看控制台')
}

const runLesson2 = () => {
  console.clear()
  axiosLearning.runLesson(2)
  ElMessage.success('第2课已运行，请查看控制台')
}

const runLesson8 = () => {
  console.clear()
  axiosLearning.runLesson(8)
  ElMessage.success('第8课已运行，请查看控制台')
}

const showProgress = () => {
  console.clear()
  axiosLearning.showProgress()
  ElMessage.info('学习进度已输出到控制台')
}

// 测试请求工具
const testGet = async () => {
  try {
    console.log('测试 GET 请求...')
    const data = await request.get('https://jsonplaceholder.typicode.com/posts/1')
    console.log('✅ GET 请求成功：', data)
    ElMessage.success('GET 请求成功')
  } catch (error) {
    console.error('❌ GET 请求失败：', error)
  }
}

const testPost = async () => {
  try {
    console.log('测试 POST 请求...')
    const data = await request.post('https://jsonplaceholder.typicode.com/posts', {
      title: 'Test',
      body: 'Test body',
      userId: 1
    })
    console.log('✅ POST 请求成功：', data)
    ElMessage.success('POST 请求成功')
  } catch (error) {
    console.error('❌ POST 请求失败：', error)
  }
}

const testError = async () => {
  try {
    console.log('测试错误处理...')
    await request.get('https://jsonplaceholder.typicode.com/posts/99999')
  } catch (error: any) {
    console.log('✅ 错误已捕获：', error.message)
    ElMessage.warning('错误处理测试完成')
  }
}

const testCancel = () => {
  console.log('测试请求取消...')
  // 这里可以添加请求取消的测试代码
  ElMessage.info('请求取消功能测试（请查看控制台）')
}

// 打开完整学习系统
const openLearningSystem = () => {
  ElMessage.info('完整学习系统组件：src/components/AxiosLearning.vue')
  console.log('你可以在路由中添加该组件，或直接在 App.vue 中使用')
}
</script>

<style scoped lang="scss">
.test-page {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 2rem;

  h1 {
    text-align: center;
    color: #409eff;
    margin-bottom: 2rem;
  }

  h2 {
    margin: 2rem 0 1rem 0;
    color: #333;
  }

  h3 {
    margin: 1rem 0;
  }

  .button-group {
    display: flex;
    gap: 1rem;
    justify-content: center;
    flex-wrap: wrap;
    margin-bottom: 3rem;
  }

  .test-section {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    margin-bottom: 2rem;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  }

  .console-output {
    background: #f5f7fa;
    padding: 1.5rem;
    border-radius: 8px;
    margin-bottom: 2rem;

    .console-content {
      background: #1e1e1e;
      color: #d4d4d4;
      padding: 1rem;
      border-radius: 4px;
      font-family: 'Consolas', 'Monaco', monospace;
      min-height: 100px;
    }
  }

  .quick-start {
    margin-bottom: 2rem;

    pre {
      background: #f5f7fa;
      padding: 1rem;
      border-radius: 4px;
      overflow-x: auto;

      code {
        font-family: 'Consolas', 'Monaco', monospace;
        font-size: 0.9rem;
        line-height: 1.6;
      }
    }
  }

  .full-learning-system {
    text-align: center;
    padding: 2rem;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border-radius: 8px;
    color: white;

    h2 {
      color: white;
    }
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>
