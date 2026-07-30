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
      className={`rounded-xl border border-border-primary bg-bg-surface p-5 transition-all duration-200 ${
        hover
          ? 'cursor-pointer hover:-translate-y-0.5 hover:border-border-hover hover:bg-bg-hover hover:shadow-lg hover:shadow-black/20'
          : ''
      } ${onClick ? 'cursor-pointer' : ''} ${className ?? ''}`}
    >
      {children}
    </div>
  )
}
