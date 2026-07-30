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
    <div className="overflow-x-auto rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Nome
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              E-mail
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Perfil
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Ativo
            </th>
            <th className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
              Ações
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 bg-white">
          {users.map((user) => (
            <tr key={user.id} className="hover:bg-gray-50">
              <td className="whitespace-nowrap px-6 py-4 text-sm font-medium text-gray-900">
                {user.nome}
              </td>
              <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {user.email}
              </td>
              <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-500">
                {user.role === 'Admin' ? 'Admin' : 'Usuário'}
              </td>
              <td className="whitespace-nowrap px-6 py-4">
                <span
                  className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${
                    user.isActive
                      ? 'bg-green-100 text-green-800'
                      : 'bg-red-100 text-red-800'
                  }`}
                >
                  {user.isActive ? 'Ativo' : 'Inativo'}
                </span>
              </td>
              <td className="whitespace-nowrap px-6 py-4 text-right text-sm">
                <Button variant="ghost" size="sm" onClick={() => onViewAddresses(user)}>
                  Endereços
                </Button>
                <Button variant="ghost" size="sm" onClick={() => onEdit(user)}>
                  Editar
                </Button>
                <Button variant="ghost" size="sm" onClick={() => onDelete(user)}>
                  Excluir
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
