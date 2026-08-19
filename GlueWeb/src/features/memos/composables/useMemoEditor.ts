import { computed, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import { renderMarkdown } from '../../../shared/utils/markdown'
import type { MemoNote } from '@/types/Index'

export type MemoViewMode = 'browse' | 'create' | 'edit'

function normalizeTags(tags?: string[]) {
  return [...new Set(
    (tags ?? [])
      .map((tag) => tag.trim())
      .filter(Boolean)
  )]
}

export function useMemoEditor(
  getActiveMemo: () => MemoNote | null,
  addMemo: (payload: { title?: string; category?: string; content?: string; tags?: string[] }) => MemoNote,
  updateMemo: (id: string, payload: { title?: string; category?: string; content?: string; tags?: string[] }) => void,
  removeMemo: (id: string) => void,
) {
  const { t } = useI18n()

  const activeMemoId = ref<string | null>(null)
  const viewMode = ref<MemoViewMode>('browse')
  const previewDrawerVisible = ref(false)

  const editorForm = reactive({
    title: '',
    category: '',
    content: '',
    tags: [] as string[],
  })

  const activeMemo = computed(() => getActiveMemo())
  const renderedMarkdown = computed(() => renderMarkdown(editorForm.content))

  function fillForm(memo: MemoNote) {
    editorForm.title = memo.title
    editorForm.category = memo.category
    editorForm.content = memo.content
    editorForm.tags = normalizeTags(memo.tags)
  }

  function resetForm() {
    editorForm.title = ''
    editorForm.category = t('memos.defaultCategory')
    editorForm.content = ''
    editorForm.tags = []
  }

  function selectMemo(memo: MemoNote) {
    activeMemoId.value = memo.id
    viewMode.value = 'browse'
    previewDrawerVisible.value = false
    fillForm(memo)
  }

  function openCreate() {
    activeMemoId.value = null
    viewMode.value = 'create'
    previewDrawerVisible.value = false
    resetForm()
  }

  function openEdit() {
    if (!activeMemo.value) return
    viewMode.value = 'edit'
    previewDrawerVisible.value = false
    fillForm(activeMemo.value)
  }

  function cancelEdit() {
    previewDrawerVisible.value = false

    if (activeMemo.value) {
      viewMode.value = 'browse'
      fillForm(activeMemo.value)
      return
    }

    viewMode.value = 'browse'
    resetForm()
  }

  function submit() {
    const payload = {
      title: editorForm.title.trim() || t('memos.untitled'),
      category: editorForm.category.trim() || t('memos.defaultCategory'),
      content: editorForm.content,
      tags: normalizeTags(editorForm.tags),
    }

    if (viewMode.value === 'create') {
      const memo = addMemo(payload)
      activeMemoId.value = memo.id
      viewMode.value = 'browse'
      previewDrawerVisible.value = false
      fillForm(memo)
      ElMessage.success(t('memos.created'))
      return
    }

    if (!activeMemo.value) return

    updateMemo(activeMemo.value.id, payload)
    viewMode.value = 'browse'
    previewDrawerVisible.value = false
    ElMessage.success(t('common.save'))
  }

  function removeCurrent() {
    if (!activeMemo.value) return
    removeMemo(activeMemo.value.id)
    activeMemoId.value = null
    previewDrawerVisible.value = false
    resetForm()
    viewMode.value = 'browse'
    ElMessage.success(t('memos.deleted'))
  }

  return {
    activeMemoId,
    activeMemo,
    viewMode,
    previewDrawerVisible,
    editorForm,
    renderedMarkdown,
    selectMemo,
    openCreate,
    openEdit,
    cancelEdit,
    submit,
    removeCurrent,
  }
}