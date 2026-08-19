<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { Component } from 'vue'
import {
  House,
  Fold,
  Expand,
  Calendar,
  Document
} from '@element-plus/icons-vue'
import { useI18n } from 'vue-i18n'
import { useSettingStore } from '@/stores/UseSettingStore'

interface NavNode {
  key: string
  icon: Component
  routeName?: string
  activeNames?: string[]
  labelKey?: string
  label?: string
  query?: Record<string, string>
  children?: NavNode[]
}

const route = useRoute()
const router = useRouter()
const settingStore = useSettingStore()
const { t } = useI18n()

const isCollapsed = computed(() => settingStore.setting.sidebarCollapsed)

const navItems: NavNode[] = [
  {
    key: 'links',
    labelKey: 'sidebar.links',
    icon: House,
    routeName: 'links',
    activeNames: ['links', 'link-preview'],
  },
  {
    key: 'tasks',
    labelKey: 'sidebar.tasks',
    icon: Calendar,
    routeName: 'tasks',
    activeNames: ['tasks', 'task-gantt'],
  },
  {
    key: 'memos',
    labelKey: 'sidebar.memos',
    icon: Document,
    routeName: 'memos',
    activeNames: ['memos'],
  }
]

function resolveLabel(item: NavNode) {
  return item.labelKey ? t(item.labelKey) : item.label || ''
}

function isRouteMatched(item: NavNode) {
  const currentName = typeof route.name === 'string' ? route.name : ''
  const routeMatched = item.activeNames?.includes(currentName) ?? false

  if (!routeMatched) {
    return false
  }

  if (!item.query) {
    return true
  }

  return Object.entries(item.query).every(([key, value]) => route.query[key] === value)
}

function findActivePath(items: NavNode[], parents: string[] = []): string[] {
  for (const item of items) {
    const currentPath = [...parents, item.key]

    if (item.children?.length) {
      const childPath = findActivePath(item.children, currentPath)
      if (childPath.length > 0) {
        return childPath
      }

      if (isRouteMatched(item)) {
        return currentPath
      }

      continue
    }

    if (isRouteMatched(item)) {
      return currentPath
    }
  }

  return []
}

const activePath = computed(() => findActivePath(navItems))
const activeMenu = computed(() => activePath.value[activePath.value.length - 1] ?? 'links')
const openedMenus = computed(() => activePath.value.slice(0, -1))

function goNode(item: NavNode) {
  if (!item.routeName) {
    return
  }

  router.push({
    name: item.routeName,
    query: item.query,
  })
}

function toggleCollapsed() {
  settingStore.updateLocalSetting({
    sidebarCollapsed: !settingStore.setting.sidebarCollapsed,
  })
}
</script>

<template>
  <div :class="isCollapsed ? 'sidebar__top--collapsed' : 'sidebar__top'">
    <img
      v-show="!isCollapsed"
      src="/icon/brand_white.png"
      alt="Logo"
      class="sidebar__brand"
    />

    <button
      class="sidebar__toggle"
      type="button"
      :title="isCollapsed ? t('sidebar.expand') : t('sidebar.collapse')"
      @click="toggleCollapsed"
    >
      <el-icon>
        <component :is="isCollapsed ? Expand : Fold" />
      </el-icon>
    </button>
  </div>

  <el-menu
    :default-active="activeMenu"
    :default-openeds="openedMenus"
    class="el-menu-vertical"
    :collapse="isCollapsed"
  >
    <template v-for="item in navItems" :key="item.key">
      <el-sub-menu v-if="item.children?.length" :index="item.key">
        <template #title>
          <el-icon><component :is="item.icon" /></el-icon>
          <span>{{ resolveLabel(item) }}</span>
        </template>

        <el-menu-item
          v-for="child in item.children"
          :key="child.key"
          :index="child.key"
          @click="goNode(child)"
        >
          <el-icon><component :is="child.icon" /></el-icon>
          <span>{{ resolveLabel(child) }}</span>
        </el-menu-item>
      </el-sub-menu>

      <el-menu-item
        v-else
        :index="item.key"
        @click="goNode(item)"
      >
        <el-icon><component :is="item.icon" /></el-icon>
        <template #title>{{ resolveLabel(item) }}</template>
      </el-menu-item>
    </template>
  </el-menu>
</template>

<style scoped>
.el-menu {
  height: calc(100vh - 70px);
  overflow: auto;
}

.el-menu-vertical.el-menu--collapse {
  width: 70px;
}

.el-menu-vertical:not(.el-menu--collapse) {
  width: 260px;
}

.sidebar__top {
  display: flex;
  padding: 10px;
  justify-content: space-between;
  border-right: solid 1px var(--el-menu-border-color);
}

.sidebar__top--collapsed {
  display: flex;
  padding: 10px;
  justify-content: center;
  border-right: solid 1px var(--el-menu-border-color);
}

.sidebar__brand {
  height: 50px;
}

.sidebar__toggle {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  margin: 5px;
  border: 0;
  border-radius: 10px;
  background: var(--glue-container-hover-bg-color);
  color: var(--el-color-primary);
  cursor: pointer;
}
</style>