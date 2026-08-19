<script setup lang="ts">
import '../styles/LinksDrawer.css'
import { useI18n } from 'vue-i18n'
import { TabOpenMode, type TabOpenMode as TabOpenModeType } from '@/types/Index'

const model = defineModel<boolean>({ required: true })

defineProps<{
  title: string
  form: {
    title: string
    url: string
    category: string
    openMode: TabOpenModeType
    image: string
    color: string
    description: string
  }
  categoryOptions: string[]
  isEditing: boolean
}>()

defineEmits<{
  submit: []
  close: []
}>()

const { t } = useI18n()
</script>

<template>
  <el-drawer
    v-model="model"
    :title="title"
    :size="`var(--glue-drawer-width)`"
    destroy-on-close
    class="link-drawer"
  >
    <div class="links-editor">
      <el-form label-position="top" class="links-editor__form">
        <section class="links-editor-section">
          <el-form-item :label="t('websitenavigation.name')" required>
            <el-input
              v-model="form.title"
              :placeholder="t('websitenavigation.nameexample')"
            />
          </el-form-item>

          <el-form-item :label="t('websitenavigation.url')" required>
            <el-input
              v-model="form.url"
              :placeholder="t('websitenavigation.urlexample')"
            />
          </el-form-item>

          <el-form-item :label="t('websitenavigation.categroy')">
            <el-select
              v-model="form.category"
              filterable
              allow-create
              default-first-option
              clearable
              style="width: 100%"
              :placeholder="t('websitenavigation.optional')"
            >
              <el-option
                v-for="category in categoryOptions"
                :key="category"
                :label="category"
                :value="category"
              />
            </el-select>
          </el-form-item>

          <el-form-item class="links-editor__description-item" :label="t('common.description')">
            <el-input
              v-model="form.description"
              type="textarea"
              :rows="4"
              :placeholder="t('websitenavigation.descriptionPlaceholder')"
            />
          </el-form-item>
        </section>

        <section class="links-editor-section">
          <el-form-item :label="t('websitenavigation.openmode')">
            <el-select
              v-model="form.openMode"
              style="width: 100%"
            >
              <el-option
                :label="t('websitenavigation.methodiframe')"
                :value="TabOpenMode.Iframe"
              />
              <el-option
                :label="t('websitenavigation.methodnewtab')"
                :value="TabOpenMode.NewTab"
              />
              <el-option
                :label="t('websitenavigation.methodnewwindow')"
                :value="TabOpenMode.NewWindow"
              />
            </el-select>
          </el-form-item>

          <el-form-item :label="t('websitenavigation.imageurl')">
            <el-input
              v-model="form.image"
              :placeholder="t('websitenavigation.imageurlexample')"
            />
          </el-form-item>

          <el-form-item class="links-editor__color-item" :label="t('websitenavigation.themecolor')">
            <div class="color-field">
              <el-color-picker v-model="form.color" />
              <span class="color-field__value">{{ form.color }}</span>
            </div>
          </el-form-item>
        </section>
      </el-form>
    </div>

    <template #footer>
      <div class="drawer__footer">
        <el-button @click="$emit('close')">
          {{ t('common.cancel') }}
        </el-button>
        <el-button type="primary" @click="$emit('submit')">
          {{ isEditing ? t('common.saveChanges') : t('common.create') }}
        </el-button>
      </div>
    </template>
  </el-drawer>
</template>