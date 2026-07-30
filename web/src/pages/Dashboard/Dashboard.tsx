import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { useUsers, useExportUsers } from '@/features/users/hooks'
import { useAddressesList } from '@/features/addresses/hooks'
import { UserModal } from '@/features/users/components/UserModal'
import { AddressModal } from '@/features/addresses/components/AddressModal'
import { Button } from '@/components/Button'
import { Badge } from '@/components/Badge'

export function Dashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const [showNewUser, setShowNewUser] = useState(false)
  const [showNewAddress, setShowNewAddress] = useState(false)
  const { data: users } = useUsers()
  const { data: addresses } = useAddressesList()
  const exportMutation = useExportUsers()

  const totalUsers = users?.length ?? 0
  const activeUsers = useMemo(() => users?.filter((u) => u.isActive).length ?? 0, [users])
  const totalAddresses = addresses?.length ?? 0
  const primaryAddresses = useMemo(() => addresses?.filter((a) => a.isPrimary).length ?? 0, [addresses])

  const statesCount = useMemo(() => {
    if (!addresses) return 0
    return new Set(addresses.map((a) => a.state)).size
  }, [addresses])

  const topCity = useMemo(() => {
    if (!addresses) return null
    const cityMap: Record<string, Set<string>> = {}
    for (const a of addresses) {
      if (!cityMap[a.city]) cityMap[a.city] = new Set()
      cityMap[a.city].add(a.userId)
    }
    const entries = Object.entries(cityMap)
    if (entries.length === 0) return null
    entries.sort((a, b) => b[1].size - a[1].size)
    const [city, userIds] = entries[0]
    return { city, count: userIds.size }
  }, [addresses])

  const usersWithoutAddress = useMemo(() => {
    if (!users || !addresses) return 0
    const userIdsWithAddress = new Set(addresses.map((a) => a.userId))
    return users.filter((u) => !userIdsWithAddress.has(u.id)).length
  }, [users, addresses])

  const avgAddressesPerUser = useMemo(() => {
    if (!totalUsers) return '0'
    return (totalAddresses / totalUsers).toFixed(1)
  }, [totalAddresses, totalUsers])

  const stateDistribution = useMemo(() => {
    if (!addresses) return []
    const stateMap: Record<string, Set<string>> = {}
    for (const a of addresses) {
      if (!stateMap[a.state]) stateMap[a.state] = new Set()
      stateMap[a.state].add(a.userId)
    }
    return Object.entries(stateMap)
      .map(([state, userIds]) => ({ state, count: userIds.size }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 6)
  }, [addresses])

  const sampleUsers = useMemo(() => users?.slice(0, 5) ?? [], [users])
  const sampleAddresses = useMemo(() => addresses?.slice(0, 5) ?? [], [addresses])

  const userNameMap = useMemo(() => {
    const map: Record<string, string> = {}
    if (users) {
      for (const u of users) {
        map[u.id] = u.nome
      }
    }
    return map
  }, [users])

  function handleExport() {
    exportMutation.mutate(undefined)
  }

  const greetings = ['Olá', 'Bem-vindo', 'Prazer em vê-lo']
  const greeting = greetings[Math.floor(Math.random() * greetings.length)]

  return (
    <div className="animate-fade-in space-y-8">
      <div className="flex flex-col gap-6 sm:flex-row sm:items-center sm:justify-between">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold text-text-primary">
            {greeting}, {user?.name?.split(' ')[0] ?? 'Usuário'}
          </h1>
          <p className="text-sm text-text-muted">
            Gerencie usuários e endereços através de um painel centralizado.
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-2">
          <Button size="sm" onClick={() => setShowNewUser(true)}>
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
            </svg>
            Novo Usuário
          </Button>
          <Button size="sm" onClick={() => setShowNewAddress(true)}>
            <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
            </svg>
            Novo Endereço
          </Button>
          <Button variant="secondary" size="sm" onClick={handleExport} isLoading={exportMutation.isPending}>
            <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
              <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
            </svg>
            Exportar CSV
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 hover:border-border-hover">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium text-text-muted">Total de Usuários</p>
              <p className="mt-1.5 text-2xl font-bold text-text-primary">{totalUsers}</p>
            </div>
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-accent/10 text-accent">
              <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
              </svg>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 hover:border-border-hover">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium text-text-muted">Usuários Ativos</p>
              <p className="mt-1.5 text-2xl font-bold text-success">{activeUsers}</p>
            </div>
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-success/10 text-success">
              <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
              </svg>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 hover:border-border-hover">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium text-text-muted">Total de Endereços</p>
              <p className="mt-1.5 text-2xl font-bold text-text-primary">{totalAddresses}</p>
            </div>
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-warning/10 text-warning">
              <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
              </svg>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 hover:border-border-hover">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-xs font-medium text-text-muted">Endereços Principais</p>
              <p className="mt-1.5 text-2xl font-bold text-accent">{primaryAddresses}</p>
            </div>
            <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-accent/10 text-accent">
              <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
              <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M3 6a3 3 0 013-3h10a1 1 0 010 2H6a1 1 0 000 2h9a1 1 0 011 1v7a1 1 0 01-1 1H6a3 3 0 01-3-3V6zm2 4v2h2v-2H5zm4 0v2h2v-2H9zm4 0v2h2v-2h-2z" clipRule="evenodd" />
              </svg>
            </div>
            <div>
              <p className="text-xs text-text-muted">Estados</p>
              <p className="text-lg font-bold text-text-primary">{statesCount}</p>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
              <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M4 4a2 2 0 012-2h8a2 2 0 012 2v12a2 2 0 01-2 2H6a2 2 0 01-2-2V4zm2 0h8v2H6V4zm0 4h8v2H6V8zm0 4h4v2H6v-2z" clipRule="evenodd" />
              </svg>
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs text-text-muted">Cidade com mais</p>
              <p className="truncate text-sm font-semibold text-text-primary">
                {topCity ? `${topCity.city} (${topCity.count})` : '---'}
              </p>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
              <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
              </svg>
            </div>
            <div>
              <p className="text-xs text-text-muted">Sem endereço</p>
              <p className="text-lg font-bold text-text-primary">{usersWithoutAddress}</p>
            </div>
          </div>
        </div>
        <div className="rounded-xl border border-border-primary bg-bg-surface p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
              <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                <path d="M10 2a1 1 0 011 1v1.1a5.003 5.003 0 013.9 3.9H16a1 1 0 010 2h-1.1a5.003 5.003 0 01-3.9 3.9V16a1 1 0 01-2 0v-1.1a5.003 5.003 0 01-3.9-3.9H4a1 1 0 010-2h1.1A5.003 5.003 0 019 4.1V3a1 1 0 011-1zm0 4a4 4 0 100 8 4 4 0 000-8z" />
              </svg>
            </div>
            <div>
              <p className="text-xs text-text-muted">Média end./usuário</p>
              <p className="text-lg font-bold text-text-primary">{avgAddressesPerUser}</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-5">
        <div className="space-y-4 lg:col-span-3">
          <h2 className="text-sm font-semibold text-text-primary">Distribuição por Estado</h2>
          {stateDistribution.length > 0 ? (
            <div className="rounded-xl border border-border-primary bg-bg-surface">
              {stateDistribution.map((item, i) => {
                const maxCount = stateDistribution[0].count
                const pct = maxCount > 0 ? Math.round((item.count / maxCount) * 100) : 0
                return (
                  <div
                    key={item.state}
                    className={`flex items-center gap-4 px-5 py-3.5 ${
                      i < stateDistribution.length - 1 ? 'border-b border-border-primary' : ''
                    }`}
                  >
                    <span className="w-8 text-sm font-semibold text-text-primary">{item.state}</span>
                    <div className="flex-1">
                      <div className="h-2 rounded-full bg-bg-hover">
                        <div
                          className="h-2 rounded-full bg-accent/60 transition-all"
                          style={{ width: `${pct}%` }}
                        />
                      </div>
                    </div>
                    <span className="text-sm font-medium text-text-secondary">
                      {item.count} {item.count === 1 ? 'usuário' : 'usuários'}
                    </span>
                  </div>
                )
              })}
            </div>
          ) : (
            <div className="flex items-center justify-center rounded-xl border border-dashed border-border-primary bg-bg-surface py-8">
              <p className="text-sm text-text-muted">Nenhum endereço cadastrado.</p>
            </div>
          )}
        </div>

        <div className="space-y-4 lg:col-span-2">
          <h2 className="text-sm font-semibold text-text-primary">Ações Rápidas</h2>
          <div className="grid grid-cols-1 gap-3">
            <button
              onClick={() => setShowNewUser(true)}
              className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-4 py-3.5 text-left transition-all duration-200 hover:border-border-hover hover:bg-bg-hover"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-accent/10 text-accent">
                <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M8 9a3 3 0 100-6 3 3 0 000 6zM8 11a6 6 0 016 6H2a6 6 0 016-6zM16 7a1 1 0 10-2 0v1h-1a1 1 0 100 2h1v1a1 1 0 102 0v-1h1a1 1 0 100-2h-1V7z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Novo Usuário</p>
                <p className="text-xs text-text-muted">Cadastrar novo usuário</p>
              </div>
            </button>
            <button
              onClick={() => setShowNewAddress(true)}
              className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-4 py-3.5 text-left transition-all duration-200 hover:border-border-hover hover:bg-bg-hover"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-warning/10 text-warning">
                <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Novo Endereço</p>
                <p className="text-xs text-text-muted">Adicionar novo endereço</p>
              </div>
            </button>
            <button
              onClick={handleExport}
              disabled={exportMutation.isPending}
              className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-4 py-3.5 text-left transition-all duration-200 hover:border-border-hover hover:bg-bg-hover"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-success/10 text-success">
                <svg className="h-4 w-4" viewBox="0 0 16 16" fill="currentColor">
                  <path d="M8 1a.5.5 0 01.5.5v7.793l2.646-2.647a.5.5 0 01.708.708l-3.5 3.5a.5.5 0 01-.708 0l-3.5-3.5a.5.5 0 01.708-.708L7.5 9.293V1.5A.5.5 0 018 1z" />
                  <path d="M1.5 10a.5.5 0 01.5.5v3a.5.5 0 00.5.5h11a.5.5 0 00.5-.5v-3a.5.5 0 011 0v3A1.5 1.5 0 0113.5 15h-11A1.5 1.5 0 011 13.5v-3a.5.5 0 01.5-.5z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Exportar CSV</p>
                <p className="text-xs text-text-muted">Baixar relatório de usuários</p>
              </div>
            </button>
            <button
              onClick={() => navigate('/users')}
              className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-4 py-3.5 text-left transition-all duration-200 hover:border-border-hover hover:bg-bg-hover"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
                <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Usuários</p>
                <p className="text-xs text-text-muted">Ver lista completa</p>
              </div>
            </button>
            <button
              onClick={() => navigate('/addresses')}
              className="flex items-center gap-3 rounded-xl border border-border-primary bg-bg-surface px-4 py-3.5 text-left transition-all duration-200 hover:border-border-hover hover:bg-bg-hover"
            >
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
                <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                </svg>
              </div>
              <div>
                <p className="text-sm font-medium text-text-primary">Endereços</p>
                <p className="text-xs text-text-muted">Ver lista completa</p>
              </div>
            </button>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <div className="space-y-4">
          <h2 className="text-sm font-semibold text-text-primary">Usuários</h2>
          <div className="rounded-xl border border-border-primary bg-bg-surface">
            {sampleUsers.length > 0 ? (
              sampleUsers.map((u, i) => (
                <div
                  key={u.id}
                  className={`flex items-center gap-3 px-5 py-3.5 ${
                    i < sampleUsers.length - 1 ? 'border-b border-border-primary' : ''
                  }`}
                >
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-accent/10 text-xs font-medium text-accent">
                    {u.nome.charAt(0).toUpperCase()}
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="text-sm font-medium text-text-primary truncate">{u.nome}</p>
                    <p className="text-xs text-text-muted truncate">{u.email}</p>
                  </div>
                  <Badge variant={u.role === 'Admin' ? 'accent' : 'default'}>
                    {u.role === 'Admin' ? 'Admin' : 'Usuário'}
                  </Badge>
                </div>
              ))
            ) : (
              <div className="flex flex-col items-center gap-3 py-8">
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-bg-hover">
                  <svg className="h-5 w-5 text-text-muted" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
                  </svg>
                </div>
                <p className="text-sm text-text-muted">Nenhum usuário cadastrado.</p>
              </div>
            )}
          </div>
        </div>

        <div className="space-y-4">
          <h2 className="text-sm font-semibold text-text-primary">Endereços</h2>
          <div className="rounded-xl border border-border-primary bg-bg-surface">
            {sampleAddresses.length > 0 ? (
              sampleAddresses.map((a, i) => (
                <div
                  key={a.id}
                  className={`flex items-center gap-3 px-5 py-3.5 ${
                    i < sampleAddresses.length - 1 ? 'border-b border-border-primary' : ''
                  }`}
                >
                  <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-bg-hover text-text-muted">
                    <svg className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                      <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                    </svg>
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="text-sm font-medium text-text-primary truncate">
                      {a.street}, {a.number}
                    </p>
                    <p className="text-xs text-text-muted truncate">
                      {a.city}/{a.state} &middot; {userNameMap[a.userId] ?? '---'}
                    </p>
                  </div>
                  {a.isPrimary ? (
                    <Badge variant="accent">Principal</Badge>
                  ) : (
                    <Badge variant="default">Secundário</Badge>
                  )}
                </div>
              ))
            ) : (
              <div className="flex flex-col items-center gap-3 py-8">
                <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-bg-hover">
                  <svg className="h-5 w-5 text-text-muted" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                  </svg>
                </div>
                <p className="text-sm text-text-muted">Nenhum endereço cadastrado.</p>
              </div>
            )}
          </div>
        </div>
      </div>

      {showNewUser && (
        <UserModal
          isOpen
          onClose={() => setShowNewUser(false)}
          mode="create"
        />
      )}

      {showNewAddress && (
        <AddressModal
          isOpen
          onClose={() => setShowNewAddress(false)}
          mode="create"
        />
      )}
    </div>
  )
}
