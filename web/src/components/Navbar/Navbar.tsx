import { useLocation, Link } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { Button } from '@/components/Button'

function usePageInfo(pathname: string, state: unknown): { title: string; parent?: { label: string; path: string } } {
  if (pathname === '/dashboard') return { title: 'Dashboard' }
  if (pathname === '/users') return { title: 'Usuários' }
  if (pathname.startsWith('/users/')) {
    const segments = pathname.split('/')
    if (segments.length === 4 && segments[3] === 'addresses') {
      const s = state as { userName?: string } | null
      const label = s?.userName ?? 'Usuários'
      return { title: 'Endereços', parent: { label, path: '/users' } }
    }
    if (segments.length === 3) {
      return { title: 'Detalhes do Usuário', parent: { label: 'Usuários', path: '/users' } }
    }
  }
  return { title: 'Senior CRUD' }
}

export function Navbar() {
  const { user, logout } = useAuth()
  const location = useLocation()
  const page = usePageInfo(location.pathname, location.state)

  return (
    <header className="flex h-16 items-center justify-between border-b border-border-primary bg-bg-secondary px-6">
      <div className="flex items-center gap-3">
        <div>
          {page.parent && (
            <Link to={page.parent.path} className="text-xs text-text-muted hover:text-text-secondary transition-colors">
              {page.parent.label}
            </Link>
          )}
          <h1 className="text-base font-semibold text-text-primary">{page.title}</h1>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-3">
          <div className="relative">
            <div className="flex h-8 w-8 items-center justify-center rounded-full bg-accent/10 text-sm font-medium text-accent ring-1 ring-accent/20">
              {user?.name?.charAt(0).toUpperCase() ?? 'U'}
            </div>
            <span className="absolute -bottom-0.5 -right-0.5 h-2.5 w-2.5 rounded-full border-2 border-bg-secondary bg-success" />
          </div>
          <div className="flex flex-col">
            <span className="text-sm font-medium text-text-primary">{user?.name ?? 'Usuário'}</span>
          </div>
        </div>
        <div className="h-6 w-px bg-border-primary" />
        <Button variant="ghost" size="sm" onClick={logout}>
          <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
            <path
              fillRule="evenodd"
              d="M3 3a1 1 0 00-1 1v12a1 1 0 001 1h5a1 1 0 100-2H4V5h4a1 1 0 100-2H3zm11.707 3.293a1 1 0 010 1.414L12.414 10l2.293 2.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z"
              clipRule="evenodd"
            />
          </svg>
          Sair
        </Button>
      </div>
    </header>
  )
}
