const TOKEN_KEY = 'seniorcrud_access_token'

export const storage = {
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY)
  },

  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token)
  },

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY)
  },

  clear(): void {
    localStorage.removeItem(TOKEN_KEY)
  },
}
