<script setup lang="ts">
import '../styles/SettingsPage.css'
import {
  Bell,
  Brush,
  Clock,
  Connection,
  InfoFilled,
  Monitor,
  Promotion,
  Setting,
  Tools,
} from '@element-plus/icons-vue'
import { useSettingsPage } from '../composables/UseSettingsPage'

const page = useSettingsPage()

const sectionIcons = {
  basic: Setting,
  startup: Promotion,
  notification: Bell,
  appearance: Brush,
  language: Connection,
  updates: Clock,
  advanced: Tools,
  about: InfoFilled,
}
</script>

<template>
  <div class="settings-feature-page">
    <el-row :gutter="16" class="settings-feature-page__stats">
      <el-col :xs="24" :sm="8">
        <el-card shadow="never" class="settings-kpi">
          <template #header>
            <div class="settings-kpi__header">
              <span>{{ page.t('settings.theme') }}</span>
              <el-icon><Brush /></el-icon>
            </div>
          </template>
          <div class="settings-kpi__value">{{ page.settingForm.theme }}</div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="8">
        <el-card shadow="never" class="settings-kpi">
          <template #header>
            <div class="settings-kpi__header">
              <span>{{ page.t('settings.defaultView') }}</span>
              <el-icon><Monitor /></el-icon>
            </div>
          </template>
          <div class="settings-kpi__value">{{ page.settingForm.defaultView }}</div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="8">
        <el-card shadow="never" class="settings-kpi">
          <template #header>
            <div class="settings-kpi__header">
              <span>{{ page.t('settings.version') }}</span>
              <el-icon><InfoFilled /></el-icon>
            </div>
          </template>
          <div class="settings-kpi__value">{{ page.appVersion.value }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" class="settings-feature-page__body">
      <el-col :xs="24" :lg="7" :xl="6" class="settings-feature-page__nav">
        <el-card shadow="never" class="settings-panel settings-panel--nav">
          <template #header>
            <div class="settings-panel__header">
              <h2>{{ page.t('settings.title') }}</h2>
            </div>
          </template>

          <el-menu
            :default-active="page.activeSection.value"
            class="settings-menu"
            @select="(key) => page.switchSection(key as typeof page.activeSection.value)"
          >
            <el-menu-item
              v-for="item in page.sectionItems"
              :key="item.key"
              :index="item.key"
            >
              <el-icon>
                <component :is="sectionIcons[item.key]" />
              </el-icon>
              <span>{{ page.t(item.labelKey) }}</span>
            </el-menu-item>
          </el-menu>
        </el-card>
      </el-col>

      <el-col :xs="24" :lg="17" :xl="18" class="settings-feature-page__content">
        <el-card shadow="never" class="settings-panel settings-panel--content">
          <template #header>
            <div class="settings-page-head">
              <div>
                <h1>{{ page.pageTitle.value }}</h1>
              </div>

              <el-button type="primary" @click="page.saveSettings">
                {{ page.t('common.save') }}
              </el-button>
            </div>
          </template>

          <el-scrollbar class="settings-scroll">
            <template v-if="page.activeSection.value === 'basic'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.hotkey')">
                        <el-input
                          :model-value="page.isRecordingHotkey.value ? page.hotkeyDisplayValue.value : page.settingForm.hotkeyCombo"
                          readonly
                          :placeholder="page.t('settings.hotkeyPlaceholder')"
                          @focus="page.startHotkeyRecording"
                          @click="page.startHotkeyRecording"
                          @blur="page.stopHotkeyRecording"
                        />
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.defaultView')">
                        <el-select v-model="page.settingForm.defaultView">
                          <el-option
                            v-for="item in page.defaultViewOptions"
                            :key="item.value"
                            :label="page.t(item.labelKey)"
                            :value="item.value"
                          />
                        </el-select>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="8">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.use24HourTime') }}</span>
                      <el-switch v-model="page.settingForm.use24HourTime" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="8">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.showWeekNumber') }}</span>
                      <el-switch v-model="page.settingForm.showWeekNumber" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="8">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.confirmBeforeExit') }}</span>
                      <el-switch v-model="page.settingForm.confirmBeforeExit" />
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'startup'">
              <el-alert class="settings-alert" type="info" :closable="false">
                {{ page.t('settings.desktopOnlyHint') }}
              </el-alert>

              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="8">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.launchAtStartup') }}</span>
                      <el-switch v-model="page.settingForm.launchAtStartup" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="16">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.startupBehavior')">
                        <el-radio-group v-model="page.settingForm.startupBehavior">
                          <el-radio
                            v-for="item in page.startupBehaviorOptions"
                            :key="item.value"
                            :value="item.value"
                          >
                            {{ page.t(item.labelKey) }}
                          </el-radio>
                        </el-radio-group>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.closeButtonBehavior')">
                        <el-radio-group v-model="page.settingForm.closeButtonBehavior">
                          <el-radio
                            v-for="item in page.closeButtonBehaviorOptions"
                            :key="item.value"
                            :value="item.value"
                          >
                            {{ page.t(item.labelKey) }}
                          </el-radio>
                        </el-radio-group>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'notification'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="16">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.notificationMethods')">
                        <el-checkbox-group v-model="page.settingForm.notificationMethods">
                          <el-checkbox
                            v-for="item in page.notificationMethodOptions"
                            :key="item.value"
                            :value="item.value"
                          >
                            {{ page.t(item.labelKey) }}
                          </el-checkbox>
                        </el-checkbox-group>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="8">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.doNotDisturb') }}</span>
                      <el-switch v-model="page.settingForm.doNotDisturbEnabled" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.doNotDisturbTimeRange')">
                        <el-time-picker
                          v-model="page.settingForm.doNotDisturbRange"
                          is-range
                          format="HH:mm"
                          value-format="HH:mm"
                          range-separator="-"
                          start-placeholder="22:00"
                          end-placeholder="08:00"
                        />
                        <div class="settings-item-card__hint">
                          {{ page.t('settings.doNotDisturbHint') }}
                        </div>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'appearance'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="14">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.theme')">
                        <el-radio-group v-model="page.settingForm.theme">
                          <el-radio value="light">{{ page.t('settings.light') }}</el-radio>
                          <el-radio value="dark">{{ page.t('settings.dark') }}</el-radio>
                          <el-radio value="auto">{{ page.t('settings.auto') }}</el-radio>
                        </el-radio-group>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="10">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.primaryColor')">
                        <el-color-picker v-model="page.settingForm.primaryColor" />
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.reduceAnimation') }}</span>
                      <el-switch v-model="page.settingForm.reduceAnimation" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.compactMode') }}</span>
                      <el-switch v-model="page.settingForm.compactMode" />
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'language'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.interfaceLanguage')">
                        <el-select v-model="page.settingForm.locale">
                          <el-option :label="page.t('settings.languageZhCn')" value="zh-CN" />
                          <el-option :label="page.t('settings.languageEnUs')" value="en-US" />
                        </el-select>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'updates'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="14">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.updateCheckMode')">
                        <el-radio-group v-model="page.settingForm.updateCheckMode">
                          <el-radio
                            v-for="item in page.updateCheckModeOptions"
                            :key="item.value"
                            :value="item.value"
                          >
                            {{ page.t(item.labelKey) }}
                          </el-radio>
                        </el-radio-group>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="10">
                    <el-card shadow="never" class="settings-item-card settings-item-card--toggle">
                      <span>{{ page.t('settings.autoCheckUpdateOnStartup') }}</span>
                      <el-switch v-model="page.settingForm.autoCheckUpdateOnStartup" />
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.updateChannel')">
                        <el-select v-model="page.settingForm.updateChannel">
                          <el-option
                            v-for="item in page.updateChannelOptions"
                            :key="item.value"
                            :label="page.t(item.labelKey)"
                            :value="item.value"
                          />
                        </el-select>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.checkUpdateNow')">
                        <el-button @click="page.manualCheckUpdate">
                          {{ page.t('settings.checkUpdateNow') }}
                        </el-button>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else-if="page.activeSection.value === 'advanced'">
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="10">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.resetKeepData')">
                        <el-switch v-model="page.settingForm.resetKeepData" />
                        <div class="settings-item-card__hint">
                          {{
                            page.settingForm.resetKeepData
                              ? page.t('settings.keepData')
                              : page.t('settings.clearData')
                          }}
                        </div>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="14">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.resetFactory')">
                        <el-button type="danger" plain @click="page.resetSettings">
                          {{ page.t('settings.resetFactory') }}
                        </el-button>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>

            <template v-else>
              <el-form label-position="top" class="settings-form">
                <el-row :gutter="16">
                  <el-col :xs="24" :md="6">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.version')">
                        <div class="settings-static-value">{{ page.appVersion.value }}</div>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="6">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.buildTime')">
                        <div class="settings-static-value">{{ page.buildTime.value }}</div>
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24" :md="12">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.feedbackContact')">
                        <el-input
                          v-model="page.settingForm.feedbackContact"
                          :placeholder="page.t('settings.feedbackContactPlaceholder')"
                        />
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.feedbackMessage')">
                        <el-input
                          v-model="page.settingForm.feedbackMessage"
                          type="textarea"
                          :rows="4"
                          :placeholder="page.t('settings.feedbackMessagePlaceholder')"
                        />
                      </el-form-item>
                    </el-card>
                  </el-col>

                  <el-col :xs="24">
                    <el-card shadow="never" class="settings-item-card">
                      <el-form-item :label="page.t('settings.feedbackMessage')">
                        <div class="settings-link-actions">
                          <el-button @click="page.copyRuntimeLog">
                            {{ page.t('settings.copyLogs') }}
                          </el-button>
                          <el-button @click="page.openGithubIssues">
                            {{ page.t('settings.openGithub') }}
                          </el-button>
                          <el-button @click="page.openSupportEmail">
                            {{ page.t('settings.sendEmail') }}
                          </el-button>
                        </div>
                      </el-form-item>
                    </el-card>
                  </el-col>
                </el-row>
              </el-form>
            </template>
          </el-scrollbar>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>