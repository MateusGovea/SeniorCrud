export interface AddressResponse {
  id: string
  userId: string
  cep: string
  street: string
  number: string
  complement: string | null
  neighborhood: string
  city: string
  state: string
  isPrimary: boolean
}
