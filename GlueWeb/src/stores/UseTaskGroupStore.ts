import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

export interface TaskGroupItem {
  id: string
  name: string
  color: string
  description?: string
  sortOrder: number
  createdAt: string
}

export interface TaskGroupLink {
  groupId: string
  taskId: string
}

const GROUP_COLOR_PALETTE = [
  '#8b5cf6',
  '#f59e0b',
  '#3b82f6',
  '#10b981',
  '#ef4444',
  '#06b6d4',
]

function normalizeGroupName(value: string) {
  return value.trim()
}

export const useTaskGroupStore = defineStore(
  'task-groups',
  () => {
    const groups = ref<TaskGroupItem[]>([
      {
        id: 'group-product',
        name: '产品规划',
        color: '#8b5cf6',
        description: '需求、评审与方案沉淀',
        sortOrder: 1,
        createdAt: new Date().toISOString(),
      },
      {
        id: 'group-design',
        name: 'UI/UX 设计',
        color: '#f59e0b',
        description: '交互、视觉与设计交付',
        sortOrder: 2,
        createdAt: new Date().toISOString(),
      },
      {
        id: 'group-engineering',
        name: '研发实现',
        color: '#3b82f6',
        description: '开发、联调与交付',
        sortOrder: 3,
        createdAt: new Date().toISOString(),
      },
    ])

    const links = ref<TaskGroupLink[]>([
      { groupId: 'group-product', taskId: 'task-wireframe' },
      { groupId: 'group-engineering', taskId: 'task-api' },
      { groupId: 'group-design', taskId: 'task-review' },
    ])

    const sortedGroups = computed(() =>
      [...groups.value].sort((a, b) => a.sortOrder - b.sortOrder)
    )

    function getTaskIdsByGroup(groupId: string) {
      return links.value
        .filter((item) => item.groupId === groupId)
        .map((item) => item.taskId)
    }

    function findGroupByName(name: string) {
      const normalizedName = normalizeGroupName(name)
      if (!normalizedName) return null

      return groups.value.find((item) => item.name === normalizedName) ?? null
    }

    function createGroup(name: string) {
      const normalizedName = normalizeGroupName(name)
      if (!normalizedName) return null

      const exists = findGroupByName(normalizedName)
      if (exists) {
        return exists
      }

      const group: TaskGroupItem = {
        id: crypto.randomUUID(),
        name: normalizedName,
        color: GROUP_COLOR_PALETTE[groups.value.length % GROUP_COLOR_PALETTE.length],
        description: '',
        sortOrder: groups.value.length
          ? Math.max(...groups.value.map((item) => item.sortOrder)) + 1
          : 1,
        createdAt: new Date().toISOString(),
      }

      groups.value.push(group)
      return group
    }

    function ensureGroup(name: string) {
      return createGroup(name)
    }

    function assignTaskToGroup(taskId: string, groupId: string) {
      const exists = links.value.some((item) => item.taskId === taskId && item.groupId === groupId)
      if (!exists) {
        links.value.push({ taskId, groupId })
      }
    }

    function removeTaskFromGroup(taskId: string, groupId: string) {
      links.value = links.value.filter((item) => !(item.taskId === taskId && item.groupId === groupId))
    }

    return {
      groups,
      links,
      sortedGroups,
      getTaskIdsByGroup,
      findGroupByName,
      createGroup,
      ensureGroup,
      assignTaskToGroup,
      removeTaskFromGroup,
    }
  },
  { persist: true }
)