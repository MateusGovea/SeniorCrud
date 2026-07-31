import { useEffect } from 'react'
import { Modal } from '@/components/Modal'
import { Button } from '@/components/Button'
import type { UserListItem } from '@/features/users/types'
import { useDeleteUser } from '@/features/users/hooks'

interface DeleteUserDialogProps {
  isOpen: boolean
  onClose: () => void
  onSuccess?: () => void
  user: UserListItem
}

export function DeleteUserDialog({ isOpen, onClose, onSuccess, user }: DeleteUserDialogProps) {
  const deleteMutation = useDeleteUser()

  useEffect(() => {
    if (deleteMutation.isSuccess) {
      onSuccess?.()
      onClose()
    }
  }, [deleteMutation.isSuccess, onClose, onSuccess])

  async function handleConfirm() {
    await deleteMutation.mutateAsync(user.id)
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div className="flex flex-col items-center text-center">
        <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-danger-light">
          <svg className="h-6 w-6 text-danger" viewBox="0 0 20 20" fill="currentColor">
            <path
              fillRule="evenodd"
              d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z"
              clipRule="evenodd"
            />
          </svg>
        </div>
        <h2 className="text-lg font-semibold text-text-primary">Excluir Usuário</h2>
        <p className="mt-1 text-sm text-text-secondary">
          Deseja realmente excluir <strong className="text-text-primary">{user.nome}</strong>?
        </p>
        <p className="mt-0.5 text-xs text-text-muted">Esta ação não pode ser desfeita.</p>
      </div>

      {deleteMutation.isError && (
        <div className="mt-4 flex items-start gap-2 rounded-lg bg-danger-light p-3 text-sm text-danger">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{deleteMutation.error?.message ?? 'Erro ao excluir usuário'}</span>
        </div>
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
          {deleteMutation.isPending ? 'Excluindo...' : 'Excluir'}
        </Button>
      </div>
    </Modal>
  )
}
