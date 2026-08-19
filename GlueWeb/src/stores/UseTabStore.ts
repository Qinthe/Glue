import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { tabApi } from '@/api/TabAPI'
import { getUserId } from '@/utils/SessionStorage'
import type { CreateTabRequest, PortalTab, UpdateTabRequest } from '@/types/Index'
import { TabOpenMode } from '@/types/Index'

export const useTabStore = defineStore(
  'tabs',
  () => {
    const tabs = ref<PortalTab[]>([])
    const hasBootstrapped = ref(false)

    const groupedTabs = computed(() => {
      const map = new Map<string, PortalTab[]>()

      for (const tab of tabs.value) {
        const key = tab.category ?? '默认'
        if (!map.has(key)) {
          map.set(key, [])
        }
        map.get(key)!.push(tab)
      }

      return map
    })

    async function ensureSeedTabs(force = false) {
      if (!force && (hasBootstrapped.value || tabs.value.length > 0)) {
        return
      }

      const userId = getUserId()
      if (!userId) {
        tabs.value = []
        hasBootstrapped.value = true
        return
      }

      if (force) {
        tabs.value = []
      }

      tabs.value = await tabApi.getAll(userId)
      hasBootstrapped.value = true
    }

    async function addTab(req: CreateTabRequest) {
      const userId = getUserId()
      if (!userId) {
        throw new Error('Missing user id')
      }

      const createdTab = await tabApi.create(userId, req)
      tabs.value.push(createdTab)
      return createdTab
    }

    async function updateTab(id: string, req: UpdateTabRequest) {
      const userId = getUserId()
      if (!userId) {
        throw new Error('Missing user id')
      }

      const updatedTab = await tabApi.update(userId, id, req)
      const index = tabs.value.findIndex((tab) => tab.id === id)

      if (index !== -1) {
        tabs.value[index] = updatedTab
      }

      return updatedTab
    }

    async function removeTab(id: string) {
      const userId = getUserId()
      if (!userId) {
        throw new Error('Missing user id')
      }

      await tabApi.remove(userId, id)
      tabs.value = tabs.value.filter((tab) => tab.id !== id)
    }

    async function reorderTabs(items: Array<{ id: string; sortOrder: number }>) {
      const userId = getUserId()
      if (!userId) {
        throw new Error('Missing user id')
      }

      await tabApi.reorder(userId, items)

      const sortOrderMap = new Map(items.map((item) => [item.id, item.sortOrder]))
      tabs.value = tabs.value
        .map((tab) => ({
          ...tab,
          sortOrder: sortOrderMap.get(tab.id) ?? tab.sortOrder,
        }))
        .sort((left, right) => left.sortOrder - right.sortOrder)
    }

    function openTab(tab: PortalTab) {
      if (tab.openMode === TabOpenMode.NewTab) {
        window.open(tab.url, '_blank')
      } else if (tab.openMode === TabOpenMode.NewWindow) {
        window.open(tab.url, '_blank', 'width=1440,height=900')
      }
    }

    return {
      tabs,
      groupedTabs,
      hasBootstrapped,
      ensureSeedTabs,
      addTab,
      updateTab,
      removeTab,
      reorderTabs,
      openTab,
    }
  },
  { persist: true },
)