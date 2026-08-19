import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

export interface AppNotification {
  id: string
  kind: 'task-reminder'
  level: 'info' | 'warning'
  title: string
  message: string
  createdAt: string
  readAt?: string
  taskId?: string
}

const MAX_NOTIFICATION_COUNT = 100
const ENABLE_NOTIFICATION_DEMO = true

function sortNotifications(items: AppNotification[]) {
  return [...items].sort((left, right) => right.createdAt.localeCompare(left.createdAt))
}

function createIsoOffset(base: Date, offsetMinutes: number) {
  return new Date(base.getTime() + offsetMinutes * 60 * 1000).toISOString()
}

function buildDemoNotifications(): AppNotification[] {
  const now = new Date()

  return sortNotifications([
    {
      id: 'demo-task-reminder-1',
      kind: 'task-reminder',
      level: 'warning',
      title: '通知中心改版评审将在 15 分钟后开始',
      message: '任务「通知中心改版」预计在今天 10:30 截止，建议先确认列表样式、未读状态和按钮排布。',
      createdAt: createIsoOffset(now, -12),
      taskId: 'task-demo-1',
    },
    {
      id: 'demo-task-reminder-2',
      kind: 'task-reminder',
      level: 'info',
      title: '补充演示数据已排期',
      message: '任务「通知示例数据准备」安排在今天中午前完成，便于联调视觉效果和筛选状态。',
      createdAt: createIsoOffset(now, -50),
      taskId: 'task-demo-2',
    },
    {
      id: 'demo-task-reminder-3',
      kind: 'task-reminder',
      level: 'warning',
      title: '本周任务看板需要同步',
      message: '任务「整理本周任务优先级」将在今天 18:00 到期，建议先处理高优先级项。',
      createdAt: createIsoOffset(now, -150),
      readAt: createIsoOffset(now, -90),
      taskId: 'task-demo-3',
    },
    {
      id: 'demo-task-reminder-4',
      kind: 'task-reminder',
      level: 'info',
      title: '移动端适配检查待确认',
      message: '任务「通知页移动端适配」还未关闭，建议在窄屏下检查按钮换行和卡片堆叠效果。',
      createdAt: createIsoOffset(now, -360),
      readAt: createIsoOffset(now, -320),
      taskId: 'task-demo-4',
    },
    {
      id: 'demo-task-reminder-5',
      kind: 'task-reminder',
      level: 'warning',
      title: '任务中心存在 2 条未处理提醒',
      message: '你有两条新的任务提醒尚未处理，建议从通知中心直接跳转到任务列表继续操作。',
      createdAt: createIsoOffset(now, -720),
      taskId: 'task-demo-5',
    },
  ])
}

export const useNotificationStore = defineStore(
  'notifications',
  () => {
    const notifications = ref<AppNotification[]>([])

    const orderedNotifications = computed(() => sortNotifications(notifications.value))

    const unreadNotifications = computed(() =>
      orderedNotifications.value.filter((item) => !item.readAt)
    )

    const unreadCount = computed(() => unreadNotifications.value.length)

    function ensureDemoNotifications() {
      if (!ENABLE_NOTIFICATION_DEMO) return
      if (notifications.value.length > 0) return

      notifications.value = buildDemoNotifications()
    }

    function replaceWithDemoNotifications() {
      notifications.value = buildDemoNotifications()
    }

    function upsertNotification(notification: AppNotification) {
      const index = notifications.value.findIndex((item) => item.id === notification.id)

      if (index === -1) {
        notifications.value.unshift(notification)
      } else {
        notifications.value[index] = {
          ...notifications.value[index],
          ...notification,
          readAt: notifications.value[index].readAt,
        }
      }

      if (notifications.value.length > MAX_NOTIFICATION_COUNT) {
        notifications.value = sortNotifications(notifications.value).slice(0, MAX_NOTIFICATION_COUNT)
      }
    }

    function markRead(id: string) {
      const index = notifications.value.findIndex((item) => item.id === id)
      if (index === -1) return
      if (notifications.value[index].readAt) return

      notifications.value[index] = {
        ...notifications.value[index],
        readAt: new Date().toISOString(),
      }
    }

    function markAllRead() {
      const now = new Date().toISOString()

      notifications.value = notifications.value.map((item) =>
        item.readAt
          ? item
          : {
              ...item,
              readAt: now,
            }
      )
    }

    function removeNotification(id: string) {
      notifications.value = notifications.value.filter((item) => item.id !== id)
    }

    function clearNotifications() {
      notifications.value = []
    }

    return {
      notifications,
      orderedNotifications,
      unreadNotifications,
      unreadCount,
      ensureDemoNotifications,
      replaceWithDemoNotifications,
      upsertNotification,
      markRead,
      markAllRead,
      removeNotification,
      clearNotifications,
    }
  },
  { persist: true }
)