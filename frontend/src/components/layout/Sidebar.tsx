import { NavLink, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../../store/authStore'

const menuItems = [
  { path: '/dashboard', icon: '📊', label: 'Dashboard', section: 'Principal' },
  
  { path: '/clientes', icon: '👥', label: 'Clientes', section: 'Operaciones' },
  { path: '/creditos', icon: '💳', label: 'Créditos', section: 'Operaciones' },
  { path: '/cobranza', icon: '🗺️', label: 'Cobranza', section: 'Operaciones' },
  { path: '/ventas', icon: '💰', label: 'Ventas', section: 'Operaciones' },
  
  { path: '/inventario', icon: '📦', label: 'Productos', section: 'Productos' },
  { path: '/proveedores', icon: '🚛', label: 'Proveedores', section: 'Productos' },
  { path: '/tarifario', icon: '🏷️', label: 'Lista Precios', section: 'Productos' },
  
  { path: '/reportes', icon: '📈', label: 'Reportes BI', section: 'Análisis' },
  
  { path: '/zonas', icon: '📍', label: 'Zonas', section: 'Parametrización' },
  { path: '/param/sectores', icon: '🏢', label: 'Sectores / Ocupac.', section: 'Parametrización' },
  { path: '/param/tipos-cliente', icon: '🏷️', label: 'Tipos Cliente', section: 'Parametrización' },
  { path: '/param/formas-cobro', icon: '💸', label: 'Formas Cobro', section: 'Parametrización' },
  { path: '/param/categorias', icon: '🗂️', label: 'Categorías (Prod)', section: 'Parametrización' },
  { path: '/param/cond-iva', icon: '🏛️', label: 'Condición IVA', section: 'Parametrización' },
  { path: '/param/cond-pago', icon: '📅', label: 'Condición Pago', section: 'Parametrización' },
  { path: '/param/unidades', icon: '📏', label: 'Unidades Medida', section: 'Parametrización' },
  { path: '/configuracion', icon: '⚙️', label: 'Config. Sistema', section: 'Parametrización' },

  { path: '/usuarios', icon: '🔐', label: 'Usuarios / Roles', section: 'Seguridad' },
]

const sections = ['Principal', 'Operaciones', 'Productos', 'Análisis', 'Parametrización', 'Seguridad']

export default function Sidebar() {
  const { usuario, logout } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const getInitials = (name: string) =>
    name.split(' ').slice(0, 2).map(n => n[0]).join('').toUpperCase()

  const getRolLabel = (rol: string) =>
    ({ SUPER_ADMIN: 'Super Admin', ADMIN: 'Administrador', VENDEDOR: 'Vendedor' }[rol] || rol)

  return (
    <nav className="sidebar">
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">💸</div>
        <div>
          <div className="sidebar-logo-text">CrediFlow</div>
          <div className="sidebar-logo-sub">Sistema de Microcréditos</div>
        </div>
      </div>

      <div className="sidebar-nav">
        {sections.map(section => {
          const items = menuItems.filter(m => m.section === section)
          if (!items.length) return null
          return (
            <div key={section}>
              <div className="sidebar-section-label">{section}</div>
              {items.map(item => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({ isActive }) =>
                    `sidebar-item${isActive ? ' active' : ''}`}
                >
                  <span className="sidebar-item-icon">{item.icon}</span>
                  {item.label}
                </NavLink>
              ))}
            </div>
          )
        })}
      </div>

      <div className="sidebar-footer">
        <div className="sidebar-user" onClick={handleLogout} title="Cerrar sesión">
          <div className="sidebar-avatar">
            {usuario ? getInitials(usuario.nombreCompleto) : 'CF'}
          </div>
          <div>
            <div className="sidebar-user-name">
              {usuario?.nombreCompleto || 'Usuario'}
            </div>
            <div className="sidebar-user-role">
              {usuario ? getRolLabel(usuario.rol) : ''}
            </div>
          </div>
        </div>
      </div>
    </nav>
  )
}
