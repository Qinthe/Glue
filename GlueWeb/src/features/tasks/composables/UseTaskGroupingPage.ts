import { computed, ref, watch, type Ref } from 'vue'
import type { TaskItem } from '@/types/Index'
import { TaskStatus } from '@/types/Index'
import { useTaskGroupStore, type TaskGroupItem } from '@/stores/UseTaskGroupStore'

const UNGROUPED_ID = '__ungrouped__'

type DisplayGroup = TaskGroupItem & {
  count: number
}

export function useTaskGroupingPage(tasks: Ref<TaskItem[]>) {
  const groupStore = useTaskGroupStore()
  const currentGroupId = ref('')

  const assignedTaskIds = computed(() => new Set(groupStore.links.map((item) => item.taskId)))

  const ungroupedTasks = computed(() =>
    tasks.value.filter((task) => !assignedTaskIds.value.has(task.id))
  )

  const groups = computed<DisplayGroup[]>(() => {
    const mappedGroups = groupStore.sortedGroups.map((group) => {
      const taskIds = new Set(groupStore.getTaskIdsByGroup(group.id))
      const count = tasks.value.filter((task) => taskIds.has(task.id)).length

      return {
        ...group,
        count,
      }
    })

    if (ungroupedTasks.value.length > 0) {
      mappedGroups.push({
        id: UNGROUPED_ID,
        name: '未分组',
        color: '#94a3b8',
        description: '尚未归入业务分组的任务',
        sortOrder: 9999,
        createdAt: '',
        count: ungroupedTasks.value.length,
      })
    }

    return mappedGroups
  })

  watch(
    groups,
    (value) => {
      if (!value.length) {
        currentGroupId.value = ''
        return
      }

      const exists = value.some((group) => group.id === currentGroupId.value)
      if (!exists) {
        currentGroupId.value = value[0].id
      }
    },
    { immediate: true }
  )

  const currentGroup = computed(() =>
    groups.value.find((item) => item.id === currentGroupId.value) ?? null
  )

  const currentTasks = computed(() => {
    if (currentGroupId.value === UNGROUPED_ID) {
      return ungroupedTasks.value
    }

    const taskIds = new Set(groupStore.getTaskIdsByGroup(currentGroupId.value))
    return tasks.value.filter((task) => taskIds.has(task.id))
  })

  const columns = computed(() => [
    {
      key: TaskStatus.Pending,
      title: '待开始',
      tasks: currentTasks.value.filter((task) => task.status === TaskStatus.Pending),
    },
    {
      key: TaskStatus.InProgress,
      title: '进行中',
      tasks: currentTasks.value.filter((task) => task.status === TaskStatus.InProgress),
    },
    {
      key: TaskStatus.Completed,
      title: '已完成',
      tasks: currentTasks.value.filter((task) => task.status === TaskStatus.Completed),
    },
  ])

  return {
    groups,
    currentGroupId,
    currentGroup,
    currentTasks,
    columns,
  }
}