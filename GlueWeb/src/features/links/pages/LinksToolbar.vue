<script setup lang="ts">
import { ref } from 'vue'
import { Filter, Grid, Plus, RefreshRight, Tickets } from '@element-plus/icons-vue'

type FilterOption = {
  text: string
  value: string
}

const props = defineProps<{
  boardMode: 'card' | 'row'
  categoryFilters: FilterOption[]
  openModeFilters: FilterOption[]
}>()

const emit = defineEmits<{
  'change-mode': [mode: 'card' | 'row']
  'filter-change': [filters: { category: string[]; openMode: string[] }]
  create: []
}>()

const filterVisible = ref(false)
const selectedCategories = ref<string[]>([])
const selectedOpenModes = ref<string[]>([])

function applyFilters() {
  emit('filter-change', {
    category: selectedCategories.value,
    openMode: selectedOpenModes.value,
  })
  filterVisible.value = false
}

function resetFilters() {
  selectedCategories.value = []
  selectedOpenModes.value = []
  emit('filter-change', {
    category: [],
    openMode: [],
  })
  filterVisible.value = false
}
</script>

<template>
  <div class="links-toolbar">
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
            <el-button circle type="primary" :icon="Filter" class="glue-toolbar-trigger">
            </el-button>
          </template>

          <div class="glue-toolbar-popover__panel">
            <div class="glue-toolbar-popover__grid">
              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="selectedCategories"
                  multiple
                  clearable
                  collapse-tags
                  collapse-tags-tooltip
                  :teleported="false"
                  placeholder="分类"
                >
                  <el-option
                    v-for="option in props.categoryFilters"
                    :key="option.value"
                    :label="option.text"
                    :value="option.value"
                  />
                </el-select>
              </div>

              <div class="glue-toolbar-popover__field">
                <el-select
                  v-model="selectedOpenModes"
                  multiple
                  clearable
                  collapse-tags
                  collapse-tags-tooltip
                  :teleported="false"
                  placeholder="打开方式"
                >
                  <el-option
                    v-for="option in props.openModeFilters"
                    :key="option.value"
                    :label="option.text"
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
      </div>

      <div class="glue-toolbar-line__group">
          <el-button type="primary" :icon="Plus" @click="$emit('create')" >
          </el-button>
          <el-button-group direction="horizontal">
            <el-button type="primary" :icon="Grid" @click="emit('change-mode', 'card')"/>
            <el-button type="primary" :icon="Tickets" @click="emit('change-mode', 'row')"/>
          </el-button-group>
        </div>
    </div>
  </div>
</template>