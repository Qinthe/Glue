import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useTaskStore } from '@/stores/UseTaskStore'
import { useTaskGroupStore } from '@/stores/UseTaskGroupStore'
import { TaskStatus, type TaskItem } from '@/types/Index'

interface TaskGroup {
  key: string
  title: string
  tasks: TaskItem[]
}

type ViewMode = 'list' | 'create' | 'edit'

function pad(value: number) {
  return String(value).padStart(2, '0')
}

function formatDateTimeInput(date: Date) {
  return [
    date.getFullYear(),
    '-',
    pad(date.getMonth() + 1),
    '-',
    pad(date.getDate()),
    'T',
    pad(date.getHours()),
    ':',
    pad(date.getMinutes()),
  ].join('')
}

function parseDateTime(value?: string) {
  if (!value) return null

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return null
  }

  return date
}

function buildDefaultRange() {
  const start = new Date()
  start.setSeconds(0, 0)
  start.setMinutes(0)
  start.setHours(start.getHours() + 1)

  const end = new Date(start.getTime() + 60 * 60 * 1000)

  return {
    startAt: start,
    endAt: end,
  }
}

function createEmptyTaskForm() {
  const range = buildDefaultRange()

  return {
    title: '',
    description: '',
    groupNames: [] as string[],
    startAt: range.startAt as Date | null,
    endAt: range.endAt as Date | null,
    progress: 0,
    reminderEnabled: true,
    reminderMinutesBefore: 30,
  }
}

export function useTasksPage() {
  const router = useRouter()
  const route = useRoute()
  const taskStore = useTaskStore()
  const groupStore = useTaskGroupStore()
  const { t, locale } = useI18n()

  onMounted(async () => {
    try {
      await taskStore.fetchTasks()
    } catch (error) {
      ElMessage.error(error instanceof Error ? error.message : t('common.loadFailed'))
    }
  })

  const viewMode = ref<ViewMode>('list')
  const selectedTaskId = ref<string | null>(null)
  const archiveVisible = ref(false)
  const activeOpenGroupNames = ref<string[]>([])
  const archivedOpenGroupNames = ref<string[]>([])

  const taskForm = reactive(createEmptyTaskForm())

  const currentBoardView = computed<'tasks' | 'gantt' | 'groups'>(() => {
    const view = route.query.view
    return view === 'gantt' || view === 'groups' ? view : 'tasks'
  })

  const selectedTask = computed(() =>
    taskStore.tasks.find((item) => item.id === selectedTaskId.value) ?? null
  )

  const isEditorView = computed(() =>
    viewMode.value === 'create' || viewMode.value === 'edit'
  )

  const editorActionText = computed(() =>
    viewMode.value === 'edit' ? t('common.saveChanges') : t('tasks.createTask')
  )

  const activeTaskCount = computed(() => taskStore.activeTasks.length)
  const archivedTaskCount = computed(() => taskStore.archivedTasks.length)

  const taskGroupOptions = computed(() =>
    groupStore.sortedGroups.map((group) => ({
      label: group.name,
      value: group.name,
    }))
  )

  const taskGroupNameMap = computed(() => {
    const entries = groupStore.sortedGroups.map((group) => [group.id, group.name] as const)
    return new Map(entries)
  })

  const taskGroupIdsMap = computed(() => {
    const result = new Map<string, string[]>()

    for (const link of groupStore.links) {
      const current = result.get(link.taskId) ?? []
      current.push(link.groupId)
      result.set(link.taskId, current)
    }

    return result
  })

  const todayTaskCount = computed(() => {
    const today = new Date()
    const todayKey = [
      today.getFullYear(),
      pad(today.getMonth() + 1),
      pad(today.getDate()),
    ].join('-')

    return taskStore.activeTasks.filter((task) => task.startAt.startsWith(todayKey)).length
  })

  function switchBoardView(target: 'tasks' | 'gantt' | 'groups') {
    router.push({
      name: 'tasks',
      query: target === 'tasks' ? {} : { view: target },
    })
  }

  function resetTaskForm() {
    Object.assign(taskForm, createEmptyTaskForm())
  }

  function getTaskGroupNames(task: TaskItem) {
    const groupIds = taskGroupIdsMap.value.get(task.id) ?? []
    return groupIds
      .map((groupId) => taskGroupNameMap.value.get(groupId))
      .filter((value): value is string => Boolean(value))
  }

  function fillTaskForm(task: TaskItem) {
    Object.assign(taskForm, {
      title: task.title,
      description: task.description || '',
      groupNames: getTaskGroupNames(task),
      startAt: parseDateTime(task.startAt),
      endAt: parseDateTime(task.endAt),
      progress: task.progress,
      reminderEnabled: task.reminderEnabled,
      reminderMinutesBefore: task.reminderMinutesBefore,
    })
  }

  function syncTaskGroups(taskId: string, nextGroupNames: string[]) {
    const normalizedNames = [...new Set(
      nextGroupNames
        .map((item) => item.trim())
        .filter(Boolean)
    )]

    const nextGroups = normalizedNames
      .map((name) => groupStore.ensureGroup(name))
      .filter((group): group is NonNullable<typeof group> => Boolean(group))

    const nextGroupIds = nextGroups.map((group) => group.id)
    const currentGroupIds = taskGroupIdsMap.value.get(taskId) ?? []

    for (const groupId of currentGroupIds) {
      if (!nextGroupIds.includes(groupId)) {
        groupStore.removeTaskFromGroup(taskId, groupId)
      }
    }

    for (const groupId of nextGroupIds) {
      if (!currentGroupIds.includes(groupId)) {
        groupStore.assignTaskToGroup(taskId, groupId)
      }
    }
  }

  function formatDateLabel(value: string) {
    const date = new Date(`${value}T00:00:00`)
    if (Number.isNaN(date.getTime())) return value

    return new Intl.DateTimeFormat(locale.value, {
      month: 'long',
      day: 'numeric',
      weekday: 'short',
    }).format(date)
  }

  function formatDateTime(value?: string) {
    if (!value) return t('tasks.unfinished')

    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return value

    return new Intl.DateTimeFormat(locale.value, {
      month: 'numeric',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date)
  }

  function formatTaskRange(task: TaskItem) {
    const start = new Date(task.startAt)
    const end = new Date(task.endAt)

    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      return `${task.startAt} - ${task.endAt}`
    }

    const dateFormatter = new Intl.DateTimeFormat(locale.value, {
      month: 'numeric',
      day: 'numeric',
    })

    const timeFormatter = new Intl.DateTimeFormat(locale.value, {
      hour: '2-digit',
      minute: '2-digit',
    })

    if (start.toDateString() === end.toDateString()) {
      return `${dateFormatter.format(start)} ${timeFormatter.format(start)} - ${timeFormatter.format(end)}`
    }

    return `${dateFormatter.format(start)} ${timeFormatter.format(start)} - ${dateFormatter.format(end)} ${timeFormatter.format(end)}`
  }

  function resolveReminderText(task: TaskItem) {
    if (!task.reminderEnabled) {
      return t('tasks.reminderOff')
    }

    return t('tasks.reminderMinutesValue', {
      minutes: task.reminderMinutesBefore,
    })
  }

  function resolveStatusText(task: TaskItem) {
    if (task.status === TaskStatus.Completed) return t('tasks.statusCompleted')
    if (task.status === TaskStatus.InProgress) return t('tasks.statusInProgress')
    return t('tasks.statusPending')
  }

  function resolveStatusType(task: TaskItem) {
    if (task.status === TaskStatus.Completed) return 'success'
    if (task.status === TaskStatus.InProgress) return 'warning'
    return 'info'
  }

  function groupTasks(tasks: TaskItem[], source: 'startAt' | 'completedAt', desc = false) {
    const groups = new Map<string, TaskGroup>()

    for (const task of tasks) {
      const key =
        source === 'startAt'
          ? task.startAt.slice(0, 10)
          : task.completedAt?.slice(0, 10) || '__unarchived__'

      if (!groups.has(key)) {
        groups.set(key, {
          key,
          title: key === '__unarchived__' ? t('tasks.unarchived') : formatDateLabel(key),
          tasks: [],
        })
      }

      groups.get(key)!.tasks.push(task)
    }

    return [...groups.values()].sort((left, right) =>
      desc ? right.key.localeCompare(left.key) : left.key.localeCompare(right.key)
    )
  }

  const activeGroups = computed(() =>
    groupTasks(
      [...taskStore.activeTasks].sort((left, right) => right.startAt.localeCompare(left.startAt)),
      'startAt',
      true
    )
  )

  const archivedGroups = computed(() =>
    groupTasks(
      [...taskStore.archivedTasks].sort((left, right) =>
        (right.completedAt || '').localeCompare(left.completedAt || '')
      ),
      'completedAt',
      true
    )
  )

  function getTodayGroupKey() {
    const now = new Date()

    return [
      now.getFullYear(),
      pad(now.getMonth() + 1),
      pad(now.getDate()),
    ].join('-')
  }

  function isTodayGroup(key: string) {
    return key === getTodayGroupKey()
  }

  watch(
    activeGroups,
    (groups) => {
      const keys = new Set(groups.map((group) => group.key))
      activeOpenGroupNames.value = activeOpenGroupNames.value.filter((key) => keys.has(key))

      if (activeOpenGroupNames.value.length === 0) {
        activeOpenGroupNames.value = groups
          .filter((group, index) => index === 0 || isTodayGroup(group.key))
          .map((group) => group.key)
      }
    },
    { immediate: true }
  )

  watch(
    archivedGroups,
    (groups) => {
      const keys = new Set(groups.map((group) => group.key))
      archivedOpenGroupNames.value = archivedOpenGroupNames.value.filter((key) => keys.has(key))

      if (archivedOpenGroupNames.value.length === 0) {
        archivedOpenGroupNames.value = groups.slice(0, 2).map((group) => group.key)
      }
    },
    { immediate: true }
  )

  function toggleArchivePanel() {
    archiveVisible.value = !archiveVisible.value
  }

  function closeArchivePanel() {
    archiveVisible.value = false
  }

  function goBackToList() {
    viewMode.value = 'list'
    selectedTaskId.value = null
    resetTaskForm()
  }

  function openCreatePage() {
    viewMode.value = 'create'
    selectedTaskId.value = null
    closeArchivePanel()
    resetTaskForm()
  }

  function openEditPage(task: TaskItem) {
    viewMode.value = 'edit'
    selectedTaskId.value = task.id
    closeArchivePanel()
    fillTaskForm(task)
  }

  function validateTaskForm() {
    const title = taskForm.title.trim()

    if (!title) {
      ElMessage.warning(t('tasks.validationTitle'))
      return false
    }

    if (!taskForm.startAt || !taskForm.endAt) {
      ElMessage.warning(t('tasks.validationTime'))
      return false
    }

    if (taskForm.endAt.getTime() <= taskForm.startAt.getTime()) {
      ElMessage.warning(t('tasks.validationTimeRange'))
      return false
    }

    if (taskForm.reminderEnabled && taskForm.reminderMinutesBefore < 1) {
      ElMessage.warning(t('tasks.validationReminder'))
      return false
    }

    return true
  }

  async function submitTask() {
    if (!validateTaskForm()) {
      return
    }

    const startAt = formatDateTimeInput(taskForm.startAt!)
    const endAt = formatDateTimeInput(taskForm.endAt!)

    const payload = {
      title: taskForm.title.trim(),
      description: taskForm.description.trim() || undefined,
      scheduledDate: startAt.slice(0, 10),
      startAt,
      endAt,
      progress: taskForm.progress,
      status: taskForm.progress >= 100
        ? TaskStatus.Completed
        : taskForm.progress > 0
          ? TaskStatus.InProgress
          : TaskStatus.Pending,
      reminderEnabled: taskForm.reminderEnabled,
      reminderMinutesBefore: taskForm.reminderMinutesBefore,
    }
    try {
      if (viewMode.value === 'edit' && selectedTaskId.value) {
        await taskStore.updateTask(selectedTaskId.value, payload)
        syncTaskGroups(selectedTaskId.value, taskForm.groupNames)
        goBackToList()

        ElMessage.success(
          payload.progress >= 100
            ? t('tasks.updatedAndArchived')
            : t('tasks.updated')
        )
        return
      }

      const task = await taskStore.addTask(payload)
      syncTaskGroups(task.id, taskForm.groupNames)
      goBackToList()

      ElMessage.success(
        payload.progress >= 100
          ? t('tasks.createdAndArchived')
          : t('tasks.created')
      )
    } catch (error) {
      ElMessage.error(
        error instanceof Error ? error.message : '保存任务失败，请稍后重试'
      )
    }
  }

  async function completeTask(task: TaskItem) {
    try {
      await taskStore.completeTask(task.id)

      if (selectedTaskId.value === task.id) {
        goBackToList()
      }

      ElMessage.success(t('tasks.completedAndArchived'))
    } catch (error) {
      ElMessage.error(
        error instanceof Error ? error.message : '完成任务失败，请稍后重试'
      )
    }
  }

  async function removeTask(id: string) {
    try {
      await taskStore.removeTask(id)

      if (selectedTaskId.value === id) {
        goBackToList()
      }

      ElMessage.success(t('tasks.deleted'))
    } catch (error) {
      ElMessage.error(
        error instanceof Error ? error.message : '删除任务失败，请稍后重试'
      )
    }
  }

  return {
    t,
    viewMode,
    selectedTaskId,
    selectedTask,
    archiveVisible,
    activeOpenGroupNames,
    archivedOpenGroupNames,
    taskForm,
    currentBoardView,
    isEditorView,
    editorActionText,
    activeTaskCount,
    archivedTaskCount,
    todayTaskCount,
    activeGroups,
    archivedGroups,
    taskGroupOptions,
    getTaskGroupNames,
    switchBoardView,
    toggleArchivePanel,
    closeArchivePanel,
    goBackToList,
    openCreatePage,
    openEditPage,
    submitTask,
    completeTask,
    removeTask,
    formatDateTime,
    formatTaskRange,
    resolveReminderText,
    resolveStatusText,
    resolveStatusType,
    isTodayGroup,
  }
}