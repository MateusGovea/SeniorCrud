import { useCallback, useEffect, useRef, useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Input } from '@/components/Input'
import { Button } from '@/components/Button'
import { Loading } from '@/components/Loading'
import { useViaCep } from '@/features/addresses/hooks'

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

type FormData = z.infer<typeof schema>

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
    getValues,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver,
    defaultValues: {
      complement: '',
      isPrimary: false,
      ...defaultValues,
    },
  })

  const [searchedCep, setSearchedCep] = useState('')
  const { data: viaCepData, isFetching: viaCepLoading, isError: viaCepIsError, error: viaCepQueryError } = useViaCep(searchedCep)
  const [viaCepErrorMessage, setViaCepErrorMessage] = useState<string | null>(null)
  const fieldSnapshotRef = useRef<Record<string, string>>({})

  const handleCepBlur = useCallback(
    (e: React.FocusEvent<HTMLInputElement>) => {
      const digits = e.target.value.replace(/\D/g, '')
      if (digits.length === 8 && digits !== searchedCep) {
        const values = getValues()
        fieldSnapshotRef.current = {
          street: values.street || '',
          neighborhood: values.neighborhood || '',
          city: values.city || '',
          state: values.state || '',
        }
        setViaCepErrorMessage(null)
        setSearchedCep(digits)
      }
    },
    [getValues, searchedCep],
  )

  useEffect(() => {
    if (viaCepData) {
      setViaCepErrorMessage(null)
      const snapshot = fieldSnapshotRef.current
      if (viaCepData.logradouro && !snapshot.street) {
        setValue('street', viaCepData.logradouro)
      }
      if (viaCepData.bairro && !snapshot.neighborhood) {
        setValue('neighborhood', viaCepData.bairro)
      }
      if (viaCepData.localidade && !snapshot.city) {
        setValue('city', viaCepData.localidade)
      }
      if (viaCepData.uf && !snapshot.state) {
        setValue('state', viaCepData.uf)
      }
    }
  }, [viaCepData, setValue])

  useEffect(() => {
    if (viaCepIsError && viaCepQueryError) {
      const msg = viaCepQueryError instanceof Error ? viaCepQueryError.message.toLowerCase() : ''
      if (msg.includes('não encontrado') || msg.includes('not found')) {
        setViaCepErrorMessage('CEP não encontrado.')
      } else {
        setViaCepErrorMessage('Não foi possível consultar o CEP.')
      }
    }
  }, [viaCepIsError, viaCepQueryError])

  const cepField = register('cep')

  return (
    <form onSubmit={handleSubmit(onSave)} className="space-y-4">
      {serverError && (
        <div className="flex items-start gap-2.5 rounded-lg bg-red-50 p-3 text-sm text-red-700">
          <svg className="mt-0.5 h-4 w-4 shrink-0" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
          </svg>
          <span>{serverError}</span>
        </div>
      )}

      <div className="flex items-end gap-2">
        <div className="flex-1">
          <Input
            label="CEP"
            placeholder="Somente números"
            error={errors.cep?.message || viaCepErrorMessage || undefined}
            {...cepField}
            onBlur={(e) => {
              cepField.onBlur(e)
              handleCepBlur(e)
            }}
          />
        </div>
        {viaCepLoading && (
          <div className="pb-2">
            <Loading size="sm" />
          </div>
        )}
      </div>

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

        <div className="flex items-center gap-2.5 rounded-lg border border-gray-200 bg-gray-50/50 px-4 py-3">
          <input
            type="checkbox"
            id="isPrimary"
            className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            {...register('isPrimary')}
          />
          <label htmlFor="isPrimary" className="text-sm font-medium text-gray-700">
            Endereço principal
          </label>
        </div>
      </div>

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
          Cancelar
        </Button>
        <Button type="submit" isLoading={isSubmitting}>
          Salvar
        </Button>
      </div>
    </form>
  )
}
