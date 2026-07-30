import { useAuth } from '@/hooks/useAuth'
import { Button } from '@/components/Button'

export function Navbar() {
  const { user, logout } = useAuth()

  return (
    <header className="flex h-16 items-center justify-end border-b border-gray-200 bg-white px-6">
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-3">
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-100 text-sm font-medium text-blue-700">
            {user?.name?.charAt(0).toUpperCase() ?? 'U'}
          </div>
          <span className="text-sm font-medium text-gray-700">{user?.name ?? 'Usuário'}</span>
        </div>
        <div className="h-6 w-px bg-gray-200" />
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
