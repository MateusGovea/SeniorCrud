import { useState } from 'react'
import { useUsers } from '@/features/users/hooks'
import { UsersTable } from '@/features/users/components/UsersTable'
import { LoadingState } from '@/features/users/components/LoadingState'
import { ErrorState } from '@/features/users/components/ErrorState'
import { EmptyState } from '@/features/users/components/EmptyState'
import { UserModal } from '@/features/users/components/UserModal'
import { DeleteUserDialog } from '@/features/users/components/DeleteUserDialog'
import { Button } from '@/components/Button'
import type { UserListItem } from '@/features/users/types'

type ModalState =
  | { type: 'closed' }
  | { type: 'create' }
  | { type: 'edit'; user: UserListItem }

type DeleteState =
  | { type: 'closed' }
  | { type: 'confirm'; user: UserListItem }

export function Users() {
  const { data: users, isLoading, isError, error, refetch } = useUsers()
  const [modal, setModal] = useState<ModalState>({ type: 'closed' })
  const [deleteDialog, setDeleteDialog] = useState<DeleteState>({ type: 'closed' })

  function handleCreate() {
    setModal({ type: 'create' })
  }

  function handleEdit(user: UserListItem) {
    setModal({ type: 'edit', user })
  }

  function handleDelete(user: UserListItem) {
    setDeleteDialog({ type: 'confirm', user })
  }

  function closeModal() {
    setModal({ type: 'closed' })
  }

  function closeDelete() {
    setDeleteDialog({ type: 'closed' })
  }

  const renderContent = () => {
    if (isLoading) return <LoadingState />
    if (isError) return <ErrorState message={error?.message} onRetry={refetch} />
    if (!users || users.length === 0) return <EmptyState />
    return <UsersTable users={users} onEdit={handleEdit} onDelete={handleDelete} />
  }

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Usuários</h1>
        <Button onClick={handleCreate}>Novo Usuário</Button>
      </div>

      {renderContent()}

      {modal.type !== 'closed' && (
        <UserModal
          isOpen
          onClose={closeModal}
          mode={modal.type}
          user={modal.type === 'edit' ? modal.user : undefined}
        />
      )}

      {deleteDialog.type === 'confirm' && (
        <DeleteUserDialog
          isOpen
          onClose={closeDelete}
          user={deleteDialog.user}
        />
      )}
    </div>
  )
}
