<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { Delete, EditPen, Link } from '@element-plus/icons-vue'
import type { PortalTab } from '@/types/Index'

defineProps<{
  tabs: PortalTab[]
  categoryFilters: Array<{ text: string; value: string }>
  openModeFilters: Array<{ text: string; value: string }>
  currentPage: number
  pageSize: number
  total: number
  resolvePreviewImage: (tab: PortalTab) => string
  resolveHost: (url: string) => string
  resolveDescription: (tab: PortalTab) => string
  resolveOpenModeText: (tab: PortalTab) => string
}>()

defineEmits<{
  'filter-change': [filters: Record<string, unknown>]
  'size-change': [value: number]
  'current-change': [value: number]
  edit: [tab: PortalTab]
  delete: [id: string]
  'tab-click': [tab: PortalTab]
}>()

const { t } = useI18n()
</script>

<template>
  <div class="table-panel glue-panel">
    <div class="table-scroll-area glue-panel__body">
      <el-table
        :data="tabs"
        height="100%"
        style="width: 100%"
        class="links-table"
        @filter-change="$emit('filter-change', $event)"
      >
        <el-table-column width="64">
          <template #default="{ row }">
            <div class="row__media">
              <img
                v-if="resolvePreviewImage(row)"
                :src="resolvePreviewImage(row)"
                :alt="row.title"
              />
              <el-icon v-else><Link /></el-icon>
            </div>
          </template>
        </el-table-column>

        <el-table-column :label="t('websitenavigation.name')" min-width="220" sortable>
          <template #default="{ row }">
            <div class="row__title-wrap">
              <span class="row__title" @click="$emit('tab-click', row)">
                {{ row.title }}
              </span>
              <span class="row__host">{{ resolveHost(row.url) }}</span>
            </div>
          </template>
        </el-table-column>

        <el-table-column
          column-key="category"
          prop="category"
          :label="t('websitenavigation.categroy')"
          :filters="categoryFilters"
          min-width="120"
          sortable
        >
          <template #default="{ row }">
            {{ row.category || t('common.unnamedGroup') }}
          </template>
        </el-table-column>

        <el-table-column :label="t('common.description')" min-width="220">
          <template #default="{ row }">
            <span class="row__desc">{{ resolveDescription(row) }}</span>
          </template>
        </el-table-column>

        <el-table-column
          column-key="openMode"
          :label="t('websitenavigation.openmode')"
          :filters="openModeFilters"
          width="180"
          sortable
        >
          <template #default="{ row }">
            <el-tag disable-transitions effect="plain">
              {{ resolveOpenModeText(row) }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column :label="t('common.edit')" width="180" fixed="right">
          <template #default="{ row }">
            <div class="row__actions">
              <el-button
                size="small"
                :icon="EditPen"
                @click.stop="$emit('edit', row)"
              >
                {{ t('common.edit') }}
              </el-button>

              <el-button
                size="small"
                type="danger"
                plain
                :icon="Delete"
                @click.stop="$emit('delete', row.id)"
              >
                {{ t('common.delete') }}
              </el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <div class="glue-panel__footer">
      <el-pagination
        :current-page="currentPage"
        :page-size="pageSize"
        :page-sizes="[10, 20, 50, 100]"
        background
        layout="total, sizes, prev, pager, next, jumper"
        :total="total"
        @size-change="$emit('size-change', $event)"
        @current-change="$emit('current-change', $event)"
      />
    </div>
  </div>
</template>