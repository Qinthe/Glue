import { http } from './Http'
import type { ApiResponse, PortalTab, CreateTabRequest, UpdateTabRequest } from '@/types/Index'

const base = (userId: string) => `/tab/${userId}`

export const tabApi = {
  async getAll(userId: string): Promise<PortalTab[]> {
    const res = await http.get<ApiResponse<PortalTab[]>>(base(userId))

    if (!res.success) {
      throw new Error(res.message || 'Load tabs failed')
    }
    console.log("API Response Message :" + res.message);

    return res.data ?? []
  },

  async create(userId: string, data: CreateTabRequest): Promise<PortalTab> {
    const payload = toCreateTabDto(data)
    const res = await http.post<ApiResponse<PortalTab>>(base(userId), payload)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Create tab failed')
    }
    console.log(res);
    console.log(res.data);

    return res.data
  },


  async update(userId: string, id: string, data: UpdateTabRequest): Promise<PortalTab> {
    console.log(data)
    const res = await http.put<ApiResponse<PortalTab>>(`${base(userId)}/${id}`, data)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Update tab failed')
    }

    return res.data
  },

  async remove(userId: string, id: string): Promise<void> {
    const res = await http.delete<ApiResponse<null>>(`${base(userId)}/${id}`)

    if (!res.success) {
      throw new Error(res.message || 'Delete tab failed')
    }
  },

  async reorder(userId: string, items: Array<{ id: string; sortOrder: number }>): Promise<void> {
    const res = await http.put<ApiResponse<null>>(`${base(userId)}/reorder`, { items })

    if (!res.success) {
      throw new Error(res.message || 'Reorder tabs failed')
    }
  },
}

type TabRequestDto = {
  id: string
  title: string
  url: string
  icon: string
  image: string
  category: string
  openMode: number,
  sortOrder: number
  isPinned: boolean
  description: string | null
  color: string
  createdAt: string | null
}

function toCreateTabDto(data: CreateTabRequest): TabRequestDto {
  return {
    id: '',
    title: data.title.trim(),
    url: data.url.trim(),
    icon: data.icon?.trim() ?? '',
    image: data.image?.trim() ?? '',
    category: data.category?.trim() ?? '',
    openMode: data.openMode,
    sortOrder: 0,
    isPinned: false,
    description: data.description?.trim() || null,
    color: data.color?.trim() ?? '',
    createdAt: null,
  }
}