import { useMutation } from '@tanstack/react-query'
import { usersApi } from '@/features/users/api'

function parseCsv(text: string): string[][] {
  const lines: string[][] = []
  let current: string[] = []
  let field = ''
  let inQuotes = false

  for (let i = 0; i < text.length; i++) {
    const ch = text[i]
    const next = text[i + 1]

    if (inQuotes) {
      if (ch === '"' && next === '"') {
        field += '"'
        i++
      } else if (ch === '"') {
        inQuotes = false
      } else {
        field += ch
      }
    } else {
      if (ch === '"') {
        inQuotes = true
      } else if (ch === ',') {
        current.push(field)
        field = ''
      } else if (ch === '\r') {
        // skip
      } else if (ch === '\n') {
        current.push(field)
        field = ''
        if (current.length > 0 && current.some((c) => c !== '')) {
          lines.push(current)
        }
        current = []
      } else {
        field += ch
      }
    }
  }

  if (field || current.length > 0) {
    current.push(field)
    if (current.some((c) => c !== '')) {
      lines.push(current)
    }
  }

  return lines
}

function translateRole(val: string): string {
  const s = val.trim().toLowerCase()
  if (s === 'admin' || s === 'administrador') return 'Administrador'
  return 'Usuário'
}

function translateStatus(val: string): string {
  return val.trim().toLowerCase() === 'true' ? 'Ativo' : 'Inativo'
}

function formatCpf(val: string): string {
  const digits = val.replace(/\D/g, '')
  if (digits.length === 11) {
    return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6, 9)}-${digits.slice(9)}`
  }
  return val
}

function formatDate(val: string): string {
  if (!val || val.trim() === '') return val
  const d = new Date(val)
  if (isNaN(d.getTime())) return val
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`
}

function formatCep(val: string): string {
  const digits = val.replace(/\D/g, '')
  if (digits.length === 8) {
    return `${digits.slice(0, 5)}-${digits.slice(5)}`
  }
  return val
}

function formatAddress(
  street: string,
  number: string,
  complement: string,
  neighborhood: string,
  city: string,
  state: string,
): string {
  if (!street && !number && !neighborhood && !city) return ''

  const parts: string[] = []
  const streetPart = `${street}${number ? `, ${number}` : ''}`
  if (streetPart) parts.push(streetPart)
  if (complement) parts.push(complement)
  if (neighborhood) parts.push(neighborhood)
  const cityState = `${city}${state ? `/${state}` : ''}`
  if (cityState && cityState !== '/') parts.push(cityState)

  return parts.join(' - ')
}

function escapeCsvField(field: string): string {
  if (field.includes(',') || field.includes('"') || field.includes('\n')) {
    return `"${field.replace(/"/g, '""')}"`
  }
  return field
}

function transformCsv(text: string): string {
  const rows = parseCsv(text)
  if (rows.length === 0) return text

  const headers = rows[0]
  const dataRows = rows.slice(1)

  const h = (name: string) =>
    headers.findIndex((h) => h.trim().toLowerCase() === name.toLowerCase())

  const idx = {
    nome: h('Nome'),
    email: h('Email'),
    cpf: h('Cpf'),
    role: h('Role'),
    isActive: h('IsActive'),
    createdAt: h('CreatedAt'),
    cep: h('Cep'),
    street: h('Street'),
    number: h('Number'),
    complement: h('Complement'),
    neighborhood: h('Neighborhood'),
    city: h('City'),
    state: h('State'),
  }

  const outputHeaders = [
    'Nome',
    'Email',
    'CPF',
    'Perfil',
    'Status',
    'Endereço Principal',
    'CEP',
    'Data de Cadastro',
  ]

  const outputRows = dataRows.map((row) => {
    const nome = row[idx.nome] ?? ''
    const email = row[idx.email] ?? ''
    const cpf = formatCpf(row[idx.cpf] ?? '')
    const role = translateRole(row[idx.role] ?? '')
    const status = translateStatus(row[idx.isActive] ?? '')
    const address = formatAddress(
      row[idx.street] ?? '',
      row[idx.number] ?? '',
      row[idx.complement] ?? '',
      row[idx.neighborhood] ?? '',
      row[idx.city] ?? '',
      row[idx.state] ?? '',
    )
    const cep = formatCep(row[idx.cep] ?? '')
    const createdAt = formatDate(row[idx.createdAt] ?? '')

    return [nome, email, cpf, role, status, address, cep, createdAt]
  })

  outputRows.sort((a, b) => a[0].localeCompare(b[0], 'pt-BR'))

  const lines: string[] = [outputHeaders.join(',')]
  for (const row of outputRows) {
    lines.push(row.map(escapeCsvField).join(','))
  }

  return '\uFEFF' + lines.join('\r\n')
}

export function useExportUsers() {
  const today = new Date()
  const pad = (n: number) => String(n).padStart(2, '0')
  const dateStr = `${today.getFullYear()}-${pad(today.getMonth() + 1)}-${pad(today.getDate())}`
  const defaultFilename = `Relatorio_Usuarios_${dateStr}.csv`

  return useMutation({
    mutationFn: (userIds?: string[]) => usersApi.exportUsersCsv(userIds),
    onSuccess: async ({ blob }) => {
      const text = await blob.text()
      const transformed = transformCsv(text)
      const newBlob = new Blob([transformed], { type: 'text/csv;charset=utf-8;' })
      const url = URL.createObjectURL(newBlob)
      const a = document.createElement('a')
      a.href = url
      a.download = defaultFilename
      a.style.display = 'none'
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
      URL.revokeObjectURL(url)
    },
  })
}
