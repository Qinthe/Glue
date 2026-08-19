<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { Bell, Close } from '@element-plus/icons-vue'
import { useTaskStore } from '@/stores/UseTaskStore'
import { useNotificationStore } from '@/stores/UseNotificationStore'
import { TaskStatus, type TaskItem } from '@/types/Index'

interface ReminderToast {
  id: string
  taskId: string
  title: string
  message: string
}

const CHECK_INTERVAL = 30 * 1000
const DISPLAY_DURATION = 5600
const LATE_GRACE_PERIOD = 60 * 1000

const taskStore = useTaskStore()
const notificationStore = useNotificationStore()
const { t, locale } = useI18n()
const toasts = ref<ReminderToast[]>([])
const dismissTimers = new Map<string, number>()

let scanTimer: number | undefined

function formatDateTime(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value

  return new Intl.DateTimeFormat(locale.value, {
    month: 'numeric',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}

function shouldNotify(task: TaskItem) {
  if (task.status === TaskStatus.Completed) return false
  if (!task.reminderEnabled) return false
  if (task.lastReminderAt) return false

  const dueAt = new Date(task.endAt).getTime()
  if (Number.isNaN(dueAt)) return false

  const now = Date.now()
  const remindAt = dueAt - task.reminderMinutesBefore * 60 * 1000

  return now >= remindAt && now <= dueAt + LATE_GRACE_PERIOD
}

function removeToast(id: string) {
  toasts.value = toasts.value.filter((item) => item.id !== id)

  const timer = dismissTimers.get(id)
  if (timer) {
    window.clearTimeout(timer)
    dismissTimers.delete(id)
  }
}

function pushToast(task: TaskItem) {
  const id = `${task.id}-${task.endAt}`
  if (toasts.value.some((item) => item.id === id)) return

  const message = t('tasks.reminderMessage', {
    title: task.title,
    time: formatDateTime(task.endAt),
    minutes: task.reminderMinutesBefore,
  })

  const toast: ReminderToast = {
    id,
    taskId: task.id,
    title: t('tasks.reminderTitle'),
    message,
  }

  toasts.value.unshift(toast)

  notificationStore.upsertNotification({
    id,
    kind: 'task-reminder',
    level: 'warning',
    title: toast.title,
    message: toast.message,
    createdAt: new Date().toISOString(),
    taskId: task.id,
  })

  taskStore.markReminderSent(task.id)

  const timer = window.setTimeout(() => {
    removeToast(id)
  }, DISPLAY_DURATION)

  dismissTimers.set(id, timer)
}

function scanTasks() {
  for (const task of taskStore.activeTasks) {
    if (shouldNotify(task)) {
      pushToast(task)
    }
  }
}

onMounted(() => {
  scanTasks()
  scanTimer = window.setInterval(scanTasks, CHECK_INTERVAL)
})

onBeforeUnmount(() => {
  if (scanTimer) {
    window.clearInterval(scanTimer)
  }

  for (const timer of dismissTimers.values()) {
    window.clearTimeout(timer)
  }

  dismissTimers.clear()
})
</script>

<template>
  <div class="task-reminder-portal">
    <transition-group name="task-toast">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="task-toast"
      >
        <div class="task-toast__glow" />
        <div class="task-toast__icon">
          <el-icon><Bell /></el-icon>
        </div>

        <div class="task-toast__content">
          <strong>{{ toast.title }}</strong>
          <p>{{ toast.message }}</p>
        </div>

        <button
          class="task-toast__close"
          type="button"
          @click="removeToast(toast.id)"
        >
          <el-icon><Close /></el-icon>
        </button>
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.task-reminder-portal {
  position: fixed;
  right: 20px;
  bottom: 20px;
  z-index: 3000;
  pointer-events: none;
}

.task-toast {
  position: relative;
  display: grid;
  grid-template-columns: 40px minmax(0, 1fr) 28px;
  align-items: start;
  gap: 12px;
  width: min(360px, calc(100vw - 32px));
  margin-top: 12px;
  padding: 14px 14px 14px 12px;
  overflow: hidden;
  border: 1px solid rgba(37, 99, 235, 0.22);
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.92);
  box-shadow:
    0 16px 38px rgba(15, 23, 42, 0.18),
    0 2px 10px rgba(37, 99, 235, 0.12);
  backdrop-filter: blur(14px);
  pointer-events: auto;
}

:root.dark .task-toast {
  background: rgba(23, 27, 35, 0.94);
  border-color: rgba(96, 165, 250, 0.26);
}

.task-toast__glow {
  position: absolute;
  inset: -40% auto auto -20%;
  width: 160px;
  height: 160px;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.18), transparent 70%);
  animation: task-toast-glow 2.2s ease-in-out infinite;
  pointer-events: none;
}

.task-toast__icon {
  position: relative;
  z-index: 1;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: linear-gradient(135deg, #2563eb, #60a5fa);
  color: #fff;
  box-shadow: 0 10px 18px rgba(37, 99, 235, 0.24);
}

.task-toast__content {
  position: relative;
  z-index: 1;
  min-width: 0;
}

.task-toast__content strong {
  display: block;
  margin-bottom: 6px;
  font-size: 14px;
  color: var(--glue-text-color);
}

.task-toast__content p {
  margin: 0;
  font-size: 12px;
  line-height: 1.6;
  color: var(--glue-subtext-text-color);
}

.task-toast__close {
  position: relative;
  z-index: 1;
  width: 28px;
  height: 28px;
  border: 0;
  border-radius: 10px;
  background: transparent;
  color: var(--glue-subtext-text-color);
  cursor: pointer;
}

.task-toast__close:hover {
  background: var(--glue-hover-bg);
  color: var(--glue-text-color);
}

.task-toast-enter-active,
.task-toast-leave-active {
  transition:
    transform 0.28s ease,
    opacity 0.22s ease,
    filter 0.22s ease;
}

.task-toast-enter-from,
.task-toast-leave-to {
  opacity: 0;
  transform: translateY(18px) scale(0.96);
  filter: blur(4px);
}

.task-toast-move {
  transition: transform 0.28s ease;
}

@keyframes task-toast-glow {
  0%,
  100% {
    transform: translate3d(0, 0, 0);
    opacity: 0.72;
  }

  50% {
    transform: translate3d(10px, 8px, 0);
    opacity: 1;
  }
}
</style>