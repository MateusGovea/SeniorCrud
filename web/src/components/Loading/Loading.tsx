interface LoadingProps {
  size?: 'sm' | 'md' | 'lg'
  className?: string
  fullPage?: boolean
}

const sizeStyles: Record<NonNullable<LoadingProps['size']>, string> = {
  sm: 'h-4 w-4',
  md: 'h-7 w-7',
  lg: 'h-10 w-10',
}

const containerStyles: Record<NonNullable<LoadingProps['size']>, string> = {
  sm: 'py-8',
  md: 'py-16',
  lg: 'py-24',
}

export function Loading({ size = 'md', className, fullPage }: LoadingProps) {
  if (fullPage) {
    return (
      <div className="flex flex-col items-center justify-center gap-3 py-20">
        <svg
          className={`animate-spin text-accent ${sizeStyles[size]}`}
          viewBox="0 0 24 24"
          fill="none"
        >
          <circle
            className="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            strokeWidth="4"
          />
          <path
            className="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
          />
        </svg>
        <p className="text-sm text-text-muted">Carregando...</p>
      </div>
    )
  }

  return (
    <div className={`flex items-center justify-center ${containerStyles[size]} ${className ?? ''}`}>
      <svg
        className={`animate-spin text-accent ${sizeStyles[size]}`}
        viewBox="0 0 24 24"
        fill="none"
      >
        <circle
          className="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          strokeWidth="4"
        />
        <path
          className="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
        />
      </svg>
    </div>
  )
}
