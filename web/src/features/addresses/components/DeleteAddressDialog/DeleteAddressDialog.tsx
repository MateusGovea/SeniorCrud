import { useState, useEffect } from 'react'
import { Button } from '@/components/Button'
import { useDeleteAddress } from '@/features/addresses/hooks'
import type { AddressResponse } from '@/features/addresses/types'

interface DeleteAddressDialogProps {
  isOpen: boolean
  onClose: () => void
  address: AddressResponse
}

export function DeleteAddressDialog({ isOpen, onClose, address }: DeleteAddressDialogProps) {
  const deleteMutation = useDeleteAddress()
  const [serverError, setServerError] = useState<string | null>(null)

  useEffect(() => {
    if (deleteMutation.isSuccess) {
      onClose()
    }
  }, [deleteMutation.isSuccess, onClose])

  if (!isOpen) return null

  async function handleConfirm() {
    setServerError(null)
    try {
      await deleteMutation.mutateAsync({ id: address.id, userId: address.userId })
    } catch (err) {
      setServerError(err instanceof Error ? err.message : 'Erro inesperado')
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50" onClick={onClose} />
      <div className="relative z-10 w-full max-w-sm rounded-lg bg-white p-6 shadow-xl">
        <h2 className="text-lg font-semibold text-gray-900">Excluir Endereço</h2>
        <p className="mt-2 text-sm text-gray-600">
          Deseja realmente excluir o endereço <strong>{address.street}, {address.number}</strong>?
        </p>

        {serverError && (
          <p className="mt-2 text-sm text-red-600">{serverError}</p>
        )}
        {deleteMutation.isError && !serverError && (
          <p className="mt-2 text-sm text-red-600">
            {deleteMutation.error?.message ?? 'Erro ao excluir endereço'}
          </p>
        )}

        <div className="mt-6 flex justify-end gap-3">
          <Button variant="outline" onClick={onClose} disabled={deleteMutation.isPending}>
            Cancelar
          </Button>
          <Button
            variant="danger"
            onClick={handleConfirm}
            isLoading={deleteMutation.isPending}
          >
            Excluir
          </Button>
        </div>
      </div>
    </div>
  )
}
