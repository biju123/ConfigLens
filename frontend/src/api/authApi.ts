import { httpClient } from './httpClient'
import type { LoginRequest, LoginResponse } from './types'

export const authApi = {
  login: (request: LoginRequest) => httpClient.post<LoginResponse>('/api/auth/login', request),
}
