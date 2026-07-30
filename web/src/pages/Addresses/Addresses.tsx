import { useState, useMemo } from 'react'
import { useAddressesList } from '@/features/addresses/hooks'
import { useUsers } from '@/features/users/hooks'
import { AddressesTable } from './AddressesTable'
import { AddressModal } from '@/features/addresses/components/AddressModal'
import { DeleteAddressDialog } from '@/features/addresses/components/DeleteAddressDialog'
import { LoadingState } from '@/features/users/components/LoadingState'
import { ErrorState } from '@/features/users/components/ErrorState'
import { EmptyState } from '@/features/users/components/EmptyState'
import { Button } from '@/components/Button'
import { Input } from '@/components/Input'
import type { AddressResponse } from '@/features/addresses/types'

type ModalState =
  | { type: 'closed' }
  | { type: 'create' }
  | { type: 'edit'; address: AddressResponse }

type DeleteState =
  | { type: 'closed' }
  | { type: 'confirm'; address: AddressResponse }

export function Addresses() {
  const [search, setSearch] = useState('')
  const [modal, setModal] = useState<ModalState>({ type: 'closed' })
  const [deleteDialog, setDeleteDialog] = useState<DeleteState>({ type: 'closed' })

  const { data: addresses, isLoading, isError, error, refetch } = useAddressesList()
  const { data: users } = useUsers()

  const userNameMap = useMemo(() => {
    const map: Record<string, string> = {}
    if (users) {
      for (const u of users) {
        map[u.id] = u.nome
      }
    }
    return map
  }, [users])

  const filtered = useMemo(() => {
    if (!addresses) return []
    if (!search) return addresses
    const q = search.toLowerCase()
    return addresses.filter((a) => {
      const userName = userNameMap[a.userId] ?? ''
      return (
        userName.toLowerCase().includes(q) ||
        a.city.toLowerCase().includes(q) ||
        a.neighborhood.toLowerCase().includes(q) ||
        a.street.toLowerCase().includes(q) ||
        a.cep.replace(/\D/g, '').includes(q)
      )
    })
  }, [addresses, search, userNameMap])

  const total = addresses?.length ?? 0
  const primaryCount = addresses?.filter((a) => a.isPrimary).length ?? 0
  const statesCount = new Set(addresses?.map((a) => a.state)).size

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

  const renderContent = () => {
    if (isLoading) return <LoadingState />
    if (isError) return <ErrorState message={error?.message} onRetry={refetch} />
    if (!addresses || addresses.length === 0) {
      return <EmptyState message="Nenhum endereço cadastrado." action={{ label: 'Novo Endereço', onClick: handleCreate }} />
    }
    if (filtered.length === 0 && search) {
      return <EmptyState message="Nenhum endereço encontrado para esta busca." />
    }
    return (
      <AddressesTable
        addresses={filtered}
        userNameMap={userNameMap}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
    )
  }

  return (
    <div className="animate-fade-in space-y-6">
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Total de Endereços</p>
          <p className="mt-1 text-xl font-bold text-text-primary">{total}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Endereços Principais</p>
          <p className="mt-1 text-xl font-bold text-accent">{primaryCount}</p>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <p className="text-xs font-medium text-text-muted">Estados Cadastrados</p>
          <p className="mt-1 text-xl font-bold text-text-primary">{statesCount}</p>
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3">
        <Input
          className="max-w-xs"
          placeholder="Buscar por usuário, cidade, bairro, rua ou CEP..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <Button size="sm" onClick={handleCreate}>
          <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
            <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
          </svg>
          Novo Endereço
        </Button>
      </div>

      {renderContent()}

      {modal.type !== 'closed' && (
        <AddressModal
          isOpen
          onClose={closeModal}
          mode={modal.type}
          userId={modal.type === 'edit' ? modal.address.userId : undefined}
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
