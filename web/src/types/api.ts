export interface ApiError {
  code: string
  description: string
}

export interface ApiValidationError {
  propertyName: string
  errorMessage: string
}

export interface ApiResult<T> {
  isSuccess: boolean
  value: T | null
  error: ApiError | null
  validationErrors: ApiValidationError[] | null
}
