import { createApp } from 'vue'
import { createPinia } from 'pinia'
import piniaPersistedstate from 'pinia-plugin-persistedstate'
import ElementPlus from 'element-plus'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css'
import './shared/styles/variables.css'
import './shared/styles/shared.css'
import './shared/styles/element-plus-overrides.css'

import App from './App.vue'
import { router } from './router'
import { i18n } from './i18n'

const pinia = createPinia()
pinia.use(piniaPersistedstate)

const app = createApp(App)

// 注册所有 Element Plus 图标为全局组件
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

app.use(pinia).use(router).use(i18n).use(ElementPlus).mount('#app')