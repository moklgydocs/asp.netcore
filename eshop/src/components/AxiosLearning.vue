<template>
  <div class="axios-learning-page">
    <header class="learning-header">
      <h1>🎓 Axios 完整学习系统</h1>
      <p>从入门到精通，完整学习 Axios 请求库</p>
    </header>

    <div class="learning-container">
      <!-- 侧边栏 -->
      <aside class="learning-sidebar">
        <div class="progress-card">
          <h3>📊 学习进度</h3>
          <el-progress
            :percentage="progress"
            :color="progressColor"
            :stroke-width="20"
          />
          <p class="progress-text">
            已完成 {{ completedCount }}/{{ totalCount }} 课
          </p>
        </div>

        <div class="stage-section" v-for="stage in stages" :key="stage.name">
          <h4>{{ stage.name }}</h4>
          <div
            v-for="lesson in stage.lessons"
            :key="lesson.id"
            class="lesson-item"
            :class="{
              active: currentLesson?.id === lesson.id,
              completed: isCompleted(lesson.id)
            }"
            @click="selectLesson(lesson)"
          >
            <span class="lesson-status">
              {{ isCompleted(lesson.id) ? '✅' : '⬜' }}
            </span>
            <span class="lesson-title">{{ lesson.id }}. {{ lesson.title }}</span>
          </div>
        </div>
      </aside>

      <!-- 主内容区 -->
      <main class="learning-main">
        <div v-if="currentLesson" class="lesson-detail">
          <div class="lesson-header">
            <h2>{{ currentLesson.title }}</h2>
            <div class="lesson-meta">
              <el-tag type="info">{{ currentLesson.difficulty }}</el-tag>
              <el-tag type="warning">{{ currentLesson.duration }}</el-tag>
            </div>
          </div>

          <div class="lesson-description">
            <p>{{ currentLesson.description }}</p>
          </div>

          <div class="lesson-actions">
            <el-button
              type="primary"
              size="large"
              :loading="running"
              @click="runCurrentLesson"
            >
              <template #icon>
                <el-icon><VideoPlay /></el-icon>
              </template>
              运行示例
            </el-button>

            <el-button
              size="large"
              @click="markAsComplete"
              v-if="!isCompleted(currentLesson.id)"
            >
              <template #icon>
                <el-icon><Check /></el-icon>
              </template>
              标记为完成
            </el-button>

            <el-button
              size="large"
              @click="nextLesson"
            >
              下一课
              <template #icon>
                <el-icon><ArrowRight /></el-icon>
              </template>
            </el-button>
          </div>

          <!-- 控制台输出 -->
          <div class="console-output">
            <div class="console-header">
              <h3>📝 控制台输出</h3>
              <el-button
                size="small"
                @click="clearConsole"
                text
              >
                清空
              </el-button>
            </div>
            <div class="console-content" ref="consoleRef">
              <div
                v-for="(log, index) in consoleLogs"
                :key="index"
                class="console-log"
                :class="`log-${log.type}`"
              >
                <span class="log-time">{{ log.time }}</span>
                <span class="log-message">{{ log.message }}</span>
              </div>
              <div v-if="consoleLogs.length === 0" class="console-empty">
                等待运行示例...
              </div>
            </div>
          </div>
        </div>

        <div v-else class="welcome-screen">
          <div class="welcome-content">
            <h2>👋 欢迎来到 Axios 学习系统</h2>
            <p>选择左侧的课程开始学习</p>
            <el-button
              type="primary"
              size="large"
              @click="startLearning"
            >
              开始学习
            </el-button>
          </div>
        </div>
      </main>
    </div>

    <!-- 快捷工具 -->
    <div class="quick-tools">
      <el-button circle @click="showProgress" title="查看进度">
        <el-icon><DataAnalysis /></el-icon>
      </el-button>
      <el-button circle @click="resetProgress" title="重置进度">
        <el-icon><RefreshLeft /></el-icon>
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  VideoPlay,
  Check,
  ArrowRight,
  DataAnalysis,
  RefreshLeft
} from '@element-plus/icons-vue'
import axiosLearning from '@/axios-learning/quick-start'

// 学习阶段
const stages = [
  { name: '📚 基础入门', lessons: axiosLearning.learningPath.basic },
  { name: '🚀 进阶使用', lessons: axiosLearning.learningPath.advanced },
  { name: '💎 高级特性', lessons: axiosLearning.learningPath.expert }
]

// 当前课程
const currentLesson = ref<any>(null)

// 学习进度
const progressTracker = new axiosLearning.LearningProgress()
const progress = ref(0)
const completedCount = ref(0)
const totalCount = ref(0)

// 控制台日志
const consoleLogs = ref<Array<{ type: string; message: string; time: string }>>([])
const consoleRef = ref<HTMLElement>()

// 运行状态
const running = ref(false)

// 计算进度颜色
const progressColor = computed(() => {
  const p = progress.value
  if (p < 30) return '#f56c6c'
  if (p < 70) return '#e6a23c'
  return '#67c23a'
})

// 检查是否完成
const isCompleted = (lessonId: number) => {
  return progressTracker.isCompleted(lessonId)
}

// 选择课程
const selectLesson = (lesson: any) => {
  currentLesson.value = lesson
  clearConsole()
}

// 运行当前课程
const runCurrentLesson = async () => {
  if (!currentLesson.value) return

  running.value = true
  clearConsole()

  // 拦截 console.log
  interceptConsole()

  try {
    await currentLesson.value.run()
    ElMessage.success('示例运行完成')
  } catch (error: any) {
    ElMessage.error('运行出错：' + error.message)
    addLog('error', error.message)
  } finally {
    running.value = false
    restoreConsole()
  }
}

// 标记为完成
const markAsComplete = () => {
  if (!currentLesson.value) return

  progressTracker.markComplete(currentLesson.value.id)
  updateProgress()
  ElMessage.success(`课程 ${currentLesson.value.id} 已完成！`)
}

// 下一课
const nextLesson = () => {
  const allLessons = [
    ...axiosLearning.learningPath.basic,
    ...axiosLearning.learningPath.advanced,
    ...axiosLearning.learningPath.expert
  ]

  const currentIndex = allLessons.findIndex(l => l.id === currentLesson.value?.id)
  const next = allLessons[currentIndex + 1]

  if (next) {
    selectLesson(next)
  } else {
    ElMessage.success('🎉 恭喜！你已经完成所有课程！')
  }
}

// 开始学习
const startLearning = () => {
  const firstLesson = axiosLearning.learningPath.basic[0]
  selectLesson(firstLesson)
}

// 更新进度
const updateProgress = () => {
  progress.value = progressTracker.getProgress()
  completedCount.value = progressTracker['completed'].size
  totalCount.value = progressTracker.getTotalLessons()
}

// 查看进度
const showProgress = () => {
  progressTracker.showProgress()
  ElMessage.info('进度详情已输出到控制台')
}

// 重置进度
const resetProgress = async () => {
  try {
    await ElMessageBox.confirm(
      '确定要重置学习进度吗？此操作不可恢复。',
      '重置进度',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    progressTracker.reset()
    updateProgress()
    ElMessage.success('进度已重置')
  } catch {
    // 取消
  }
}

// 添加日志
const addLog = (type: string, message: string) => {
  const time = new Date().toLocaleTimeString()
  consoleLogs.value.push({ type, message, time })

  nextTick(() => {
    if (consoleRef.value) {
      consoleRef.value.scrollTop = consoleRef.value.scrollHeight
    }
  })
}

// 清空控制台
const clearConsole = () => {
  consoleLogs.value = []
}

// 拦截 console
let originalConsole: any = {}

const interceptConsole = () => {
  originalConsole = {
    log: console.log,
    error: console.error,
    warn: console.warn
  }

  console.log = (...args: any[]) => {
    originalConsole.log(...args)
    addLog('log', args.join(' '))
  }

  console.error = (...args: any[]) => {
    originalConsole.error(...args)
    addLog('error', args.join(' '))
  }

  console.warn = (...args: any[]) => {
    originalConsole.warn(...args)
    addLog('warn', args.join(' '))
  }
}

const restoreConsole = () => {
  if (originalConsole.log) {
    console.log = originalConsole.log
    console.error = originalConsole.error
    console.warn = originalConsole.warn
  }
}

// 初始化
onMounted(() => {
  updateProgress()
})
</script>

<style scoped lang="scss">
.axios-learning-page {
  min-height: 100vh;
  background: #f5f7fa;
}

.learning-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  text-align: center;

  h1 {
    margin: 0 0 0.5rem 0;
    font-size: 2rem;
  }

  p {
    margin: 0;
    opacity: 0.9;
  }
}

.learning-container {
  display: flex;
  max-width: 1400px;
  margin: 2rem auto;
  gap: 2rem;
  padding: 0 1rem;
}

.learning-sidebar {
  width: 300px;
  flex-shrink: 0;

  .progress-card {
    background: white;
    padding: 1.5rem;
    border-radius: 8px;
    margin-bottom: 1rem;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);

    h3 {
      margin: 0 0 1rem 0;
    }

    .progress-text {
      text-align: center;
      margin: 1rem 0 0 0;
      color: #666;
    }
  }

  .stage-section {
    background: white;
    padding: 1rem;
    border-radius: 8px;
    margin-bottom: 1rem;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);

    h4 {
      margin: 0 0 0.5rem 0;
      padding-bottom: 0.5rem;
      border-bottom: 2px solid #f0f0f0;
    }

    .lesson-item {
      padding: 0.75rem;
      margin: 0.5rem 0;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.3s;
      display: flex;
      align-items: center;
      gap: 0.5rem;

      &:hover {
        background: #f5f7fa;
      }

      &.active {
        background: #ecf5ff;
        border-left: 3px solid #409eff;
      }

      &.completed {
        .lesson-title {
          color: #67c23a;
        }
      }

      .lesson-title {
        font-size: 0.9rem;
      }
    }
  }
}

.learning-main {
  flex: 1;

  .lesson-detail {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);

    .lesson-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;

      h2 {
        margin: 0;
      }

      .lesson-meta {
        display: flex;
        gap: 0.5rem;
      }
    }

    .lesson-description {
      padding: 1rem;
      background: #f5f7fa;
      border-radius: 4px;
      margin-bottom: 1.5rem;
    }

    .lesson-actions {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
    }
  }

  .welcome-screen {
    background: white;
    padding: 4rem 2rem;
    border-radius: 8px;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
    text-align: center;

    .welcome-content {
      max-width: 500px;
      margin: 0 auto;

      h2 {
        margin-bottom: 1rem;
      }

      p {
        color: #666;
        margin-bottom: 2rem;
      }
    }
  }
}

.console-output {
  margin-top: 2rem;

  .console-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;

    h3 {
      margin: 0;
    }
  }

  .console-content {
    background: #1e1e1e;
    color: #d4d4d4;
    padding: 1rem;
    border-radius: 4px;
    font-family: 'Consolas', 'Monaco', monospace;
    font-size: 0.9rem;
    max-height: 400px;
    overflow-y: auto;

    .console-log {
      margin: 0.25rem 0;

      .log-time {
        color: #888;
        margin-right: 0.5rem;
      }

      &.log-error {
        color: #f56c6c;
      }

      &.log-warn {
        color: #e6a23c;
      }
    }

    .console-empty {
      color: #888;
      text-align: center;
      padding: 2rem;
    }
  }
}

.quick-tools {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>
