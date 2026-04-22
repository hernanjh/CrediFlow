import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { seguridadApi, extendAuthInfo } from '../api/client'
import { toast } from 'react-hot-toast'

export default function UsuariosPage() {
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState<'usuarios' | 'perfiles'>('usuarios')
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingUsuario, setEditingUsuario] = useState<any>(null)

  const initialForm = {
    nombreCompleto: '',
    email: '',
    password: '',
    rol: 'VENDEDOR',
    perfilId: '',
    porcentajeComision: 0
  }
  const [formData, setFormData] = useState(initialForm)

  // Queries
  const { data: usuarios = [], isLoading: loadUsuarios } = useQuery({ 
    queryKey: ['usuarios'], 
    queryFn: () => seguridadApi.getUsuarios().then(r => r.data) 
  })
  const { data: perfiles = [], isLoading: loadPerfiles } = useQuery({ 
    queryKey: ['perfiles'], 
    queryFn: () => seguridadApi.getPerfiles().then(r => r.data) 
  })
  const { data: permisos = [] } = useQuery({ 
    queryKey: ['permisos'], 
    queryFn: () => seguridadApi.getPermisos().then(r => r.data) 
  })

  const upsertMutation = useMutation({
    mutationFn: (data: any) => {
      if (editingUsuario) {
        return seguridadApi.updateUsuario(editingUsuario.id, data)
      }
      return extendAuthInfo.register(data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] })
      setIsModalOpen(false)
      toast.success(editingUsuario ? 'Usuario actualizado con éxito' : 'Nueva cuenta creada')
      setEditingUsuario(null)
      setFormData(initialForm)
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Error en la operación')
  })

  const reactivateMutation = useMutation({
    mutationFn: (id: string) => seguridadApi.reactivarUsuario(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] })
      toast.success('Acceso reactivado')
    }
  })

  const handleEdit = (u: any) => {
    setEditingUsuario(u)
    setFormData({
      nombreCompleto: u.nombreCompleto,
      email: u.email,
      password: '',
      rol: u.rol,
      perfilId: u.perfilId || '',
      porcentajeComision: u.porcentajeComision || 0
    })
    setIsModalOpen(true)
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Centro de Gestión de Usuarios</h1>
          <p className="page-subtitle">Control de accesos, roles operativos y perfiles de seguridad</p>
        </div>
        <button className="btn btn-primary" onClick={() => { setEditingUsuario(null); setFormData(initialForm); setIsModalOpen(true); }}>
          + Generar Nueva Cuenta
        </button>
      </div>

      <div className="tabs mb-8">
        <button onClick={() => setActiveTab('usuarios')} className={`tab-btn ${activeTab === 'usuarios' ? 'active' : ''}`}>
          👥 Cuentas de Usuario
        </button>
        <button onClick={() => setActiveTab('perfiles')} className={`tab-btn ${activeTab === 'perfiles' ? 'active' : ''}`}>
          ⚖️ Perfiles de Seguridad
        </button>
      </div>

      {activeTab === 'usuarios' ? (
        <div className="data-table-wrap">
          {loadUsuarios ? (
            <div className="loading-overlay"><span className="spinner" /> Sincronizando usuarios...</div>
          ) : (
            <table className="data-table">
              <thead>
                <tr>
                  <th>Usuario y Contacto</th>
                  <th>Rol Sistémico</th>
                  <th>Perfil de Permisos</th>
                  <th className="text-center">Estado</th>
                  <th className="text-right">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {usuarios.map((u: any) => (
                  <tr key={u.id} className={!u.activo ? 'row-inactive' : ''}>
                    <td>
                      <div className="flex-center gap-4" style={{ justifyContent: 'flex-start' }}>
                        <div className="sidebar-avatar" style={{ width: '36px', height: '36px', fontSize: '12px' }}>
                          {u.nombreCompleto?.substring(0,2).toUpperCase()}
                        </div>
                        <div>
                          <div className="font-bold text-primary">{u.nombreCompleto}</div>
                          <div className="text-xs text-muted font-mono">{u.email}</div>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span className={`badge ${u.rol === 'SUPER_ADMIN' ? 'badge-primary' : 'badge-neutral'}`}>
                        {u.rol}
                      </span>
                    </td>
                    <td>
                      {u.perfilNombre ? (
                        <span className="badge badge-info">{u.perfilNombre}</span>
                      ) : (
                        <span className="text-xs italic text-muted">Acceso Estándar</span>
                      )}
                    </td>
                    <td className="text-center">
                      <span className={`badge ${u.activo ? 'badge-success' : 'badge-danger'}`}>
                        {u.activo ? 'Activo' : 'Baja'}
                      </span>
                    </td>
                    <td className="text-right">
                      <div className="flex-gap-2" style={{ justifyContent: 'flex-end' }}>
                        <button className="btn-icon" onClick={() => handleEdit(u)} title="Editar información">✏️</button>
                        {!u.activo && (
                          <button className="btn-icon text-success" onClick={() => reactivateMutation.mutate(u.id)} title="Reactivar acceso">✅</button>
                        )}
                        <button className="btn-icon text-warning" title="Resetear contraseña">🔑</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      ) : (
        <PerfilesSection permisos={permisos} perfiles={perfiles} queryClient={queryClient} />
      )}

      {isModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content modal-md">
            <div className="modal-header">
              <h3 className="modal-title">{editingUsuario ? 'Editar Credenciales' : 'Registro de Operador'}</h3>
              <button className="modal-close" onClick={() => setIsModalOpen(false)}>&times;</button>
            </div>
            <div className="modal-body">
              <form className="flex-column gap-6" onSubmit={(e) => { e.preventDefault(); upsertMutation.mutate(formData); }}>
                <div className="grid-2 gap-4">
                  <div className="form-group">
                    <label className="form-label">Nombre Completo</label>
                    <input required className="form-input" value={formData.nombreCompleto} onChange={e => setFormData({ ...formData, nombreCompleto: e.target.value })} placeholder="Ej: Juan Pérez" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Email de Acceso</label>
                    <input required type="email" className="form-input" value={formData.email} onChange={e => setFormData({ ...formData, email: e.target.value })} placeholder="juan@crediflow.com" />
                  </div>
                </div>

                <div className="grid-2 gap-4">
                  <div className="form-group">
                    <label className="form-label">{editingUsuario ? 'Clave Nueva (Opcional)' : 'Contraseña'}</label>
                    <input required={!editingUsuario} type="password" className="form-input" value={formData.password} onChange={e => setFormData({ ...formData, password: e.target.value })} placeholder="••••••••" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Rol Sistémico</label>
                    <select className="form-input" value={formData.rol} onChange={e => setFormData({ ...formData, rol: e.target.value })}>
                      <option value="ADMIN">Administrador</option>
                      <option value="VENDEDOR">Cobrador / Vendedor</option>
                      <option value="SUPER_ADMIN">System Root</option>
                    </select>
                  </div>
                </div>

                <div className="grid-2 gap-4">
                  <div className="form-group">
                    <label className="form-label">Perfil Detallado</label>
                    <select className="form-input" value={formData.perfilId} onChange={e => setFormData({ ...formData, perfilId: e.target.value })}>
                      <option value="">-- Sin Perfil Personalizado --</option>
                      {perfiles.map((p: any) => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                    </select>
                  </div>
                  {formData.rol === 'VENDEDOR' && (
                    <div className="form-group">
                      <label className="form-label">% Comisión de Cobro</label>
                      <input type="number" step="0.1" className="form-input" value={formData.porcentajeComision} onChange={e => setFormData({ ...formData, porcentajeComision: Number(e.target.value) })} />
                    </div>
                  )}
                </div>

                <div className="modal-footer mt-6" style={{ margin: '0 -24px -24px -24px' }}>
                  <button type="button" className="btn btn-ghost" onClick={() => setIsModalOpen(false)}>Descartar</button>
                  <button type="submit" className="btn btn-primary" disabled={upsertMutation.isPending}>
                    {upsertMutation.isPending ? 'Guardando...' : 'Confirmar Usuario'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

function PerfilesSection({ permisos, perfiles, queryClient }: any) {
  const [newPerfil, setNewPerfil] = useState({ nombre: '', descripcion: '', permisosIds: [] as string[] })

  const createMutation = useMutation({
    mutationFn: (data: any) => seguridadApi.createPerfil(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['perfiles'] })
      toast.success('Nuevo perfil configurado')
      setNewPerfil({ nombre: '', descripcion: '', permisosIds: [] })
    }
  })

  return (
    <div className="grid-2" style={{ gridTemplateColumns: '440px 1fr', gap: '32px', alignItems: 'start' }}>
      <div className="card border-primary-glow">
        <h3 className="font-bold mb-6">Crear Perfil Académico</h3>
        <div className="flex-column gap-5">
          <div className="form-group">
            <label className="form-label">Nombre del Perfil</label>
            <input className="form-input" value={newPerfil.nombre} onChange={e => setNewPerfil({ ...newPerfil, nombre: e.target.value })} placeholder="Ej: Supervisor de Zona" />
          </div>
          <div className="form-group">
            <label className="form-label">Asignación de Permisos</label>
            <div className="data-table-wrap overflow-auto" style={{ maxHeight: '400px' }}>
              <table className="data-table" style={{ fontSize: '11px' }}>
                <tbody>
                  {permisos.map((p: any) => (
                    <tr key={p.id}>
                      <td style={{ width: '40px' }}>
                        <input type="checkbox" checked={newPerfil.permisosIds.includes(p.id)} onChange={() => {
                          const ids = newPerfil.permisosIds.includes(p.id)
                            ? newPerfil.permisosIds.filter(x => x !== p.id)
                            : [...newPerfil.permisosIds, p.id]
                          setNewPerfil({ ...newPerfil, permisosIds: ids })
                        }} />
                      </td>
                      <td>
                        <div className="font-bold">{p.nombre}</div>
                        <div className="text-muted">{p.modulo}</div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
          <button className="btn btn-primary w-full py-4 mt-2" onClick={() => createMutation.mutate(newPerfil)} disabled={!newPerfil.nombre}>
            Implementar Perfil
          </button>
        </div>
      </div>

      <div className="grid-2 gap-6" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))' }}>
        {perfiles.map((p: any) => (
          <div key={p.id} className="card hover-shadow">
            <div className="flex-between mb-4">
              <h4 className="font-extrabold text-primary">{p.nombre}</h4>
              <span className={`badge ${p.activa ? 'badge-success' : 'badge-danger'}`}>{p.activa ? 'Activo' : 'Inactivo'}</span>
            </div>
            <p className="text-xs text-muted mb-6 leading-relaxed">{p.descripcion || 'Perfil operativo con acceso modular granular.'}</p>
            <div className="flex flex-wrap gap-2 mb-6">
              {p.permisos?.slice(0, 4).map((perm: any) => (
                <span key={perm.id} className="badge badge-neutral text-[10px]">{perm.nombre}</span>
              ))}
              {(p.permisos?.length || 0) > 4 && <span className="badge badge-neutral text-[10px]">+{p.permisos.length - 4} más</span>}
            </div>
            <button className="btn btn-ghost btn-full btn-sm">Editar Permisos</button>
          </div>
        ))}
        {perfiles.length === 0 && (
          <div className="col-span-2 empty-state">
            <span className="empty-state-icon">🛡️</span>
            <p>No hay perfiles personalizados definidos.</p>
          </div>
        )}
      </div>
    </div>
  )
}

