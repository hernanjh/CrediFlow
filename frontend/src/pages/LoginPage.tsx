import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { authApi } from '../api/client'
import { useAuthStore } from '../store/authStore'

export default function LoginPage() {
  const navigate = useNavigate()
  const { login } = useAuthStore()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setError('')
    try {
      const res = await authApi.login(email, password)
      const { accessToken, refreshToken, usuario } = res.data
      login(accessToken, refreshToken, usuario)
      navigate('/dashboard')
    } catch (err: any) {
      setError(err.response?.data?.error || 'Credenciales inválidas. Verifique email y contraseña.')
    } finally {
      setLoading(false)
    }
  }

  const fillDemo = (role: string) => {
    const creds: Record<string, { email: string; password: string }> = {
      admin: { email: 'superadmin@crediflow.com', password: 'Admin123!' },
      vendedor: { email: 'carlos@crediflow.com', password: 'Vendedor123!' },
    }
    const c = creds[role]
    if (c) { setEmail(c.email); setPassword(c.password) }
  }

  const enterDemoMode = (role: 'admin' | 'vendedor') => {
    const mockUser = {
      id: 'demo-1',
      nombreCompleto: role === 'admin' ? 'Super Admin (Demo)' : 'Carlos Rodríguez (Demo)',
      email: role === 'admin' ? 'superadmin@crediflow.com' : 'carlos@crediflow.com',
      rol: (role === 'admin' ? 'SUPER_ADMIN' : 'VENDEDOR') as 'SUPER_ADMIN' | 'VENDEDOR',
      porcentajeComision: role === 'admin' ? 2 : 5,
    }
    login('demo-token', 'demo-refresh', mockUser)
    navigate('/dashboard')
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-logo">
          <div className="login-logo-icon">💸</div>
          <div className="login-title">CrediFlow</div>
          <div className="login-subtitle">Sistema de Gestión de Microcréditos</div>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              id="email"
              type="email"
              className="form-input"
              value={email}
              onChange={e => setEmail(e.target.value)}
              placeholder="usuario@crediflow.com"
              required
              autoFocus
            />
          </div>

          <div className="form-group">
            <label className="form-label">Contraseña</label>
            <input
              id="password"
              type="password"
              className="form-input"
              value={password}
              onChange={e => setPassword(e.target.value)}
              placeholder="••••••••"
              required
            />
          </div>

          {error && (
            <div className="alerta-item alerta-danger" style={{ marginBottom: 16 }}>
              <span className="alerta-icon">⚠️</span>
              {error}
            </div>
          )}

          <button
            id="btn-login"
            type="submit"
            className="btn btn-primary btn-full"
            disabled={loading}
            style={{ height: 44, fontSize: 14 }}
          >
            {loading ? <span className="spinner" /> : '🔐 Ingresar al Sistema'}
          </button>
        </form>

          <div style={{ marginTop: 24, paddingTop: 20, borderTop: '1px solid var(--color-border)' }}>
          <div style={{ fontSize: 11, color: 'var(--text-muted)', marginBottom: 10, textAlign: 'center' }}>
            Acceso directo (modo demo — sin backend)
          </div>
          <div style={{ display: 'flex', gap: 8 }}>
            <button
              className="btn btn-ghost btn-sm"
              style={{ flex: 1 }}
              onClick={() => enterDemoMode('admin')}
            >
              👑 SuperAdmin Demo
            </button>
            <button
              className="btn btn-ghost btn-sm"
              style={{ flex: 1 }}
              onClick={() => enterDemoMode('vendedor')}
            >
              🏃 Vendedor Demo
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
