<script setup lang="ts">
import '../styles/TasksGanttPage.css'
import { computed, ref } from 'vue'
import { Calendar, EditPen } from '@element-plus/icons-vue'
import type { TaskItem } from '@/types/Index'
import { useTasksGanttBoard } from '../composables/UseTasksGanttBoard'

const props = defineProps<{
  tasks: TaskItem[]
  formatTaskRange: (task: TaskItem) => string
  resolveStatusText: (task: TaskItem) => string
}>()

const emit = defineEmits<{
  edit: [task: TaskItem]
}>()

const gantt = useTasksGanttBoard(
  computed(() => props.tasks)
)

const sidebarBodyRef = ref<HTMLElement | null>(null)
const timelineScrollerRef = ref<HTMLElement | null>(null)

let syncingScroll = false

function syncSidebarFromTimeline() {
  if (syncingScroll) return
  if (!sidebarBodyRef.value || !timelineScrollerRef.value) return

  syncingScroll = true
  sidebarBodyRef.value.scrollTop = timelineScrollerRef.value.scrollTop
  requestAnimationFrame(() => {
    syncingScroll = false
  })
}

function syncTimelineFromSidebar() {
  if (syncingScroll) return
  if (!sidebarBodyRef.value || !timelineScrollerRef.value) return

  syncingScroll = true
  timelineScrollerRef.value.scrollTop = sidebarBodyRef.value.scrollTop
  requestAnimationFrame(() => {
    syncingScroll = false
  })
}
</script>

<template>
  <div class="tasks-gantt-board">
    <div v-if="gantt.rows.value.length" class="tasks-gantt-board__body glue-panel">
      <div class="tasks-gantt-board__grid">
        <div class="tasks-gantt-board__sidebar">
          <div class="tasks-gantt-board__sidebar-head">任务</div>

          <div
            ref="sidebarBodyRef"
            class="tasks-gantt-board__sidebar-body"
            @scroll="syncTimelineFromSidebar"
          >
            <div
              v-for="row in gantt.rows.value"
              :key="row.task.id"
              class="tasks-gantt-board__task"
            >
              <div class="tasks-gantt-board__task-title">
                <strong>{{ row.task.title }}</strong>
                <el-tag size="small" round>{{ props.resolveStatusText(row.task) }}</el-tag>
              </div>
              <div class="tasks-gantt-board__task-meta">
                <el-icon><Calendar /></el-icon>
                <span>{{ props.formatTaskRange(row.task) }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="tasks-gantt-board__timeline-panel">
          <div
            ref="timelineScrollerRef"
            class="tasks-gantt-board__timeline-scroller"
            @scroll="syncSidebarFromTimeline"
          >
            <div
              class="tasks-gantt-board__timeline"
              :style="{ '--gantt-columns': String(gantt.columns.value.length) }"
            >
              <div class="tasks-gantt-board__header">
                <div
                  v-for="column in gantt.columns.value"
                  :key="column.key"
                  class="tasks-gantt-board__header-cell"
                  :class="{ 'is-today': column.isToday }"
                >
                  <span>{{ column.label }}</span>
                </div>
              </div>

              <div class="tasks-gantt-board__rows">
                <div
                  v-for="row in gantt.rows.value"
                  :key="row.task.id"
                  class="tasks-gantt-board__row"
                >
                  <div
                    v-for="column in gantt.columns.value"
                    :key="column.key"
                    class="tasks-gantt-board__cell"
                    :class="{ 'is-today': column.isToday }"
                  />
                  <button
                    type="button"
                    class="tasks-gantt-board__bar"
                    :style="{
                      left: `${row.left}%`,
                      width: `${row.width}%`,
                      backgroundColor: row.color,
                    }"
                    @click="emit('edit', row.task)"
                  >
                    <span>{{ row.task.title }}</span>
                    <el-icon><EditPen /></el-icon>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <el-empty
      v-else
      description="当前条件下没有可展示的任务"
      :image-size="92"
      class="glue-panel"
    />
  </div>
</template>