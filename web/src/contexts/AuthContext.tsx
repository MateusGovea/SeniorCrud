import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { useNavigate } from 'react-router-dom'
import axios from 'axios'
import { api } from '@/api/axios'
import type { AuthUser, LoginRequest } from '@/types/auth'
import { authService, AuthError } from '@/services/auth'
import { storage } from '@/utils/storage'
import { decodeJwtPayload, isTokenExpired } from '@/utils/jwt'

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  isLoading: boolean
  login: (request: LoginRequest) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

function restoreUserFromToken(): AuthUser | null {
  const token = storage.getToken()
  if (!token) return null

  if (isTokenExpired(token)) {
    storage.clear()
    return null
  }

  const payload = decodeJwtPayload(token)
  if (!payload?.sub) {
    storage.clear()
    return null
  }

  return {
    id: payload.sub,
    name: (payload.name as string) ?? 'Usuário',
  }
}

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const navigate = useNavigate()
  const [user, setUser] = useState<AuthUser | null>(() => restoreUserFromToken())
  const [isLoading, setIsLoading] = useState(false)

  const clearAuth = useCallback(() => {
    storage.clear()
    setUser(null)
  }, [])

  const login = useCallback(
    async (request: LoginRequest) => {
      setIsLoading(true)
      try {
        const response = await authService.login(request)
        storage.setToken(response.accessToken)
        setUser({ id: response.userId, name: response.nome })
        navigate('/dashboard', { replace: true })
      } catch (error) {
        if (error instanceof AuthError) {
          throw error
        }
        throw new AuthError('Erro de conexão. Tente novamente.')
      } finally {
        setIsLoading(false)
      }
    },
    [navigate],
  )

  const logout = useCallback(() => {
    clearAuth()
    navigate('/login', { replace: true })
  }, [clearAuth, navigate])

  useEffect(() => {
    const interceptor = api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          clearAuth()
          navigate('/login', { replace: true })
        }
        return Promise.reject(error)
      },
    )
    return () => api.interceptors.response.eject(interceptor)
  }, [clearAuth, navigate])

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      isLoading,
      login,
      logout,
    }),
    [user, isLoading, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuthContext(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuthContext must be used within an AuthProvider')
  }
  return context
}
