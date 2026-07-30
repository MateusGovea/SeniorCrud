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
        <div className="rounded-md bg-red-50 p-3 text-sm text-red-700">
          {serverError}
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

      <div className="flex flex-col gap-1">
        <label className="text-sm font-medium text-gray-700">Perfil</label>
        <select
          className={`h-10 rounded-md border border-gray-300 px-3 text-sm transition-colors focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 ${
            errors.role?.message ? 'border-red-500' : ''
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
          <span className="text-xs text-red-500">{errors.role.message}</span>
        )}
      </div>

      {mode === 'edit' && (
        <div className="flex items-center gap-2">
          <input
            type="checkbox"
            id="isActive"
            className="h-4 w-4 rounded border-gray-300 text-blue-600"
            {...register('isActive')}
          />
          <label htmlFor="isActive" className="text-sm text-gray-700">
            Usuário ativo
          </label>
        </div>
      )}

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          {mode === 'create' ? 'Criar' : 'Salvar'}
        </Button>
      </div>
    </form>
  )
}
