export interface AuthUser {
  id: string
  name: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  userId: string
  nome: string
  accessToken: string
  expiresAtUtc: string
}
