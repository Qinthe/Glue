<script setup lang="ts">
import '../styles/TasksGroupPage.css'
import { computed } from 'vue'
import { Calendar, Bell, EditPen } from '@element-plus/icons-vue'
import type { TaskItem } from '@/types/Index'
import { useTaskGroupingPage } from '../composables/UseTaskGroupingPage'

const props = defineProps<{
  tasks: TaskItem[]
  formatTaskRange: (task: TaskItem) => string
  resolveReminderText: (task: TaskItem) => string
}>()

const emit = defineEmits<{
  edit: [task: TaskItem]
}>()

const grouping = useTaskGroupingPage(computed(() => props.tasks))
</script>

<template>
  <div class="task-groups-board">
    <aside class="task-groups-board__sidebar glue-panel">
      <div class="task-groups-board__sidebar-head">
        <span>任务分组</span>
        <el-tag round size="small">{{ grouping.groups.value.length }}</el-tag>
      </div>

      <div v-if="grouping.groups.value.length" class="task-groups-list">
        <button
          v-for="group in grouping.groups.value"
          :key="group.id"
          type="button"
          class="task-groups-list__item"
          :class="{ 'is-active': grouping.currentGroupId.value === group.id }"
          @click="grouping.currentGroupId.value = group.id"
        >
          <span
            class="task-groups-list__dot"
            :style="{ backgroundColor: group.color }"
          />
          <span class="task-groups-list__meta">
            <strong>{{ group.name }}</strong>
            <span>{{ group.description || '当前分组暂无说明' }}</span>
          </span>
          <el-tag round size="small">{{ group.count }}</el-tag>
        </button>
      </div>

      <el-empty
        v-else
        description="暂无任务分组"
        :image-size="72"
      />
    </aside>

    <section class="task-groups-board__main">
      <div
        v-if="grouping.currentGroup.value"
        class="task-groups-board__summary glue-panel"
      >
        <div class="task-groups-board__summary-head">
          <div class="task-groups-board__summary-title">
            <span
              class="task-groups-list__dot"
              :style="{ backgroundColor: grouping.currentGroup.value.color }"
            />
            <h3>{{ grouping.currentGroup.value.name }}</h3>
          </div>
          <el-tag round>{{ grouping.currentTasks.value.length }} 个任务</el-tag>
        </div>
        <p>
          {{ grouping.currentGroup.value.description || '查看当前分组下的任务内容' }}
        </p>
      </div>

      <div
        v-if="grouping.currentGroup.value && grouping.currentTasks.value.length"
        class="task-groups-board__list"
      >
        <article
          v-for="task in grouping.currentTasks.value"
          :key="task.id"
          class="task-group-card glue-panel"
        >
          <div class="task-group-card__header">
            <strong>{{ task.title }}</strong>
            <el-button text :icon="EditPen" @click="emit('edit', task)">
              编辑
            </el-button>
          </div>

          <div class="task-group-card__meta">
            <span>
              <el-icon><Calendar /></el-icon>
              {{ props.formatTaskRange(task) }}
            </span>
            <span>
              <el-icon><Bell /></el-icon>
              {{ props.resolveReminderText(task) }}
            </span>
          </div>

          <div class="task-group-card__footer">
            <div class="task-group-card__progress">
              <el-progress :percentage="task.progress" :stroke-width="6" :show-text="false" />
              <span>{{ task.progress }}%</span>
            </div>
          </div>
        </article>
      </div>

      <el-empty
        v-else-if="grouping.currentGroup.value"
        description="当前分组没有任务"
        :image-size="72"
        class="glue-panel task-groups-board__empty"
      />
    </section>
  </div>
</template>