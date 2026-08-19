import { computed, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useAuthStore } from '@/stores/UseAuthStore'
import { useSettingStore } from '@/stores/UseSettingStore'

type AuthMode = 'login' | 'register'

export function useAuthPage() {
  const route = useRoute()
  const router = useRouter()
  const authStore = useAuthStore()
  const settingStore = useSettingStore()
  const { t } = useI18n()

  const activeMode = ref<AuthMode>('login')

  const loginForm = reactive({
    email: '',
    password: '',
  })

  const registerForm = reactive({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  })

  const headerTitle = computed(() =>
    activeMode.value === 'login'
      ? t('auth.login.title')
      : t('auth.register.title')
  )

  const headerSubtitle = computed(() =>
    activeMode.value === 'login'
      ? t('auth.login.subtitle')
      : t('auth.register.subtitle')
  )

  function switchToRegister() {
    activeMode.value = 'register'
  }

  function switchToLogin() {
    activeMode.value = 'login'
  }

  function goHome() {
    const redirect = route.query.redirect
    if (typeof redirect === 'string' && redirect) {
      router.push(redirect)
      return
    }

    router.push({ name: 'links' })
  }

  function handleForgotPassword() {
    ElMessage.info(t('auth.login.forgotPasswordUnavailable'))
  }

  async function handleLogin() {
    if (!loginForm.email.trim()) {
      ElMessage.error(t('auth.login.validationEmailRequired'))
      return
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(loginForm.email.trim())) {
      ElMessage.error(t('auth.login.validationEmailFormat'))
      return
    }

    if (!loginForm.password.trim()) {
      ElMessage.error(t('auth.login.validationPasswordRequired'))
      return
    }

    if (loginForm.password.trim().length < 6) {
      ElMessage.error(t('auth.login.validationPasswordLength'))
      return
    }

    try {
      const user = await authStore.login({
        email: loginForm.email.trim(),
        password: loginForm.password.trim(),
      })

      settingStore.userId = user.id
      ElMessage.success(t('auth.login.success'))
      goHome()
    } catch (error) {
      const message =
        error instanceof Error ? error.message : t('auth.login.invalidCredentials')
      ElMessage.error(message)
    }
  }

  async function handleRegister() {
    if (!registerForm.username.trim()) {
      ElMessage.warning(t('auth.register.validationUsernameRequired'))
      return
    }

    if (!registerForm.email.trim() || !registerForm.password.trim()) {
      ElMessage.warning(t('auth.register.validationFormIncomplete'))
      return
    }

    if (registerForm.password !== registerForm.confirmPassword) {
      ElMessage.warning(t('auth.register.validationPasswordMismatch'))
      return
    }

    try {
      const user = await authStore.register({
        userName: registerForm.username.trim(),
        email: registerForm.email.trim(),
        password: registerForm.password.trim(),
        confirmPassword: registerForm.confirmPassword.trim(),
      })

      settingStore.userId = user.id
      ElMessage.success(t('auth.register.success'))
      goHome()
    } catch (error) {
      const message =
        error instanceof Error ? error.message : t('auth.register.emailAlreadyExists')
      ElMessage.error(message)
    }
  }

  return {
    t,
    activeMode,
    loginForm,
    registerForm,
    headerTitle,
    headerSubtitle,
    switchToRegister,
    switchToLogin,
    handleForgotPassword,
    handleLogin,
    handleRegister,
  }
}