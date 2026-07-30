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
            className="text-sm font-medium text-gray-700"
          >
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          className={`h-10 w-full rounded-lg border px-3 text-sm text-gray-900 transition-all duration-150 placeholder:text-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:cursor-not-allowed disabled:bg-gray-50 disabled:text-gray-500 ${
            error
              ? 'border-red-400 focus:border-red-500 focus:ring-red-500'
              : 'border-gray-300 hover:border-gray-400'
          } ${className ?? ''}`}
          {...props}
        />
        {error && (
          <span className="flex items-center gap-1 text-xs text-red-500">
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
