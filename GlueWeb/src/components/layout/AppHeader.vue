<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowDown, Bell, Menu, Refresh, Search, Setting, SwitchButton } from '@element-plus/icons-vue'
import { useI18n } from 'vue-i18n'
import { useSettingStore } from '@/stores/UseSettingStore'
import { useAuthStore } from '@/stores/UseAuthStore'
import { useNotificationStore } from '@/stores/UseNotificationStore'

const route = useRoute()
const router = useRouter()
const settingStore = useSettingStore()
const authStore = useAuthStore()
const notificationStore = useNotificationStore()
const { t } = useI18n()

const headerPlaceholder = computed(() => {
  if (route.name === 'plugin' || route.query.module === 'tools' || route.name === 'tools') {
    return t('header.searchTools')
  }

  if (route.name === 'link-preview') {
    return t('header.searchCurrentContent')
  }
  if (route.name === 'tasks') return t('header.searchTasks')
  if (route.name === 'memos') return t('header.searchMemos')
  if (route.name === 'auth') return t('header.loginOrRegister')

  return t('header.search')
})

const profileText = computed(() => {
  const source = authStore.currentUser?.username || settingStore.userId
  return source.slice(0, 1).toUpperCase()
})

const profileName = computed(() => authStore.currentUser?.username || t('header.loginRegister'))

const hasUnreadNotifications = computed(() => notificationStore.unreadCount > 0)

const unreadBadgeText = computed(() => {
  if (notificationStore.unreadCount > 99) return '99+'
  return String(notificationStore.unreadCount)
})

function handleRefresh() {
  window.location.reload()
  ElMessage({
    type: 'success',
    message: t('common.refreshSuccess'),
  })
}

function goNotifications() {
  router.push({ name: 'notifications' })
}

function goSettings() {
  router.push({ name: 'settings' })
}

function goHome() {
  router.push({ name: 'links' })
}

function logout() {
  ElMessageBox.confirm(
    t('auth.common.logoutConfirm'),
    {
      confirmButtonText: t('common.confirm'),
      cancelButtonText: t('common.cancel'),
      type: 'warning',
    }
  )
    .then(() => {
      ElMessage({
        type: 'success',
        message: t('auth.common.logoutSuccess'),
      })
      authStore.logout()
      router.push({ name: 'auth' })
    })
 
}

function goAuth() {
  router.push({ name: 'auth' })
}
</script>

<template>
  <div class="topbar">
    <label class="topbar__search">
      <input :placeholder="headerPlaceholder" />
      <el-icon><Search /></el-icon>
    </label>

    <div class="topbar__actions">
      <button class="topbar__icon" type="button" :title="t('header.refresh')" @click="handleRefresh">
        <el-icon><Refresh /></el-icon>
      </button>

      <button
        class="topbar__icon"
        :class="{ 'topbar__icon--badge': hasUnreadNotifications }"
        type="button"
        :title="t('header.notifications')"
        @click="goNotifications"
      >
        <el-icon><Bell /></el-icon>
        <span v-if="hasUnreadNotifications">{{ unreadBadgeText }}</span>
      </button>

      <button class="topbar__icon" type="button" @click="goSettings">
        <el-icon><Setting /></el-icon>
      </button>

      <button class="topbar__icon" type="button" @click="goHome">
        <el-icon><Menu /></el-icon>
      </button>

      <button class="topbar__icon" type="button" @click="logout">
        <el-icon><SwitchButton /></el-icon>
      </button>

      <button class="profile-chip" type="button" @click="goAuth">
        <div class="profile-chip__avatar">{{ profileText }}</div>
        <span class="profile-chip__name">{{ profileName }}</span>
        <el-icon class="profile-chip__arrow"><ArrowDown /></el-icon>
      </button>
    </div>
  </div>
</template>

<style scoped>
.topbar {
  display: grid;
  grid-template-columns: 0.5fr 1fr;
  align-items: center;
  gap: 24px;
  min-height: 60px;
  box-sizing: border-box;
}

.topbar__brand {
  display: flex;
  align-items: center;
  height: 100%;
  padding-left: 36px;
  cursor: pointer;
  user-select: none;
}

.topbar__brand strong {
  font-size: 24px;
  font-weight: 800;
  letter-spacing: 0.01em;
  color: #16181d;
  font-family: Georgia, 'Times New Roman', serif;
}

.topbar__search {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  height: 40px;
  padding: 0 18px 0 22px;
  border-radius: 999px;
  background: var(--glue-container-bg-color);
  box-shadow: 0 10px 28px rgba(15, 23, 42, 0.06);
  color: var(--glue-text-color);
  border: 1px solid var(--glue-container-border-color);
}

.topbar__search input {
  flex: 1;
  height: 100%;
  border: 0;
  outline: 0;
  background: transparent;
  color: var(--glue-text-color);
  font-size: var(--glue-text_2-font-size);
}

.topbar__search:has(:focus-within) {
  border: 1px solid var(--el-color-primary);
}

.topbar__actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 14px;
}

.topbar__icon {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 38px;
  height: 38px;
  border: 0;
  border-radius: 20%;
  background: transparent;
  color: var(--glue-text-color);
  cursor: pointer;
}

.topbar__icon:hover {
  background: var(--glue-container-hover-bg-color);
}

.topbar__icon .el-icon {
  font-size: 20px;
}

.topbar__icon:hover .el-icon svg {
  color: var(--el-color-primary);
}

.topbar__icon--badge span {
  position: absolute;
  top: -2px;
  right: -2px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 18px;
  height: 18px;
  padding: 0 4px;
  border-radius: 999px;
  background: var(--el-color-primary);
  color: #fff;
  font-size: 10px;
  font-weight: 700;
}

.profile-chip {
  display: inline-flex;
  align-items: center;
  gap: 10px;
  border: 0;
  background: transparent;
  cursor: pointer;
  padding: 3px 6px;
  border-radius: var(--glue-container-border-radius);
}

.profile-chip:hover {
  background-color: var(--glue-container-hover-bg-color);
}

.profile-chip:hover span {
  color: var(--el-color-primary);
}

.profile-chip__avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: linear-gradient(135deg, #cbd5e1, #94a3b8);
  color: var(--glue-text-color);
  font-size: 13px;
  font-weight: 700;
}

.profile-chip__name {
  font-size: 13px;
  font-weight: 600;
  color: var(--glue-text-color);
}

.profile-chip__arrow {
  color: var(--glue-subtext_1-text-color);
}

@media (max-width: 800px) {
  .topbar {
    grid-template-columns: 1fr auto;
    gap: 10px;
    height: auto;
    padding: 12px 16px;
  }

  .topbar__search {
    grid-column: 1 / -1;
    order: 3;
  }

  .profile-chip__name {
    display: none;
  }
}
</style>