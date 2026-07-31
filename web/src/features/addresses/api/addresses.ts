import { api, API_V1_PREFIX } from '@/api/axios'
import { unwrap, unwrapVoid } from '@/utils/api'
import type { ApiResult } from '@/types/api'
import type { AddressResponse, ViaCepResponseDto } from '@/features/addresses/types'

export interface CreateAddressRequest {
  userId: string
  cep: string
  street: string
  number: string
  complement?: string | null
  neighborhood: string
  city: string
  state: string
  isPrimary: boolean
}

export interface UpdateAddressRequest {
  cep: string
  street: string
  number: string
  complement?: string | null
  neighborhood: string
  city: string
  state: string
  isPrimary: boolean
}

export const addressesApi = {
  async getAddresses(pageNumber: number = 1, pageSize: number = 100, search?: string): Promise<AddressResponse[]> {
    const { data } = await api.get<ApiResult<AddressResponse[]>>(`${API_V1_PREFIX}/addresses`, {
      params: { pageNumber, pageSize, search },
    })
    return unwrap(data)
  },

  async getAddressByCep(cep: string): Promise<ViaCepResponseDto> {
    const { data } = await api.get<ApiResult<ViaCepResponseDto>>(`${API_V1_PREFIX}/viacep/${cep}`)
    return unwrap(data)
  },

  async getAddressesByUser(userId: string): Promise<AddressResponse[]> {
    const { data } = await api.get<ApiResult<AddressResponse[]>>(`${API_V1_PREFIX}/users/${userId}/addresses`)
    return unwrap(data)
  },

  async getAddressById(id: string): Promise<AddressResponse> {
    const { data } = await api.get<ApiResult<AddressResponse>>(`${API_V1_PREFIX}/addresses/${id}`)
    return unwrap(data)
  },

  async createAddress(body: CreateAddressRequest): Promise<AddressResponse> {
    const { data } = await api.post<ApiResult<AddressResponse>>(`${API_V1_PREFIX}/addresses`, body)
    return unwrap(data)
  },

  async updateAddress(id: string, body: UpdateAddressRequest): Promise<AddressResponse> {
    const { data } = await api.put<ApiResult<AddressResponse>>(`${API_V1_PREFIX}/addresses/${id}`, body)
    return unwrap(data)
  },

  async deleteAddress(id: string): Promise<void> {
    const { data } = await api.delete<ApiResult<unknown>>(`${API_V1_PREFIX}/addresses/${id}`)
    unwrapVoid(data)
  },
}
