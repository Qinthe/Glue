<script setup lang="ts">
import '../styles/MemosPage.css'
import { useMemosPage } from '../composables/useMemosPage'
import MemosToolbar from './MemosToolbar.vue'
import MemosSidebar from './MemosSidebar.vue'
import MemoEditorPanel from './MemoEditorPanel.vue'

const page = useMemosPage()
</script>

<template>
  <div class="memos-feature-page">
    <div class="glue-page memos-page-shell">
      <MemosToolbar
        v-model:keyword="page.workbench.keyword.value"
        v-model:category-filter="page.workbench.categoryFilter.value"
        v-model:sort-mode="page.workbench.sortMode.value"
        :category-options="page.workbench.categoryOptions.value"
        :summary="page.workbench.summary.value"
        @reset="page.workbench.resetFilters"
        @create="page.editor.openCreate"
      />

      <div class="glue-page__content memos-page-shell__content">
        <el-container class="memos-page-shell__container">
          <el-aside width="280px" class="memos-page-shell__aside">
            <MemosSidebar
              :tree="page.workbench.memoTree.value"
              :active-memo-id="page.editor.activeMemoId.value"
              :placeholder="page.t('memos.placeholderContent')"
              :format-date-time="page.formatDateTime"
              @select="page.editor.selectMemo"
              @reorder="page.handleSidebarReorder"
            />
          </el-aside>

          <el-main class="memos-page-shell__main">
            <MemoEditorPanel
              :t="page.t"
              :active-memo="page.editor.activeMemo.value"
              :view-mode="page.editor.viewMode.value"
              :drawer-visible="page.editor.previewDrawerVisible.value"
              :editor-form="page.editor.editorForm"
              :rendered-markdown="page.editor.renderedMarkdown.value"
              :group-options="page.workbench.categoryOptions.value"
              :tag-options="page.workbench.tagOptions.value"
              :format-date-time="page.formatDateTime"
              @update:drawer-visible="page.editor.previewDrawerVisible.value = $event"
              @edit="page.editor.openEdit"
              @cancel="page.editor.cancelEdit"
              @submit="page.editor.submit"
              @delete="page.editor.removeCurrent"
            />
          </el-main>
        </el-container>
      </div>
    </div>
  </div>
</template>