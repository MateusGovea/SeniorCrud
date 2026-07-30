import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useAuth } from '@/hooks/useAuth'
import { Input } from '@/components/Input'
import { Button } from '@/components/Button'
import { AuthError } from '@/services/auth'

const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'E-mail é obrigatório')
    .email('E-mail inválido'),
  password: z
    .string()
    .min(1, 'Senha é obrigatória')
    .min(6, 'Senha deve ter no mínimo 6 caracteres'),
})

type LoginFormData = z.infer<typeof loginSchema>

export function Login() {
  const { login } = useAuth()
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  })

  async function onSubmit(data: LoginFormData) {
    try {
      await login(data)
    } catch (error) {
      if (error instanceof AuthError) {
        setError('root', { message: error.message })
      } else {
        setError('root', { message: 'Erro inesperado. Tente novamente.' })
      }
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 px-4">
      <div className="w-full max-w-sm animate-in">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-blue-600 text-lg font-bold text-white shadow-sm">
            S
          </div>
          <h1 className="text-xl font-semibold text-gray-900">SeniorCrud</h1>
          <p className="mt-1 text-sm text-gray-500">Faça login para continuar</p>
        </div>

        <div className="rounded-xl border border-gray-200 bg-white p-8 shadow-sm">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {errors.root && (
              <div className="flex items-start gap-2.5 rounded-lg bg-red-50 p-3 text-sm text-red-700">
                <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
                  <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
                </svg>
                <span>{errors.root.message}</span>
              </div>
            )}

            <Input
              label="E-mail"
              type="email"
              placeholder="seu@email.com"
              error={errors.email?.message}
              {...register('email')}
            />

            <Input
              label="Senha"
              type="password"
              placeholder="Sua senha"
              error={errors.password?.message}
              {...register('password')}
            />

            <Button type="submit" className="w-full" size="lg" isLoading={isSubmitting}>
              Entrar
            </Button>
          </form>
        </div>
      </div>
    </div>
  )
}
