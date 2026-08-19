<script setup lang="ts">
import { computed } from 'vue'
import type { TaskTreeNode } from '../composables/UseTaskWorkbench'

const props = defineProps<{
  total: number
  treeData: TaskTreeNode[]
  currentKey: string
  collapsed: boolean
}>()

const emit = defineEmits<{
  select: [key: string]
  'toggle-collapse': []
}>()

function handleSelect(key: string) {
  if (key === 'schedule-root') return
  emit('select', key)
}

function handleNodeClick(node: TaskTreeNode) {
  handleSelect(node.key)
}

function flattenSelectableNodes(nodes: TaskTreeNode[]): TaskTreeNode[] {
  return nodes.flatMap((node) => {
    const current = node.key === 'schedule-root' ? [] : [node]
    const children = node.children ? flattenSelectableNodes(node.children) : []
    return [...current, ...children]
  })
}

function resolveCollapsedText(label: string) {
  const text = label.trim()
  return text ? text.slice(0, 1) : '-'
}

const collapsedItems = computed(() => flattenSelectableNodes(props.treeData))
</script>

<template>
  <div v-if="!props.collapsed" class="tasks-sidebar-card__body">
    <el-tree
      node-key="key"
      :data="props.treeData"
      :expand-on-click-node="false"
      :highlight-current="true"
      :default-expand-all="true"
      :current-node-key="props.currentKey"
      @node-click="handleNodeClick"
    >
      <template #default="{ data }">
        <div class="tasks-tree-node">
          <span class="tasks-tree-node__label">{{ data.label }}</span>
          <el-tag
            v-if="data.count !== null && data.count !== undefined"
            size="small"
            effect="plain"
            round
            class="tasks-tree-node__count"
          >
            {{ data.count }}
          </el-tag>
        </div>
      </template>
    </el-tree>
  </div>

  <div v-else class="tasks-sidebar-card__collapsed">
    <div
      v-for="item in collapsedItems"
      :key="item.key"
      class="tasks-sidebar-card__collapsed-item"
      :title="item.label"
      @click="handleSelect(item.key)"
    >
      {{ resolveCollapsedText(item.label) }}
    </div>
  </div>
</template>