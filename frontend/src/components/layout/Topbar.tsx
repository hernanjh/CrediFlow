import { useLocation } from 'react-router-dom'
import { useState, useEffect } from 'react'

const pageTitles: Record<string, { title: string; subtitle: string }> = {
  '/dashboard': { title: 'Dashboard', subtitle: 'Resumen ejecutivo en tiempo real' },
  '/clientes': { title: 'Gestión de Clientes', subtitle: 'ABM de clientes y zonas' },
  '/creditos': { title: 'Créditos y Cuotas', subtitle: 'Motor de cuotas y seguimiento' },
  '/cobranza': { title: 'Hoja de Ruta', subtitle: 'Cobranza domiciliaria diaria' },
  '/inventario': { title: 'Inventario', subtitle: 'Stock y productos' },
  '/reportes': { title: 'Reportes BI', subtitle: 'Análisis financiero y de mora' },
}

export default function Topbar() {
  const { pathname } = useLocation()
  const page = pageTitles[pathname] || { title: 'CrediFlow', subtitle: '' }
  const [isOnline, setIsOnline] = useState(navigator.onLine)

  useEffect(() => {
    const handleOnline = () => setIsOnline(true)
    const handleOffline = () => setIsOnline(false)
    window.addEventListener('online', handleOnline)
    window.addEventListener('offline', handleOffline)
    return () => {
      window.removeEventListener('online', handleOnline)
      window.removeEventListener('offline', handleOffline)
    }
  }, [])

  const now = new Date()
  const dateStr = now.toLocaleDateString('es-AR', {
    weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
  })
  const dateFormatted = dateStr.charAt(0).toUpperCase() + dateStr.slice(1)

  return (
    <header className="topbar">
      <div>
        <div className="topbar-title">{page.title}</div>
        <div className="topbar-subtitle">{dateFormatted}</div>
      </div>
      <div className="topbar-right">
        <div className="status-badge" style={{ background: isOnline ? 'var(--color-surface-2)' : '#411317', color: isOnline ? 'var(--text-primary)' : '#ff6b6b' }}>
          <span className="status-dot" style={{ background: isOnline ? 'var(--color-success)' : 'var(--color-danger)' }} />
          {isOnline ? 'Sistema activo' : 'MODO OFFLINE'}
        </div>
      </div>
    </header>
  )
}

