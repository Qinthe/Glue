<script setup lang="ts">
import { useI18n } from 'vue-i18n'

defineProps<{
  form: {
    email: string
    password: string
  }
}>()

defineEmits<{
  submit: []
  'forgot-password': []
  'switch-register': []
}>()

const { t } = useI18n()
</script>

<template>
  <div class="auth-form">
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

    <el-button type="primary" size="large" class="auth-submit" @click="$emit('submit')">
      {{ t('auth.login.submit') }}
    </el-button>

    <div class="auth-links">
      <button type="button" class="auth-links__item" @click="$emit('forgot-password')">
        {{ t('auth.login.forgotPassword') }}
      </button>
      <button type="button" class="auth-links__item" @click="$emit('switch-register')">
        {{ t('auth.login.switchToRegister') }}
      </button>
    </div>
  </div>
</template>