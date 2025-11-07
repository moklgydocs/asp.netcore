/**
 * Axios 学习 - 快速开始指南
 *
 * 这个文件提供了快速学习和测试的入口
 */

// 导入所有课程
import lesson01 from './01-basic/01-introduction'
import lesson02 from './01-basic/02-http-methods'
import lesson03 from './01-basic/03-request-config'
import lesson04 from './01-basic/04-response-structure'
import lesson05 from './02-advanced/05-concurrent-requests'
import lesson06 from './02-advanced/06-cancel-request'
import lesson08 from './02-advanced/08-interceptors'
import lesson09 from './03-expert/09-error-handling'

// ===========================
// 学习路线
// ===========================

export const learningPath = {
  // 第一阶段：基础入门（1-2天）
  basic: [
    {
      id: 1,
      title: 'Axios 简介与安装',
      description: '了解 Axios 是什么，为什么选择 Axios，如何安装和引入',
      run: lesson01.runLesson01,
      duration: '30分钟',
      difficulty: '⭐'
    },
    {
      id: 2,
      title: 'HTTP 请求方法详解',
      description: '掌握 GET、POST、PUT、DELETE、PATCH 等请求方法',
      run: lesson02.runLesson02,
      duration: '1小时',
      difficulty: '⭐⭐'
    },
    {
      id: 3,
      title: '请求配置详解',
      description: '学习所有常用的请求配置选项',
      run: lesson03.runLesson03,
      duration: '1.5小时',
      difficulty: '⭐⭐'
    },
    {
      id: 4,
      title: '响应结构详解',
      description: '理解 Axios 响应对象的结构',
      run: lesson04.runLesson04,
      duration: '1小时',
      difficulty: '⭐⭐'
    }
  ],

  // 第二阶段：进阶使用（2-3天）
  advanced: [
    {
      id: 5,
      title: '并发请求处理',
      description: '掌握多个请求的并发处理，Promise.all、Promise.race',
      run: lesson05.runLesson05,
      duration: '1.5小时',
      difficulty: '⭐⭐⭐'
    },
    {
      id: 6,
      title: '请求取消机制',
      description: '学习使用 AbortController 取消请求',
      run: lesson06.runLesson06,
      duration: '1小时',
      difficulty: '⭐⭐⭐'
    },
    {
      id: 8,
      title: '请求和响应拦截器',
      description: 'Axios 最强大的功能之一',
      run: lesson08.runLesson08,
      duration: '2小时',
      difficulty: '⭐⭐⭐'
    }
  ],

  // 第三阶段：高级特性（3-4天）
  expert: [
    {
      id: 9,
      title: '错误处理与重试机制',
      description: '完整的错误处理方案和智能重试',
      run: lesson09.runLesson09,
      duration: '2小时',
      difficulty: '⭐⭐⭐⭐'
    }
  ]
}

// ===========================
// 学习进度跟踪
// ===========================

export class LearningProgress {
  private completed: Set<number> = new Set()

  constructor() {
    this.loadProgress()
  }

  // 标记课程完成
  markComplete(lessonId: number) {
    this.completed.add(lessonId)
    this.saveProgress()
    console.log(`✅ 课程 ${lessonId} 已完成！`)
  }

  // 检查是否完成
  isCompleted(lessonId: number): boolean {
    return this.completed.has(lessonId)
  }

  // 获取完成进度
  getProgress(): number {
    const total = this.getTotalLessons()
    return Math.round((this.completed.size / total) * 100)
  }

  // 获取总课程数
  getTotalLessons(): number {
    return learningPath.basic.length +
           learningPath.advanced.length +
           learningPath.expert.length
  }

  // 重置进度
  reset() {
    this.completed.clear()
    this.saveProgress()
    console.log('🔄 学习进度已重置')
  }

  // 保存进度
  private saveProgress() {
    localStorage.setItem('axios_learning_progress',
      JSON.stringify(Array.from(this.completed))
    )
  }

  // 加载进度
  private loadProgress() {
    const saved = localStorage.getItem('axios_learning_progress')
    if (saved) {
      this.completed = new Set(JSON.parse(saved))
    }
  }

  // 显示进度
  showProgress() {
    console.log('==========================================')
    console.log('📊 学习进度报告')
    console.log('==========================================')
    console.log(`总体进度：${this.getProgress()}%`)
    console.log(`已完成：${this.completed.size}/${this.getTotalLessons()} 课`)
    console.log('')

    this.showStageProgress('基础入门', learningPath.basic)
    this.showStageProgress('进阶使用', learningPath.advanced)
    this.showStageProgress('高级特性', learningPath.expert)
  }

  private showStageProgress(stage: string, lessons: any[]) {
    const completed = lessons.filter(l => this.isCompleted(l.id)).length
    const total = lessons.length
    const percent = Math.round((completed / total) * 100)

    console.log(`${stage}：${completed}/${total} (${percent}%)`)
    lessons.forEach(lesson => {
      const status = this.isCompleted(lesson.id) ? '✅' : '⬜'
      console.log(`  ${status} ${lesson.id}. ${lesson.title}`)
    })
    console.log('')
  }
}

// ===========================
// 学习助手
// ===========================

export class LearningHelper {
  private progress: LearningProgress

  constructor() {
    this.progress = new LearningProgress()
  }

  // 开始学习
  start() {
    console.log('==========================================')
    console.log('🎓 欢迎学习 Axios！')
    console.log('==========================================')
    console.log('')
    console.log('📚 学习路线：')
    console.log('  第一阶段：基础入门（1-2天）')
    console.log('  第二阶段：进阶使用（2-3天）')
    console.log('  第三阶段：高级特性（3-4天）')
    console.log('  第四阶段：实战封装（3-5天）')
    console.log('')
    console.log('💡 学习建议：')
    console.log('  1. 循序渐进，不要跳过基础部分')
    console.log('  2. 每个示例都要亲自运行和修改')
    console.log('  3. 理解原理，不只是记住用法')
    console.log('  4. 完成每课的实践任务')
    console.log('')
    console.log('🚀 使用方法：')
    console.log('  - runLesson(1)  运行第1课')
    console.log('  - showProgress()  查看学习进度')
    console.log('  - nextLesson()  运行下一课')
    console.log('  - reset()  重置学习进度')
    console.log('')
  }

  // 运行指定课程
  runLesson(lessonId: number) {
    const allLessons = [
      ...learningPath.basic,
      ...learningPath.advanced,
      ...learningPath.expert
    ]

    const lesson = allLessons.find(l => l.id === lessonId)

    if (!lesson) {
      console.error(`❌ 课程 ${lessonId} 不存在`)
      return
    }

    console.log('==========================================')
    console.log(`📖 第${lesson.id}课：${lesson.title}`)
    console.log('==========================================')
    console.log(`难度：${lesson.difficulty}`)
    console.log(`时长：${lesson.duration}`)
    console.log(`描述：${lesson.description}`)
    console.log('')

    // 运行课程
    lesson.run()

    // 标记完成
    setTimeout(() => {
      this.progress.markComplete(lessonId)
    }, 1000)
  }

  // 运行下一课
  nextLesson() {
    const allLessons = [
      ...learningPath.basic,
      ...learningPath.advanced,
      ...learningPath.expert
    ]

    const nextLesson = allLessons.find(l => !this.progress.isCompleted(l.id))

    if (nextLesson) {
      this.runLesson(nextLesson.id)
    } else {
      console.log('🎉 恭喜！你已经完成所有课程！')
      console.log('💪 现在可以开始实战项目了！')
    }
  }

  // 查看进度
  showProgress() {
    this.progress.showProgress()
  }

  // 重置进度
  reset() {
    this.progress.reset()
  }

  // 获取推荐课程
  getRecommendation() {
    const allLessons = [
      ...learningPath.basic,
      ...learningPath.advanced,
      ...learningPath.expert
    ]

    const nextLesson = allLessons.find(l => !this.progress.isCompleted(l.id))

    if (nextLesson) {
      console.log('💡 推荐学习：')
      console.log(`   ${nextLesson.id}. ${nextLesson.title}`)
      console.log(`   难度：${nextLesson.difficulty}`)
      console.log(`   时长：${nextLesson.duration}`)
      console.log(`   运行：runLesson(${nextLesson.id})`)
    } else {
      console.log('🎉 你已经完成所有课程！')
    }
  }
}

// ===========================
// 导出学习助手实例
// ===========================

const helper = new LearningHelper()

// 全局方法（方便在控制台使用）
export const start = () => helper.start()
export const runLesson = (id: number) => helper.runLesson(id)
export const nextLesson = () => helper.nextLesson()
export const showProgress = () => helper.showProgress()
export const reset = () => helper.reset()
export const recommend = () => helper.getRecommendation()

// 默认导出
export default {
  start,
  runLesson,
  nextLesson,
  showProgress,
  reset,
  recommend,
  learningPath,
  LearningProgress,
  LearningHelper
}

// ===========================
// 使用示例
// ===========================

/**
 * 在浏览器控制台中使用：
 *
 * import axiosLearning from '@/axios-learning/quick-start'
 *
 * // 开始学习
 * axiosLearning.start()
 *
 * // 运行第1课
 * axiosLearning.runLesson(1)
 *
 * // 运行下一课
 * axiosLearning.nextLesson()
 *
 * // 查看进度
 * axiosLearning.showProgress()
 *
 * // 获取推荐
 * axiosLearning.recommend()
 *
 * // 重置进度
 * axiosLearning.reset()
 */
