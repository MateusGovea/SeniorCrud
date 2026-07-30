import type { AddressResponse } from '@/features/addresses/types'
import { Button } from '@/components/Button'

interface AddressTableProps {
  addresses: AddressResponse[]
  onEdit: (address: AddressResponse) => void
  onDelete: (address: AddressResponse) => void
}

export function AddressTable({ addresses, onEdit, onDelete }: AddressTableProps) {
  return (
    <div className="overflow-hidden rounded-xl border border-border-primary">
      <table className="min-w-full divide-y divide-border-primary">
        <thead>
          <tr className="bg-bg-secondary/50">
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              CEP
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Logradouro
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Número
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Bairro
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Cidade
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Estado
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-text-muted">
              Principal
            </th>
            <th className="px-5 py-3.5 text-right text-xs font-semibold uppercase tracking-wider text-text-muted">
              Ações
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-border-primary bg-bg-surface">
          {addresses.map((address) => (
            <tr key={address.id} className="transition-colors hover:bg-bg-hover">
              <td className="whitespace-nowrap px-5 py-4 text-sm font-mono text-text-primary">
                {address.cep}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-text-secondary">
                {address.street}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-text-secondary">
                {address.number}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-text-secondary">
                {address.neighborhood}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-text-secondary">
                {address.city}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-text-secondary">
                {address.state}
              </td>
              <td className="whitespace-nowrap px-5 py-4">
                {address.isPrimary ? (
                  <span className="inline-flex items-center gap-1 rounded-full bg-accent/10 px-2.5 py-0.5 text-xs font-medium text-accent">
                    <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
                      <path d="M13.854 3.646a.5.5 0 010 .708l-7 7a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 11.708-.708L6.5 10.293l6.646-6.647a.5.5 0 01.708 0z" />
                    </svg>
                    Principal
                  </span>
                ) : (
                  <span className="text-xs text-text-muted">—</span>
                )}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-right">
                <div className="flex items-center justify-end gap-1">
                  <Button variant="ghost" size="sm" onClick={() => onEdit(address)}>
                    Editar
                  </Button>
                  <Button variant="ghost" size="sm" onClick={() => onDelete(address)}>
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
