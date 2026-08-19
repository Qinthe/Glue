import axios, { AxiosHeaders } from 'axios'
import { resolveResponseErrorMessage } from '@/utils/PromptMessageUtils'
import { clearAuthSession, getAccessToken } from '@/utils/SessionStorage'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api'

export const instance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
})

instance.interceptors.request.use((config) => {
  const token = getAccessToken()

  if (!(config.headers instanceof AxiosHeaders)) {
    config.headers = new AxiosHeaders(config.headers)
  }

  const isAuthRequest =
    config.url?.startsWith('/user/login') ||
    config.url?.startsWith('/user/register')

  if (isAuthRequest || !token) {
    config.headers.delete('Authorization')
  } else {
    config.headers.set('Authorization', `Bearer ${token}`)
  }

  return config
})

instance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      clearAuthSession()

      if (window.location.pathname !== '/auth') {
        const redirect = encodeURIComponent(window.location.pathname + window.location.search)
        window.location.href = `/auth?redirect=${redirect}`
      }
    }
    console.error('API Request Error:', {
      method: error.config?.method,
      url: error.config?.url,
      requestData: error.config?.data,
      status: error.response?.status,
      responseData: error.response?.data,
    })
    console.error("API Response Message :" + error?.response?.data?.message);

    return Promise.reject(new Error(resolveResponseErrorMessage(error)))
  }
)


export const http = {
  async get<T>(url: string, config?: object): Promise<T> {
    const res = await instance.get<T>(url, config)
    return res.data
  },

  async post<T>(url: string, data?: unknown, config?: object): Promise<T> {
    const res = await instance.post<T>(url, data, config)
    return res.data
  },

  async put<T>(url: string, data?: unknown, config?: object): Promise<T> {
    const res = await instance.put<T>(url, data, config)
    return res.data
  },
  async patch<T>(url: string, data?: unknown, config?: object): Promise<T> {
    const res = await instance.patch<T>(url, data, config)
    return res.data
  },
  async delete<T>(url: string, config?: object): Promise<T> {
    const res = await instance.delete<T>(url, config)
    return res.data
  },

}