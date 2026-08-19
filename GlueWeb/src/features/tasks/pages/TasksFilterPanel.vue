<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import {
  Filter,
  Plus,
  RefreshRight,
  Sort,
  DataBoard,
  Tickets,
  Postcard,
} from '@element-plus/icons-vue'
import { TaskStatus } from '@/types/Index'
import type {
  GroupFilter,
  ReminderFilter,
  SortMode,
  StatusFilter,
  TaskSummary,
  TimeFilter,
} from '../composables/UseTaskWorkbench'

type BoardView = 'tasks' | 'gantt' | 'groups'

type Option = {
  label: string
  value: string
}

const props = withDefaults(defineProps<{
  keyword: string
  statusFilter: StatusFilter
  reminderFilter: ReminderFilter
  timeFilter: TimeFilter
  sortMode: SortMode
  groupFilter: GroupFilter
  groupOptions: Option[]
  dateRange: [Date, Date] | null
  summary: TaskSummary
  collapsed: boolean
  currentGroupLabel?: string
  boardView?: BoardView
}>(), {
  currentGroupLabel: '全部任务',
  boardView: 'tasks',
})

const emit = defineEmits<{
  'update:keyword': [value: string]
  'update:status-filter': [value: StatusFilter]
  'update:reminder-filter': [value: ReminderFilter]
  'update:time-filter': [value: TimeFilter]
  'update:sort-mode': [value: SortMode]
  'update:group-filter': [value: GroupFilter]
  'update:date-range': [value: [Date, Date] | null]
  'update:board-view': [value: BoardView]
  reset: []
  create: []
  'toggle-collapse': []
}>()

const { t } = useI18n()
const filterVisible = ref(false)

const draftKeyword = ref(props.keyword)
const draftStatusFilter = ref<StatusFilter>(props.statusFilter)
const draftReminderFilter = ref<ReminderFilter>(props.reminderFilter)
const draftTimeFilter = ref<TimeFilter>(props.timeFilter)
const draftGroupFilter = ref<GroupFilter>([...props.groupFilter])
const draftDateRange = ref<[Date, Date] | null>(props.dateRange)

watch(
  () => filterVisible.value,
  (visible) => {
    if (!visible) return

    draftKeyword.value = props.keyword
    draftStatusFilter.value = props.statusFilter
    draftReminderFilter.value = props.reminderFilter
    draftTimeFilter.value = props.timeFilter
    draftGroupFilter.value = [...props.groupFilter]
    draftDateRange.value = props.dateRange
  }
)

function handleSortChange(command: string | number | object) {
  emit('update:sort-mode', command as SortMode)
}

function handleApplyFilters() {
  emit('update:keyword', draftKeyword.value)
  emit('update:status-filter', draftStatusFilter.value)
  emit('update:reminder-filter', draftReminderFilter.value)
  emit('update:time-filter', draftTimeFilter.value)
  emit('update:group-filter', draftGroupFilter.value)
  emit('update:date-range', draftDateRange.value)
  filterVisible.value = false
}

function handleReset() {
  draftKeyword.value = ''
  draftStatusFilter.value = 'all'
  draftReminderFilter.value = 'all'
  draftTimeFilter.value = 'all'
  draftGroupFilter.value = []
  draftDateRange.value = null
  emit('reset')
  filterVisible.value = false
}

function handleCreate() {
  emit('create')
  filterVisible.value = false
}

const sortModeBadgeText = computed(() => {
  switch (props.sortMode) {
    case 'start-asc':
      return '时间升序'
    case 'updated-desc':
      return '最近更新'
    case 'progress-desc':
      return '进度优先'
    default:
      return '时间降序'
  }
})
</script>

<template>
  <div class="tasks-filter-card__body">
    <div class="glue-toolbar-line">
      <div class="glue-toolbar-line__group">
        <el-popover
          v-model:visible="filterVisible"
          trigger="click"
          placement="bottom-start"
          :width="560"
          popper-class="glue-toolbar-popover"
        >
          <template #reference>
            <el-button circle type="primary" :icon="Filter" class="glue-toolbar-trigger">
            </el-button>
          </template>

          <div class="glue-toolbar-popover__panel">
            <div class="glue-toolbar-popover__grid">
              <div class="glue-toolbar-popover__field glue-toolbar-popover__field--wide">
                <el-input
                  v-model="draftKeyword"
                  clearable
                  placeholder="搜索任务标题、描述或分组"
                />
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="draftStatusFilter"
                  placeholder="状态"
                  :teleported="false"
                >
                  <el-option label="全部状态" value="all" />
                  <el-option :label="t('tasks.statusPending')" :value="TaskStatus.Pending" />
                  <el-option :label="t('tasks.statusInProgress')" :value="TaskStatus.InProgress" />
                  <el-option :label="t('tasks.statusCompleted')" :value="TaskStatus.Completed" />
                </el-select>
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="draftReminderFilter"
                  placeholder="提醒"
                  :teleported="false"
                >
                  <el-option label="全部提醒" value="all" />
                  <el-option label="已开启提醒" value="enabled" />
                  <el-option label="未开启提醒" value="disabled" />
                </el-select>
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="draftTimeFilter"
                  placeholder="时间"
                  :teleported="false"
                >
                  <el-option label="全部时间" value="all" />
                  <el-option label="今天" value="today" />
                  <el-option label="本周" value="week" />
                  <el-option label="逾期" value="overdue" />
                </el-select>
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="draftGroupFilter"
                  multiple
                  clearable
                  filterable
                  collapse-tags
                  collapse-tags-tooltip
                  placeholder="分组"
                  :teleported="false"
                >
                  <el-option
                    v-for="option in props.groupOptions"
                    :key="option.value"
                    :label="option.label"
                    :value="option.value"
                  />
                </el-select>
              </div>

              <div class="glue-toolbar-popover__field">
                <el-date-picker
                  v-model="draftDateRange"
                  type="daterange"
                  range-separator="至"
                  start-placeholder="开始日期"
                  end-placeholder="结束日期"
                  style="width: 100%"
                  :teleported="false"
                />
              </div>
            </div>

            <div class="glue-toolbar-popover__footer">
              <el-button :icon="RefreshRight" @click="handleReset">
                重置筛选
              </el-button>
              <el-button type="primary" @click="handleApplyFilters">
                应用
              </el-button>
            </div>
          </div>
        </el-popover>

        <el-dropdown trigger="click" @command="handleSortChange">
          <el-badge :value="sortModeBadgeText" class="glue-toolbar-trigger-badge">
            <el-button circle type="primary" :icon="Sort" class="glue-toolbar-trigger">
            </el-button>
          </el-badge>

          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="start-desc">开始时间从近到远</el-dropdown-item>
              <el-dropdown-item command="start-asc">开始时间从远到近</el-dropdown-item>
              <el-dropdown-item command="updated-desc">最近更新优先</el-dropdown-item>
              <el-dropdown-item command="progress-desc">进度从高到低</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>

      <div class="glue-toolbar-line__group">
        <el-button type="primary" :icon="Plus" @click="handleCreate" />
        <el-button-group direction="horizontal">
          <el-button type="primary" :icon="Tickets" @click="emit('update:board-view', 'tasks')" />
          <el-button type="primary" :icon="Postcard" @click="emit('update:board-view', 'groups')" />
          <el-button type="primary" :icon="DataBoard" @click="emit('update:board-view', 'gantt')" />
        </el-button-group>
      </div>
    </div>

    <div class="glue-toolbar-summary">
      <el-tag round>全部 {{ props.summary.total }}</el-tag>
      <el-tag type="danger" round>逾期 {{ props.summary.overdue }}</el-tag>
      <el-tag type="warning" round>今天 {{ props.summary.today }}</el-tag>
      <el-tag type="success" round>已完成 {{ props.summary.completed }}</el-tag>
    </div>
  </div>
</template>