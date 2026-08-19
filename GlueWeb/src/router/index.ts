import { createRouter, createWebHistory } from 'vue-router'
import type { RouteLocationNormalized, RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/UseAuthStore'

const routes: RouteRecordRaw[] = [
  {
    path: '/auth',
    name: 'auth',
    component: () => import('@/features/auth').then((module) => module.AuthPage),
    meta: { guestOnly: true },
  },
  {
    path: '/',
    component: () => import('@/layouts/MainLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: { name: 'links' },
      },
      {
        path: 'links',
        name: 'links',
        component: () => import('@/features/links').then((module) => module.LinksPage),
      },
      {
        path: 'link-preview/:id',
        name: 'link-preview',
        component: () => import('@/features/links/pages/LinkPreview.vue'),
      },
      {
        path: 'memos',
        name: 'memos',
        component: () => import('@/features/memos').then((module) => module.MemosPage),
      },
      {
        path: 'tasks',
        name: 'tasks',
        component: () => import('@/features/tasks').then((module) => module.TasksPage),
      },
      {
        path: 'settings',
        name: 'settings',
        component: () => import('@/features/settings').then((module) => module.SettingsPage),
      }
    ],
  },
  { path: '/:pathMatch(.*)*', redirect: '/' },
]


export const router = createRouter({
  history: createWebHistory(),
  routes,
})


/**
 * 从路由 query 中提取登录后应回跳的地址。
 * 只有 query.redirect 是字符串时才使用，否则回到首页。
 */
// function getRedirectPath(to: RouteLocationNormalized) {
//   return typeof to.query.redirect === 'string' && to.query.redirect
//     ? to.query.redirect
//     : '/'
// }


/**
 * 判断当前目标路由是否要求登录。
 * 这里用 matched 处理嵌套路由，父路由配置了 requiresAuth 时，子路由也会继承。
 */
function requiresAuth(to: RouteLocationNormalized) {
  return to.matched.some((record) => record.meta.requiresAuth)
}


/**
 * 判断当前目标路由是否只允许游客访问。
 * 典型场景就是登录页，已登录用户不应该再次进入。
 */
function isGuestOnly(to: RouteLocationNormalized) {
  return to.matched.some((record) => record.meta.guestOnly)
}


/**
 * 全局前置守卫：
 * 1. 未登录访问受保护页面时，跳转登录页，并带上 redirect 参数。
 * 2. 已登录访问登录页时，直接跳到 redirect 或首页。
 */
router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (requiresAuth(to) && !authStore.isAuthenticated) {
    return {
      name: 'auth',
      query: { redirect: to.fullPath },
    }
  }

  if (isGuestOnly(to) && authStore.isAuthenticated) {
    return getRedirectPath(to)
  }

  return true
})