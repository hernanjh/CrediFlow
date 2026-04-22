import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { zonasApi, seguridadApi } from '../api/client'
import { toast } from 'react-hot-toast'

export default function ZonasPage() {
  const queryClient = useQueryClient()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [showInactivos, setShowInactivos] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)

  const initialForm = {
      nombre: '', 
      descripcion: '',
      colorHex: '#3b82f6',
      vendedorId: ''
  }
  const [formData, setFormData] = useState(initialForm)

  const { data: zonas = [], isLoading } = useQuery({
    queryKey: ['zonas', showInactivos],
    queryFn: () => zonasApi.getAll(!showInactivos).then(r => r.data)
  })

  const { data: vendedores = [] } = useQuery({
    queryKey: ['vendedores'],
    queryFn: () => seguridadApi.getUsuarios().then(r => r.data.filter((u: any) => u.rol === 'VENDEDOR' || u.rol === 'ADMIN'))
  })

  const createMutation = useMutation({
    mutationFn: (data: typeof formData) => zonasApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['zonas'] })
      setIsModalOpen(false)
      toast.success('Zona registrada')
    }
  })

  const updateMutation = useMutation({
    mutationFn: (data: any) => zonasApi.update(data.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['zonas'] })
      setIsModalOpen(false)
      toast.success('Zona actualizada')
    }
  })

  const statusMutation = useMutation({
    mutationFn: ({id, op}: {id: string, op: 'reactivar'|'delete'}) => 
      op === 'delete' ? zonasApi.delete(id) : zonasApi.reactivar(id),
    onSuccess: (_, vars) => {
      queryClient.invalidateQueries({ queryKey: ['zonas'] })
      toast.success(vars.op === 'delete' ? 'Zona inactivada' : 'Zona reactivada')
    }
  })

  const handleEdit = (z: any) => {
    setEditingId(z.id)
    setFormData({ 
        nombre: z.nombre || '', 
        descripcion: z.descripcion || '',
        colorHex: z.colorHex || '#3b82f6',
        vendedorId: z.vendedorId || ''
    })
    setIsModalOpen(true)
  }

  const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault();
      if (editingId) updateMutation.mutate({...formData, id: editingId})
      else createMutation.mutate(formData)
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Áreas y Zonas de Operación</h1>
          <p className="page-subtitle">Organice la atención comercial y logística.</p>
        </div>
        <button onClick={() => { setEditingId(null); setFormData(initialForm); setIsModalOpen(true); }} className="btn btn-primary">
            + Nueva Zona
        </button>
      </div>

      <div className="filter-bar">
        <label className="flex-center" style={{ cursor: 'pointer', fontSize: '13px', color: 'var(--text-muted)' }}>
          <input type="checkbox" className="form-input" style={{ width: 18, height: 18, marginRight: 8 }} checked={showInactivos} onChange={e => setShowInactivos(e.target.checked)} />
          Ver zonas inactivas
        </label>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
          <div className="loading-overlay"><span className="spinner" /> Cargando zonas...</div>
        ) : zonas.length === 0 ? (
          <div className="empty-state">
              <span className="empty-state-icon">📍</span>
              <div className="empty-state-title">Directorio de Zonas Vacío</div>
              <div className="empty-state-subtitle">Cree su primera zona para asignar clientes.</div>
          </div>
        ) : (
          <table className="cierre-table">
            <thead>
              <tr>
                <th style={{ width: 60 }}>Color</th>
                <th>Nombre de la Zona</th>
                <th>Descripción / Detalles</th>
                <th style={{ textAlign: 'center' }}>Total Clientes</th>
                <th>Vendedor Asignado</th>
                <th style={{ textAlign: 'center' }}>Estado</th>
                <th style={{ textAlign: 'right' }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {zonas.map((z: any) => (
                <tr key={z.id} style={{ opacity: z.activa ? 1 : 0.5 }}>
                  <td style={{ textAlign: 'center' }}>
                      <div style={{ width: 20, height: 20, borderRadius: '50%', background: z.colorHex || '#bbb', margin: '0 auto', border: '2px solid rgba(0,0,0,0.1)' }}></div>
                  </td>
                  <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{z.nombre}</td>
                  <td style={{ color: 'var(--text-muted)' }}>{z.descripcion || '---'}</td>
                  <td style={{ textAlign: 'center', fontWeight: 600 }}>{z.cantidadClientes || 0}</td>
                  <td>
                      <div className="flex items-center gap-2">
                        <span className="text-sm font-medium">{z.vendedorNombre || 'Sin Asignar'}</span>
                      </div>
                  </td>
                  <td style={{ textAlign: 'center' }}>
                    <span className={`badge ${z.activa ? 'badge-success' : 'badge-danger'}`}>
                      {z.activa ? 'ACTIVA' : 'INACTIVA'}
                    </span>
                  </td>
                  <td style={{ textAlign: 'right' }}>
                    <button onClick={() => handleEdit(z)} className="btn btn-ghost btn-sm">Editar</button>
                    {z.activa ? (
                        <button onClick={() => { if(confirm('¿Desactivar zona?')) statusMutation.mutate({id: z.id, op: 'delete'}) }} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-danger)' }}>Desactivar</button>
                    ) : (
                        <button onClick={() => statusMutation.mutate({id: z.id, op: 'reactivar'})} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-success)' }}>Reactivar</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {isModalOpen && (
        <div className="modal-backdrop">
          <div className="modal">
            <div className="modal-header">
                <div>
                     <h2 className="modal-title">{editingId ? 'Editar Zona' : 'Nueva Zona'}</h2>
                     <p className="modal-subtitle">Configure la sectorización.</p>
                </div>
                <button className="modal-close" onClick={() => setIsModalOpen(false)}>×</button>
            </div>
            
            <div className="modal-body">
              <form id="zonaForm" onSubmit={handleSubmit} className="form-grid">
                
                <div className="mb-4">
                    <label className="form-label">Nombre de la Zona *</label>
                    <input required className="form-input" placeholder="Ej: Zona Norte" value={formData.nombre} onChange={e => setFormData({...formData, nombre: e.target.value})} />
                </div>
                <div className="mb-4">
                    <label className="form-label">Descripción</label>
                    <textarea className="form-input" rows={2} value={formData.descripcion} onChange={e => setFormData({...formData, descripcion: e.target.value})} />
                </div>
                <div className="mb-4">
                    <label className="form-label">Color Distintivo</label>
                    <input type="color" style={{ width: 80, height: 40, padding: 0, border: 'none', background: 'transparent' }} value={formData.colorHex} onChange={e => setFormData({...formData, colorHex: e.target.value})} />
                </div>
                <div className="mb-4">
                    <label className="form-label">Vendedor Responsable</label>
                    <select className="form-input" value={formData.vendedorId} onChange={e => setFormData({...formData, vendedorId: e.target.value})}>
                        <option value="">-- Sin Vendedor Asignado --</option>
                        {vendedores.map((v: any) => (
                            <option key={v.id} value={v.id}>{v.nombreCompleto}</option>
                        ))}
                    </select>
                </div>
              </form>
            </div>
            <div className="modal-footer">
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-ghost">Cancelar</button>
                <button type="submit" form="zonaForm" className="btn btn-primary" disabled={createMutation.isPending || updateMutation.isPending}>
                  {editingId ? 'Guardar Cambios' : 'Registrar Zona'}
                </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
