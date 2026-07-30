import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { addressesApi } from '@/features/addresses/api'
import type { CreateAddressRequest, UpdateAddressRequest } from '@/features/addresses/api'

export function useAddresses(userId: string | undefined) {
  return useQuery({
    queryKey: ['addresses', userId],
    queryFn: () => addressesApi.getAddressesByUser(userId!),
    enabled: !!userId,
  })
}

export function useAddress(id: string | undefined) {
  return useQuery({
    queryKey: ['addresses', 'detail', id],
    queryFn: () => addressesApi.getAddressById(id!),
    enabled: !!id,
  })
}

export function useCreateAddress() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateAddressRequest) => addressesApi.createAddress(data),
    onSuccess: (response) => {
      queryClient.invalidateQueries({ queryKey: ['addresses', response.userId] })
    },
  })
}

export function useUpdateAddress() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (variables: { id: string; data: UpdateAddressRequest; userId: string }) =>
      addressesApi.updateAddress(variables.id, variables.data),
    onSuccess: (_response, variables) => {
      queryClient.invalidateQueries({ queryKey: ['addresses', variables.userId] })
    },
  })
}

export function useDeleteAddress() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (variables: { id: string; userId: string }) =>
      addressesApi.deleteAddress(variables.id),
    onSuccess: (_response, variables) => {
      queryClient.invalidateQueries({ queryKey: ['addresses', variables.userId] })
    },
  })
}
