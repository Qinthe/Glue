import { http } from './Http'
import type {
  ApiResponse,
  CreateTaskRequest,
  TaskItem,
  UpdateTaskRequest,
} from '@/types/Index'

const base = '/Task'

export const taskApi = {
  async getAll(): Promise<TaskItem[]> {
    const res = await http.get<ApiResponse<TaskItem[]>>(base)

    if (!res.success) {
      throw new Error(res.message || 'Load tasks failed')
    }

    return res.data ?? []
  },

  async getById(id: string): Promise<TaskItem> {
    const res = await http.get<ApiResponse<TaskItem>>(`${base}/${id}`)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Load task failed')
    }

    return res.data
  },

  async create(data: CreateTaskRequest): Promise<TaskItem> {
    const res = await http.post<ApiResponse<TaskItem>>(base, data)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Create task failed')
    }

    return res.data
  },

  async update(id: string, data: UpdateTaskRequest): Promise<TaskItem> {
    const res = await http.put<ApiResponse<TaskItem>>(`${base}/${id}`, data)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Update task failed')
    }

    return res.data
  },

  async complete(id: string): Promise<TaskItem> {
    const res = await http.patch<ApiResponse<TaskItem>>(`${base}/${id}/complete`)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Complete task failed')
    }

    return res.data
  },

  async updateReminder(
    id: string,
    data: Partial<
      Pick<
        UpdateTaskRequest,
        'reminderEnabled' | 'reminderMinutesBefore' | 'lastReminderAt'
      >
    >
  ): Promise<TaskItem> {
    const res = await http.patch<ApiResponse<TaskItem>>(
      `${base}/${id}/reminder`,
      data
    )

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Update reminder failed')
    }

    return res.data
  },

  async remove(id: string): Promise<void> {
    const res = await http.delete<ApiResponse<null>>(`${base}/${id}`)

    if (!res.success) {
      throw new Error(res.message || 'Delete task failed')
    }
  },
}