export interface AuthUser {
  id: string
  username: string
  email: string
  role?: string
  balance?: number
}

export interface AuthSession {
  accessToken: string
  refreshToken?: string | null
  user: AuthUser
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest extends LoginRequest {
  userName: string
  confirmPassword: string
}

export interface AuthApiUser {
  id: string
  userName: string
  email: string
  role?: string
  balance?: number
}

export interface AuthApiSession {
  accessToken: string
  refreshToken?: string | null
  user: AuthApiUser
}