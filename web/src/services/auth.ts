import { api, API_V1_PREFIX } from '@/api/axios'
import type { LoginRequest, LoginResponse } from '@/types/auth'
import type { ApiResult } from '@/types/api'

export class AuthError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'AuthError'
  }
}

export const authService = {
  async login(request: LoginRequest): Promise<LoginResponse> {
    const { data } = await api.post<ApiResult<LoginResponse>>(`${API_V1_PREFIX}/auth/login`, request)

    if (!data.isSuccess || !data.value) {
      throw new AuthError(data.error?.description ?? 'Erro ao autenticar')
    }

    return data.value
  },
}
