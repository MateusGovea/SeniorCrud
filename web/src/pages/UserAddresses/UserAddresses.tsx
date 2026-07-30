import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useAddresses } from '@/features/addresses/hooks'
import { AddressTable } from '@/features/addresses/components/AddressTable'
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

  const renderContent = () => {
    if (isLoading) {
      return (
        <div className="flex flex-col items-center justify-center gap-3 py-20">
          <Loading size="lg" />
          <p className="text-sm text-gray-400">Carregando endereços...</p>
        </div>
      )
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
  }

  return (
    <div className="animate-in">
      <div className="mb-6">
        <Link
          to="/users"
          className="mb-3 inline-flex items-center gap-1 text-sm font-medium text-blue-600 transition-colors hover:text-blue-800"
        >
          <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
            <path
              fillRule="evenodd"
              d="M11.354 1.646a.5.5 0 010 .708L5.707 8l5.647 5.646a.5.5 0 01-.708.708l-6-6a.5.5 0 010-.708l6-6a.5.5 0 01.708 0z"
              clipRule="evenodd"
            />
          </svg>
          Voltar para Usuários
        </Link>
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Endereços</h1>
            <p className="mt-0.5 text-sm text-gray-500">Gerencie os endereços deste usuário</p>
          </div>
          {addresses && addresses.length > 0 && (
            <Button onClick={handleCreate}>
              <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                <path d="M8 2a.5.5 0 01.5.5v5h5a.5.5 0 010 1h-5v5a.5.5 0 01-1 0v-5h-5a.5.5 0 010-1h5v-5A.5.5 0 018 2z" />
              </svg>
              Novo Endereço
            </Button>
          )}
        </div>
      </div>

      {renderContent()}

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
