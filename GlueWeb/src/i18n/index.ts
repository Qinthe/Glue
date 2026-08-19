import { createI18n } from 'vue-i18n'
import zhCN from './locales/ZH-CN'
import enUS from './locales/EN-US'

export const SUPPORTED_LOCALES = ['zh-CN', 'en-US'] as const
export type AppLocale = typeof SUPPORTED_LOCALES[number]

export const DEFAULT_LOCALE: AppLocale = 'zh-CN'

export function getStoredLocale(): AppLocale {
  const locale = localStorage.getItem('locale')
  return SUPPORTED_LOCALES.includes(locale as AppLocale)
    ? (locale as AppLocale)
    : DEFAULT_LOCALE
}

export const i18n = createI18n({
  legacy: false,
  locale: getStoredLocale(),
  fallbackLocale: 'en-US',
  messages: {
    'zh-CN': zhCN,
    'en-US': enUS,
  },
})