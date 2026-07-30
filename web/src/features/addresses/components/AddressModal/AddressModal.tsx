import { Modal } from '@/components/Modal'
import { Loading } from '@/components/Loading'
import { AddressForm } from '@/features/addresses/components/AddressForm'
import { useAddress, useCreateAddress, useUpdateAddress } from '@/features/addresses/hooks'
import type { AddressResponse } from '@/features/addresses/types'
import { useState } from 'react'

interface AddressModalProps {
  isOpen: boolean
  onClose: () => void
  mode: 'create' | 'edit'
  userId: string
  address?: AddressResponse
}

export function AddressModal({ isOpen, onClose, mode, userId, address }: AddressModalProps) {
  const { data: addressDetail, isLoading } = useAddress(mode === 'edit' ? address?.id : undefined)
  const createMutation = useCreateAddress()
  const updateMutation = useUpdateAddress()
  const [serverError, setServerError] = useState<string | null>(null)

  const title = mode === 'create' ? 'Novo Endereço' : 'Editar Endereço'
  const subtitle = mode === 'create'
    ? 'Preencha os campos para adicionar um endereço'
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
          userId,
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
          userId,
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
