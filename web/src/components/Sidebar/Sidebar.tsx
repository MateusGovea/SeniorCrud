import { NavLink } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'

const navGroups = [
  {
    label: 'Principal',
    items: [
      {
        label: 'Dashboard',
        path: '/dashboard',
        icon: (
          <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
            <path d="M2 10a8 8 0 018-8v8h8a8 8 0 11-16 0z" />
            <path d="M12 2.252A8.014 8.014 0 0117.748 8H12V2.252z" />
          </svg>
        ),
      },
      {
        label: 'Usuários',
        path: '/users',
        icon: (
          <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
          </svg>
        ),
      },
      {
        label: 'Endereços',
        path: '/addresses',
        icon: (
          <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
            <path
              fillRule="evenodd"
              d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
              clipRule="evenodd"
            />
          </svg>
        ),
      },
    ],
  },
]

export function Sidebar() {
  const { user } = useAuth()

  return (
    <aside className="flex w-60 flex-col border-r border-border-primary bg-bg-secondary">
      <div className="flex h-16 items-center gap-3 border-b border-border-primary px-4">
        <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-linear-to-br from-accent to-accent-hover shadow-lg shadow-accent/25">
          <span className="text-sm font-bold text-white">S</span>
        </div>
        <div className="flex flex-col leading-tight">
          <span className="text-[13px] font-semibold tracking-tight text-text-primary">Senior CRUD</span>
          <span className="text-[9px] font-medium uppercase tracking-[0.18em] text-text-muted">Enterprise</span>
        </div>
      </div>

      <nav className="flex-1 space-y-6 overflow-y-auto px-3 py-5">
        {navGroups.map((group) => (
          <div key={group.label} className="space-y-2">
            <div className="px-3">
              <span className="text-[10px] font-semibold uppercase tracking-[0.16em] text-text-muted/80">
                {group.label}
              </span>
            </div>
            <div className="space-y-1">
              {group.items.map((item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  end
                  className={({ isActive }) =>
                    `group relative flex items-center gap-3 rounded-xl py-2.5 pl-3.5 pr-3 text-[13.5px] font-medium transition-all duration-200 ${
                      isActive
                        ? 'bg-linear-to-r from-accent/15 to-accent/5 text-text-primary shadow-sm ring-1 ring-inset ring-accent/25'
                        : 'text-text-secondary hover:bg-bg-hover hover:text-text-primary'
                    }`
                  }
                >
                  {({ isActive }) => (
                    <>
                      {isActive && (
                        <span className="absolute left-0 top-1/2 h-6 w-1 -translate-y-1/2 rounded-r-full bg-accent" />
                      )}
                      <span
                        className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-lg transition-colors duration-200 ${
                          isActive
                            ? 'bg-accent/15 text-accent'
                            : 'text-text-muted group-hover:bg-bg-elevated/60 group-hover:text-text-secondary'
                        }`}
                      >
                        {item.icon}
                      </span>
                      <span className="flex-1">{item.label}</span>
                      {isActive && <span className="h-1.5 w-1.5 rounded-full bg-accent/70" />}
                    </>
                  )}
                </NavLink>
              ))}
            </div>
          </div>
        ))}
      </nav>

      <div className="border-t border-border-primary p-3">
        <div className="space-y-2">
          <div className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-3 py-3">
            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-accent/10 text-sm font-medium text-accent ring-1 ring-accent/20">
              {user?.name?.charAt(0).toUpperCase() ?? 'U'}
            </div>
            <div className="min-w-0 flex-1">
              <p className="truncate text-xs font-medium text-text-primary">{user?.name ?? 'Usuário'}</p>
              <div className="flex items-center gap-1.5">
                <span className="h-1.5 w-1.5 rounded-full bg-success" />
                <span className="text-[10px] text-text-muted">Online</span>
              </div>
            </div>
          </div>
          <div className="flex items-center justify-between px-1">
            <span className="text-[10px] text-text-muted">v1.0.0</span>
            <span className="rounded-md border border-border-primary bg-bg-surface px-1.5 py-0.5 text-[10px] font-medium text-text-muted">
              PROD
            </span>
          </div>
        </div>
      </div>
    </aside>
  )
}
