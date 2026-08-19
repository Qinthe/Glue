<script setup lang="ts">
import '../styles/AuthPage.css'
import AuthHeader from '../components/AuthHeader.vue'
import LoginForm from '../components/LoginForm.vue'
import RegisterForm from '../components/RegisterForm.vue'
import { useAuthPage } from '../composables/UseAuthPage.ts'

const page = useAuthPage()
</script>

<template>
  <div class="auth-page">
    <section class="auth-shell">
      <div class="auth-visual"></div>

      <div class="auth-panel">
        <AuthHeader
          :mode="page.activeMode.value"
          :title="page.headerTitle.value"
          :subtitle="page.headerSubtitle.value"
        />

        <LoginForm
          v-if="page.activeMode.value === 'login'"
          :form="page.loginForm"
          @submit="page.handleLogin"
          @forgot-password="page.handleForgotPassword"
          @switch-register="page.switchToRegister"
        />

        <RegisterForm
          v-else
          :form="page.registerForm"
          @submit="page.handleRegister"
          @switch-login="page.switchToLogin"
        />
      </div>
    </section>
  </div>
</template>