import { Button } from '@/components/Button'
import type { UserListItem } from '@/features/users/types'
import { useDeleteUser } from '@/features/users/hooks'
import { useEffect } from 'react'

interface DeleteUserDialogProps {
  isOpen: boolean
  onClose: () => void
  user: UserListItem
}

export function DeleteUserDialog({ isOpen, onClose, user }: DeleteUserDialogProps) {
  const deleteMutation = useDeleteUser()

  useEffect(() => {
    if (deleteMutation.isSuccess) {
      onClose()
    }
  }, [deleteMutation.isSuccess, onClose])

  if (!isOpen) return null

  async function handleConfirm() {
    await deleteMutation.mutateAsync(user.id)
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div className="absolute inset-0 bg-black/50" onClick={onClose} />
      <div className="relative z-10 w-full max-w-sm rounded-lg bg-white p-6 shadow-xl">
        <h2 className="text-lg font-semibold text-gray-900">Excluir Usuário</h2>
        <p className="mt-2 text-sm text-gray-600">
          Deseja realmente excluir <strong>{user.nome}</strong>?
        </p>

        {deleteMutation.isError && (
          <p className="mt-2 text-sm text-red-600">
            {deleteMutation.error?.message ?? 'Erro ao excluir usuário'}
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
