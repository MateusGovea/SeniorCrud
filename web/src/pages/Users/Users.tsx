import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
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
  const navigate = useNavigate()
  const { data: users, isLoading, isError, error, refetch } = useUsers()
  const [modal, setModal] = useState<ModalState>({ type: 'closed' })
  const [deleteDialog, setDeleteDialog] = useState<DeleteState>({ type: 'closed' })

  const handleViewAddresses = useCallback(
    (user: UserListItem) => navigate(`/users/${user.id}/addresses`),
    [navigate],
  )

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
    if (!users || users.length === 0) return <EmptyState message="Nenhum usuário cadastrado." action={{ label: 'Novo Usuário', onClick: handleCreate }} />
    return (
      <UsersTable
        users={users}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onViewAddresses={handleViewAddresses}
      />
    )
  }

  return (
    <div className="animate-in">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Usuários</h1>
          <p className="mt-0.5 text-sm text-gray-500">Gerencie os usuários do sistema</p>
        </div>
        <Button onClick={handleCreate}>
          <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 2a.5.5 0 01.5.5v5h5a.5.5 0 010 1h-5v5a.5.5 0 01-1 0v-5h-5a.5.5 0 010-1h5v-5A.5.5 0 018 2z" />
          </svg>
          Novo Usuário
        </Button>
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
