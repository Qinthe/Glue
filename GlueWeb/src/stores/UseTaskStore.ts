import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { taskApi } from '@/api/TaskAPI'
import type {
  CreateTaskRequest,
  TaskItem,
  UpdateTaskRequest,
} from '@/types/Index'
import { TaskStatus } from '@/types/Index'

export const useTaskStore = defineStore('tasks', () => {
  const tasks = ref<TaskItem[]>([])
  const isLoading = ref(false)

  const activeTasks = computed(() =>
    tasks.value.filter((task) => task.status !== TaskStatus.Completed)
  )

  const archivedTasks = computed(() =>
    tasks.value.filter((task) => task.status === TaskStatus.Completed)
  )

  async function fetchTasks() {
    isLoading.value = true

    try {
      tasks.value = await taskApi.getAll()
    } finally {
      isLoading.value = false
    }
  }

  async function addTask(req: CreateTaskRequest) {
    const task = await taskApi.create(req)
    tasks.value.unshift(task)
    return task
  }

  async function updateTask(id: string, req: UpdateTaskRequest) {
    const updatedTask = await taskApi.update(id, req)
    const index = tasks.value.findIndex((task) => task.id === id)

    if (index !== -1) {
      tasks.value[index] = updatedTask
    }

    return updatedTask
  }

  async function setTaskProgress(id: string, progress: number) {
    return updateTask(id, { progress })
  }

  async function completeTask(id: string) {
    const updatedTask = await taskApi.complete(id)
    const index = tasks.value.findIndex((task) => task.id === id)

    if (index !== -1) {
      tasks.value[index] = updatedTask
    }

    return updatedTask
  }

  async function updateReminder(
    id: string,
    data: Partial<
      Pick<
        UpdateTaskRequest,
        'reminderEnabled' | 'reminderMinutesBefore' | 'lastReminderAt'
      >
    >
  ) {
    const updatedTask = await taskApi.updateReminder(id, data)
    const index = tasks.value.findIndex((task) => task.id === id)

    if (index !== -1) {
      tasks.value[index] = updatedTask
    }

    return updatedTask
  }

  async function markReminderSent(id: string) {
    return updateReminder(id, {
      lastReminderAt: new Date().toISOString(),
    })
  }

  async function resetReminder(id: string) {
    return updateReminder(id, {
      lastReminderAt: undefined,
    })
  }

  async function removeTask(id: string) {
    await taskApi.remove(id)
    tasks.value = tasks.value.filter((task: TaskItem) => task.id !== id)
  }

  return {
    tasks,
    isLoading,
    activeTasks,
    archivedTasks,
    fetchTasks,
    addTask,
    updateTask,
    setTaskProgress,
    completeTask,
    updateReminder,
    markReminderSent,
    resetReminder,
    removeTask,
  }
})