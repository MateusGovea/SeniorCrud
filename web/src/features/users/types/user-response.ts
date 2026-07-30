export interface UserResponse {
  id: string
  nome: string
  email: string
  cpf: string | null
  birthDate: string | null
  isActive: boolean
  role: string
}
