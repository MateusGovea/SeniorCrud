import type { AddressResponse } from '@/features/addresses/types'
import { Button } from '@/components/Button'

interface AddressTableProps {
  addresses: AddressResponse[]
  onEdit: (address: AddressResponse) => void
  onDelete: (address: AddressResponse) => void
}

export function AddressTable({ addresses, onEdit, onDelete }: AddressTableProps) {
  return (
    <div className="overflow-x-auto rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              CEP
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Logradouro
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Número
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Bairro
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Cidade
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Estado
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Principal
            </th>
            <th className="px-4 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
              Ações
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 bg-white">
          {addresses.map((address) => (
            <tr key={address.id} className="hover:bg-gray-50">
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-900">
                {address.cep}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-500">
                {address.street}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-500">
                {address.number}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-500">
                {address.neighborhood}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-500">
                {address.city}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-sm text-gray-500">
                {address.state}
              </td>
              <td className="whitespace-nowrap px-4 py-4">
                {address.isPrimary ? (
                  <span className="inline-flex rounded-full bg-blue-100 px-2 text-xs font-semibold leading-5 text-blue-800">
                    Principal
                  </span>
                ) : (
                  <span className="text-sm text-gray-400">—</span>
                )}
              </td>
              <td className="whitespace-nowrap px-4 py-4 text-right text-sm">
                <Button variant="ghost" size="sm" onClick={() => onEdit(address)}>
                  Editar
                </Button>
                <Button variant="ghost" size="sm" onClick={() => onDelete(address)}>
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
