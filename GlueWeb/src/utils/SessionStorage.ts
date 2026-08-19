interface PersistedAuthUser {
  id?: string
}

interface PersistedAuthState {
  accessToken?: string
  refreshToken?: string
  currentUser?: PersistedAuthUser | null
}

function readPersistedState<T>(key: string): Partial<T> | null {
  try {
    const raw = localStorage.getItem(key)
    return raw ? (JSON.parse(raw) as Partial<T>) : null
  } catch {
    return null
  }
}

export function getPersistedAuthState() {
  return readPersistedState<PersistedAuthState>('auth')
}

export function getAccessToken() {
  return getPersistedAuthState()?.accessToken?.trim() ?? ''
}

export function getRefreshToken() {
  return getPersistedAuthState()?.refreshToken?.trim() ?? ''
}

export function getUserId() {
  return getPersistedAuthState()?.currentUser?.id?.trim() ?? ''
}

export function clearAuthSession() {
  localStorage.removeItem('auth')
}