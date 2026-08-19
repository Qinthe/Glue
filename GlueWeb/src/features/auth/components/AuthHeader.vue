<script setup lang="ts">
defineProps<{
  mode: 'login' | 'register'
  title: string
  subtitle: string
}>()
</script>

<template>
  <div class="auth-panel__header">
    <h2>{{ title }}</h2>
    <p>{{ subtitle }}</p>
  </div>
</template>