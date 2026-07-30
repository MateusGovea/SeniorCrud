import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useUsers, useExportUsers } from '@/features/users/hooks'
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
  const exportMutation = useExportUsers()

  const handleViewAddresses = useCallback(
    (user: UserListItem) => navigate(`/users/${user.id}/addresses`),
    [navigate],
  )

  function handleExport() {
    exportMutation.mutate(undefined)
  }

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
        <div className="flex items-center gap-3">
          <Button variant="outline" onClick={handleExport} disabled={exportMutation.isPending}>
            {exportMutation.isPending ? (
              <>
                <svg className="h-4 w-4 animate-spin" viewBox="0 0 24 24" fill="none">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                </svg>
                Exportando...
              </>
            ) : (
              <>
                <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                  <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
                  <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
                </svg>
                Exportar CSV
              </>
            )}
          </Button>
          <Button onClick={handleCreate}>
            <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 2a.5.5 0 01.5.5v5h5a.5.5 0 010 1h-5v5a.5.5 0 01-1 0v-5h-5a.5.5 0 010-1h5v-5A.5.5 0 018 2z" />
            </svg>
            Novo Usuário
          </Button>
        </div>
      </div>

      {exportMutation.isError && (
        <div className="mb-4 flex items-start gap-2.5 rounded-lg bg-red-50 p-3 text-sm text-red-700">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{exportMutation.error?.message ?? 'Erro ao exportar usuários'}</span>
        </div>
      )}

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
