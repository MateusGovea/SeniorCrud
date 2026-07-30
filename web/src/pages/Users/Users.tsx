import { useUsers } from '@/features/users/hooks'
import { UsersTable } from '@/features/users/components/UsersTable'
import { LoadingState } from '@/features/users/components/LoadingState'
import { ErrorState } from '@/features/users/components/ErrorState'
import { EmptyState } from '@/features/users/components/EmptyState'

export function Users() {
  const { data: users, isLoading, isError, error, refetch } = useUsers()

  const content = () => {
    if (isLoading) return <LoadingState />
    if (isError) return <ErrorState message={error?.message} onRetry={refetch} />
    if (!users || users.length === 0) return <EmptyState />
    return <UsersTable users={users} />
  }

  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold text-gray-900">Usuários</h1>
      {content()}
    </div>
  )
}
