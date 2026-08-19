import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { UserSetting } from '@/types/Index'
import { getStoredLocale } from '@/i18n'

export const DEFAULT_SETTING: UserSetting = {
  locale: getStoredLocale(),
  theme: 'auto',
  hotkeyCombo: 'ctrl+space',
  primaryColor: '#409eff',
  sidebarCollapsed: true,

  launchAtStartup: false,
  startupBehavior: 'show-main',
  closeButtonBehavior: 'minimize-to-tray',

  defaultView: 'tasks',

  notificationMethods: ['system', 'sound'],
  doNotDisturbEnabled: false,
  doNotDisturbRange: ['22:00', '08:00'],

  updateCheckMode: 'auto',
  autoCheckUpdateOnStartup: true,
  updateChannel: 'stable',

  use24HourTime: true,
  showWeekNumber: false,
  confirmBeforeExit: true,
  reduceAnimation: false,
  compactMode: false,

  feedbackContact: '',
  feedbackMessage: '',
  resetKeepData: true,
}

export const useSettingStore = defineStore(
  'setting',
  () => {
    const userId = ref('00000000-0000-0000-0000-000000000001')
    const setting = ref<UserSetting>({ ...DEFAULT_SETTING })

    function updateLocalSetting(patch: Partial<UserSetting>) {
      setting.value = { ...setting.value, ...patch }
    }

    function resetLocalSetting() {
      setting.value = { ...DEFAULT_SETTING }
    }

    return { userId, setting, updateLocalSetting, resetLocalSetting }
  },
  { persist: true },
)