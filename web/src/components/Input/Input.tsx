import { forwardRef, type InputHTMLAttributes } from 'react'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, id, className, ...props }, ref) => {
    const inputId = id ?? (label ? label.toLowerCase().replace(/\s+/g, '-') : undefined)

    return (
      <div className="flex flex-col gap-1.5">
        {label && (
          <label
            htmlFor={inputId}
            className="text-sm font-medium text-text-secondary"
          >
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          className={`h-9 w-full rounded-lg border bg-bg-surface px-3 text-sm text-text-primary transition-all duration-150 placeholder:text-text-muted focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent/50 disabled:cursor-not-allowed disabled:opacity-40 ${
            error
              ? 'border-danger/50 focus:border-danger focus:ring-danger/30'
              : 'border-border-primary hover:border-border-hover'
          } ${className ?? ''}`}
          {...props}
        />
        {error && (
          <span className="flex items-center gap-1 text-xs text-danger">
            <svg className="h-3.5 w-3.5 shrink-0" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 1a7 7 0 100 14A7 7 0 008 1zM7 5a1 1 0 012 0v3a1 1 0 01-2 0V5zm1 7a1 1 0 110-2 1 1 0 010 2z" />
            </svg>
            {error}
          </span>
        )}
      </div>
    )
  },
)

Input.displayName = 'Input'
