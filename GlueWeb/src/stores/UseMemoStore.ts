import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import type { MemoNote, CreateMemoRequest, UpdateMemoRequest } from '@/types/Index'

function normalizeTags(tags?: string[]) {
  return [...new Set(
    (tags ?? [])
      .map((tag) => tag.trim())
      .filter(Boolean)
  )]
}

function createSampleMemos(): MemoNote[] {
  const now = new Date().toISOString()

  return [
    {
      id: 'memo-design-system',
      title: '设计系统待办',
      content: '# 设计系统待办\n\n- 收敛按钮尺寸\n- 统一面板阴影\n- 检查移动端间距',
      category: '工作/设计系统',
      tags: ['UI', '规范'],
      createdAt: now,
      updatedAt: now,
    },
    {
      id: 'memo-reading-list',
      title: '学习清单',
      content: 'Vue 3.5 响应式更新\nPinia 持久化策略\nElement Plus 表单模式',
      category: '学习/前端',
      tags: ['Vue', 'Pinia'],
      createdAt: now,
      updatedAt: now,
    },
  ]
}

export const useMemoStore = defineStore(
  'memos',
  () => {
    const memos = ref<MemoNote[]>(createSampleMemos())

    const groupedMemos = computed(() => {
      const groups = new Map<string, MemoNote[]>()

      for (const memo of memos.value) {
        const key = memo.category.trim() || '默认'
        if (!groups.has(key)) {
          groups.set(key, [])
        }
        groups.get(key)!.push(memo)
      }

      return groups
    })

    function addMemo(req: CreateMemoRequest = {}) {
      const now = new Date().toISOString()
      const memo: MemoNote = {
        id: crypto.randomUUID(),
        title: req.title?.trim() || '未命名备忘录',
        content: req.content ?? '',
        category: req.category?.trim() || '默认',
        tags: normalizeTags(req.tags),
        createdAt: now,
        updatedAt: now,
      }

      memos.value.unshift(memo)
      return memo
    }

    function updateMemo(id: string, req: UpdateMemoRequest) {
      const index = memos.value.findIndex((item) => item.id === id)
      if (index === -1) return

      const current = memos.value[index]
      memos.value[index] = {
        ...current,
        ...req,
        title: req.title?.trim() || current.title,
        category: req.category?.trim() || '默认',
        tags: req.tags ? normalizeTags(req.tags) : current.tags,
        updatedAt: new Date().toISOString(),
      }
    }

    function removeMemo(id: string) {
      memos.value = memos.value.filter((item) => item.id !== id)
    }

    return {
      memos,
      groupedMemos,
      addMemo,
      updateMemo,
      removeMemo,
    }
  },
  { persist: true }
)