import { useMutation } from '@tanstack/react-query'
import { usersApi } from '@/features/users/api'

export function useExportUsers() {
  return useMutation({
    mutationFn: (userIds?: string[]) => usersApi.exportUsersCsv(userIds),
    onSuccess: ({ blob, filename }) => {
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = filename
      a.style.display = 'none'
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
      URL.revokeObjectURL(url)
    },
  })
}
