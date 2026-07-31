import type { ApiResult } from '@/types/api'

export class ApiError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

function failureMessage(result: ApiResult<unknown>): string {
  if (result.validationErrors && result.validationErrors.length > 0) {
    return result.validationErrors[0].errorMessage
  }
  return result.error?.description ?? 'Erro inesperado'
}

export function unwrap<T>(result: ApiResult<T>): T {
  if (!result.isSuccess || result.value === null || result.value === undefined) {
    throw new ApiError(failureMessage(result))
  }
  return result.value
}

export function unwrapVoid(result: ApiResult<unknown>): void {
  if (!result.isSuccess) {
    throw new ApiError(failureMessage(result))
  }
}
