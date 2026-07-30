import { Link } from 'react-router-dom'
import { Button } from '@/components/Button'

export function NotFound() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 bg-bg-primary">
      <div className="flex h-20 w-20 items-center justify-center rounded-2xl bg-bg-surface border border-border-primary">
        <span className="text-3xl font-bold text-text-muted">404</span>
      </div>
      <p className="text-base text-text-secondary">Página não encontrada</p>
      <Link to="/dashboard">
        <Button>Voltar ao Dashboard</Button>
      </Link>
    </div>
  )
}
