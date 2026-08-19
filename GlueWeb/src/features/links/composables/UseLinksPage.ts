import {
  computed,
  nextTick,
  onBeforeUnmount,
  onMounted,
  reactive,
  ref,
  watch,
} from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useTabStore } from '@/stores/UseTabStore'
import { TabOpenMode, type PortalTab } from '@/types/Index'

type BoardMode = 'card' | 'row'

type TabFormState = {
  title: string
  url: string
  category: string
  openMode: TabOpenMode
  image: string
  color: string
  description: string
}

const LIST_PAGE_SIZE = 10
const CARD_BATCH_SIZE = 12

export function useLinksPage() {
  const { t } = useI18n()
  const router = useRouter()
  const tabStore = useTabStore()

  const boardMode = ref<BoardMode>('card')
  const drawerVisible = ref(false)
  const editingTabId = ref<string | null>(null)

  const currentPage = ref(1)
  const pageSize = ref(LIST_PAGE_SIZE)

  const cardScrollArea = ref<HTMLElement | null>(null)
  const cardLoadTrigger = ref<HTMLElement | null>(null)
  const visibleCardCount = ref(CARD_BATCH_SIZE)

  let cardObserver: IntersectionObserver | null = null

  const tabForm = reactive<TabFormState>({
    title: '',
    url: '',
    category: '',
    openMode: TabOpenMode.Iframe,
    image: '',
    color: '#3b82f6',
    description: '',
  })

  const activeFilters = reactive<{
    category: string[]
    openMode: string[]
  }>({
    category: [],
    openMode: [],
  })

  const sortedTabs = computed(() =>
    [...tabStore.tabs].sort((left, right) => left.sortOrder - right.sortOrder)
  )

  const categoryOptions = computed(() =>
    [...new Set(
      sortedTabs.value
        .map((tab) => tab.category?.trim() ?? '')
        .filter((category) => category.length > 0)
    )]
  )

  const categoryFilters = computed(() =>
    categoryOptions.value.map((category) => ({
      text: category,
      value: category,
    }))
  )

  const openModeFilters = computed(() => [
    {
      text: t('websitenavigation.methodiframe'),
      value: String(TabOpenMode.Iframe),
    },
    {
      text: t('websitenavigation.methodnewtab'),
      value: String(TabOpenMode.NewTab),
    },
    {
      text: t('websitenavigation.methodnewwindow'),
      value: String(TabOpenMode.NewWindow),
    },
  ])

  const filteredTabs = computed(() =>
    sortedTabs.value.filter((tab) => {
      const matchesCategory =
        activeFilters.category.length === 0 ||
        activeFilters.category.includes(tab.category ?? '')

      const matchesOpenMode =
        activeFilters.openMode.length === 0 ||
        activeFilters.openMode.includes(String(tab.openMode))

      return matchesCategory && matchesOpenMode
    })
  )

  const pagedTabs = computed(() => {
    const start = (currentPage.value - 1) * pageSize.value
    return filteredTabs.value.slice(start, start + pageSize.value)
  })

  const visibleCardTabs = computed(() =>
    filteredTabs.value.slice(0, visibleCardCount.value)
  )

  const drawerTitle = computed(() =>
    editingTabId.value
      ? t('websitenavigation.editTitle')
      : t('websitenavigation.createTitle')
  )

  watch(filteredTabs, async () => {
    const maxPage = Math.max(1, Math.ceil(filteredTabs.value.length / pageSize.value))
    if (currentPage.value > maxPage) {
      currentPage.value = maxPage
    }

    resetVisibleCards()
    await nextTick()
    setupCardObserver()
  })

  watch(boardMode, async (mode) => {
    if (mode === 'card') {
      await nextTick()
      setupCardObserver()
      return
    }

    disconnectCardObserver()
  })

  onMounted(async () => {
    tabStore.ensureSeedTabs(true)
    await nextTick()
    setupCardObserver()
  })

  onBeforeUnmount(() => {
    disconnectCardObserver()
  })

  function setBoardMode(mode: BoardMode) {
    boardMode.value = mode
  }

  function handleFilterChange(filters: Record<string, unknown>) {
    activeFilters.category = Array.isArray(filters.category)
      ? filters.category.map(String)
      : []

    activeFilters.openMode = Array.isArray(filters.openMode)
      ? filters.openMode.map(String)
      : []

    currentPage.value = 1
  }

  function handleSizeChange(value: number) {
    pageSize.value = value
    currentPage.value = 1
  }

  function handleCurrentChange(value: number) {
    currentPage.value = value
  }

  function loadMoreCards() {
    if (visibleCardCount.value >= filteredTabs.value.length) {
      return
    }

    visibleCardCount.value = Math.min(
      visibleCardCount.value + CARD_BATCH_SIZE,
      filteredTabs.value.length
    )
  }

  function resetVisibleCards() {
    visibleCardCount.value = CARD_BATCH_SIZE
  }

  function disconnectCardObserver() {
    if (cardObserver) {
      cardObserver.disconnect()
      cardObserver = null
    }
  }

  function setupCardObserver() {
    disconnectCardObserver()

    if (boardMode.value !== 'card') {
      return
    }

    if (!cardScrollArea.value || !cardLoadTrigger.value) {
      return
    }

    cardObserver = new IntersectionObserver(
      (entries) => {
        const [entry] = entries
        if (entry?.isIntersecting) {
          loadMoreCards()
        }
      },
      {
        root: cardScrollArea.value,
        rootMargin: '0px 0px 120px 0px',
        threshold: 0.1,
      }
    )

    cardObserver.observe(cardLoadTrigger.value)
  }

  function resetTabForm() {
    tabForm.title = ''
    tabForm.url = ''
    tabForm.category = ''
    tabForm.openMode = TabOpenMode.Iframe
    tabForm.image = ''
    tabForm.color = '#3b82f6'
    tabForm.description = ''
  }

  function fillTabForm(tab: PortalTab) {
    tabForm.title = tab.title
    tabForm.url = tab.url
    tabForm.category = tab.category ?? ''
    tabForm.openMode = tab.openMode
    tabForm.image = tab.image ?? ''
    tabForm.color = tab.color ?? '#3b82f6'
    tabForm.description = tab.description ?? ''
  }

  function openCreateDrawer() {
    editingTabId.value = null
    resetTabForm()
    drawerVisible.value = true
  }

  function openEditDrawer(tab: PortalTab) {
    editingTabId.value = tab.id
    fillTabForm(tab)
    drawerVisible.value = true
  }

  function closeDrawer() {
    drawerVisible.value = false
    editingTabId.value = null
    resetTabForm()
  }

  function isValidUrl(value: string) {
    try {
      new URL(value)
      return true
    } catch {
      return false
    }
  }

  function submitTabForm() {
    if (!tabForm.title.trim()) {
      ElMessage.warning(t('websitenavigation.validationName'))
      return
    }

    if (!tabForm.url.trim() || !isValidUrl(tabForm.url.trim())) {
      ElMessage.warning(t('websitenavigation.validationUrl'))
      return
    }

    const payload = {
      title: tabForm.title.trim(),
      url: tabForm.url.trim(),
      category: tabForm.category.trim() || undefined,
      openMode: tabForm.openMode,
      image: tabForm.image.trim() || undefined,
      color: tabForm.color,
      description: tabForm.description.trim() || undefined,
    }

    if (editingTabId.value) {
      tabStore.updateTab(editingTabId.value, payload)
      ElMessage.success(t('websitenavigation.updated'))
    } else {
      tabStore.addTab(payload)
      ElMessage.success(t('websitenavigation.added'))
    }

    closeDrawer()
  }

  function handleDelete(id: string) {
    tabStore.removeTab(id)

    if (editingTabId.value === id) {
      closeDrawer()
    }

    ElMessage.success(t('websitenavigation.deleted'))
  }

  function onTabClick(tab: PortalTab) {
    if (tab.openMode === TabOpenMode.Iframe) {
      router.push({ name: 'link-preview', params: { id: tab.id } })
      return
    }

    tabStore.openTab(tab)
  }

  function resolvePreviewImage(tab: PortalTab) {
    if (tab.image) return tab.image

    try {
      const host = new URL(tab.url).hostname
      return `https://www.google.com/s2/favicons?sz=128&domain_url=${host}`
    } catch {
      return ''
    }
  }

  function resolveHost(url: string) {
    try {
      return new URL(url).hostname
    } catch {
      return url
    }
  }

  function resolveDescription(tab: PortalTab) {
    return tab.description?.trim() || resolveHost(tab.url)
  }

  function resolveOpenModeText(tab: PortalTab) {
    if (tab.openMode === TabOpenMode.Iframe) return t('websitenavigation.methodiframe')
    if (tab.openMode === TabOpenMode.NewWindow) return t('websitenavigation.methodnewwindow')
    return t('websitenavigation.methodnewtab')
  }

  return {
    t,
    boardMode,
    drawerVisible,
    editingTabId,
    currentPage,
    pageSize,
    cardScrollArea,
    cardLoadTrigger,
    tabForm,
    categoryOptions,
    categoryFilters,
    openModeFilters,
    filteredTabs,
    pagedTabs,
    visibleCardTabs,
    drawerTitle,
    setBoardMode,
    handleFilterChange,
    handleSizeChange,
    handleCurrentChange,
    openCreateDrawer,
    openEditDrawer,
    closeDrawer,
    submitTabForm,
    handleDelete,
    onTabClick,
    setupCardObserver,
    resolvePreviewImage,
    resolveHost,
    resolveDescription,
    resolveOpenModeText,
  }
}