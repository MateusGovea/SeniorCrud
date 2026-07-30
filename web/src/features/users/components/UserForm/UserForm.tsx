import { useForm, type Resolver } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Input } from '@/components/Input'
import { Button } from '@/components/Button'

const schema = z.object({
  nome: z
    .string()
    .min(3, 'Mínimo de 3 caracteres')
    .max(120, 'Máximo de 120 caracteres'),
  email: z.string().min(1, 'Obrigatório').email('E-mail inválido').max(255),
  password: z.string().optional(),
  cpf: z.string().optional(),
  role: z.string().min(1, 'Selecione um perfil'),
  isActive: z.boolean().optional(),
})

type FormData = {
  nome: string
  email: string
  password?: string
  cpf?: string
  role: string
  isActive?: boolean
}

const resolver = zodResolver(schema) as Resolver<FormData>

interface UserFormProps {
  mode: 'create' | 'edit'
  defaultValues?: Partial<FormData>
  onSave: (data: FormData) => Promise<void>
  onCancel: () => void
  serverError?: string | null
}

const roles = [
  { value: 'Admin', label: 'Admin' },
  { value: 'User', label: 'Usuário' },
]

export function UserForm({ mode, defaultValues, onSave, onCancel, serverError }: UserFormProps) {
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver,
    defaultValues: {
      role: 'User',
      isActive: true,
      cpf: '',
      ...defaultValues,
    },
  })

  async function onSubmit(data: FormData) {
    if (mode === 'create' && (!data.password || data.password.length < 6)) {
      setError('password', { message: 'Mínimo de 6 caracteres' })
      return
    }
    await onSave(data)
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {serverError && (
        <div className="flex items-start gap-2.5 rounded-lg bg-red-50 p-3 text-sm text-red-700">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{serverError}</span>
        </div>
      )}

      <Input
        label="Nome"
        placeholder="Nome completo"
        error={errors.nome?.message}
        {...register('nome')}
      />

      <Input
        label="E-mail"
        type="email"
        placeholder="seu@email.com"
        error={errors.email?.message}
        {...register('email')}
      />

      {mode === 'create' && (
        <Input
          label="Senha"
          type="password"
          placeholder="Mínimo 6 caracteres"
          error={errors.password?.message}
          {...register('password')}
        />
      )}

      <Input
        label="CPF"
        placeholder="Apenas números"
        error={errors.cpf?.message}
        {...register('cpf')}
      />

      <div className="flex flex-col gap-1.5">
        <label className="text-sm font-medium text-gray-700">Perfil</label>
        <select
          className={`h-10 w-full rounded-lg border px-3 text-sm text-gray-900 transition-all duration-150 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 ${
            errors.role?.message
              ? 'border-red-400'
              : 'border-gray-300 hover:border-gray-400'
          }`}
          {...register('role')}
        >
          <option value="">Selecione...</option>
          {roles.map((r) => (
            <option key={r.value} value={r.value}>
              {r.label}
            </option>
          ))}
        </select>
        {errors.role?.message && (
          <span className="flex items-center gap-1 text-xs text-red-500">
            <svg className="h-3.5 w-3.5 shrink-0" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
            </svg>
            {errors.role.message}
          </span>
        )}
      </div>

      {mode === 'edit' && (
        <div className="flex items-center gap-2.5 rounded-lg border border-gray-200 bg-gray-50/50 px-4 py-3">
          <input
            type="checkbox"
            id="isActive"
            className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            {...register('isActive')}
          />
          <label htmlFor="isActive" className="text-sm font-medium text-gray-700">
            Usuário ativo
          </label>
        </div>
      )}

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {mode === 'create' ? 'Criar Usuário' : 'Salvar Alterações'}
        </Button>
      </div>
    </form>
  )
}
