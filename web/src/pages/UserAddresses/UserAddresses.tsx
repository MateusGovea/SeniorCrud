import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useAddresses } from '@/features/addresses/hooks'
import { AddressTable } from '@/features/addresses/components/AddressTable'
import { AddressModal } from '@/features/addresses/components/AddressModal'
import { DeleteAddressDialog } from '@/features/addresses/components/DeleteAddressDialog'
import { Button } from '@/components/Button'
import { Loading } from '@/components/Loading'
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
        <div className="flex items-center justify-center py-12">
          <Loading size="lg" />
        </div>
      )
    }
    if (isError) {
      return (
        <div className="flex flex-col items-center justify-center gap-4 py-12">
          <p className="text-sm text-red-600">
            {error?.message ?? 'Erro ao carregar endereços.'}
          </p>
          <Button variant="outline" size="sm" onClick={() => refetch()}>
            Tentar novamente
          </Button>
        </div>
      )
    }
    if (!addresses || addresses.length === 0) {
      return (
        <div className="flex flex-col items-center justify-center gap-4 py-12">
          <p className="text-gray-500">Nenhum endereço encontrado.</p>
          <Button onClick={handleCreate}>Adicionar Endereço</Button>
        </div>
      )
    }
    return <AddressTable addresses={addresses} onEdit={handleEdit} onDelete={handleDelete} />
  }

  return (
    <div>
      <div className="mb-6">
        <Link
          to="/users"
          className="mb-2 inline-block text-sm text-blue-600 hover:text-blue-800"
        >
          &larr; Voltar para Usuários
        </Link>
        <div className="flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">Endereços</h1>
          {addresses && addresses.length > 0 && (
            <Button onClick={handleCreate}>Novo Endereço</Button>
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
