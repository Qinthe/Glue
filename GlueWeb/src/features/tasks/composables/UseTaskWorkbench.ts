import { computed, ref, type Ref } from 'vue'
import { TaskStatus, type TaskItem } from '@/types/Index'

interface TaskGroup {
  key: string
  title: string
  tasks: TaskItem[]
}

export type StatusFilter = 'all' | TaskStatus
export type ReminderFilter = 'all' | 'enabled' | 'disabled'
export type TimeFilter = 'all' | 'today' | 'week' | 'overdue'
export type SortMode = 'start-desc' | 'start-asc' | 'updated-desc' | 'progress-desc'
export type GroupFilter = string[]

export interface TaskTreeNode {
  key: string
  label: string
  count?: number | null
  children?: TaskTreeNode[]
}

export interface TaskSummary {
  total: number
  overdue: number
  today: number
  completed: number
}

interface UseTaskWorkbenchOptions {
  activeGroups: Ref<TaskGroup[]>
  archivedGroups: Ref<TaskGroup[]>
  resolveTaskGroupNames: (task: TaskItem) => string[]
}

export function useTaskWorkbench(options: UseTaskWorkbenchOptions) {
  const treeKey = ref('all')
  const keyword = ref('')
  const statusFilter = ref<StatusFilter>('all')
  const reminderFilter = ref<ReminderFilter>('all')
  const timeFilter = ref<TimeFilter>('all')
  const sortMode = ref<SortMode>('start-desc')
  const groupFilter = ref<GroupFilter>([])
  const dateRange = ref<[Date, Date] | null>(null)

  const activeTasks = computed(() =>
    options.activeGroups.value.flatMap((group) => group.tasks)
  )

  const archivedTasks = computed(() =>
    options.archivedGroups.value.flatMap((group) => group.tasks)
  )

  const allTasks = computed(() => [...activeTasks.value, ...archivedTasks.value])

  function toTime(value?: string) {
    if (!value) return 0
    const date = new Date(value)
    return Number.isNaN(date.getTime()) ? 0 : date.getTime()
  }

  function isToday(task: TaskItem) {
    const date = new Date(task.startAt)
    const now = new Date()

    return (
      date.getFullYear() === now.getFullYear() &&
      date.getMonth() === now.getMonth() &&
      date.getDate() === now.getDate()
    )
  }

  function isThisWeek(task: TaskItem) {
    const date = new Date(task.startAt)
    if (Number.isNaN(date.getTime())) return false

    const now = new Date()
    const currentDay = now.getDay() || 7
    const monday = new Date(now)
    monday.setHours(0, 0, 0, 0)
    monday.setDate(now.getDate() - currentDay + 1)

    const sunday = new Date(monday)
    sunday.setDate(monday.getDate() + 6)
    sunday.setHours(23, 59, 59, 999)

    return date.getTime() >= monday.getTime() && date.getTime() <= sunday.getTime()
  }

  function isOverdue(task: TaskItem) {
    if (task.status === TaskStatus.Completed) return false
    return toTime(task.endAt) < Date.now()
  }

  function filterByTree(task: TaskItem) {
    switch (treeKey.value) {
      case 'active':
        return task.status !== TaskStatus.Completed
      case 'pending':
        return task.status === TaskStatus.Pending
      case 'in-progress':
        return task.status === TaskStatus.InProgress
      case 'completed':
        return task.status === TaskStatus.Completed
      case 'today':
        return isToday(task)
      case 'week':
        return isThisWeek(task)
      case 'overdue':
        return isOverdue(task)
      case 'no-reminder':
        return !task.reminderEnabled
      default:
        return true
    }
  }

  function filterByTime(task: TaskItem) {
    switch (timeFilter.value) {
      case 'today':
        return isToday(task)
      case 'week':
        return isThisWeek(task)
      case 'overdue':
        return isOverdue(task)
      default:
        return true
    }
  }

  function filterByDateRange(task: TaskItem) {
    if (!dateRange.value) return true

    const [start, end] = dateRange.value
    const taskStart = toTime(task.startAt)

    return taskStart >= start.getTime() && taskStart <= end.getTime()
  }

  const filteredTasks = computed(() => {
    const text = keyword.value.trim().toLowerCase()

    const result = allTasks.value
      .filter(filterByTree)
      .filter((task) => {
        const groupNames = options.resolveTaskGroupNames(task)

        if (!text) return true

        return [task.title, task.description || '', ...groupNames]
          .join(' ')
          .toLowerCase()
          .includes(text)
      })
      .filter((task) => {
        if (statusFilter.value === 'all') return true
        return task.status === statusFilter.value
      })
      .filter((task) => {
        if (reminderFilter.value === 'all') return true
        return reminderFilter.value === 'enabled'
          ? task.reminderEnabled
          : !task.reminderEnabled
      })
      .filter((task) => {
        if (groupFilter.value.length === 0) return true

        const groupNames = options.resolveTaskGroupNames(task)
        return groupFilter.value.some((groupName) => groupNames.includes(groupName))
      })
      .filter(filterByTime)
      .filter(filterByDateRange)

    return [...result].sort((left, right) => {
      if (sortMode.value === 'start-asc') {
        return toTime(left.startAt) - toTime(right.startAt)
      }

      if (sortMode.value === 'updated-desc') {
        return toTime(right.updatedAt) - toTime(left.updatedAt)
      }

      if (sortMode.value === 'progress-desc') {
        return right.progress - left.progress
      }

      return toTime(right.startAt) - toTime(left.startAt)
    })
  })

  const treeData = computed<TaskTreeNode[]>(() => [
    {
      key: 'all',
      label: '全部任务',
      count: allTasks.value.length,
      children: [
        {
          key: 'active',
          label: '未完成',
          count: allTasks.value.filter((task) => task.status !== TaskStatus.Completed).length,
        },
        {
          key: 'pending',
          label: '待开始',
          count: allTasks.value.filter((task) => task.status === TaskStatus.Pending).length,
        },
        {
          key: 'in-progress',
          label: '进行中',
          count: allTasks.value.filter((task) => task.status === TaskStatus.InProgress).length,
        },
        {
          key: 'completed',
          label: '已归档',
          count: allTasks.value.filter((task) => task.status === TaskStatus.Completed).length,
        },
      ],
    },
    {
      key: 'schedule-root',
      label: '时间视图',
      count: null,
      children: [
        {
          key: 'today',
          label: '今天',
          count: allTasks.value.filter(isToday).length,
        },
        {
          key: 'week',
          label: '本周',
          count: allTasks.value.filter(isThisWeek).length,
        },
        {
          key: 'overdue',
          label: '逾期',
          count: allTasks.value.filter(isOverdue).length,
        },
        {
          key: 'no-reminder',
          label: '无提醒',
          count: allTasks.value.filter((task) => !task.reminderEnabled).length,
        },
      ],
    },
  ])

  const summary = computed<TaskSummary>(() => ({
    total: filteredTasks.value.length,
    overdue: filteredTasks.value.filter(isOverdue).length,
    today: filteredTasks.value.filter(isToday).length,
    completed: filteredTasks.value.filter((task) => task.status === TaskStatus.Completed).length,
  }))

  function resetFilters() {
    keyword.value = ''
    statusFilter.value = 'all'
    reminderFilter.value = 'all'
    timeFilter.value = 'all'
    sortMode.value = 'start-desc'
    groupFilter.value = []
    dateRange.value = null
    treeKey.value = 'all'
  }

  function handleTreeNodeClick(key: string) {
    if (key === 'schedule-root') return
    treeKey.value = key
  }

  function resolveRowClassName({ row }: { row: TaskItem }) {
    if (row.status === TaskStatus.Completed) return 'task-row--completed'
    if (isOverdue(row)) return 'task-row--overdue'
    return ''
  }

  return {
    treeKey,
    keyword,
    statusFilter,
    reminderFilter,
    timeFilter,
    sortMode,
    groupFilter,
    dateRange,
    activeTasks,
    archivedTasks,
    allTasks,
    filteredTasks,
    treeData,
    summary,
    isToday,
    isThisWeek,
    isOverdue,
    resetFilters,
    handleTreeNodeClick,
    resolveRowClassName,
  }
}