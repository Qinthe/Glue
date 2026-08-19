import { computed, type ComputedRef } from 'vue'
import type { TaskItem } from '@/types/Index'

interface GanttColumn {
  key: string
  label: string
  isToday: boolean
  date: Date
}

interface GanttRow {
  task: TaskItem
  left: number
  width: number
  color: string
}

function startOfDay(date: Date) {
  const value = new Date(date)
  value.setHours(0, 0, 0, 0)
  return value
}

function endOfDay(date: Date) {
  const value = new Date(date)
  value.setHours(23, 59, 59, 999)
  return value
}

function addDays(date: Date, days: number) {
  const value = new Date(date)
  value.setDate(value.getDate() + days)
  return value
}

function toDate(value?: string) {
  if (!value) return null
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

function resolveColor(progress: number) {
  if (progress >= 100) return '#16a34a'
  if (progress >= 60) return '#2563eb'
  if (progress >= 30) return '#f59e0b'
  return '#94a3b8'
}

export function useTasksGanttBoard(tasks: ComputedRef<TaskItem[]>) {
  const range = computed(() => {
    const dates = tasks.value.flatMap((task) => {
      const start = toDate(task.startAt)
      const end = toDate(task.endAt)
      return [start, end].filter(Boolean) as Date[]
    })

    const today = new Date()

    if (!dates.length) {
      const start = startOfDay(addDays(today, -3))
      const end = endOfDay(addDays(today, 10))
      return { start, end }
    }

    const sorted = [...dates].sort((a, b) => a.getTime() - b.getTime())
    const start = startOfDay(addDays(sorted[0], -3))
    const end = endOfDay(addDays(sorted[sorted.length - 1], 10))
    return { start, end }
  })

  const columns = computed<GanttColumn[]>(() => {
    const result: GanttColumn[] = []
    const todayKey = startOfDay(new Date()).toISOString().slice(0, 10)
    const cursor = new Date(range.value.start)

    while (cursor <= range.value.end) {
      const key = cursor.toISOString().slice(0, 10)
      result.push({
        key,
        date: new Date(cursor),
        label: `${cursor.getMonth() + 1}/${cursor.getDate()}`,
        isToday: key === todayKey,
      })
      cursor.setDate(cursor.getDate() + 1)
    }

    return result
  })

  const rows = computed<GanttRow[]>(() => {
    const rangeStart = range.value.start.getTime()
    const rangeEnd = range.value.end.getTime()
    const totalRange = Math.max(1, rangeEnd - rangeStart)
    const minWidth = columns.value.length ? 100 / columns.value.length : 0

    return tasks.value
      .map((task) => {
        const start = toDate(task.startAt)
        const end = toDate(task.endAt)

        if (!start || !end) return null

        const taskStart = Math.max(startOfDay(start).getTime(), rangeStart)
        const taskEnd = Math.min(endOfDay(end).getTime(), rangeEnd)

        const left = ((taskStart - rangeStart) / totalRange) * 100
        const width = Math.max(((taskEnd - taskStart) / totalRange) * 100, minWidth)

        return {
          task,
          left,
          width,
          color: resolveColor(task.progress),
        }
      })
      .filter(Boolean) as GanttRow[]
  })

  return {
    range,
    columns,
    rows,
  }
}