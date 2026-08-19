<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { Link } from '@element-plus/icons-vue'
import type { PortalTab } from '@/types/Index'

const props = defineProps<{
  tabs: PortalTab[]
  allTabsCount: number
  resolvePreviewImage: (tab: PortalTab) => string
  resolveHost: (url: string) => string
  resolveDescription: (tab: PortalTab) => string
  resolveOpenModeText: (tab: PortalTab) => string
}>()

const emit = defineEmits<{
  'tab-click': [tab: PortalTab]
  'load-more': []
}>()

onMounted(() => {
  emit('load-more')
})

watch(
  () => [props.tabs.length, props.allTabsCount],
  () => {
    emit('load-more')
  }
)
</script>

<template>
  <div class="card-view glue-scroll-y">
    <div class="card-grid">
      <article
        v-for="tab in tabs"
        :key="tab.id"
        :title="resolveDescription(tab)"
        class="nav-card"
        :style="{ '--accent': tab.color || '#2563eb' }"
        @click="$emit('tab-click', tab)"
      >
        <div class="nav-card__media">
          <img
            v-if="resolvePreviewImage(tab)"
            :src="resolvePreviewImage(tab)"
            :alt="tab.title"
          />
          <el-icon v-else><Link /></el-icon>
        </div>

        <div class="nav-card__content">
          <strong>{{ tab.title }}</strong>
          <span class="nav-card__host">{{ resolveHost(tab.url) }}</span>
          <p class="nav-card__description">{{ resolveDescription(tab) }}</p>
        </div>

        <el-tag type="primary" effect="plain" round>
          {{ resolveOpenModeText(tab) }}
        </el-tag>
      </article>
    </div>

    <div v-if="tabs.length < allTabsCount" class="card-load-trigger">
      加载更多中...
    </div>
  </div>
</template>