import { appErrorFromProblemDetails, networkAppError, type AppError } from './errors'

declare global {
  interface Window {
    __CONFIGLENS_API_BASE__?: string
  }
}

const TOKEN_STORAGE_KEY = 'configlens.token'
const USERNAME_STORAGE_KEY = 'configlens.username'

/** Resolution order: runtime env-config.js (Docker) -> Vite build-time env (local dev) -> same-origin. */
function apiBaseUrl(): string {
  return window.__CONFIGLENS_API_BASE__ || import.meta.env.VITE_API_BASE_URL || ''
}

export function getToken(): string | null {
  return sessionStorage.getItem(TOKEN_STORAGE_KEY)
}

export function getUsername(): string | null {
  return sessionStorage.getItem(USERNAME_STORAGE_KEY)
}

export function setSession(token: string | null, username: string | null): void {
  if (token && username) {
    sessionStorage.setItem(TOKEN_STORAGE_KEY, token)
    sessionStorage.setItem(USERNAME_STORAGE_KEY, username)
  } else {
    sessionStorage.removeItem(TOKEN_STORAGE_KEY)
    sessionStorage.removeItem(USERNAME_STORAGE_KEY)
  }
}

type UnauthorizedListener = () => void
let unauthorizedListener: UnauthorizedListener | null = null

/** Registered once at app startup (see app/store.ts) so a 401 can clear session state without httpClient depending on Redux. */
export function onUnauthorized(listener: UnauthorizedListener): void {
  unauthorizedListener = listener
}

export class ApiRequestError extends Error {
  readonly appError: AppError

  constructor(appError: AppError) {
    super(appError.message)
    this.appError = appError
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const headers = new Headers(init?.headers)
  headers.set('Accept', 'application/json')
  if (init?.body) {
    headers.set('Content-Type', 'application/json')
  }
  const token = getToken()
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  let response: Response
  try {
    response = await fetch(`${apiBaseUrl()}${path}`, { ...init, headers })
  } catch {
    throw new ApiRequestError(networkAppError())
  }

  if (response.status === 401) {
    setSession(null, null)
    unauthorizedListener?.()
  }

  if (!response.ok) {
    const body = await response.json().catch(() => undefined)
    throw new ApiRequestError(appErrorFromProblemDetails(response.status, body))
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

export const httpClient = {
  get: <T>(path: string) => request<T>(path, { method: 'GET' }),
  post: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: 'POST', body: body !== undefined ? JSON.stringify(body) : undefined }),
}
