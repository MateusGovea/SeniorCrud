import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useAddresses } from '@/features/addresses/hooks'
import { AddressTable } from './AddressTable'
import { AddressModal } from '@/features/addresses/components/AddressModal'
import { DeleteAddressDialog } from '@/features/addresses/components/DeleteAddressDialog'
import { Button } from '@/components/Button'
import { Loading } from '@/components/Loading'
import { EmptyState } from '@/features/users/components/EmptyState'
import { ErrorState } from '@/features/users/components/ErrorState'
import type { AddressResponse } from '@/features/addresses/types'

type ModalState =
  | { type: 'closed' }
  | { type: 'create' }
  | { type: 'edit'; address: AddressResponse }

type DeleteState =
  | { type: 'closed' }
  | { type: 'confirm'; address: AddressResponse }

export function UserAddresses() {
  const { id } = useParams<{ id: string }>()
  const { data: addresses, isLoading, isError, error, refetch } = useAddresses(id)
  const [modal, setModal] = useState<ModalState>({ type: 'closed' })
  const [deleteDialog, setDeleteDialog] = useState<DeleteState>({ type: 'closed' })

  function handleCreate() {
    setModal({ type: 'create' })
  }

  function handleEdit(address: AddressResponse) {
    setModal({ type: 'edit', address })
  }

  function handleDelete(address: AddressResponse) {
    setDeleteDialog({ type: 'confirm', address })
  }

  function closeModal() {
    setModal({ type: 'closed' })
  }

  function closeDelete() {
    setDeleteDialog({ type: 'closed' })
  }

  if (!id) return null

  const total = addresses?.length ?? 0
  const principais = addresses?.filter((a) => a.isPrimary).length ?? 0

  return (
    <div className="animate-fade-in space-y-6">
      <div className="flex items-center gap-2 text-xs text-text-muted">
        <Link to="/users" className="hover:text-text-secondary transition-colors">Usuários</Link>
        <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
          <path fillRule="evenodd" d="M6.22 3.22a.75.75 0 011.06 0l4.25 4.25a.75.75 0 010 1.06l-4.25 4.25a.75.75 0 01-1.06-1.06L9.94 8 6.22 4.28a.75.75 0 010-1.06z" />
        </svg>
        <span className="text-text-primary font-medium">Endereços</span>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Total de Endereços</p>
          <p className="mt-1 text-xl font-bold text-text-primary">{total}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Endereço Principal</p>
          <p className="mt-1 text-xl font-bold text-accent">{principais}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Secundários</p>
          <p className="mt-1 text-xl font-bold text-text-muted">{total - principais}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <div />
        <Button size="sm" onClick={handleCreate}>
          <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
            <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
          </svg>
          Novo Endereço
        </Button>
      </div>

      {(() => {
        if (isLoading) {
          return <Loading fullPage />
        }
        if (isError) {
          return <ErrorState message={error?.message ?? 'Erro ao carregar endereços.'} onRetry={refetch} />
        }
        if (!addresses || addresses.length === 0) {
          return (
            <EmptyState
              message="Nenhum endereço cadastrado para este usuário."
              action={{ label: 'Adicionar Endereço', onClick: handleCreate }}
            />
          )
        }
        return <AddressTable addresses={addresses} onEdit={handleEdit} onDelete={handleDelete} />
      })()}

      {modal.type !== 'closed' && (
        <AddressModal
          isOpen
          onClose={closeModal}
          mode={modal.type}
          userId={id}
          address={modal.type === 'edit' ? modal.address : undefined}
        />
      )}

      {deleteDialog.type === 'confirm' && (
        <DeleteAddressDialog
          isOpen
          onClose={closeDelete}
          address={deleteDialog.address}
        />
      )}
    </div>
  )
}
