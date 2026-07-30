import { Button } from '@/components/Button'

interface EmptyStateProps {
  message?: string
  action?: { label: string; onClick: () => void }
}

export function EmptyState({ message, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center gap-4 rounded-xl border border-dashed border-gray-300 bg-white py-16">
      <div className="flex h-14 w-14 items-center justify-center rounded-full bg-gray-100">
        <svg className="h-7 w-7 text-gray-400" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
          <path strokeLinecap="round" strokeLinejoin="round" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
      </div>
      <div className="text-center">
        <p className="text-sm font-medium text-gray-900">Nenhum registro encontrado</p>
        <p className="mt-0.5 text-sm text-gray-500">
          {message ?? 'Nenhum item para exibir no momento.'}
        </p>
      </div>
      {action && (
        <Button variant="outline" size="sm" onClick={action.onClick}>
          {action.label}
        </Button>
      )}
    </div>
  )
}
