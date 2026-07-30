import { useQuery } from '@tanstack/react-query'
import { usersApi } from '@/features/users/api'

const usersQueryKey = ['users'] as const

export function useUsers() {
  return useQuery({
    queryKey: usersQueryKey,
    queryFn: usersApi.getUsers,
  })
}
