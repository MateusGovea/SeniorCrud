import { useState } from 'react'
import type { UserListItem } from '@/features/users/types'
import { Badge } from '@/components/Badge'
import { Button } from '@/components/Button'

interface UsersTableProps {
  users: UserListItem[]
  onEdit: (user: UserListItem) => void
  onDelete: (user: UserListItem) => void
  onViewAddresses: (user: UserListItem) => void
  onExport: (user: UserListItem) => void
  exportingUserId?: string | null
}

const ROWS_PER_PAGE = 8

function UserAvatar({ nome, email }: { nome: string; email: string }) {
  const initials = nome
    .split(' ')
    .slice(0, 2)
    .map((n) => n.charAt(0))
    .join('')
    .toUpperCase()

  return (
    <div className="flex h-9 w-9 items-center justify-center rounded-full bg-accent/10 text-xs font-medium text-accent ring-1 ring-accent/20">
      {initials || email.charAt(0).toUpperCase()}
    </div>
  )
}

export function UsersTable({ users, onEdit, onDelete, onViewAddresses, onExport, exportingUserId }: UsersTableProps) {
  const [page, setPage] = useState(0)

  const totalPages = Math.ceil(users.length / ROWS_PER_PAGE)
  const paged = users.slice(page * ROWS_PER_PAGE, (page + 1) * ROWS_PER_PAGE)

  if (users.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center gap-3 rounded-xl border border-border-primary bg-bg-surface py-12">
        <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-bg-hover">
          <svg className="h-6 w-6 text-text-muted" viewBox="0 0 20 20" fill="currentColor">
            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
          </svg>
        </div>
        <p className="text-sm font-medium text-text-primary">Nenhum resultado encontrado</p>
        <p className="text-xs text-text-muted">Tente ajustar sua busca.</p>
      </div>
    )
  }

  return (
    <div>
      <div className="overflow-hidden rounded-xl border border-border-primary">
        <table className="w-full">
          <thead>
            <tr className="border-b border-border-primary bg-bg-secondary">
              <th className="px-5 py-3.5 text-left text-xs font-semibold tracking-wider text-text-muted uppercase">Usuário</th>
              <th className="px-5 py-3.5 text-left text-xs font-semibold tracking-wider text-text-muted uppercase">E-mail</th>
              <th className="px-5 py-3.5 text-left text-xs font-semibold tracking-wider text-text-muted uppercase">Função</th>
              <th className="px-5 py-3.5 text-left text-xs font-semibold tracking-wider text-text-muted uppercase">Status</th>
              <th className="px-5 py-3.5 text-right text-xs font-semibold tracking-wider text-text-muted uppercase">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border-primary">
            {paged.map((user, index) => (
              <tr
                key={user.id ?? index}
                className="group bg-bg-surface transition-colors hover:bg-bg-hover"
              >
                <td className="px-5 py-4">
                  <div className="flex items-center gap-3">
                    <UserAvatar nome={user.nome} email={user.email} />
                    <div>
                      <p className="text-sm font-medium text-text-primary">{user.nome}</p>
                      <p className="text-[11px] text-text-muted">ID: {user.id ?? '—'}</p>
                    </div>
                  </div>
                </td>
                <td className="px-5 py-4">
                  <p className="text-sm text-text-secondary">{user.email}</p>
                </td>
                <td className="px-5 py-4">
                  <Badge variant={user.role === 'Admin' ? 'accent' : 'default'}>
                    {user.role === 'Admin' ? 'Admin' : 'Usuário'}
                  </Badge>
                </td>
                <td className="px-5 py-4">
                  <div className="flex items-center gap-1.5">
                    <span
                      className={`h-2 w-2 rounded-full ${
                        user.isActive ? 'bg-success' : 'bg-text-muted'
                      }`}
                    />
                    <span className="text-sm text-text-secondary">
                      {user.isActive ? 'Ativo' : 'Inativo'}
                    </span>
                  </div>
                </td>
                <td className="px-5 py-4">
                  <div className="flex items-center justify-end gap-1">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onEdit(user)}
                      title="Editar"
                    >
                      <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                        <path d="M12.146.146a.5.5 0 01.708 0l3 3a.5.5 0 010 .708l-10 10a.5.5 0 01-.168.11l-5 2a.5.5 0 01-.65-.65l2-5a.5.5 0 01.11-.168l10-10z" />
                      </svg>
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onViewAddresses(user)}
                      title="Endereços"
                    >
                      <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                        <path fillRule="evenodd" d="M8 1a5 5 0 015 5c0 2.5-2 4.5-5 7-3-2.5-5-4.5-5-7a5 5 0 015-5zm0 4a1 1 0 100 2 1 1 0 000-2z" clipRule="evenodd" />
                      </svg>
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onExport(user)}
                      title="Exportar CSV"
                      isLoading={exportingUserId === user.id}
                    >
                      <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                        <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
                        <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
                      </svg>
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onDelete(user)}
                      title="Excluir"
                      className="text-text-muted hover:text-danger"
                    >
                      <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                        <path d="M5.5 5.5A.5.5 0 016 6v6a.5.5 0 01-1 0V6a.5.5 0 01.5-.5zm2.5 0a.5.5 0 01.5.5v6a.5.5 0 01-1 0V6a.5.5 0 01.5-.5zm3 .5a.5.5 0 00-1 0v6a.5.5 0 001 0V6z" />
                        <path fillRule="evenodd" d="M14.5 3a1 1 0 01-1 1H13v9a2 2 0 01-2 2H5a2 2 0 01-2-2V4h-.5a1 1 0 01-1-1V2a1 1 0 011-1H6a1 1 0 011-1h2a1 1 0 011 1h3.5a1 1 0 011 1v1zM4.118 4L4 4.059V13a1 1 0 001 1h6a1 1 0 001-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z" />
                      </svg>
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <div className="mt-4 flex items-center justify-between">
          <p className="text-xs text-text-muted">
            Mostrando {(page * ROWS_PER_PAGE) + 1}–{Math.min((page + 1) * ROWS_PER_PAGE, users.length)} de {users.length}
          </p>
          <div className="flex items-center gap-1.5">
            <Button
              variant="ghost"
              size="sm"
              disabled={page === 0}
              onClick={() => setPage((p) => p - 1)}
            >
              Anterior
            </Button>
            {Array.from({ length: totalPages }, (_, i) => (
              <Button
                key={i}
                variant={i === page ? 'primary' : 'ghost'}
                size="sm"
                onClick={() => setPage(i)}
              >
                {i + 1}
              </Button>
            ))}
            <Button
              variant="ghost"
              size="sm"
              disabled={page === totalPages - 1}
              onClick={() => setPage((p) => p + 1)}
            >
              Próximo
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
