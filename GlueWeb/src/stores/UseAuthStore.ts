import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { authApi } from '@/api/AuthAPI'
import type { AuthSession, AuthUser, LoginRequest, RegisterRequest } from '@/types/User'

export const useAuthStore = defineStore(
  'auth',
  () => {
    const accessToken = ref('')
    const refreshToken = ref('')
    const currentUser = ref<AuthUser | null>(null)

    const isAuthenticated = computed(() => {
      return Boolean(accessToken.value && currentUser.value)
    })

    function applySession(session: AuthSession) {
      accessToken.value = session.accessToken
      refreshToken.value = session.refreshToken ?? ''
      currentUser.value = session.user
    }

    async function login(payload: LoginRequest) {
      const session = await authApi.login({
        email: payload.email.trim(),
        password: payload.password,
      })

      applySession(session)
      return session.user
    }

    async function register(payload: RegisterRequest) {
      const session = await authApi.register({
        email: payload.email.trim(),
        password: payload.password,
        userName: payload.userName.trim(),
        confirmPassword: payload.confirmPassword,
      })

      applySession(session)
      return session.user
    }

    function logout() {
      accessToken.value = ''
      refreshToken.value = ''
      currentUser.value = null
    }

    return {
      accessToken,
      refreshToken,
      currentUser,
      isAuthenticated,
      login,
      register,
      logout,
    }
  },
  { persist: true }
)