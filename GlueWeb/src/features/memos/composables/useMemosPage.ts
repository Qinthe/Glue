import { computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useMemoStore } from '@/stores/UseMemoStore.ts'
import { useMemoWorkbench } from './useMemoWorkbench'
import { useMemoEditor } from './useMemoEditor'
import type { MemoReorderPayload } from '../pages/MemosSidebar.vue'

export function useMemosPage() {
  const { t, locale } = useI18n()
  const memoStore = useMemoStore()

  const workbench = useMemoWorkbench(() => memoStore.memos)

  const editor = useMemoEditor(
    () => memoStore.memos.find((item) => item.id === editor.activeMemoId.value) ?? null,
    memoStore.addMemo,
    memoStore.updateMemo,
    memoStore.removeMemo,
  )

  function formatDateTime(value: string) {
    const date = new Date(value)

    if (Number.isNaN(date.getTime())) {
      return value
    }

    return new Intl.DateTimeFormat(locale.value, {
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date)
  }

  function handleSidebarReorder(_payload: MemoReorderPayload) {
    // 这里只保留签名对齐，避免页面报错。
    // 真正的多级拖拽持久化需要 store 支持路径迁移和排序写回。
  }

  watch(
    () => memoStore.memos.map((item) => item.id).join('|'),
    () => {
      if (memoStore.memos.length === 0) {
        editor.activeMemoId.value = null
        return
      }

      if (!memoStore.memos.some((item) => item.id === editor.activeMemoId.value)) {
        editor.selectMemo(memoStore.memos[0])
      }
    },
    { immediate: true }
  )

  const activeGroupLabel = computed(() => {
    const current = editor.activeMemo.value
    return current?.category?.trim() || t('memos.defaultCategory')
  })

  return {
    t,
    workbench,
    editor,
    formatDateTime,
    activeGroupLabel,
    handleSidebarReorder,
  }
}