import { computed, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useSettingStore } from '@/stores/UseSettingStore'
import { useHotkeyRecorder } from '@/composables/UseHotkey'
import type { UserSetting } from '@/types/Index'

export type SettingSection =
  | 'basic'
  | 'startup'
  | 'notification'
  | 'appearance'
  | 'language'
  | 'updates'
  | 'advanced'
  | 'about'

const GITHUB_ISSUES_URL = 'https://github.com/your-org/your-repo/issues'
const SUPPORT_EMAIL = 'mailto:support@example.com?subject=Glue%20Feedback'

export function useSettingsPage() {
  const { t, locale } = useI18n()
  const settingStore = useSettingStore()

  const activeSection = ref<SettingSection>('basic')
  const settingForm = reactive<UserSetting>({ ...settingStore.setting })

  const {
    value: hotkeyValue,
    displayValue: hotkeyDisplayValue,
    recording: isRecordingHotkey,
    start: startHotkeyRecording,
    stop: stopHotkeyRecording,
  } = useHotkeyRecorder(settingForm.hotkeyCombo)

  watch(hotkeyValue, (value) => {
    settingForm.hotkeyCombo = value
  })

  const sectionItems: Array<{ key: SettingSection; labelKey: string }> = [
    { key: 'basic', labelKey: 'settings.basic' },
    { key: 'startup', labelKey: 'settings.startup' },
    { key: 'notification', labelKey: 'settings.notification' },
    { key: 'appearance', labelKey: 'settings.appearance' },
    { key: 'language', labelKey: 'settings.language' },
    { key: 'updates', labelKey: 'settings.updates' },
    { key: 'advanced', labelKey: 'settings.advanced' },
    { key: 'about', labelKey: 'settings.about' },
  ]

  const startupBehaviorOptions = [
    { value: 'show-main', labelKey: 'settings.startupShowMain' },
    { value: 'tray-only', labelKey: 'settings.startupTrayOnly' },
    { value: 'silent', labelKey: 'settings.startupSilent' },
  ] as const

  const closeButtonBehaviorOptions = [
    { value: 'exit', labelKey: 'settings.closeButtonExit' },
    { value: 'minimize-to-tray', labelKey: 'settings.closeButtonTray' },
  ] as const

  const defaultViewOptions = [
    { value: 'tasks', labelKey: 'settings.defaultViewTasks' },
    { value: 'memos', labelKey: 'settings.defaultViewMemos' },
    { value: 'links', labelKey: 'settings.defaultViewLinks' },
  ] as const

  const notificationMethodOptions = [
    { value: 'system', labelKey: 'settings.notificationMethodSystem' },
    { value: 'sound', labelKey: 'settings.notificationMethodSound' },
    { value: 'tray-flash', labelKey: 'settings.notificationMethodTrayFlash' },
  ] as const

  const updateCheckModeOptions = [
    { value: 'manual', labelKey: 'settings.updateCheckManual' },
    { value: 'auto', labelKey: 'settings.updateCheckAuto' },
  ] as const

  const updateChannelOptions = [
    { value: 'stable', labelKey: 'settings.updateChannelStable' },
    { value: 'beta', labelKey: 'settings.updateChannelBeta' },
  ] as const

  const pageTitle = computed(() => {
    const item = sectionItems.find((section) => section.key === activeSection.value)
    return item ? t(item.labelKey) : t('settings.title')
  })

  const appVersion = computed(() => import.meta.env.VITE_APP_VERSION || '0.0.0')
  const buildTime = computed(() => import.meta.env.VITE_BUILD_TIME || '-')

  function switchSection(section: SettingSection) {
    activeSection.value = section
  }

  function hexToRgb(hex: string) {
    const normalized = hex.replace('#', '')
    const value = normalized.length === 3
      ? normalized.split('').map((char) => char + char).join('')
      : normalized

    const red = Number.parseInt(value.slice(0, 2), 16)
    const green = Number.parseInt(value.slice(2, 4), 16)
    const blue = Number.parseInt(value.slice(4, 6), 16)

    return { red, green, blue }
  }

  function rgbToHex(red: number, green: number, blue: number) {
    const toHex = (value: number) => value.toString(16).padStart(2, '0')
    return `#${toHex(red)}${toHex(green)}${toHex(blue)}`
  }

  function mixColor(color: string, target: string, weight: number) {
    const sourceRgb = hexToRgb(color)
    const targetRgb = hexToRgb(target)

    const red = Math.round(sourceRgb.red * (1 - weight) + targetRgb.red * weight)
    const green = Math.round(sourceRgb.green * (1 - weight) + targetRgb.green * weight)
    const blue = Math.round(sourceRgb.blue * (1 - weight) + targetRgb.blue * weight)

    return rgbToHex(red, green, blue)
  }

  function applyPrimaryColor(color: string) {
    const root = document.documentElement

    root.style.setProperty('--el-color-primary', color)
    root.style.setProperty('--el-color-primary-light-3', mixColor(color, '#ffffff', 0.3))
    root.style.setProperty('--el-color-primary-light-5', mixColor(color, '#ffffff', 0.5))
    root.style.setProperty('--el-color-primary-light-7', mixColor(color, '#ffffff', 0.7))
    root.style.setProperty('--el-color-primary-dark-2', mixColor(color, '#000000', 0.2))
  }

  function applyTheme(theme: UserSetting['theme']) {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
    const shouldUseDark = theme === 'dark' || (theme === 'auto' && prefersDark)
    document.documentElement.classList.toggle('dark', shouldUseDark)
  }

  function saveSettings() {
    stopHotkeyRecording()
    settingStore.updateLocalSetting({ ...settingForm })
    locale.value = settingForm.locale
    localStorage.setItem('locale', settingForm.locale)
    applyTheme(settingForm.theme)
    applyPrimaryColor(settingForm.primaryColor)
    ElMessage.success(t('settings.saved'))
  }

  async function resetSettings() {
    await ElMessageBox.confirm(
      t('settings.resetConfirmMessage', {
        preserve: settingForm.resetKeepData ? t('settings.keepData') : t('settings.clearData'),
      }),
      t('settings.resetConfirmTitle'),
      {
        type: 'warning',
        confirmButtonText: t('common.confirm'),
        cancelButtonText: t('common.cancel'),
      }
    )

    settingStore.resetLocalSetting()
    Object.assign(settingForm, { ...settingStore.setting })
    locale.value = settingForm.locale
    localStorage.setItem('locale', settingForm.locale)
    applyTheme(settingForm.theme)
    applyPrimaryColor(settingForm.primaryColor)

    if (!settingForm.resetKeepData) {
      ElMessage.warning(t('settings.resetDataNotImplemented'))
    } else {
      ElMessage.success(t('settings.resetDone'))
    }
  }

  async function copyRuntimeLog() {
    const runtimeLog = [
      `version=${appVersion.value}`,
      `buildTime=${buildTime.value}`,
      `locale=${settingForm.locale}`,
      `theme=${settingForm.theme}`,
      `defaultView=${settingForm.defaultView}`,
      `updateChannel=${settingForm.updateChannel}`,
      `startupBehavior=${settingForm.startupBehavior}`,
      `closeButtonBehavior=${settingForm.closeButtonBehavior}`,
      `notificationMethods=${settingForm.notificationMethods.join(',')}`,
      `feedbackContact=${settingForm.feedbackContact || '-'}`,
      `feedbackMessage=${settingForm.feedbackMessage || '-'}`,
    ].join('\n')

    try {
      await navigator.clipboard.writeText(runtimeLog)
      ElMessage.success(t('settings.logCopied'))
    } catch {
      ElMessage.error(t('settings.logCopyFailed'))
    }
  }

  function openGithubIssues() {
    window.open(GITHUB_ISSUES_URL, '_blank', 'noopener,noreferrer')
  }

  function openSupportEmail() {
    window.open(SUPPORT_EMAIL, '_blank', 'noopener,noreferrer')
  }

  function manualCheckUpdate() {
    ElMessage.info(t('settings.updateManualPending'))
  }

  return {
    t,
    activeSection,
    settingForm,
    hotkeyDisplayValue,
    isRecordingHotkey,
    startHotkeyRecording,
    stopHotkeyRecording,
    sectionItems,
    startupBehaviorOptions,
    closeButtonBehaviorOptions,
    defaultViewOptions,
    notificationMethodOptions,
    updateCheckModeOptions,
    updateChannelOptions,
    pageTitle,
    appVersion,
    buildTime,
    switchSection,
    saveSettings,
    resetSettings,
    copyRuntimeLog,
    openGithubIssues,
    openSupportEmail,
    manualCheckUpdate,
  }
}