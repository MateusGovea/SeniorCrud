import type { ReactNode } from 'react'

interface CardProps {
  children: ReactNode
  className?: string
  hover?: boolean
  onClick?: () => void
}

export function Card({ children, className, hover, onClick }: CardProps) {
  return (
    <div
      onClick={onClick}
      className={`rounded-xl border border-gray-200 bg-white p-6 shadow-sm transition-all duration-150 ${
        hover
          ? 'cursor-pointer hover:-translate-y-0.5 hover:border-gray-300 hover:shadow-md'
          : ''
      } ${onClick ? 'cursor-pointer' : ''} ${className ?? ''}`}
    >
      {children}
    </div>
  )
}
