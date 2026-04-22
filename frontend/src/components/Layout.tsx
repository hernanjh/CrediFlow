import React from 'react'

interface LayoutProps {
  title: string
  children: React.ReactNode
}

export default function Layout({ title, children }: LayoutProps) {
  return (
    <div className="page-layout">
      <div className="page-header mb-6">
        <h1 className="text-2xl font-bold text-white">{title}</h1>
      </div>
      <div className="page-content">
        {children}
      </div>
    </div>
  )
}
