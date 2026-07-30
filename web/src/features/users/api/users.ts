import { api } from '@/api/axios'
import { unwrap } from '@/utils/api'
import type { ApiResult } from '@/types/api'
import type { UserListItem } from '@/features/users/types'

export const usersApi = {
  async getUsers(): Promise<UserListItem[]> {
    const { data } = await api.get<ApiResult<UserListItem[]>>('/api/users')
    return unwrap(data)
  },
}
