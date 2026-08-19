<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Filter, Plus, RefreshRight, Sort } from '@element-plus/icons-vue'

type Option = {
  label: string
  value: string
}

type SortMode = 'updated-desc' | 'updated-asc' | 'title-asc'

const props = defineProps<{
  keyword: string
  categoryFilter: string
  sortMode: SortMode
  categoryOptions: Option[]
  summary: {
    total: number
    filtered: number
    categories: number
    updatedToday: number
  }
}>()

const emit = defineEmits<{
  'update:keyword': [value: string]
  'update:category-filter': [value: string]
  'update:sort-mode': [value: SortMode]
  reset: []
  create: []
}>()

const filterVisible = ref(false)
const draftKeyword = ref(props.keyword)
const draftCategoryFilter = ref(props.categoryFilter)

watch(
  () => filterVisible.value,
  (visible) => {
    if (!visible) return
    draftKeyword.value = props.keyword
    draftCategoryFilter.value = props.categoryFilter
  }
)

function applyFilters() {
  emit('update:keyword', draftKeyword.value)
  emit('update:category-filter', draftCategoryFilter.value)
  filterVisible.value = false
}

function resetFilters() {
  draftKeyword.value = ''
  draftCategoryFilter.value = 'all'
  emit('reset')
  filterVisible.value = false
}

const sortModeBadgeText = computed(() => {
  switch (props.sortMode) {
    case 'updated-asc':
      return '最早更新'
    case 'title-asc':
      return '标题 A-Z'
    default:
      return '最近更新'
  }
})
</script>

<template>
  <div class="memos-toolbar-shell">
    <div class="glue-toolbar-line">
      <div class="glue-toolbar-line__group">
        <el-popover
          v-model:visible="filterVisible"
          trigger="click"
          placement="bottom-start"
          :width="420"
          popper-class="glue-toolbar-popover"
        >
          <template #reference>
            <el-button type="primary" circle :icon="Filter" class="glue-toolbar-trigger">
            </el-button>
          </template>

          <div class="glue-toolbar-popover__panel">
            <div class="glue-toolbar-popover__grid">
              <div class="glue-toolbar-popover__field glue-toolbar-popover__field--wide">
                <el-input v-model="draftKeyword" clearable placeholder="搜索标题、分组、标签或内容" />
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="draftCategoryFilter"
                  :teleported="false"
                  placeholder="分组"
                >
                  <el-option label="全部分组" value="all" />
                  <el-option
                    v-for="option in props.categoryOptions"
                    :key="option.value"
                    :label="option.label"
                    :value="option.value"
                  />
                </el-select>
              </div>
            </div>

            <div class="glue-toolbar-popover__footer">
              <el-button :icon="RefreshRight" @click="resetFilters">
                重置筛选
              </el-button>
              <el-button type="primary" @click="applyFilters">
                应用
              </el-button>
            </div>
          </div>
        </el-popover>

        <el-dropdown trigger="click" @command="emit('update:sort-mode', $event as SortMode)">
          <el-badge :value="sortModeBadgeText" class="glue-toolbar-trigger-badge">
            <el-button type="primary" circle :icon="Sort" class="glue-toolbar-trigger">
            </el-button>
          </el-badge>

          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="updated-desc">最近更新</el-dropdown-item>
              <el-dropdown-item command="updated-asc">最早更新</el-dropdown-item>
              <el-dropdown-item command="title-asc">标题 A-Z</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>

      <div class="glue-toolbar-line__group">
        <el-button type="primary" :icon="Plus" @click="$emit('create')" />
      </div>
    </div>
  </div>
</template>