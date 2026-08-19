<script setup lang="ts">
import '../styles/MemosSidebar.css'
import type { MemoNote } from '@/types/Index'

export type MemoTreeNode = {
  id: string
  label: string
  kind: 'folder' | 'memo'
  path: string
  memo?: MemoNote
  updatedAt?: string
  children?: MemoTreeNode[]
  count?: number
}

type AllowDropType = 'prev' | 'inner' | 'next'
type DropType = 'before' | 'after' | 'inner'

export type MemoReorderPayload = {
  memoId: string
  fromPath: string
  toPath: string
  targetNodeId: string | null
  targetNodeKind: 'folder' | 'memo'
  dropType: DropType
}

const props = defineProps<{
  tree: MemoTreeNode[]
  activeMemoId: string | null
  placeholder: string
  formatDateTime: (value: string) => string
}>()

const emit = defineEmits<{
  select: [memo: MemoNote]
  reorder: [payload: MemoReorderPayload]
}>()

function collectExpandedKeys(nodes: MemoTreeNode[]): string[] {
  return nodes.flatMap((node) => {
    if (node.kind !== 'folder') {
      return []
    }

    return [
      node.id,
      ...(node.children ? collectExpandedKeys(node.children) : []),
    ]
  })
}

const expandedKeys = collectExpandedKeys(props.tree)

function handleNodeClick(node: MemoTreeNode) {
  if (node.kind === 'memo' && node.memo) {
    emit('select', node.memo)
  }
}

function allowDrag(node: any) {
  return node?.data?.kind === 'memo'
}

function allowDrop(draggingNode: any, dropNode: any, dropType: AllowDropType) {
  if (draggingNode?.data?.kind !== 'memo') {
    return false
  }

  if (dropNode?.data?.kind === 'folder') {
    return dropType === 'inner'
  }

  return dropType === 'prev' || dropType === 'next'
}

function handleNodeDrop(
  draggingNode: any,
  dropNode: any,
  dropType: DropType,
  _event: DragEvent,
) {
  const dragged = draggingNode?.data as MemoTreeNode | undefined
  const target = dropNode?.data as MemoTreeNode | undefined

  if (!dragged || !target || dragged.kind !== 'memo') {
    return
  }

  emit('reorder', {
    memoId: dragged.id,
    fromPath: dragged.path,
    toPath: target.kind === 'folder' ? target.path : target.path,
    targetNodeId: target.id,
    targetNodeKind: target.kind,
    dropType,
  })
}
</script>

<template>
  <div class="glue-panel memos-sidebar-panel">
    <div v-if="props.tree.length > 0" class="glue-panel__body memos-sidebar-panel__body">
      <el-scrollbar>
        <el-tree
          node-key="id"
          :data="props.tree"
          :expand-on-click-node="false"
          :highlight-current="true"
          :default-expanded-keys="expandedKeys"
          :current-node-key="props.activeMemoId ?? undefined"
          draggable
          :allow-drag="allowDrag"
          :allow-drop="allowDrop"
          @node-click="handleNodeClick"
          @node-drop="handleNodeDrop"
        >
          <template #default="{ data }">
            <div v-if="data.kind === 'folder'" class="memos-tree-group">
              <span class="memos-tree-group__label">{{ data.label }}</span>
              <el-tag size="small" effect="plain" round>
                {{ data.count ?? 0 }}
              </el-tag>
            </div>

            <div v-else class="memos-tree-memo">
              <strong class="memos-tree-memo__title">{{ data.label }}</strong>
              <span class="memos-tree-memo__meta">
                {{ props.formatDateTime(data.updatedAt || '') }}
              </span>
            </div>
          </template>
        </el-tree>
      </el-scrollbar>
    </div>

    <el-empty
      v-else
      description="还没有备忘录"
      :image-size="88"
    />
  </div>
</template>