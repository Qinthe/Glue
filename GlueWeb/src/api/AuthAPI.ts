import { http } from './Http'
import type { ApiResponse } from '@/types/Index'
import type {
  AuthApiSession,
  AuthSession,
  LoginRequest,
  RegisterRequest,
} from '@/types/User'

function mapSession(payload: AuthApiSession): AuthSession {
  return {
    accessToken: payload.accessToken,
    refreshToken: payload.refreshToken ?? '',
    user: {
      id: payload.user.id,
      username: payload.user.userName,
      email: payload.user.email,
      role: payload.user.role,
      balance: payload.user.balance,
    },
  }
}

export const authApi = {
  async login(data: LoginRequest): Promise<AuthSession> {
    const res = await http.post<ApiResponse<AuthApiSession>>('/user/login', data)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Login failed')
    }

    return mapSession(res.data)
  },

  async register(data: RegisterRequest): Promise<AuthSession> {
    const res = await http.post<ApiResponse<AuthApiSession>>('/user/register', data)

    if (!res.success || !res.data) {
      throw new Error(res.message || 'Registration failed')
    }

    return mapSession(res.data)
  }
}
