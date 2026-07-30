import { useQuery } from '@tanstack/react-query'
import { addressesApi } from '@/features/addresses/api'

export function useViaCep(cep: string) {
  return useQuery({
    queryKey: ['viacep', cep],
    queryFn: () => addressesApi.getAddressByCep(cep),
    enabled: cep.length === 8,
    retry: false,
    staleTime: 5 * 60 * 1000,
  })
}
