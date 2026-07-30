import { useState } from 'react'
import { Modal } from '@/components/Modal'
import { Loading } from '@/components/Loading'
import { Button } from '@/components/Button'
import { AddressForm } from '@/features/addresses/components/AddressForm'
import { useAddress, useCreateAddress, useUpdateAddress } from '@/features/addresses/hooks'
import { useUsers } from '@/features/users/hooks'
import type { AddressResponse } from '@/features/addresses/types'

interface AddressModalProps {
  isOpen: boolean
  onClose: () => void
  mode: 'create' | 'edit'
  userId?: string
  address?: AddressResponse
}

export function AddressModal({ isOpen, onClose, mode, userId, address }: AddressModalProps) {
  const { data: addressDetail, isLoading } = useAddress(mode === 'edit' ? address?.id : undefined)
  const createMutation = useCreateAddress()
  const updateMutation = useUpdateAddress()
  const { data: users } = useUsers()
  const [serverError, setServerError] = useState<string | null>(null)
  const [selectedUserId, setSelectedUserId] = useState(userId ?? '')

  const needsUserSelection = mode === 'create' && !selectedUserId

  const title = mode === 'create' ? 'Novo Endereço' : 'Editar Endereço'
  const subtitle = mode === 'create'
    ? (needsUserSelection ? 'Selecione o usuário para o novo endereço' : 'Preencha os campos para adicionar um endereço')
    : `Editando ${address?.street}, ${address?.number}`

  function sanitizeCep(cep: string): string {
    return cep.replace(/\D/g, '')
  }

  async function handleSave(data: {
    cep: string
    street: string
    number: string
    complement?: string
    neighborhood: string
    city: string
    state: string
    isPrimary?: boolean
  }) {
    setServerError(null)
    try {
      if (mode === 'create') {
        await createMutation.mutateAsync({
          userId: selectedUserId,
          cep: sanitizeCep(data.cep),
          street: data.street,
          number: data.number,
          complement: data.complement || null,
          neighborhood: data.neighborhood,
          city: data.city,
          state: data.state,
          isPrimary: data.isPrimary ?? false,
        })
      } else {
        await updateMutation.mutateAsync({
          id: address!.id,
          userId: userId!,
          data: {
            cep: sanitizeCep(data.cep),
            street: data.street,
            number: data.number,
            complement: data.complement || null,
            neighborhood: data.neighborhood,
            city: data.city,
            state: data.state,
            isPrimary: data.isPrimary ?? false,
          },
        })
      }
      onClose()
    } catch (err) {
      setServerError(err instanceof Error ? err.message : 'Erro inesperado')
    }
  }

  const isFetching = mode === 'edit' && isLoading

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title} subtitle={subtitle}>
      {isFetching ? (
        <div className="flex items-center justify-center py-12">
          <Loading size="lg" />
        </div>
      ) : needsUserSelection ? (
        <div className="space-y-3">
          {users && users.length === 0 && (
            <div className="flex items-start gap-2.5 rounded-lg bg-warning/10 p-3 text-sm text-warning">
              <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
              </svg>
              <span>Nenhum usuário cadastrado. Crie um usuário primeiro.</span>
            </div>
          )}
          <div className="max-h-60 space-y-1 overflow-y-auto">
            {users?.map((u) => (
              <button
                key={u.id}
                type="button"
                onClick={() => setSelectedUserId(u.id)}
                className={`w-full rounded-lg border px-4 py-3 text-left text-sm transition-all ${
                  selectedUserId === u.id
                    ? 'border-accent bg-accent/5 text-accent'
                    : 'border-border-primary bg-bg-surface text-text-primary hover:border-border-hover hover:bg-bg-hover'
                }`}
              >
                <span className="font-medium">{u.nome}</span>
                <span className="ml-2 text-xs text-text-muted">{u.email}</span>
              </button>
            ))}
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="outline" onClick={onClose}>
              Cancelar
            </Button>
          </div>
        </div>
      ) : (
        <AddressForm
          serverError={serverError}
          defaultValues={
            mode === 'edit' && addressDetail
              ? {
                  cep: addressDetail.cep,
                  street: addressDetail.street,
                  number: addressDetail.number,
                  complement: addressDetail.complement ?? '',
                  neighborhood: addressDetail.neighborhood,
                  city: addressDetail.city,
                  state: addressDetail.state,
                  isPrimary: addressDetail.isPrimary,
                }
              : undefined
          }
          onSave={handleSave}
          onCancel={onClose}
        />
      )}
    </Modal>
  )
}
