import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/hooks/useAuth'
import { Card } from '@/components/Card'

export function Dashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()

  return (
    <div className="animate-in">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">
          Olá, {user?.name?.split(' ')[0] ?? 'Usuário'}
        </h1>
        <p className="mt-1 text-sm text-gray-500">Bem-vindo ao SeniorCrud</p>
      </div>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
        <Card hover onClick={() => navigate('/users')}>
          <div className="mb-3 flex h-10 w-10 items-center justify-center rounded-lg bg-blue-100 text-blue-600">
            <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z" />
            </svg>
          </div>
          <h2 className="text-base font-semibold text-gray-900">Usuários</h2>
          <p className="mt-1 text-sm text-gray-500">
            Gerenciar cadastro de usuários do sistema
          </p>
        </Card>

        <Card hover onClick={() => navigate('/users')}>
          <div className="mb-3 flex h-10 w-10 items-center justify-center rounded-lg bg-emerald-100 text-emerald-600">
            <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
              <path
                fillRule="evenodd"
                d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z"
                clipRule="evenodd"
              />
            </svg>
          </div>
          <h2 className="text-base font-semibold text-gray-900">Endereços</h2>
          <p className="mt-1 text-sm text-gray-500">
            Visualizar e gerenciar endereços dos usuários
          </p>
        </Card>
      </div>
    </div>
  )
}
