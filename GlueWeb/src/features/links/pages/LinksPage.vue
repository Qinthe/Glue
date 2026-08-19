<script setup lang="ts">
import '../styles/LinksPage.css'
import LinksToolbar from './LinksToolbar.vue'
import LinksCardList from './LinksCardList.vue'
import LinksTable from './LinksTable.vue'
import LinksDrawer from './LinksDrawer.vue'
import { useLinksPage } from '../composables/UseLinksPage.ts'

const page = useLinksPage()
</script>

<template>
  <div class="glue-page">
    <LinksToolbar
      :board-mode="page.boardMode.value"
      :category-filters="page.categoryFilters.value"
      :open-mode-filters="page.openModeFilters.value"
      @change-mode="page.setBoardMode"
      @filter-change="page.handleFilterChange"
      @create="page.openCreateDrawer"
    />

    <div class="glue-page__content">
      <el-empty
        v-if="page.filteredTabs.value.length === 0"
        :description="page.t('websitenavigation.emptyDescription')"
        :image-size="92"
      />

      <LinksCardList
        v-else-if="page.boardMode.value === 'card'"
        :tabs="page.visibleCardTabs.value"
        :all-tabs-count="page.filteredTabs.value.length"
        :resolve-preview-image="page.resolvePreviewImage"
        :resolve-host="page.resolveHost"
        :resolve-description="page.resolveDescription"
        :resolve-open-mode-text="page.resolveOpenModeText"
        @tab-click="page.onTabClick"
        @load-more="page.setupCardObserver"
      />

      <LinksTable
        v-else
        :tabs="page.pagedTabs.value"
        :category-filters="page.categoryFilters.value"
        :open-mode-filters="page.openModeFilters.value"
        :current-page="page.currentPage.value"
        :page-size="page.pageSize.value"
        :total="page.filteredTabs.value.length"
        :resolve-preview-image="page.resolvePreviewImage"
        :resolve-host="page.resolveHost"
        :resolve-description="page.resolveDescription"
        :resolve-open-mode-text="page.resolveOpenModeText"
        @filter-change="page.handleFilterChange"
        @size-change="page.handleSizeChange"
        @current-change="page.handleCurrentChange"
        @edit="page.openEditDrawer"
        @delete="page.handleDelete"
        @tab-click="page.onTabClick"
      />
    </div>

    <LinksDrawer
      v-model="page.drawerVisible.value"
      :title="page.drawerTitle.value"
      :form="page.tabForm"
      :category-options="page.categoryOptions.value"
      :is-editing="Boolean(page.editingTabId.value)"
      @submit="page.submitTabForm"
      @close="page.closeDrawer"
    />
  </div>
</template>