import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { useUsers, useExportUsers } from '@/features/users/hooks'
import { Card } from '@/components/Card'

export function Dashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const { data: users } = useUsers()
  const exportMutation = useExportUsers()

  const totalUsers = users?.length ?? 0
  const activeUsers = users?.filter((u) => u.isActive).length ?? 0
  const admins = users?.filter((u) => u.role === 'Admin').length ?? 0

  const metrics = [
    {
      label: 'Usuários Cadastrados',
      value: totalUsers,
      icon: (
        <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
          <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
        </svg>
      ),
      color: 'text-accent',
      bg: 'bg-accent/10',
      onClick: () => navigate('/users', { state: { filter: 'all' } }),
    },
    {
      label: 'Usuários Ativos',
      value: activeUsers,
      icon: (
        <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
          <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
        </svg>
      ),
      color: 'text-success',
      bg: 'bg-success/10',
      onClick: () => navigate('/users', { state: { filter: 'active' } }),
    },
    {
      label: 'Administradores',
      value: admins,
      icon: (
        <svg className="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
          <path fillRule="evenodd" d="M10 2a1 1 0 00-1 1v1a1 1 0 002 0V3a1 1 0 00-1-1zM4 4h3a3 3 0 006 0h3a2 2 0 012 2v2a2 2 0 01-2 2H4a2 2 0 01-2-2V6a2 2 0 012-2zm2.5 7a1.5 1.5 0 100 3 1.5 1.5 0 000-3zm7 0a1.5 1.5 0 100 3 1.5 1.5 0 000-3z" clipRule="evenodd" />
        </svg>
      ),
      color: 'text-warning',
      bg: 'bg-warning/10',
      onClick: () => navigate('/users', { state: { filter: 'admin' } }),
    },
  ]

  return (
    <div className="animate-fade-in space-y-8">
      <div>
        <h1 className="text-2xl font-bold text-text-primary">
          Bem-vindo novamente, {user?.name?.split(' ')[0] ?? 'Usuário'} <span className="inline-block animate-pulse-soft">👋</span>
        </h1>
        <p className="mt-1 text-sm text-text-muted">
          Gerencie usuários, endereços e acompanhe o sistema.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
        {metrics.map((metric) => (
          <div
            key={metric.label}
            onClick={metric.onClick}
            role="button"
            tabIndex={0}
            onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); metric.onClick() } }}
            className="rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 hover:border-border-hover hover:shadow-lg hover:shadow-black/20 cursor-pointer"
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs font-medium text-text-muted">{metric.label}</p>
                <p className="mt-1.5 text-2xl font-bold text-text-primary">{metric.value}</p>
              </div>
              <div className={`flex h-12 w-12 items-center justify-center rounded-xl ${metric.bg} ${metric.color}`}>
                {metric.icon}
              </div>
            </div>
          </div>
        ))}
      </div>

      <div>
        <h2 className="text-sm font-semibold text-text-primary mb-3">Ações Rápidas</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Card hover onClick={() => navigate('/users', { state: { openCreateUser: true } })}>
            <div className="flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-accent/10 text-accent">
                <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Novo Usuário</p>
                <p className="text-xs text-text-muted">Cadastrar novo usuário no sistema</p>
              </div>
            </div>
          </Card>
          <Card hover onClick={() => exportMutation.mutate(undefined)}>
            <div className="flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-success/10 text-success">
                <svg className="h-5 w-5" viewBox="0 0 16 16" fill="currentColor">
                  <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
                  <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Exportar CSV</p>
                <p className="text-xs text-text-muted">Exportar lista de usuários</p>
              </div>
            </div>
          </Card>
          <Card hover onClick={() => navigate('/users', { state: { showAddressesHint: true } })}>
            <div className="flex items-center gap-4">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-warning/10 text-warning">
                <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Gerenciar Endereços</p>
                <p className="text-xs text-text-muted">Visualizar endereços dos usuários</p>
              </div>
            </div>
          </Card>
        </div>
      </div>

      <div>
        <h2 className="text-sm font-semibold text-text-primary mb-3">Atividade Recente</h2>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-8">
          <div className="flex flex-col items-center justify-center gap-3 py-6">
            <div className="flex h-12 w-12 items-center justify-center rounded-xl bg-bg-hover">
              <svg className="h-6 w-6 text-text-muted" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd" />
              </svg>
            </div>
            <div className="text-center">
              <p className="text-sm font-medium text-text-primary">Histórico de Atividades</p>
              <p className="mt-0.5 text-xs text-text-muted">Em breve você poderá acompanhar todas as ações realizadas no sistema.</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
