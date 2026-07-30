import type { AddressResponse } from '@/features/addresses/types'
import { Button } from '@/components/Button'

interface AddressTableProps {
  addresses: AddressResponse[]
  onEdit: (address: AddressResponse) => void
  onDelete: (address: AddressResponse) => void
}

export function AddressTable({ addresses, onEdit, onDelete }: AddressTableProps) {
  return (
    <div className="overflow-hidden rounded-xl border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead>
          <tr className="border-b border-gray-100 bg-gray-50/80">
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              CEP
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Logradouro
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Número
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Bairro
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Cidade
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Estado
            </th>
            <th className="px-5 py-3.5 text-left text-xs font-semibold uppercase tracking-wider text-gray-500">
              Principal
            </th>
            <th className="px-5 py-3.5 text-right text-xs font-semibold uppercase tracking-wider text-gray-500">
              Ações
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100 bg-white">
          {addresses.map((address) => (
            <tr key={address.id} className="transition-colors hover:bg-gray-50/60">
              <td className="whitespace-nowrap px-5 py-4 text-sm font-mono text-gray-900">
                {address.cep}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-gray-500">
                {address.street}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-gray-500">
                {address.number}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-gray-500">
                {address.neighborhood}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-gray-500">
                {address.city}
              </td>
              <td className="whitespace-nowrap px-5 py-4 text-sm text-gray-500">
                {address.state}
              </td>
              <td className="whitespace-nowrap px-5 py-4">
                {address.isPrimary ? (
                  <span className="inline-flex items-center gap-1 rounded-full bg-blue-100 px-2.5 py-0.5 text-xs font-medium text-blue-700">
                    <svg className="h-3 w-3" viewBox="0 0 16 16" fill="currentColor">
                      <path fillRule="evenodd" d="M10.854 6.146a.5.5 0 010 .708l-3 3a.5.5 0 01-.708 0l-1.5-1.5a.5.5 0 01.708-.708L7.5 8.793l2.646-2.647a.5.5 0 01.708 0z" clipRule="evenodd" />
                    </svg>
                    Principal
                  </span>
                ) : (
                  <span className="text-sm text-gray-300">—</span>
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
