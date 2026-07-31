import { useState, useCallback, useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useUsers, useExportUsers, useExportUser } from '@/features/users/hooks'
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
  const location = useLocation()
  const { data: users, isLoading, isError, error, refetch } = useUsers()
  const [search, setSearch] = useState('')
  const [modal, setModal] = useState<ModalState>({ type: 'closed' })
  const [deleteDialog, setDeleteDialog] = useState<DeleteState>({ type: 'closed' })
  const [showAddressesHint, setShowAddressesHint] = useState(false)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const exportMutation = useExportUsers()
  const exportUserMutation = useExportUser()

  useEffect(() => {
    if (!successMessage) return
    const t = setTimeout(() => setSuccessMessage(null), 4000)
    return () => clearTimeout(t)
  }, [successMessage])

  useEffect(() => {
    const state = location.state as Record<string, unknown> | null
    if (!state) return

    let shouldReplace = false

    if (state.openCreateUser === true) {
      setModal({ type: 'create' })
      shouldReplace = true
    }

    if (state.showAddressesHint === true) {
      setShowAddressesHint(true)
      shouldReplace = true
    }

    if (typeof state.search === 'string' && state.search) {
      setSearch(state.search)
      shouldReplace = true
    }

    if (shouldReplace) {
      navigate(location.pathname, { replace: true })
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleViewAddresses = useCallback(
    (user: UserListItem) => navigate(`/users/${user.id}/addresses`, {
      state: { userName: user.nome, fromUsersSearch: search },
    }),
    [navigate, search],
  )

  function handleExport() {
    exportMutation.mutate(undefined)
  }

  function handleExportUser(user: UserListItem) {
    exportUserMutation.mutate(user.id)
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

  const exportingUserId = exportUserMutation.isPending ? exportUserMutation.variables : null

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
        onExport={handleExportUser}
        exportingUserId={exportingUserId}
      />
    )
  }

  return (
    <div className="animate-fade-in space-y-6">
      {successMessage && (
        <div className="flex items-start gap-2.5 rounded-lg bg-success/10 p-3 text-sm text-success">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{successMessage}</span>
        </div>
      )}

      {exportMutation.isError && (
        <div className="flex items-start gap-2.5 rounded-lg bg-danger-light p-3 text-sm text-danger">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{exportMutation.error?.message ?? 'Erro ao exportar usuários'}</span>
        </div>
      )}

      {exportUserMutation.isError && (
        <div className="flex items-start gap-2.5 rounded-lg bg-danger-light p-3 text-sm text-danger">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{exportUserMutation.error?.message ?? 'Erro ao exportar usuário'}</span>
        </div>
      )}

      {showAddressesHint && (
        <div className="flex items-start gap-2.5 rounded-lg bg-accent/10 p-3 text-sm text-accent">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
          </svg>
          <div className="flex-1">
            <p className="font-medium">Gerenciar Endereços</p>
            <p className="mt-0.5 text-accent/80">Selecione um usuário na tabela abaixo e clique no ícone de endereço para visualizar ou gerenciar seus endereços.</p>
          </div>
          <button
            onClick={() => setShowAddressesHint(false)}
            className="shrink-0 text-accent/60 hover:text-accent transition-colors"
          >
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
            </svg>
          </button>
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
            {exportMutation.isPending ? 'Exportando...' : 'Exportar CSV'}
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
          onSuccess={() => setSuccessMessage(modal.type === 'create' ? 'Usuário criado com sucesso.' : 'Usuário atualizado com sucesso.')}
          mode={modal.type}
          user={modal.type === 'edit' ? modal.user : undefined}
        />
      )}

      {deleteDialog.type === 'confirm' && (
        <DeleteUserDialog
          isOpen
          onClose={closeDelete}
          onSuccess={() => setSuccessMessage('Usuário excluído com sucesso.')}
          user={deleteDialog.user}
        />
      )}
    </div>
  )
}
