import type { UserListItem } from '@/features/users/types'
import { Button } from '@/components/Button'

interface UsersTableProps {
  users: UserListItem[]
  onEdit: (user: UserListItem) => void
  onDelete: (user: UserListItem) => void
  onViewAddresses: (user: UserListItem) => void
}

export function UsersTable({ users, onEdit, onDelete, onViewAddresses }: UsersTableProps) {
  return (
    <div className="overflow-hidden rounded-xl border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead>
          <tr className="border-b border-gray-100 bg-gray-50/80">
            <th className="px-6 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Nome
            </th>
            <th className="px-6 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              E-mail
            </th>
            <th className="px-6 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Perfil
            </th>
            <th className="px-6 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Ativo
            </th>
            <th className="px-6 py-3.5 text-right text-xs font-semibold uppercase tracking-wider text-gray-500">
              Ações
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100 bg-white">
          {users.map((user) => (
            <tr key={user.id} className="transition-colors hover:bg-gray-50/60">
              <td className="whitespace-nowrap px-6 py-4 text-sm font-medium text-gray-900">
                {user.nome}
              </td>
              <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {user.email}
              </td>
              <td className="whitespace-nowrap px-6 py-4">
                <span
                  className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium ${
                    user.role === 'Admin'
                      ? 'bg-purple-100 text-purple-700'
                      : 'bg-gray-100 text-gray-600'
                  }`}
                >
                  {user.role === 'Admin' ? (
                    <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
                      <path d="M8 1a3 3 0 100 6 3 3 0 000-6zM5 9a3 3 0 000 6h6a3 3 0 000-6H5z" />
                    </svg>
                  ) : (
                    <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
                      <path d="M8 8a3 3 0 100-6 3 3 0 000 6zm-5 8a5 5 0 0110 0H3z" />
                    </svg>
                  )}
                  {user.role === 'Admin' ? 'Admin' : 'Usuário'}
                </span>
              </td>
              <td className="whitespace-nowrap px-6 py-4">
                <span
                  className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium ${
                    user.isActive
                      ? 'bg-emerald-100 text-emerald-700'
                      : 'bg-red-100 text-red-700'
                  }`}
                >
                  <span className={`h-1.5 w-1.5 rounded-full ${
                    user.isActive ? 'bg-emerald-500' : 'bg-red-500'
                  }`} />
                  {user.isActive ? 'Ativo' : 'Inativo'}
                </span>
              </td>
              <td className="whitespace-nowrap px-6 py-4 text-right">
                <div className="flex items-center justify-end gap-1">
                  <Button variant="ghost" size="sm" onClick={() => onViewAddresses(user)}>
                    Endereços
                  </Button>
                  <Button variant="ghost" size="sm" onClick={() => onEdit(user)}>
                    Editar
                  </Button>
                  <Button variant="ghost" size="sm" onClick={() => onDelete(user)}>
                    Excluir
                  </Button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
