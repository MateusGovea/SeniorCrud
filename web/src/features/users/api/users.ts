import { AxiosError } from 'axios'
import { api, API_V1_PREFIX } from '@/api/axios'
import { unwrap, unwrapVoid, ApiError } from '@/utils/api'
import type { ApiResult } from '@/types/api'
import type { UserListItem, UserResponse } from '@/features/users/types'

export interface CreateUserRequest {
  nome: string
  email: string
  password: string
  cpf?: string | null
  role: string
}

export interface UpdateUserRequest {
  nome: string
  email: string
  cpf?: string | null
  isActive: boolean
  role: string
}

async function requestCsv(
  url: string,
  params: Record<string, unknown> | undefined,
  fallbackFilename: string,
  fallbackError: string,
): Promise<{ blob: Blob; filename: string }> {
  try {
    const response = await api.get<Blob>(url, {
      params,
      responseType: 'blob',
    })

    const disposition = response.headers['content-disposition']
    let filename = fallbackFilename
    if (typeof disposition === 'string') {
      const match = /filename=(.+)/.exec(disposition)
      if (match) filename = match[1].replace(/['"]/g, '').trim()
    }

    return { blob: response.data, filename }
  } catch (error) {
    if (error instanceof AxiosError && error.response?.data instanceof Blob) {
      const text = await error.response.data.text()
      throw new ApiError(text || fallbackError)
    }
    throw error
  }
}

export const usersApi = {
  async getUsers(): Promise<UserListItem[]> {
    const { data } = await api.get<ApiResult<UserListItem[]>>(`${API_V1_PREFIX}/users`)
    return unwrap(data)
  },

  async getUserById(id: string): Promise<UserResponse> {
    const { data } = await api.get<ApiResult<UserResponse>>(`${API_V1_PREFIX}/users/${id}`)
    return unwrap(data)
  },

  async createUser(body: CreateUserRequest): Promise<UserResponse> {
    const { data } = await api.post<ApiResult<UserResponse>>(`${API_V1_PREFIX}/users`, body)
    return unwrap(data)
  },

  async updateUser(id: string, body: UpdateUserRequest): Promise<UserResponse> {
    const { data } = await api.put<ApiResult<UserResponse>>(`${API_V1_PREFIX}/users/${id}`, body)
    return unwrap(data)
  },

  async deleteUser(id: string): Promise<void> {
    const { data } = await api.delete<ApiResult<unknown>>(`${API_V1_PREFIX}/users/${id}`)
    unwrapVoid(data)
  },

  async exportUsersCsv(userIds?: string[]): Promise<{ blob: Blob; filename: string }> {
    return requestCsv(
      `${API_V1_PREFIX}/users/export/csv`,
      { userIds },
      'users.csv',
      'Erro ao exportar usuários',
    )
  },

  async exportUserCsv(id: string): Promise<{ blob: Blob; filename: string }> {
    return requestCsv(
      `${API_V1_PREFIX}/users/${id}/export/csv`,
      undefined,
      `user_${id}.csv`,
      'Erro ao exportar usuário',
    )
  },
}
