export const TabOpenMode = {
  Iframe: 0,
  NewTab: 1,
  NewWindow: 2,
} as const
export type TabOpenMode = typeof TabOpenMode[keyof typeof TabOpenMode]

export const PluginCategory = {
  Tool: 0,
  Widget: 1,
  Monitor: 2,
  Custom: 3,
} as const
export type PluginCategory = typeof PluginCategory[keyof typeof PluginCategory]

export const PluginSize = {
  Small: 0,
  Medium: 1,
  Large: 2,
  Fullscreen: 3,
} as const
export type PluginSize = typeof PluginSize[keyof typeof PluginSize]

export const DesktopAppLaunchMode = {
  Protocol: 'protocol',
  Native: 'native',
} as const
export type DesktopAppLaunchMode =
  typeof DesktopAppLaunchMode[keyof typeof DesktopAppLaunchMode]

export const TaskStatus = {
  Pending: 'pending',
  InProgress: 'in-progress',
  Completed: 'completed',
} as const
export type TaskStatus = typeof TaskStatus[keyof typeof TaskStatus]

export interface PortalTab {
  id: string
  title: string
  url: string
  icon?: string
  image?: string
  category?: string
  openMode: TabOpenMode
  sortOrder: number
  isPinned: boolean
  description?: string
  color?: string
  createdAt: string
}

export interface CreateTabRequest {
  title: string
  url: string
  icon?: string
  image?: string
  category?: string
  openMode: TabOpenMode
  description?: string
  color?: string
}

export interface UpdateTabRequest {
  title?: string
  url?: string
  icon?: string
  image?: string
  category?: string
  openMode?: TabOpenMode
  sortOrder?: number
  isPinned?: boolean
  description?: string
  color?: string
}

export interface UserSetting {
  locale: string
  theme: 'light' | 'dark' | 'auto'
  hotkeyCombo: string
  primaryColor: string
  sidebarCollapsed: boolean

  launchAtStartup: boolean
  startupBehavior: 'show-main' | 'tray-only' | 'silent'
  closeButtonBehavior: 'exit' | 'minimize-to-tray'

  defaultView: 'tasks' | 'memos' | 'links'

  notificationMethods: Array<'system' | 'sound' | 'tray-flash'>
  doNotDisturbEnabled: boolean
  doNotDisturbRange: [string, string]

  updateCheckMode: 'manual' | 'auto'
  autoCheckUpdateOnStartup: boolean
  updateChannel: 'stable' | 'beta'

  use24HourTime: boolean
  showWeekNumber: boolean
  confirmBeforeExit: boolean
  reduceAnimation: boolean
  compactMode: boolean

  feedbackContact: string
  feedbackMessage: string
  resetKeepData: boolean
}

export interface DesktopAppShortcut {
  id: string
  title: string
  icon?: string
  category?: string
  description?: string
  color?: string
  launchMode: DesktopAppLaunchMode
  protocolUrl?: string
  launchTarget?: string
  sortOrder: number
  createdAt: string
}

export interface CreateDesktopAppRequest {
  title: string
  icon?: string
  category?: string
  description?: string
  color?: string
  launchMode: DesktopAppLaunchMode
  protocolUrl?: string
  launchTarget?: string
}

export interface UpdateDesktopAppRequest {
  title?: string
  icon?: string
  category?: string
  description?: string
  color?: string
  launchMode?: DesktopAppLaunchMode
  protocolUrl?: string
  launchTarget?: string
  sortOrder?: number
}

export interface TaskItem {
  id: string
  title: string
  description?: string
  scheduledDate: string
  startAt: string
  endAt: string
  progress: number
  status: TaskStatus
  reminderEnabled: boolean
  reminderMinutesBefore: number
  lastReminderAt?: string
  completedAt?: string
  createdAt: string
  updatedAt: string
}

export interface CreateTaskRequest {
  title: string
  description?: string
  scheduledDate?: string
  startAt?: string
  endAt?: string
  progress?: number
  status?: TaskStatus
  reminderEnabled?: boolean
  reminderMinutesBefore?: number
}

export interface UpdateTaskRequest {
  title?: string
  description?: string
  scheduledDate?: string
  startAt?: string
  endAt?: string
  progress?: number
  status?: TaskStatus
  reminderEnabled?: boolean
  reminderMinutesBefore?: number
  lastReminderAt?: string
  completedAt?: string
}

export interface MemoNote {
  id: string
  title: string
  content: string
  category: string
  tags: string[]
  createdAt: string
  updatedAt: string
}

export interface CreateMemoRequest {
  title?: string
  content?: string
  category?: string
  tags?: string[]
}

export interface UpdateMemoRequest {
  title?: string
  content?: string
  category?: string
  tags?: string[]
}

export interface ApiResponse<T> {
  success: boolean
  statusCode: number
  message: string,
  Prompttextcode:string,
  data: T
}