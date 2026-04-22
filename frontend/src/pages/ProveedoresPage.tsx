import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { proveedoresApi, parametricasApi } from '../api/client'
import { toast } from 'react-hot-toast'

function ProveedoresPage() {
  const queryClient = useQueryClient()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [showInactivos, setShowInactivos] = useState(false)
  const [search, setSearch] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)

  const initialForm = {
      razonSocial: '', 
      nombreFantasia: '',
      cuit: '', 
      contactoPrincipal: '',
      telefono: '', 
      celular: '',
      email: '', 
      web: '',
      direccion: '',
      localidad: '',
      provincia: '',
      condicionPago: '',
      condicionIVA: '',
      notas: ''
  }
  const [formData, setFormData] = useState(initialForm)
  const [activeTab, setActiveTab] = useState('general')

  const { data: proveedores = [], isLoading } = useQuery({
    queryKey: ['proveedores', showInactivos, search],
    queryFn: () => proveedoresApi.getAll({ soloActivos: !showInactivos, search }).then(r => r.data)
  })

  // -- Parametricas Fetch
  const { data: condicionesIva = [] } = useQuery({ queryKey: ['iva'], queryFn: () => parametricasApi.getCondicionesIva(true).then(r => r.data) })
  const { data: condicionesPago = [] } = useQuery({ queryKey: ['pago'], queryFn: () => parametricasApi.getCondicionesPago(true).then(r => r.data) })

  const createMutation = useMutation({
    mutationFn: (data: typeof formData) => proveedoresApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      setIsModalOpen(false)
      toast.success('Proveedor registrado')
      setFormData(initialForm)
    }
  })

  const updateMutation = useMutation({
    mutationFn: (data: any) => proveedoresApi.update(data.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      setIsModalOpen(false)
      setEditingId(null)
      toast.success('Proveedor actualizado')
    }
  })

  const reactivateMutation = useMutation({
    mutationFn: (id: string) => proveedoresApi.reactivar(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      toast.success('Proveedor reactivado')
    }
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => proveedoresApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      toast.success('Proveedor dado de baja')
    }
  })

  const handleEdit = (p: any) => {
    setEditingId(p.id)
    setFormData({ 
        razonSocial: p.razonSocial || '', 
        nombreFantasia: p.nombreFantasia || '',
        cuit: p.cuit || '', 
        contactoPrincipal: p.contactoPrincipal || '',
        telefono: p.telefono || '', 
        celular: p.celular || '',
        email: p.email || '', 
        web: p.web || '',
        direccion: p.direccion || '',
        localidad: p.localidad || '',
        provincia: p.provincia || '',
        condicionPago: p.condicionPago || '',
        condicionIVA: p.condicionIVA || '',
        notas: p.notas || ''
    })
    setActiveTab('general')
    setIsModalOpen(true)
  }

  const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault();
      if (editingId) {
          updateMutation.mutate({...formData, id: editingId})
      } else {
          createMutation.mutate(formData)
      }
  }

  const handleExportExcel = () => {
      const data = proveedores.map((c: any) => ({
          'Razón Social': c.razonSocial,
          'Fantasía': c.nombreFantasia || '',
          'CUIT': c.cuit || '',
          'Contacto': c.contactoPrincipal || '',
          'Teléfono': c.celular || c.telefono || '',
          'Email': c.email || '',
          'Localidad': c.localidad || '',
          'Condición IVA': c.condicionIVA || '',
          'Estado': c.activo ? 'ACTIVA' : 'INACTIVA'
      }));
      import('../utils/exportUtils').then(u => u.exportToExcel(data, `Proveedores_${new Date().toISOString().split('T')[0]}`));
  }

  const handleExportPDF = () => {
      const headers = ['CUIT', 'Razón Social', 'Fantasía', 'Contacto / Tel.', 'Localidad', 'Estado'];
      const data = proveedores.map((c: any) => [
          c.cuit,
          c.razonSocial,
          c.nombreFantasia || '---',
          `${c.contactoPrincipal || ''} - ${c.celular || c.telefono || ''}`,
          c.localidad,
          c.activo ? 'ACTIVO' : 'INACTIVO'
      ]);
      import('../utils/exportUtils').then(u => u.exportToPDF(headers, data, `Directorio de Proveedores`, `Proveedores_${new Date().toISOString().split('T')[0]}`));
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Directorio de Proveedores</h1>
          <p className="page-subtitle">Contactos, cuentas y condiciones referenciales.</p>
        </div>
        <button onClick={() => { setEditingId(null); setFormData(initialForm); setIsModalOpen(true); }} className="btn btn-primary">
            + Nuevo Proveedor
        </button>
      </div>

      <div className="filter-bar">
        <div className="search-input-wrap">
          <span className="search-icon">🔍</span>
          <input 
              type="text" 
              className="form-input" 
              placeholder="Buscar por Razón Social o CUIT..." 
              value={search} 
              onChange={e => setSearch(e.target.value)} 
          />
        </div>
        <div style={{flex: 1}}></div>
        <label className="flex-center" style={{ cursor: 'pointer', fontSize: '12px', color: 'var(--text-muted)' }}>
          <input type="checkbox" className="form-input" style={{ width: 18, height: 18, marginRight: 8 }} checked={showInactivos} onChange={e => setShowInactivos(e.target.checked)} />
          Mostrar proveedores inactivos
        </label>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportExcel} disabled={proveedores.length === 0}>
            XLSX
        </button>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportPDF} disabled={proveedores.length === 0}>
            PDF
        </button>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
          <div className="loading-overlay"><span className="spinner" /> Cargando directorio...</div>
        ) : proveedores.length === 0 ? (
          <div className="empty-state">
              <span className="empty-state-icon">🏢</span>
              <div className="empty-state-title">No hay proveedores</div>
              <div className="empty-state-subtitle">No se encontraron resultados en la base de datos de proveedores.</div>
          </div>
        ) : (
          <table className="cierre-table">
            <thead>
              <tr>
                <th>Razón Social / Fantasía</th>
                <th>CUIT / IVA</th>
                <th>Contacto Directo</th>
                <th>Datos de Cobranza</th>
                <th style={{ textAlign: 'center' }}>Estado</th>
                <th style={{ textAlign: 'right' }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {proveedores?.map((p: any) => (
                <tr key={p.id} style={{ opacity: p.activo ? 1 : 0.5 }}>
                  <td>
                    <div style={{ color: 'var(--text-primary)', fontWeight: '600' }}>{p.razonSocial}</div>
                    <div style={{ fontSize: '11px', color: 'var(--text-muted)' }}>{p.nombreFantasia ? `Fantasia: ${p.nombreFantasia}` : ''}</div>
                  </td>
                  <td>
                    <div style={{ fontWeight: 500, fontSize: 13 }}>{p.cuit || '---'}</div>
                    <div style={{ fontSize: 11, color: 'var(--text-secondary)' }}>{p.condicionIVA || 'No espe.'}</div>
                  </td>
                  <td>
                    <div style={{ fontSize: 13 }}>{p.contactoPrincipal || '---'}</div>
                    <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>{p.celular || p.telefono || 'Sin tel.'} - {p.email}</div>
                  </td>
                  <td>
                    <div style={{ fontSize: 12 }}>{p.condicionPago || 'Standar / A convenir'}</div>
                  </td>
                  <td style={{ textAlign: 'center' }}>
                    <span className={`badge ${p.activo ? 'badge-success' : 'badge-danger'}`}>
                      {p.activo ? 'ACTIVO' : 'INACTIVO'}
                    </span>
                  </td>
                  <td style={{ textAlign: 'right', whiteSpace: 'nowrap' }}>
                    <button onClick={() => handleEdit(p)} className="btn btn-ghost btn-sm">Editar</button>
                    {p.activo ? (
                        <button onClick={() => { if(confirm('Dar de baja proveedor?')) deleteMutation.mutate(p.id) }} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-danger)', marginLeft: 8 }}>Inactivar</button>
                    ) : (
                        <button onClick={() => reactivateMutation.mutate(p.id)} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-success)', marginLeft: 8 }}>Reactivar</button>
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
          <div className="modal modal-lg">
            <div className="modal-header">
                <div>
                     <h2 className="modal-title">{editingId ? 'Editar Ficha Proveedor' : 'Nuevo Proveedor'}</h2>
                     <p className="modal-subtitle">Complete la ficha de la empresa proveedora de mercadería.</p>
                </div>
                <button className="modal-close" onClick={() => setIsModalOpen(false)}>×</button>
            </div>
            
            <div className="modal-body">
              <div className="tabs">
                  <button className={`tab-btn ${activeTab === 'general' ? 'active' : ''}`} onClick={() => setActiveTab('general')}>Datos Generales</button>
                  <button className={`tab-btn ${activeTab === 'contacto' ? 'active' : ''}`} onClick={() => setActiveTab('contacto')}>Contacto</button>
                  <button className={`tab-btn ${activeTab === 'comercial' ? 'active' : ''}`} onClick={() => setActiveTab('comercial')}>Comercial</button>
              </div>

              <form id="proveedorForm" onSubmit={handleSubmit} className="form-grid">
                
                {/* TABS */}
                <div style={{ display: activeTab === 'general' ? 'block' : 'none' }}>
                    <div className="grid-2 mb-4">
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Razón Social *</label>
                            <input required className="form-input" placeholder="Nombre legal de la empresa" value={formData.razonSocial} onChange={e => setFormData({...formData, razonSocial: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Nombre de Fantasía</label>
                            <input className="form-input" placeholder="Opcional" value={formData.nombreFantasia} onChange={e => setFormData({...formData, nombreFantasia: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">CUIT *</label>
                            <input required className="form-input" placeholder="30-12345678-9" value={formData.cuit} onChange={e => setFormData({...formData, cuit: e.target.value})} />
                        </div>
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Condición de I.V.A.</label>
                            <select className="form-input" value={formData.condicionIVA} onChange={e => setFormData({...formData, condicionIVA: e.target.value})}>
                                <option value="">Seleccionar...</option>
                                {condicionesIva.map((c:any) => <option key={c.id} value={c.nombre}>{c.nombre}</option>)}
                            </select>
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'contacto' ? 'block' : 'none' }}>
                    <div className="grid-2 mb-4">
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Contacto Principal (Nombre del Vendedor)</label>
                            <input className="form-input" value={formData.contactoPrincipal} onChange={e => setFormData({...formData, contactoPrincipal: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Teléfono Administrativo</label>
                            <input className="form-input" placeholder="011-XXXX-XXXX" value={formData.telefono} onChange={e => setFormData({...formData, telefono: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Celular / WhatsApp</label>
                            <input className="form-input" placeholder="11-XXXX-XXXX" value={formData.celular} onChange={e => setFormData({...formData, celular: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Email Oficial</label>
                            <input className="form-input" type="email" value={formData.email} onChange={e => setFormData({...formData, email: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Sitio Web</label>
                            <input className="form-input" type="url" placeholder="https://" value={formData.web} onChange={e => setFormData({...formData, web: e.target.value})} />
                        </div>
                        
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Dirección / Depósito</label>
                            <input className="form-input" value={formData.direccion} onChange={e => setFormData({...formData, direccion: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Localidad</label>
                            <input className="form-input" value={formData.localidad} onChange={e => setFormData({...formData, localidad: e.target.value})} />
                        </div>
                        <div>
                            <label className="form-label">Provincia</label>
                            <input className="form-input" value={formData.provincia} onChange={e => setFormData({...formData, provincia: e.target.value})} />
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'comercial' ? 'block' : 'none' }}>
                     <div className="grid-2 mb-4">
                         <div style={{ gridColumn: 'span 2' }}>
                             <label className="form-label">Condiciones de Pago Habituales</label>
                             <select className="form-input" value={formData.condicionPago} onChange={e => setFormData({...formData, condicionPago: e.target.value})}>
                                 <option value="">A convenir / Otra</option>
                                 {condicionesPago.map((c:any) => <option key={c.id} value={c.nombre}>{c.nombre} ({c.diasPlazo} días)</option>)}
                             </select>
                         </div>
                         <div style={{ gridColumn: 'span 2' }}>
                             <label className="form-label">Notas Adicionales sobre el Proveedor</label>
                             <textarea className="form-input" rows={4} value={formData.notas} onChange={e => setFormData({...formData, notas: e.target.value})}></textarea>
                         </div>
                     </div>
                </div>

              </form>
            </div>
            <div className="modal-footer">
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn btn-ghost">Cancelar</button>
                <button type="submit" form="proveedorForm" className="btn btn-primary" disabled={createMutation.isPending || updateMutation.isPending}>
                  {editingId ? 'Actualizar Ficha de Proveedor' : 'Guardar Proveedor'}
                </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

export default ProveedoresPage
