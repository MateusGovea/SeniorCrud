import { useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { useUsers, useExportUsers } from '@/features/users/hooks'
import { UsersTable } from './UsersTable'
import { UserModal } from '@/features/users/components/UserModal'
import { DeleteUserDialog } from '@/features/users/components/DeleteUserDialog'
import { LoadingState } from '@/features/users/components/LoadingState'
import { ErrorState } from '@/features/users/components/ErrorState'
import { EmptyState } from '@/features/users/components/EmptyState'
import { Button } from '@/components/Button'
import { Input } from '@/components/Input'
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
  const [search, setSearch] = useState('')
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

  const filtered = users?.filter((u) =>
    u.nome.toLowerCase().includes(search.toLowerCase()) ||
    u.email.toLowerCase().includes(search.toLowerCase())
  ) ?? []

  const total = users?.length ?? 0
  const ativos = users?.filter((u) => u.isActive).length ?? 0

  const renderContent = () => {
    if (isLoading) return <LoadingState />
    if (isError) return <ErrorState message={error?.message} onRetry={refetch} />
    if (!users || users.length === 0) return <EmptyState message="Nenhum usuário cadastrado." action={{ label: 'Novo Usuário', onClick: handleCreate }} />
    return (
      <UsersTable
        users={search ? filtered : users}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onViewAddresses={handleViewAddresses}
      />
    )
  }

  return (
    <div className="animate-fade-in space-y-6">
      {exportMutation.isError && (
        <div className="flex items-start gap-2.5 rounded-lg bg-danger-light p-3 text-sm text-danger">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{exportMutation.error?.message ?? 'Erro ao exportar usuários'}</span>
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Total</p>
          <p className="mt-1 text-xl font-bold text-text-primary">{total}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Ativos</p>
          <p className="mt-1 text-xl font-bold text-success">{ativos}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Inativos</p>
          <p className="mt-1 text-xl font-bold text-text-muted">{total - ativos}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <Input
          className="max-w-xs"
          placeholder="Buscar por nome ou e-mail..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <div className="flex items-center gap-2">
          <Button variant="secondary" size="sm" onClick={handleExport} isLoading={exportMutation.isPending}>
            <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
              <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
            </svg>
            Exportar CSV
          </Button>
          <Button size="sm" onClick={handleCreate}>
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
            </svg>
            Novo Usuário
          </Button>
        </div>
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
