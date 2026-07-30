import { Loading } from '@/components/Loading'

export function LoadingState() {
  return (
    <div className="flex flex-col items-center justify-center gap-3 py-20">
      <Loading size="lg" />
      <p className="text-sm text-gray-400">Carregando...</p>
    </div>
  )
}
