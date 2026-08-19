<script setup lang="ts">
import { computed } from 'vue'
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import en from 'element-plus/es/locale/lang/en'
import { useI18n } from 'vue-i18n'
import TaskReminderPortal from '@/components/task/TaskReminderPortal.vue'

const { locale } = useI18n()

const elementLocale = computed(() => {
  return locale.value === 'en-US' ? en : zhCn
})
</script>

<template>
  <ElConfigProvider :locale="elementLocale">
    <RouterView />
    <TaskReminderPortal />
  </ElConfigProvider>
</template>