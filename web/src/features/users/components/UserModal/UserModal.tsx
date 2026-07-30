import { Modal } from '@/components/Modal'
import { Loading } from '@/components/Loading'
import { UserForm } from '@/features/users/components/UserForm'
import { useUser, useCreateUser, useUpdateUser } from '@/features/users/hooks'
import type { UserListItem } from '@/features/users/types'

interface UserModalProps {
  isOpen: boolean
  onClose: () => void
  mode: 'create' | 'edit'
  user?: UserListItem
}

export function UserModal({ isOpen, onClose, mode, user }: UserModalProps) {
  const { data: userDetail, isLoading } = useUser(mode === 'edit' ? user?.id : undefined)
  const createMutation = useCreateUser()
  const updateMutation = useUpdateUser()

  const title = mode === 'create' ? 'Novo Usuário' : 'Editar Usuário'
  const subtitle = mode === 'create'
    ? 'Preencha os campos para criar um novo usuário'
    : `Editando ${user?.nome ?? 'usuário'}`
  const serverError =
    createMutation.error?.message ??
    updateMutation.error?.message ??
    null

  function sanitizeCpf(cpf?: string): string | null {
    if (!cpf) return null
    const digits = cpf.replace(/\D/g, '')
    return digits || null
  }

  async function handleSave(data: {
    nome: string
    email: string
    password?: string
    cpf?: string
    role: string
    isActive?: boolean
  }) {
    if (mode === 'create') {
      await createMutation.mutateAsync({
        nome: data.nome,
        email: data.email,
        password: data.password!,
        cpf: sanitizeCpf(data.cpf),
      })
    } else {
      await updateMutation.mutateAsync({
        id: user!.id,
        data: {
          nome: data.nome,
          email: data.email,
          cpf: sanitizeCpf(data.cpf),
          isActive: data.isActive ?? true,
        },
      })
    }
    onClose()
  }

  const isFetchingUser = mode === 'edit' && isLoading

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title} subtitle={subtitle}>
      {isFetchingUser ? (
        <div className="flex items-center justify-center py-12">
          <Loading size="lg" />
        </div>
      ) : (
        <UserForm
          mode={mode}
          serverError={serverError}
          defaultValues={
            mode === 'edit' && userDetail
              ? {
                  nome: userDetail.nome,
                  email: userDetail.email,
                  cpf: userDetail.cpf ?? '',
                  role: userDetail.role,
                  isActive: userDetail.isActive,
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
