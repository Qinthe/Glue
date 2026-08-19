<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useTabStore } from '@/stores/UseTabStore'

const route = useRoute()
const router = useRouter()
const tabStore = useTabStore()

const tab = computed(() =>
  tabStore.tabs.find((item) => item.id === route.params.id)
)
</script>

<template>
  <div style="height: 100%">
    <iframe
      v-if="tab"
      :src="tab.url"
      :title="tab.title"
      style="display: block; width: 100%; height: 100%; border: 0"
    />

    <el-empty v-else description="标签不存在">
      <el-button type="primary" @click="router.push({ name: 'links' })">
        返回链接页
      </el-button>
    </el-empty>
  </div>
</template>