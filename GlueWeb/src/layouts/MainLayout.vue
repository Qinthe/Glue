<script setup lang="ts">
import AppSidebar from '@/components/layout/AppSidebar.vue'
import AppHeader from '@/components/layout/AppHeader.vue'
</script>

<template>
  <div class="main-layout">
    <el-container class="main-layout__container">
      <el-aside class="main-layout__sidebar">
        <AppSidebar />
      </el-aside>

      <el-container class="main-layout__content">
        <el-header class="main-layout__topbar">
          <AppHeader />
        </el-header>

        <el-main class="main-layout__main">
          <RouterView />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<style scoped>
.main-layout {
  height: 100vh;
  overflow: hidden;
}

.main-layout__container {
  height: 100%;
}

.main-layout__content {
  min-width: 0;
  min-height: 0;
}

.main-layout__sidebar {
  width: auto;
  height: 100%;
  overflow: hidden;
}

.main-layout__topbar {
  height: auto;
  flex-shrink: 0;
}

.main-layout__main {
  min-height: 0;
  overflow: hidden;
  padding: 4px 20px;
}
</style>