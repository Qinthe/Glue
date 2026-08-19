<script setup lang="ts">
import '../styles/MemoEditor.css'
import { Delete, EditPen, View, Check, CloseBold } from '@element-plus/icons-vue'
import type { MemoNote } from '@/types/Index'

type ViewMode = 'browse' | 'create' | 'edit'

type Option = {
  label: string
  value: string
}

const props = defineProps<{
  t: (key: string) => string
  activeMemo: MemoNote | null
  viewMode: ViewMode
  drawerVisible: boolean
  editorForm: {
    title: string
    category: string
    content: string
    tags: string[]
  }
  renderedMarkdown: string
  groupOptions: Option[]
  tagOptions: Option[]
  formatDateTime: (value: string) => string
}>()

const emit = defineEmits<{
  'update:drawer-visible': [value: boolean]
  edit: []
  cancel: []
  submit: []
  delete: []
}>()

const isCreateMode = computed(() => props.viewMode === 'create')
const isEditMode = computed(() => props.viewMode === 'edit')
const isBrowseMode = computed(() => props.viewMode === 'browse')
const isEditingMode = computed(() => props.viewMode === 'create' || props.viewMode === 'edit')
</script>

<template>
  <div class="glue-panel memo-editor-panel">
    <div class="memo-editor-panel__header">
      <div class="memo-editor-panel__title">
        <h2>
          {{ isCreateMode ? props.t('memos.createMemo') : props.activeMemo?.title || props.t('memos.title') }}
        </h2>
      </div>

      <div class="memo-editor-panel__actions">
        <template v-if="isBrowseMode && props.activeMemo">
          <el-tooltip content="编辑">
            <el-button
              type="primary"
              circle
              :icon="EditPen"
              @click="emit('edit')"
            />
          </el-tooltip>

          <el-popconfirm title="删除" @confirm="emit('delete')">
            <template #reference>
              <el-button
                type="danger"
                circle
                :icon="Delete"
              />
            </template>
          </el-popconfirm>
        </template>

        <template v-else-if="isEditingMode">
          <el-tooltip content="预览 Markdown">
            <el-button
              type="primary"
              plain
              circle
              :icon="View"
              @click="emit('update:drawer-visible', true)"
            />
          </el-tooltip>

          <el-tooltip :content="props.t('common.cancel')">
            <el-button
              circle
              :icon="CloseBold"
              @click="emit('cancel')"
            />
          </el-tooltip>

          <el-tooltip :content="isCreateMode ? props.t('memos.createMemo') : props.t('common.save')">
            <el-button
              type="primary"
              circle
              :icon="Check"
              @click="emit('submit')"
            />
          </el-tooltip>

          <template v-if="props.activeMemo && isEditMode">
            <el-popconfirm title="删除" @confirm="emit('delete')">
              <template #reference>
                <el-button
                  type="danger"
                  circle
                  :icon="Delete"
                />
              </template>
            </el-popconfirm>
          </template>
        </template>
      </div>
    </div>

    <div v-if="isCreateMode || props.activeMemo" class="memo-editor-panel__body">
      <template v-if="isBrowseMode">
        <div class="memo-render-panel">
          <div
            v-if="props.activeMemo?.category || props.activeMemo?.tags?.length"
            class="memo-render-panel__meta"
          >
            <el-tag v-if="props.activeMemo?.category" effect="plain" round>
              {{ props.activeMemo.category }}
            </el-tag>

            <el-tag
              v-for="tag in props.activeMemo?.tags ?? []"
              :key="tag"
              type="success"
              effect="plain"
              round
            >
              # {{ tag }}
            </el-tag>
          </div>

          <div class="memo-render-panel__body">
            <div class="memo-markdown" v-html="props.renderedMarkdown" />
          </div>
        </div>
      </template>

      <template v-else>
        <div class="memo-editor-layout">
          <div class="memo-editor-layout__editor">
            <el-form label-position="top">
              <el-row :gutter="16">
                <el-col :xs="24" :md="14">
                  <el-form-item :label="props.t('memos.labelTitle')">
                    <el-input
                      v-model="props.editorForm.title"
                      :placeholder="props.t('memos.placeholderTitle')"
                    />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :md="10">
                  <el-form-item :label="props.t('memos.labelGroup')">
                    <el-select
                      v-model="props.editorForm.category"
                      filterable
                      allow-create
                      default-first-option
                      clearable
                      style="width: 100%"
                      :placeholder="props.t('memos.placeholderGroupPath')"
                    >
                      <el-option
                        v-for="option in props.groupOptions"
                        :key="option.value"
                        :label="option.label"
                        :value="option.value"
                      />
                    </el-select>
                  </el-form-item>
                </el-col>
              </el-row>

              <el-form-item :label="props.t('memos.labelTags')">
                <el-select
                  v-model="props.editorForm.tags"
                  multiple
                  filterable
                  allow-create
                  default-first-option
                  clearable
                  :reserve-keyword="false"
                  style="width: 100%"
                  :placeholder="props.t('memos.placeholderTags')"
                >
                  <el-option
                    v-for="option in props.tagOptions"
                    :key="option.value"
                    :label="option.label"
                    :value="option.value"
                  />
                </el-select>
              </el-form-item>

              <el-form-item :label="props.t('memos.labelContent')">
                <el-input
                  v-model="props.editorForm.content"
                  type="textarea"
                  :rows="22"
                  resize="vertical"
                  :placeholder="props.t('memos.placeholderContent')"
                />
              </el-form-item>
            </el-form>
          </div>
        </div>
      </template>

      <div v-if="props.activeMemo" class="memo-editor-panel__footer">
        <el-tag effect="plain" round>
          {{ props.t('memos.createdAt') }} {{ props.formatDateTime(props.activeMemo.createdAt) }}
        </el-tag>
        <el-tag type="success" effect="plain" round>
          {{ props.t('memos.updatedAt') }} {{ props.formatDateTime(props.activeMemo.updatedAt) }}
        </el-tag>
      </div>
    </div>

    <el-empty
      v-else
      :description="props.t('memos.selectOrCreate')"
      :image-size="96"
    />

    <el-drawer
      :model-value="props.drawerVisible"
      title="Markdown 预览"
      :size="`var(--glue-drawer-width)`"
      destroy-on-close
      class="memo-preview-drawer"
      @update:model-value="emit('update:drawer-visible', $event)"
    >
      <div class="memo-preview-drawer__body">
        <div class="memo-markdown" v-html="props.renderedMarkdown" />
      </div>

      <template #footer>
        <div class="memo-preview-drawer__footer">
          <el-button circle :icon="CloseBold" @click="emit('update:drawer-visible', false)" />
        </div>
      </template>
    </el-drawer>
  </div>
</template>