<script setup lang="ts">
import {
  Bell,
  Calendar,
  CircleCheck,
  Delete,
  EditPen,
} from '@element-plus/icons-vue'
import { TaskStatus, type TaskItem } from '@/types/Index'

type TagType = 'primary' | 'success' | 'warning' | 'info' | 'danger'

const props = defineProps<{
  tasks: TaskItem[]
  isOverdue: (task: TaskItem) => boolean
  resolveRowClassName: (payload: { row: TaskItem }) => string
  resolveStatusType: (task: TaskItem) => TagType
  resolveStatusText: (task: TaskItem) => string
  formatTaskRange: (task: TaskItem) => string
  resolveReminderText: (task: TaskItem) => string
  resolveGroupNames: (task: TaskItem) => string[]
  emptyText: string
}>()

const emit = defineEmits<{
  complete: [task: TaskItem]
  edit: [task: TaskItem]
  delete: [id: string]
}>()
</script>

<template>
  <el-empty
    v-if="props.tasks.length === 0"
    :description="props.emptyText"
    :image-size="92"
  />

  <el-table
    v-else
    :data="props.tasks"
    stripe
    table-layout="fixed"
    class="tasks-table"
    :row-class-name="props.resolveRowClassName"
  >
    <el-table-column label="任务" min-width="280">
      <template #default="{ row }">
        <div class="tasks-title-cell">
          <div class="tasks-title-cell__head">
            <strong>{{ row.title }}</strong>
            <el-tag
              v-if="props.isOverdue(row)"
              type="danger"
              size="small"
              round
            >
              逾期
            </el-tag>
          </div>
        </div>
      </template>
    </el-table-column>

    <el-table-column label="分组" min-width="180">
      <template #default="{ row }">
        <div class="tasks-table__meta">
          <el-space wrap>
            <el-tag
              v-for="groupName in props.resolveGroupNames(row)"
              :key="groupName"
              size="small"
              round
            >
              {{ groupName }}
            </el-tag>
            <span v-if="props.resolveGroupNames(row).length === 0">未分组</span>
          </el-space>
        </div>
      </template>
    </el-table-column>

    <el-table-column label="状态" width="120" align="center">
      <template #default="{ row }">
        <el-tag :type="props.resolveStatusType(row)" round>
          {{ props.resolveStatusText(row) }}
        </el-tag>
      </template>
    </el-table-column>

    <el-table-column label="时间安排" min-width="220">
      <template #default="{ row }">
        <div class="tasks-table__meta">
          <span>
            <el-icon><Calendar /></el-icon>
            {{ props.formatTaskRange(row) }}
          </span>
        </div>
      </template>
    </el-table-column>

    <el-table-column label="提醒" min-width="180">
      <template #default="{ row }">
        <div class="tasks-table__meta">
          <span>
            <el-icon><Bell /></el-icon>
            {{ props.resolveReminderText(row) }}
          </span>
        </div>
      </template>
    </el-table-column>

    <el-table-column label="进度" width="180">
      <template #default="{ row }">
        <div class="tasks-progress-cell">
          <el-progress
            :percentage="row.progress"
            :stroke-width="8"
            :show-text="false"
          />
          <span>{{ row.progress }}%</span>
        </div>
      </template>
    </el-table-column>

    <el-table-column label="操作" width="320" fixed="right">
      <template #default="{ row }">
        <div class="tasks-table__actions">
          <el-button
            v-if="row.status !== TaskStatus.Completed"
            type="success"
            plain
            size="small"
            :icon="CircleCheck"
            @click="emit('complete', row)"
          >
            完成
          </el-button>

          <el-button
            type="primary"
            plain
            size="small"
            :icon="EditPen"
            @click="emit('edit', row)"
          >
            编辑
          </el-button>

          <el-button
            type="danger"
            plain
            size="small"
            :icon="Delete"
            @click="emit('delete', row.id)"
          >
            删除
          </el-button>
        </div>
      </template>
    </el-table-column>
  </el-table>
</template>