import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import type { MemoNote } from '@/types/Index.ts'
import { stripMarkdown } from '../../../shared/utils/markdown'
import type { MemoTreeNode } from '../pages/MemosSidebar.vue'

export type MemoSortMode = 'updated-desc' | 'updated-asc' | 'title-asc'

export type MemoGroupOption = {
  label: string
  value: string
  children?: MemoGroupOption[]
}

function normalizeCategoryPath(value: string, fallback: string) {
  return value
    .split('/')
    .map((item) => item.trim())
    .filter(Boolean)
    .join('/') || fallback
}

function normalizeTags(tags?: string[]) {
  return [...new Set(
    (tags ?? [])
      .map((tag) => tag.trim())
      .filter(Boolean)
  )]
}

function createFolderId(path: string) {
  return `folder:${path}`
}

function appendMemoToTree(
  roots: MemoTreeNode[],
  memo: MemoNote,
  placeholder: string,
  defaultCategory: string,
) {
  const categoryPath = normalizeCategoryPath(memo.category.trim(), defaultCategory)
  const segments = categoryPath.split('/')

  let currentLevel = roots
  let currentPath = ''

  for (const segment of segments) {
    currentPath = currentPath ? `${currentPath}/${segment}` : segment

    let folder = currentLevel.find(
      (item) => item.kind === 'folder' && item.path === currentPath,
    )

    if (!folder) {
      folder = {
        id: createFolderId(currentPath),
        label: segment,
        kind: 'folder',
        path: currentPath,
        count: 0,
        children: [],
      }
      currentLevel.push(folder)
    }

    folder.count = (folder.count ?? 0) + 1
    currentLevel = folder.children!
  }

  currentLevel.push({
    id: memo.id,
    label: memo.title.trim() || placeholder || '未命名备忘录',
    kind: 'memo',
    path: categoryPath,
    memo,
    updatedAt: memo.updatedAt,
  })
}

function appendGroupToTree(
  roots: MemoGroupOption[],
  path: string,
) {
  const segments = path.split('/').filter(Boolean)

  let currentLevel = roots
  let currentPath = ''

  for (const segment of segments) {
    currentPath = currentPath ? `${currentPath}/${segment}` : segment

    let node = currentLevel.find((item) => item.value === currentPath)
    if (!node) {
      node = {
        label: segment,
        value: currentPath,
        children: [],
      }
      currentLevel.push(node)
    }

    currentLevel = node.children!
  }
}

function sortTree(nodes: MemoTreeNode[], locale: string) {
  nodes.sort((left, right) => {
    if (left.kind !== right.kind) {
      return left.kind === 'folder' ? -1 : 1
    }

    return left.label.localeCompare(right.label, locale)
  })

  for (const node of nodes) {
    if (node.children?.length) {
      sortTree(node.children, locale)
    }
  }

  return nodes
}

function sortGroupTree(nodes: MemoGroupOption[], locale: string) {
  nodes.sort((left, right) => left.label.localeCompare(right.label, locale))

  for (const node of nodes) {
    if (node.children?.length) {
      sortGroupTree(node.children, locale)
    }
  }

  return nodes
}

export function useMemoWorkbench(memos: () => MemoNote[]) {
  const { t, locale } = useI18n()

  const keyword = ref('')
  const categoryFilter = ref('all')
  const sortMode = ref<MemoSortMode>('updated-desc')

  const categoryOptions = computed(() => {
    const values = [...new Set(
      memos()
        .map((item) => normalizeCategoryPath(item.category.trim(), t('memos.defaultCategory')))
        .filter(Boolean)
    )]

    return values
      .sort((left, right) => left.localeCompare(right, locale.value))
      .map((value) => ({ label: value, value }))
  })

  const groupTreeOptions = computed<MemoGroupOption[]>(() => {
    const roots: MemoGroupOption[] = []

    for (const option of categoryOptions.value) {
      appendGroupToTree(roots, option.value)
    }

    return sortGroupTree(roots, locale.value)
  })

  const tagOptions = computed(() => {
    const values = [...new Set(
      memos().flatMap((memo) => normalizeTags(memo.tags))
    )]

    return values
      .sort((left, right) => left.localeCompare(right, locale.value))
      .map((value) => ({ label: value, value }))
  })

  const filteredMemos = computed(() => {
    const search = keyword.value.trim().toLowerCase()

    return memos()
      .filter((memo) => {
        const normalizedCategory = normalizeCategoryPath(
          memo.category,
          t('memos.defaultCategory'),
        )

        if (categoryFilter.value !== 'all' && normalizedCategory !== categoryFilter.value) {
          return false
        }

        if (!search) {
          return true
        }

        return [
          memo.title,
          memo.category,
          stripMarkdown(memo.content),
          ...normalizeTags(memo.tags),
        ].some((field) => field.toLowerCase().includes(search))
      })
      .sort((left, right) => {
        if (sortMode.value === 'updated-asc') {
          return left.updatedAt.localeCompare(right.updatedAt)
        }

        if (sortMode.value === 'title-asc') {
          return left.title.localeCompare(right.title, locale.value)
        }

        return right.updatedAt.localeCompare(left.updatedAt)
      })
  })

  const memoTree = computed<MemoTreeNode[]>(() => {
    const roots: MemoTreeNode[] = []

    for (const memo of filteredMemos.value) {
      appendMemoToTree(
        roots,
        memo,
        t('memos.placeholderContent'),
        t('memos.defaultCategory'),
      )
    }

    return sortTree(roots, locale.value)
  })

  const summary = computed(() => {
    const today = new Date()
    const todayKey = [
      today.getFullYear(),
      String(today.getMonth() + 1).padStart(2, '0'),
      String(today.getDate()).padStart(2, '0'),
    ].join('-')

    return {
      total: memos().length,
      filtered: filteredMemos.value.length,
      categories: categoryOptions.value.length,
      updatedToday: memos().filter((memo) => memo.updatedAt.startsWith(todayKey)).length,
    }
  })

  function resetFilters() {
    keyword.value = ''
    categoryFilter.value = 'all'
    sortMode.value = 'updated-desc'
  }

  return {
    keyword,
    categoryFilter,
    sortMode,
    categoryOptions,
    groupTreeOptions,
    tagOptions,
    filteredMemos,
    memoTree,
    summary,
    resetFilters,
  }
}