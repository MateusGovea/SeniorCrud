import { useState } from 'react'
import { UsersTable } from './UsersTable'
import { useUsers } from '@/features/users/hooks'
import { useExportUsers } from '@/features/users/hooks/use-export-users'
import { Button } from '@/components/Button'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { Input } from '@/components/Input'
import { Loading } from '@/components/Loading'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'
import type { UserListItem } from '@/features/users/types'

export function Users() {
  const { data: users, isLoading, isError, error, refetch } = useUsers()
  const exportCsv = useExportUsers()
  const [search, setSearch] = useState('')
  const [selectedUser, setSelectedUser] = useState<UserListItem | null>(null)
  const [deleteConfirm, setDeleteConfirm] = useState<UserListItem | null>(null)

  const filtered = users?.filter((u) =>
    u.nome.toLowerCase().includes(search.toLowerCase()) ||
    u.email.toLowerCase().includes(search.toLowerCase())
  ) ?? []

  const total = users?.length ?? 0
  const ativos = users?.filter((u) => u.isActive).length ?? 0

  if (isLoading) {
    return <Loading fullPage />
  }

  if (isError) {
    return <ErrorState message={(error as Error)?.message ?? 'Erro ao carregar usuários'} onRetry={() => refetch()} />
  }

  if (!users || users.length === 0) {
    return <EmptyState onCreateNew={() => {}} />
  }

  return (
    <div className="animate-fade-in space-y-6">
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
          <Button variant="secondary" size="sm" onClick={() => exportCsv.mutateAsync(undefined).catch(() => {})} isLoading={exportCsv.isPending}>
            <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
              <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
            </svg>
            Exportar CSV
          </Button>
          <Button size="sm" onClick={() => setSelectedUser({} as UserListItem)}>
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
            </svg>
            Novo Usuário
          </Button>
        </div>
      </div>

      <UsersTable
        users={filtered}
        onEdit={(user) => setSelectedUser(user)}
        onDelete={(user) => setDeleteConfirm(user)}
      />

      {selectedUser && (
        <div>Form placeholder</div>
      )}

      {deleteConfirm && (
        <ConfirmDialog
          title="Excluir Usuário"
          description={`Tem certeza que deseja excluir "${deleteConfirm.nome}"? Esta ação não pode ser desfeita.`}
          confirmLabel="Excluir"
          onConfirm={() => setDeleteConfirm(null)}
          onCancel={() => setDeleteConfirm(null)}
        />
      )}
    </div>
  )
}
