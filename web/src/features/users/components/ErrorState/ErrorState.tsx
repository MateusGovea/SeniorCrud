import { Button } from '@/components/Button'

interface ErrorStateProps {
  message?: string
  onRetry?: () => void
}

export function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-4 rounded-xl border border-red-200 bg-red-50/50 py-16">
      <div className="flex h-14 w-14 items-center justify-center rounded-full bg-red-100">
        <svg className="h-7 w-7 text-red-500" viewBox="0 0 20 20" fill="currentColor">
          <path
            fillRule="evenodd"
            d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
            clipRule="evenodd"
          />
        </svg>
      </div>
      <div className="text-center">
        <p className="text-sm font-medium text-red-800">Erro ao carregar dados</p>
        <p className="mt-0.5 text-sm text-red-600">
          {message ?? 'Não foi possível carregar as informações.'}
        </p>
      </div>
      {onRetry && (
        <Button variant="outline" size="sm" onClick={onRetry}>
          Tentar novamente
        </Button>
      )}
    </div>
  )
}
