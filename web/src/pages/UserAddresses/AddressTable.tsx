import { useState } from 'react'
import type { AddressResponse } from '@/features/addresses/types'
import { Badge } from '@/components/Badge'
import { Button } from '@/components/Button'

interface AddressTableProps {
  addresses: AddressResponse[]
  onEdit: (address: AddressResponse) => void
  onDelete: (address: AddressResponse) => void
}

const ROWS_PER_PAGE = 8

export function AddressTable({ addresses, onEdit, onDelete }: AddressTableProps) {
  const [page, setPage] = useState(0)

  const totalPages = Math.ceil(addresses.length / ROWS_PER_PAGE)
  const paged = addresses.slice(page * ROWS_PER_PAGE, (page + 1) * ROWS_PER_PAGE)

  if (addresses.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center gap-3 rounded-xl border border-border-primary bg-bg-surface py-12">
        <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-bg-hover">
          <svg className="h-6 w-6 text-text-muted" viewBox="0 0 16 16" fill="currentColor">
            <path fillRule="evenodd" d="M8 1a5 5 0 015 5c0 2.5-2 4.5-5 7-3-2.5-5-4.5-5-7a5 5 0 015-5zm0 4a1 1 0 100 2 1 1 0 000-2z" clipRule="evenodd" />
          </svg>
        </div>
        <p className="text-sm font-medium text-text-primary">Nenhum endereço encontrado</p>
        <p className="text-xs text-text-muted">Nenhum endereço cadastrado para este usuário.</p>
      </div>
    )
  }

  return (
    <div>
      <div className="overflow-hidden rounded-xl border border-border-primary">
        <table className="w-full">
          <thead>
            <tr className="border-b border-border-primary bg-bg-secondary">
              <th className="px-4 py-3 text-left text-[11px] font-semibold tracking-wider text-text-muted uppercase">Logradouro</th>
              <th className="px-4 py-3 text-left text-[11px] font-semibold tracking-wider text-text-muted uppercase">Cidade</th>
              <th className="px-4 py-3 text-left text-[11px] font-semibold tracking-wider text-text-muted uppercase">Estado</th>
              <th className="px-4 py-3 text-left text-[11px] font-semibold tracking-wider text-text-muted uppercase">CEP</th>
              <th className="px-4 py-3 text-left text-[11px] font-semibold tracking-wider text-text-muted uppercase">Tipo</th>
              <th className="px-4 py-3 text-right text-[11px] font-semibold tracking-wider text-text-muted uppercase">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border-primary">
            {paged.map((addr, index) => (
              <tr key={addr.id ?? index} className="group bg-bg-surface transition-colors hover:bg-bg-hover">
                <td className="px-4 py-3.5">
                  <div>
                    <p className="text-sm font-medium text-text-primary">{addr.street}, {addr.number}</p>
                    {addr.complement && (
                      <p className="text-[11px] text-text-muted">{addr.complement}</p>
                    )}
                  </div>
                </td>
                <td className="px-4 py-3.5 text-sm text-text-secondary">{addr.city}</td>
                <td className="px-4 py-3.5 text-sm text-text-secondary">{addr.state}</td>
                <td className="px-4 py-3.5 text-sm font-mono text-text-secondary">{addr.cep}</td>
                <td className="px-4 py-3.5">
                  {addr.isPrimary ? (
                    <Badge variant="accent">Principal</Badge>
                  ) : (
                    <Badge variant="default">Secundário</Badge>
                  )}
                </td>
                <td className="px-4 py-3.5">
                  <div className="flex items-center justify-end gap-1">
                    <Button variant="ghost" size="sm" onClick={() => onEdit(addr)} title="Editar">
                      <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                        <path d="M12.146.146a.5.5 0 01.708 0l3 3a.5.5 0 010 .708l-10 10a.5.5 0 01-.168.11l-5 2a.5.5 0 01-.65-.65l2-5a.5.5 0 01.11-.168l10-10z" />
                      </svg>
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onDelete(addr)}
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
            Mostrando {(page * ROWS_PER_PAGE) + 1}–{Math.min((page + 1) * ROWS_PER_PAGE, addresses.length)} de {addresses.length}
          </p>
          <div className="flex items-center gap-1.5">
            <Button variant="ghost" size="sm" disabled={page === 0} onClick={() => setPage((p) => p - 1)}>
              Anterior
            </Button>
            {Array.from({ length: totalPages }, (_, i) => (
              <Button key={i} variant={i === page ? 'primary' : 'ghost'} size="sm" onClick={() => setPage(i)}>
                {i + 1}
              </Button>
            ))}
            <Button variant="ghost" size="sm" disabled={page === totalPages - 1} onClick={() => setPage((p) => p + 1)}>
              Próximo
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
