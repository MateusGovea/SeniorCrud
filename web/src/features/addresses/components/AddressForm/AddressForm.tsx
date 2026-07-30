import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Input } from '@/components/Input'
import { Button } from '@/components/Button'

const schema = z.object({
  cep: z
    .string()
    .min(1, 'Obrigatório')
    .transform((val) => val.replace(/\D/g, ''))
    .refine((val) => val.length === 8, 'CEP deve conter 8 dígitos'),
  street: z.string().min(1, 'Obrigatório').max(150, 'Máximo de 150 caracteres'),
  number: z.string().min(1, 'Obrigatório').max(20, 'Máximo de 20 caracteres'),
  complement: z.string().optional(),
  neighborhood: z.string().min(1, 'Obrigatório').max(120, 'Máximo de 120 caracteres'),
  city: z.string().min(1, 'Obrigatório').max(120, 'Máximo de 120 caracteres'),
  state: z
    .string()
    .min(1, 'Obrigatório')
    .length(2, 'Estado deve ter 2 caracteres')
    .transform((val) => val.toUpperCase()),
  isPrimary: z.boolean().optional(),
})

type FormData = {
  cep: string
  street: string
  number: string
  complement?: string
  neighborhood: string
  city: string
  state: string
  isPrimary?: boolean
}

const resolver = zodResolver(schema)

interface AddressFormProps {
  defaultValues?: Partial<FormData>
  onSave: (data: FormData) => Promise<void>
  onCancel: () => void
  serverError?: string | null
}

export function AddressForm({ defaultValues, onSave, onCancel, serverError }: AddressFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: resolver as never,
    defaultValues: {
      complement: '',
      isPrimary: false,
      ...defaultValues,
    },
  })

  return (
    <form onSubmit={handleSubmit(onSave)} className="space-y-4">
      {serverError && (
        <div className="rounded-md bg-red-50 p-3 text-sm text-red-700">
          {serverError}
        </div>
      )}

      <Input
        label="CEP"
        placeholder="Somente números"
        error={errors.cep?.message}
        {...register('cep')}
      />

      <div className="grid grid-cols-3 gap-4">
        <div className="col-span-2">
          <Input
            label="Logradouro"
            placeholder="Nome da rua, avenida..."
            error={errors.street?.message}
            {...register('street')}
          />
        </div>
        <Input
          label="Número"
          placeholder="Nº"
          error={errors.number?.message}
          {...register('number')}
        />
      </div>

      <Input
        label="Complemento"
        placeholder="Apto, Bloco, etc. (opcional)"
        error={errors.complement?.message}
        {...register('complement')}
      />

      <div className="grid grid-cols-2 gap-4">
        <Input
          label="Bairro"
          placeholder="Bairro"
          error={errors.neighborhood?.message}
          {...register('neighborhood')}
        />
        <Input
          label="Cidade"
          placeholder="Cidade"
          error={errors.city?.message}
          {...register('city')}
        />
      </div>

      <div className="flex items-center gap-4">
        <div className="w-24">
          <Input
            label="Estado"
            placeholder="UF"
            maxLength={2}
            error={errors.state?.message}
            {...register('state')}
          />
        </div>

        <div className="flex items-center gap-2 pt-5">
          <input
            type="checkbox"
            id="isPrimary"
            className="h-4 w-4 rounded border-gray-300 text-blue-600"
            {...register('isPrimary')}
          />
          <label htmlFor="isPrimary" className="text-sm text-gray-700">
            Endereço principal
          </label>
        </div>
      </div>

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          Salvar
        </Button>
      </div>
    </form>
  )
}
