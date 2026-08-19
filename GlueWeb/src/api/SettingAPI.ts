import { http } from './Http'
import type { UserSetting } from '@/types/Index'

export const settingApi = {
  get: (userId: string): Promise<UserSetting> =>
    http.get(`/users/${userId}/settings`),
  update: (userId: string, data: Partial<UserSetting>): Promise<UserSetting> =>
    http.put(`/users/${userId}/settings`, data),
}