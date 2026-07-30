import { useParams, Link } from 'react-router-dom'
import { useUser } from '@/features/users/hooks'
import { useAddresses } from '@/features/addresses/hooks'
import { useExportUsers } from '@/features/users/hooks/use-export-users'
import { AddressTable } from './AddressTable'
import { Button } from '@/components/Button'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { useState } from 'react'
import type { AddressResponse } from '@/features/addresses/types'
import { Loading } from '@/components/Loading'
import { EmptyState } from '@/components/EmptyState'
import { ErrorState } from '@/components/ErrorState'

export function UserAddresses() {
  const { userId } = useParams<{ userId: string }>()
  const { data: user } = useUser(userId!)
  const { data: addresses, isLoading, isError, error, refetch } = useAddresses(userId!)
  const exportCsv = useExportUsers()
  const [deleteConfirm, setDeleteConfirm] = useState<AddressResponse | null>(null)

  const total = addresses?.length ?? 0
  const principais = addresses?.filter((a) => a.isPrimary).length ?? 0

  if (isLoading) return <Loading fullPage />
  if (isError) return <ErrorState message={(error as Error)?.message ?? 'Erro ao carregar endereços'} onRetry={() => refetch()} />
  if (!addresses || addresses.length === 0) {
    return <EmptyState onCreateNew={() => {}} />
  }

  return (
    <div className="animate-fade-in space-y-6">
      <div className="flex items-center gap-2 text-xs text-text-muted">
        <Link to="/users" className="hover:text-text-secondary transition-colors">Usuários</Link>
        <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
          <path fillRule="evenodd" d="M6.22 3.22a.75.75 0 011.06 0l4.25 4.25a.75.75 0 010 1.06l-4.25 4.25a.75.75 0 01-1.06-1.06L9.94 8 6.22 4.28a.75.75 0 010-1.06z" />
        </svg>
        <span className="text-text-primary font-medium">{user?.nome ?? 'Carregando...'}</span>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Total de Endereços</p>
          <p className="mt-1 text-xl font-bold text-text-primary">{total}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Endereços Principal</p>
          <p className="mt-1 text-xl font-bold text-accent">{principais}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Secundários</p>
          <p className="mt-1 text-xl font-bold text-text-muted">{total - principais}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <div />
        <div className="flex items-center gap-2">
          <Button variant="secondary" size="sm" onClick={() => exportCsv.mutateAsync(undefined).catch(() => {})} isLoading={exportCsv.isPending}>
            <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
              <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
            </svg>
            Exportar CSV
          </Button>
          <Button size="sm" onClick={() => {}}>
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
            </svg>
            Novo Endereço
          </Button>
        </div>
      </div>

      <AddressTable
        addresses={addresses}
        onDelete={(addr) => setDeleteConfirm(addr)}
      />

      {deleteConfirm && (
        <ConfirmDialog
          title="Excluir Endereço"
          description="Tem certeza que deseja excluir este endereço? Esta ação não pode ser desfeita."
          confirmLabel="Excluir"
          onConfirm={() => setDeleteConfirm(null)}
          onCancel={() => setDeleteConfirm(null)}
        />
      )}
    </div>
  )
}
