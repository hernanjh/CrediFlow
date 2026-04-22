import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '../api/client'

interface ParamItem {
  id: string
  nombre: string
  activa: boolean
  simbolo?: string
  diasPlazo?: number
}

interface ParametricaPageProps {
  title: string
  endpoint: string
  hasSymbol?: boolean
  hasPlazo?: boolean
}

export default function ParametricaPage({ title, endpoint, hasSymbol, hasPlazo }: ParametricaPageProps) {
  const queryClient = useQueryClient()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<ParamItem | null>(null)
  const [formData, setFormData] = useState({ nombre: '', simbolo: '', diasPlazo: 0 })

  const { data: items, isLoading } = useQuery<ParamItem[]>({
    queryKey: [endpoint],
    queryFn: async () => {
      const res = await api.get(`/Parametricas/${endpoint}?soloActivas=false`)
      return res.data
    }
  })

  const saveMutation = useMutation({
    mutationFn: async (data: any) => {
      if (editingItem) {
        return api.put(`/Parametricas/${endpoint}/${editingItem.id}`, data)
      }
      return api.post(`/Parametricas/${endpoint}`, data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [endpoint] })
      setIsModalOpen(false)
      setEditingItem(null)
    }
  })

  const toggleMutation = useMutation({
    mutationFn: async ({ id, activa }: { id: string, activa: boolean }) => {
      if (activa) {
        return api.delete(`/Parametricas/${endpoint}/${id}`)
      }
      return api.post(`/Parametricas/${endpoint}/${id}/reactivar`)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [endpoint] })
    }
  })

  const handleEdit = (item: ParamItem) => {
    setEditingItem(item)
    setFormData({ 
      nombre: item.nombre, 
      simbolo: item.simbolo || '', 
      diasPlazo: item.diasPlazo || 0 
    })
    setIsModalOpen(true)
  }

  const handleAddNew = () => {
    setEditingItem(null)
    setFormData({ nombre: '', simbolo: '', diasPlazo: 0 })
    setIsModalOpen(true)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    saveMutation.mutate(formData)
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">{title}</h1>
          <p className="page-subtitle">Administración de parámetros del sistema</p>
        </div>
        <button className="btn btn-primary" onClick={handleAddNew}>
          <span>+</span> Nueva Entrada
        </button>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
          <div className="loading-overlay"><span className="spinner" /> Cargando parámetros...</div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Nombre</th>
                {hasSymbol && <th>Símbolo</th>}
                {hasPlazo && <th>Días Plazo</th>}
                <th>Estado</th>
                <th className="text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {items?.map(item => (
                <tr key={item.id} className={!item.activa ? 'row-inactive' : ''}>
                  <td className="font-medium">{item.nombre}</td>
                  {hasSymbol && <td>{item.simbolo}</td>}
                  {hasPlazo && <td>{item.diasPlazo}</td>}
                  <td>
                    <span className={`badge ${item.activa ? 'badge-success' : 'badge-danger'}`}>
                      {item.activa ? 'Activo' : 'Inactivo'}
                    </span>
                  </td>
                  <td className="text-right">
                    <button className="btn-icon" onClick={() => handleEdit(item)} title="Editar">
                      ✏️
                    </button>
                    <button 
                      className="btn-icon" 
                      onClick={() => toggleMutation.mutate({ id: item.id, activa: item.activa })}
                      title={item.activa ? 'Desactivar' : 'Reactivar'}
                    >
                      {item.activa ? '❌' : '✅'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {isModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content modal-sm">
            <div className="modal-header">
              <h3 className="modal-title">{editingItem ? 'Editar' : 'Nueva'} {title}</h3>
              <button className="modal-close" onClick={() => setIsModalOpen(false)}>&times;</button>
            </div>
            <div className="modal-body">
              <form onSubmit={handleSubmit} className="flex-column gap-4">
                <div className="form-group">
                  <label className="form-label">Nombre</label>
                  <input 
                    type="text" 
                    className="form-input"
                    value={formData.nombre}
                    onChange={e => setFormData({ ...formData, nombre: e.target.value })}
                    placeholder="Ej: Consumidor Final"
                    required
                  />
                </div>
                {hasSymbol && (
                  <div className="form-group">
                    <label className="form-label">Símbolo / Abreviatura</label>
                    <input 
                      type="text" 
                      className="form-input"
                      value={formData.simbolo}
                      onChange={e => setFormData({ ...formData, simbolo: e.target.value })}
                      placeholder="Ej: un, kg, lt"
                    />
                  </div>
                )}
                {hasPlazo && (
                  <div className="form-group">
                    <label className="form-label">Días de Plazo</label>
                    <input 
                      type="number" 
                      className="form-input"
                      value={formData.diasPlazo}
                      onChange={e => setFormData({ ...formData, diasPlazo: parseInt(e.target.value) })}
                    />
                  </div>
                )}
                <div className="modal-footer mt-6" style={{ margin: '0 -24px -24px -24px', borderRadius: '0 0 var(--radius-xl) var(--radius-xl)' }}>
                  <button type="button" className="btn btn-ghost" onClick={() => setIsModalOpen(false)}>
                    Cancelar
                  </button>
                  <button type="submit" className="btn btn-primary" disabled={saveMutation.isPending}>
                    {saveMutation.isPending ? 'Guardando...' : 'Guardar'}
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
