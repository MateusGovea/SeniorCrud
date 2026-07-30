import { api } from '@/api/axios'
import { unwrap, unwrapVoid } from '@/utils/api'
import type { ApiResult } from '@/types/api'
import type { UserListItem, UserResponse } from '@/features/users/types'

export interface CreateUserRequest {
  nome: string
  email: string
  password: string
  cpf?: string | null
}

export interface UpdateUserRequest {
  nome: string
  email: string
  cpf?: string | null
  isActive: boolean
}

export const usersApi = {
  async getUsers(): Promise<UserListItem[]> {
    const { data } = await api.get<ApiResult<UserListItem[]>>('/api/users')
    return unwrap(data)
  },

  async getUserById(id: string): Promise<UserResponse> {
    const { data } = await api.get<ApiResult<UserResponse>>(`/api/users/${id}`)
    return unwrap(data)
  },

  async createUser(body: CreateUserRequest): Promise<UserResponse> {
    const { data } = await api.post<ApiResult<UserResponse>>('/api/users', body)
    return unwrap(data)
  },

  async updateUser(id: string, body: UpdateUserRequest): Promise<UserResponse> {
    const { data } = await api.put<ApiResult<UserResponse>>(`/api/users/${id}`, body)
    return unwrap(data)
  },

  async deleteUser(id: string): Promise<void> {
    const { data } = await api.delete<ApiResult<unknown>>(`/api/users/${id}`)
    unwrapVoid(data)
  },
}
