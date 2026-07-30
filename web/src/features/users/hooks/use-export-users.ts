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

const headerMap: Record<string, string> = {
  Id: 'ID',
  Nome: 'Nome',
  Email: 'Email',
  Cpf: 'CPF',
  Role: 'Perfil',
  IsActive: 'Status',
  CreatedAt: 'Data de Cadastro',
  Cep: 'CEP',
  Street: 'Logradouro',
  Number: 'Número',
  Complement: 'Complemento',
  Neighborhood: 'Bairro',
  City: 'Cidade',
  State: 'Estado',
  IsPrimary: 'Principal',
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

function transformCsv(text: string): string {
  const rows = parseCsv(text)
  if (rows.length === 0) return text

  const headers = rows[0]
  const dataRows = rows.slice(1)

  const transformedHeaders = headers.map((h) => {
    const trimmed = h.trim()
    return headerMap[trimmed] ?? trimmed
  })

  const roleIdx = headers.findIndex((h) => h.trim().toLowerCase() === 'role')
  const statusIdx = headers.findIndex((h) => h.trim().toLowerCase() === 'isactive')
  const cpfIdx = headers.findIndex((h) => h.trim().toLowerCase() === 'cpf')
  const dateIdx = headers.findIndex((h) => h.trim().toLowerCase() === 'createdat')

  const outputRows: string[] = [transformedHeaders.join(',')]

  for (const row of dataRows) {
    const newRow = [...row]
    if (roleIdx >= 0 && roleIdx < newRow.length) {
      newRow[roleIdx] = translateRole(newRow[roleIdx])
    }
    if (statusIdx >= 0 && statusIdx < newRow.length) {
      newRow[statusIdx] = translateStatus(newRow[statusIdx])
    }
    if (cpfIdx >= 0 && cpfIdx < newRow.length) {
      newRow[cpfIdx] = formatCpf(newRow[cpfIdx])
    }
    if (dateIdx >= 0 && dateIdx < newRow.length) {
      newRow[dateIdx] = formatDate(newRow[dateIdx])
    }
    const escaped = newRow.map((f) => {
      if (f.includes(',') || f.includes('"') || f.includes('\n')) {
        return `"${f.replace(/"/g, '""')}"`
      }
      return f
    })
    outputRows.push(escaped.join(','))
  }

  return '\uFEFF' + outputRows.join('\r\n')
}

export function useExportUsers() {
  return useMutation({
    mutationFn: (userIds?: string[]) => usersApi.exportUsersCsv(userIds),
    onSuccess: async ({ blob, filename }) => {
      const text = await blob.text()
      const transformed = transformCsv(text)
      const newBlob = new Blob([transformed], { type: 'text/csv;charset=utf-8;' })
      const url = URL.createObjectURL(newBlob)
      const a = document.createElement('a')
      a.href = url
      a.download = filename && filename !== 'users.csv' ? filename : 'usuarios.csv'
      a.style.display = 'none'
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
      URL.revokeObjectURL(url)
    },
  })
}
