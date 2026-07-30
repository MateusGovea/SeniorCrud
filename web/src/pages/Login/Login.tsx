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
    <div className="flex min-h-screen bg-bg-primary">
      <div className="relative hidden w-1/2 flex-col justify-between overflow-hidden bg-bg-secondary p-12 lg:flex">
        <div className="relative z-10">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-accent shadow-lg shadow-accent/20">
              <span className="text-lg font-bold text-white">S</span>
            </div>
            <div className="flex flex-col">
              <span className="text-base font-semibold text-text-primary">Senior CRUD</span>
              <span className="text-[10px] tracking-wider text-text-muted">ENTERPRISE DEMO</span>
            </div>
          </div>
        </div>

        <div className="relative z-10 max-w-md">
          <div className="mb-8 space-y-4">
            <h2 className="text-3xl font-bold leading-tight text-text-primary">
              Gerencie seus usuários<br />com excelência
            </h2>
            <p className="text-base leading-relaxed text-text-secondary">
              Plataforma completa para cadastro, controle e exportação de dados de usuários e endereços.
            </p>
          </div>
          <div className="space-y-4">
            {[
              { icon: <path d="M13.854 3.646a.5.5 0 010 .708l-7 7a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 11.708-.708L6.5 10.293l6.646-6.647a.5.5 0 01.708 0z" />, text: 'Cadastro completo de usuários' },
              { icon: <path d="M13.854 3.646a.5.5 0 010 .708l-7 7a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 11.708-.708L6.5 10.293l6.646-6.647a.5.5 0 01.708 0z" />, text: 'Gestão de endereços integrada' },
              { icon: <path d="M13.854 3.646a.5.5 0 010 .708l-7 7a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 11.708-.708L6.5 10.293l6.646-6.647a.5.5 0 01.708 0z" />, text: 'Exportação de relatórios em CSV' },
              { icon: <path d="M13.854 3.646a.5.5 0 010 .708l-7 7a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 11.708-.708L6.5 10.293l6.646-6.647a.5.5 0 01.708 0z" />, text: 'Autenticação segura com JWT' },
            ].map((item, i) => (
              <div key={i} className="flex items-center gap-3">
                <div className="flex h-6 w-6 items-center justify-center rounded-full bg-accent/10">
                  <svg className="h-3.5 w-3.5 text-accent" viewBox="0 0 16 16" fill="currentColor">{item.icon}</svg>
                </div>
                <span className="text-sm text-text-secondary">{item.text}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="relative z-10 text-xs text-text-muted">
          &copy; 2026 Senior CRUD. Todos os direitos reservados.
        </div>

        <div className="pointer-events-none absolute -right-20 -top-20 h-80 w-80 rounded-full bg-accent/5 blur-3xl" />
        <div className="pointer-events-none absolute -bottom-20 -left-20 h-60 w-60 rounded-full bg-accent/5 blur-3xl" />
      </div>

      <div className="flex w-full items-center justify-center px-6 lg:w-1/2">
        <div className="w-full max-w-sm animate-slide-up">
          <div className="mb-8 text-center lg:hidden">
            <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-2xl bg-accent shadow-lg shadow-accent/20">
              <span className="text-lg font-bold text-white">S</span>
            </div>
            <h1 className="text-xl font-semibold text-text-primary">Senior CRUD</h1>
            <p className="mt-1 text-sm text-text-muted">Faça login na sua conta</p>
          </div>
          <div className="hidden lg:mb-8 lg:block">
            <h1 className="text-2xl font-bold text-text-primary">Acessar plataforma</h1>
            <p className="mt-1 text-sm text-text-muted">Insira suas credenciais para continuar</p>
          </div>

          <div className="rounded-xl border border-border-primary bg-bg-surface p-8 shadow-xl shadow-black/20">
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
              {errors.root && (
                <div className="flex items-start gap-2.5 rounded-lg bg-danger-light p-3 text-sm text-danger">
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

          <p className="mt-6 text-center text-xs text-text-muted">
            Ambiente seguro &bull; Dados criptografados
          </p>
        </div>
      </div>
    </div>
  )
}
