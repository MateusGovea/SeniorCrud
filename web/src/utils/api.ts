import type { ApiResult } from '@/types/api'

export class ApiError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

export function unwrap<T>(result: ApiResult<T>): T {
  if (!result.isSuccess || result.value === null || result.value === undefined) {
    throw new ApiError(result.error?.description ?? 'Erro inesperado')
  }
  return result.value
}

export function unwrapVoid(result: ApiResult<unknown>): void {
  if (!result.isSuccess) {
    throw new ApiError(result.error?.description ?? 'Erro inesperado')
  }
}
