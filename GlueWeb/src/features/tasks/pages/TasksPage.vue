<script setup lang="ts">
import '../styles/TasksPage.css'
import { computed, ref } from 'vue'
import { useTasksPage } from '../composables/UseTasksPage.ts'
import { useTaskWorkbench, type TaskTreeNode } from '../composables/UseTaskWorkbench.ts'
import TasksViewTree from './TasksViewTree.vue'
import TasksFilterPanel from './TasksFilterPanel.vue'
import TasksTableBoard from './TasksTableBoard.vue'
import TaskEditorPanel from './TaskEditorPanel.vue'
import TasksGroupBoard from './TasksGroupBoard.vue'
import TasksGanttBoard from './TasksGanttBoard.vue'

const page = useTasksPage()
const workbench = useTaskWorkbench({
  activeGroups: page.activeGroups,
  archivedGroups: page.archivedGroups,
  resolveTaskGroupNames: page.getTaskGroupNames,
})

const treeCollapsed = ref(false)

function handleBoardViewChange(value: string | number | boolean | undefined) {
  if (value === 'tasks' || value === 'gantt' || value === 'groups') {
    page.switchBoardView(value)
  }
}

function findTreeLabel(nodes: TaskTreeNode[], key: string): string | null {
  for (const node of nodes) {
    if (node.key === key) {
      return node.label
    }

    if (node.children?.length) {
      const childLabel = findTreeLabel(node.children, key)
      if (childLabel) {
        return childLabel
      }
    }
  }
  return null
}

const currentGroupLabel = computed(() =>
  findTreeLabel(workbench.treeData.value, workbench.treeKey.value) || '全部任务'
)

const drawerVisible = computed({
  get: () => page.isEditorView.value,
  set: (value: boolean) => {
    if (!value) {
      page.goBackToList()
    }
  },
})

const drawerTitle = computed(() => {
  if (page.viewMode.value === 'create') {
    return page.t('tasks.createTask')
  }

  return page.selectedTask.value?.title || page.t('common.edit')
})
</script>

<template>
  <div class="tasks-feature-page">
    <div class="common-layout tasks-page-shell">
      <el-container class="tasks-page-shell__container">
        <el-header class="tasks-page-shell__header">
          <TasksFilterPanel
            :keyword="workbench.keyword.value"
            :status-filter="workbench.statusFilter.value"
            :reminder-filter="workbench.reminderFilter.value"
            :time-filter="workbench.timeFilter.value"
            :sort-mode="workbench.sortMode.value"
            :group-filter="workbench.groupFilter.value"
            :group-options="page.taskGroupOptions.value"
            :date-range="workbench.dateRange.value"
            :summary="workbench.summary.value"
            :collapsed="false"
            :current-group-label="currentGroupLabel"
            :board-view="page.currentBoardView.value"
            @update:keyword="workbench.keyword.value = $event"
            @update:status-filter="workbench.statusFilter.value = $event"
            @update:reminder-filter="workbench.reminderFilter.value = $event"
            @update:time-filter="workbench.timeFilter.value = $event"
            @update:sort-mode="workbench.sortMode.value = $event"
            @update:group-filter="workbench.groupFilter.value = $event"
            @update:date-range="workbench.dateRange.value = $event"
            @update:board-view="handleBoardViewChange"
            @reset="workbench.resetFilters"
            @create="page.openCreatePage"
          />
        </el-header>

        <el-container style="gap: 10px; height: 100vh;">
          <el-aside width="220px">
            <div class="tasks-page-shell__sidebar">
              <TasksViewTree
                :total="workbench.allTasks.value.length"
                :tree-data="workbench.treeData.value"
                :current-key="workbench.treeKey.value"
                :collapsed="treeCollapsed"
                @select="workbench.handleTreeNodeClick"
                @toggle-collapse="treeCollapsed = !treeCollapsed"
              />
            </div>
          </el-aside>

          <el-main style="padding: 0; overflow: auto;">
            <div class="tasks-page-shell__content">
              <TasksTableBoard
                v-if="page.currentBoardView.value === 'tasks'"
                :tasks="workbench.filteredTasks.value"
                :is-overdue="workbench.isOverdue"
                :resolve-row-class-name="workbench.resolveRowClassName"
                :resolve-status-type="page.resolveStatusType"
                :resolve-status-text="page.resolveStatusText"
                :format-task-range="page.formatTaskRange"
                :resolve-reminder-text="page.resolveReminderText"
                :resolve-group-names="page.getTaskGroupNames"
                empty-text="没有匹配到任务，试试放宽筛选条件"
                @complete="page.completeTask"
                @edit="page.openEditPage"
                @delete="page.removeTask"
              />

              <TasksGroupBoard
                v-else-if="page.currentBoardView.value === 'groups'"
                :tasks="workbench.filteredTasks.value"
                :format-task-range="page.formatTaskRange"
                :resolve-reminder-text="page.resolveReminderText"
                @edit="page.openEditPage"
              />

              <TasksGanttBoard
                v-else
                :tasks="workbench.filteredTasks.value"
                :format-task-range="page.formatTaskRange"
                :resolve-status-text="page.resolveStatusText"
                @edit="page.openEditPage"
              />
            </div>
          </el-main>
        </el-container>
      </el-container>
    </div>

    <el-drawer
      v-model="drawerVisible"
      :title="drawerTitle"
      direction="rtl"
      size="760px"
      destroy-on-close
      class="tasks-editor-drawer"
    >
      <TaskEditorPanel
        :page="page"
        :is-create-mode="page.viewMode.value === 'create'"
        :is-edit-mode="page.viewMode.value === 'edit'"
        @cancel="drawerVisible = false"
        @submit="page.submitTask"
      />
    </el-drawer>
  </div>
</template>