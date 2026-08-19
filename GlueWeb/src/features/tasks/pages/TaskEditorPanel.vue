<script setup lang="ts">
import { type PropType } from 'vue'
import { useTasksPage } from '../composables/UseTasksPage'

const props = defineProps({
  page: {
    type: Object as PropType<ReturnType<typeof useTasksPage>>,
    required: true,
  },
  isCreateMode: {
    type: Boolean,
    required: true,
  },
  isEditMode: {
    type: Boolean,
    required: true,
  },
})

const emit = defineEmits<{
  cancel: []
  submit: []
}>()
</script>

<template>
  <div class="tasks-editor">
    <el-form label-position="top" class="tasks-editor__form">
      <section class="tasks-editor-section">
        <el-form-item class="tasks-editor__title-item" :label="props.page.t('tasks.taskTitle')">
          <el-input
            v-model="props.page.taskForm.title"
            clearable
            :placeholder="props.page.t('tasks.taskTitlePlaceholder')"
          />
        </el-form-item>

        <el-form-item :label="props.page.t('common.description')">
          <el-input
            v-model="props.page.taskForm.description"
            type="textarea"
            :rows="6"
            :placeholder="props.page.t('common.optionalDescription')"
          />
        </el-form-item>

        <el-form-item label="分组">
          <el-select
            v-model="props.page.taskForm.groupNames"
            multiple
            filterable
            allow-create
            default-first-option
            clearable
            collapse-tags
            collapse-tags-tooltip
            style="width: 100%"
            placeholder="输入新分组或选择已有分组"
          >
            <el-option
              v-for="group in props.page.taskGroupOptions.value"
              :key="group.value"
              :label="group.label"
              :value="group.value"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="props.page.t('tasks.progress')">
          <div class="tasks-progress-field">
            <el-slider
              v-model="props.page.taskForm.progress"
              :min="0"
              :max="100"
            />
            <el-tag type="info" round class="tasks-progress-field__value">
              {{ props.page.taskForm.progress }}%
            </el-tag>
          </div>
        </el-form-item>
      </section>

      <section class="tasks-editor-section">
        <div class="tasks-editor-grid">
          <el-form-item :label="props.page.t('tasks.startTime')">
            <el-date-picker
              v-model="props.page.taskForm.startAt"
              type="datetime"
              style="width: 100%"
            />
          </el-form-item>

          <el-form-item :label="props.page.t('tasks.endTime')">
            <el-date-picker
              v-model="props.page.taskForm.endAt"
              type="datetime"
              style="width: 100%"
            />
          </el-form-item>
        </div>

        <el-form-item :label="props.page.t('tasks.reminder')">
          <div class="tasks-reminder-field">
            <el-switch v-model="props.page.taskForm.reminderEnabled" />
            <el-input-number
              v-model="props.page.taskForm.reminderMinutesBefore"
              :min="1"
              :max="1440"
              :disabled="!props.page.taskForm.reminderEnabled"
            />
            <span>{{ props.page.t('tasks.reminderBefore') }}</span>
          </div>
        </el-form-item>
      </section>
    </el-form>

    <div class="tasks-editor__topbar">
      <div class="tasks-editor__topbar-spacer" />
      <div class="tasks-editor__actions">
        <el-button @click="emit('cancel')">
          {{ props.page.t('common.cancel') }}
        </el-button>
        <el-button type="primary" @click="emit('submit')">
          {{ props.page.editorActionText.value }}
        </el-button>
      </div>
    </div>
  </div>
</template>