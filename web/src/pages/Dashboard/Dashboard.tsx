import { Link } from 'react-router-dom'
import { Card } from '@/components/Card'
import { Button } from '@/components/Button'

export function Dashboard() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold text-gray-900">Dashboard</h1>
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <Card>
          <h2 className="text-lg font-semibold text-gray-900">Usuários</h2>
          <p className="mt-2 text-sm text-gray-500">
            Gerencie os usuários do sistema
          </p>
          <Link to="/users" className="mt-4 inline-block">
            <Button>Ver usuários</Button>
          </Link>
        </Card>
      </div>
    </div>
  )
}
