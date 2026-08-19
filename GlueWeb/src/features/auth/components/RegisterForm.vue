<script setup lang="ts">
import { useI18n } from 'vue-i18n'

defineProps<{
  form: {
    username: string
    email: string
    password: string
    confirmPassword: string
  }
}>()

defineEmits<{
  submit: []
  'switch-login': []
}>()

const { t } = useI18n()
</script>

<template>
  <div class="auth-form">
    <label>{{ t('auth.common.username') }}</label>
    <el-input
      v-model="form.username"
      :placeholder="t('auth.common.inputUsername')"
      clearable
    />

    <label>{{ t('auth.common.email') }}</label>
    <el-input
      v-model="form.email"
      :placeholder="t('auth.common.inputEmail')"
      clearable
    />

    <label>{{ t('auth.common.password') }}</label>
    <el-input
      v-model="form.password"
      type="password"
      show-password
      :placeholder="t('auth.common.inputPassword')"
    />

    <label>{{ t('auth.common.confirmPassword') }}</label>
    <el-input
      v-model="form.confirmPassword"
      type="password"
      show-password
      :placeholder="t('auth.common.confirmPasswordPlaceholder')"
    />

    <el-button type="primary" size="large" class="auth-submit" @click="$emit('submit')">
      {{ t('auth.register.submit') }}
    </el-button>

    <div class="auth-links auth-links--center">
      <span class="auth-links__tip">{{ t('auth.register.hasAccount') }}</span>
      <button type="button" class="auth-links__item" @click="$emit('switch-login')">
        {{ t('auth.register.switchToLogin') }}
      </button>
    </div>
  </div>
</template>