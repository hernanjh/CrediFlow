import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { clientesApi, zonasApi, parametricasApi } from '../api/client'
import { toast } from 'react-hot-toast'

export default function ClientesPage() {
  const queryClient = useQueryClient()
  const [search, setSearch] = useState('')
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editClientId, setEditClientId] = useState<string | null>(null)
  const [filterEstado, setFilterEstado] = useState<string | undefined>(undefined)
  const [filterTipo, setFilterTipo] = useState<string | undefined>(undefined)
  
  const initialForm = {
    nombreCompleto: '',
    dni: '',
    cuil: '',
    fechaNacimiento: '',
    tipoClienteId: '',
    ocupacionId: '',
    sectorId: '',
    sexo: '',
    direccion: '',
    barrio: '',
    localidad: '',
    codigoPostal: '',
    telefono: '',
    telefonoAlternativo: '',
    email: '',
    limiteCredito: 0,
    zonaId: '',
    fiadorNombre: '',
    fiadorDNI: '',
    fiadorTelefono: '',
    fiadorDireccion: '',
    fotoDNIFrente: '',
    fotoDNIDorso: ''
  }
  const [formData, setFormData] = useState(initialForm)
  const [activeTab, setActiveTab] = useState('personales')

  const { data: clientes = [], isLoading, error } = useQuery({
    queryKey: ['clientes', search, filterEstado, filterTipo],
    queryFn: () => clientesApi.getAll({ 
      search, 
      estado: filterEstado,
      tipoCliente: filterTipo
    }).then(r => r.data)
  })

  const { data: zonas = [] } = useQuery({
    queryKey: ['zonas'],
    queryFn: () => zonasApi.getAll(true).then(r => r.data)
  })

  const { data: ocupaciones = [] } = useQuery({
    queryKey: ['ocupaciones'],
    queryFn: () => parametricasApi.getOcupaciones(true).then(r => r.data)
  })

  const { data: sectores = [] } = useQuery({
    queryKey: ['sectores'],
    queryFn: () => parametricasApi.getSectores(true).then(r => r.data)
  })

  const { data: tiposCliente = [] } = useQuery({
    queryKey: ['tiposCliente'],
    queryFn: () => parametricasApi.getTiposCliente(true).then(r => r.data)
  })

  const createMutation = useMutation({
    mutationFn: (newClient: any) => clientesApi.create(newClient),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] })
      toast.success('Cliente creado con éxito')
      handleCloseModal()
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Error al crear cliente')
  })

  // Full fetch to edit
  const handleOpenEdit = async (id: string) => {
    try {
      const res = await clientesApi.getById(id)
      const data = res.data
      setEditClientId(id)
      setFormData({
        nombreCompleto: data.nombreCompleto || '',
        dni: data.dni || '',
        cuil: data.cuil || '',
        fechaNacimiento: data.fechaNacimiento ? data.fechaNacimiento.split('T')[0] : '',
        tipoClienteId: data.tipoClienteId || '',
        ocupacionId: data.ocupacionId || '',
        sectorId: data.sectorId || '',
        sexo: data.sexo || '',
        direccion: data.direccion || '',
        barrio: data.barrio || '',
        localidad: data.localidad || '',
        codigoPostal: data.codigoPostal || '',
        telefono: data.telefono || '',
        telefonoAlternativo: data.telefonoAlternativo || '',
        email: data.email || '',
        limiteCredito: data.limiteCredito || 0,
        zonaId: data.zonaId || (zonas.length > 0 ? zonas[0].id : ''),
        fiadorNombre: data.fiadorNombre || '',
        fiadorDNI: data.fiadorDNI || '',
        fiadorTelefono: data.fiadorTelefono || '',
        fiadorDireccion: data.fiadorDireccion || '',
        fotoDNIFrente: data.fotoDNIFrente || '',
        fotoDNIDorso: data.fotoDNIDorso || ''
      })
      setActiveTab('personales')
      setIsModalOpen(true)
    } catch {
      toast.error('Error al cargar datos del cliente')
    }
  }

  const updateMutation = useMutation({
    mutationFn: (data: { id: string, payload: any }) => clientesApi.update(data.id, data.payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] })
      toast.success('Cliente actualizado')
      handleCloseModal()
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Error al actualizar')
  })

  const statusMutation = useMutation({
    mutationFn: ({id, op}: {id: string, op: 'reactivar'|'inactivar'|'bloquear'}) => 
      clientesApi[op](id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] })
      toast.success('Estado actualizado')
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Error de operación')
  })

  const handleCloseModal = () => {
    setIsModalOpen(false)
    setEditClientId(null)
    setFormData(initialForm)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!formData.zonaId && zonas.length > 0) {
        formData.zonaId = zonas[0].id;
    }
    const payload = {
        ...formData,
        fechaNacimiento: formData.fechaNacimiento ? new Date(formData.fechaNacimiento).toISOString() : null,
        limiteCredito: Number(formData.limiteCredito)
    }
    
    if (editClientId) {
      updateMutation.mutate({ id: editClientId, payload })
    } else {
      createMutation.mutate(payload)
    }
  }

  const isPending = createMutation.isPending || updateMutation.isPending

  const getTipoBadge = (tipo: string) => {
      if (tipo === 'VIP') return 'badge-warning';
      if (tipo === 'MAYORISTA') return 'badge-info';
      if (tipo === 'MOROSO' || tipo === 'LISTA_NEGRA') return 'badge-danger';
      return 'badge-neutral';
  }

  const handleExportExcel = () => {
      const data = clientes.map((c: any) => ({
          'ID Interno': c.id,
          'Nombre Completo': c.nombreCompleto,
          'DNI': c.dni,
          'CUIL': c.cuil || '',
          'Teléfono': c.telefono,
          'Localidad': c.localidad,
          'Zona': c.zonaNombre || 'No Asignada',
          'Límite Crédito': c.limiteCredito,
          'Tipo': c.tipoCliente,
          'Estado': c.estado
      }));
      import('../utils/exportUtils').then(u => u.exportToExcel(data, `Directorio_Clientes_${new Date().toISOString().split('T')[0]}`));
  }

  const handleExportPDF = () => {
      const headers = ['DNI / CUIL', 'Nombre Completo', 'Teléfono', 'Localidad', 'Límite ($)', 'Estado'];
      const data = clientes.map((c: any) => [
          c.cuil || c.dni,
          c.nombreCompleto,
          c.telefono,
          c.localidad,
          `$${c.limiteCredito}`,
          c.estado
      ]);
      import('../utils/exportUtils').then(u => u.exportToPDF(headers, data, `Directorio de Clientes (${filterEstado || 'Todos'})`, `Directorio_Clientes_${new Date().toISOString().split('T')[0]}`));
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Directorio de Clientes</h1>
          <p className="page-subtitle">Total: {clientes.length} registrados</p>
        </div>
        <button className="btn btn-primary" onClick={() => {
            setFormData({...initialForm, zonaId: zonas.length ? zonas[0].id : ''});
            setIsModalOpen(true);
        }}>
          + Nuevo Cliente
        </button>
      </div>

      <div className="filter-bar">
        <div className="search-input-wrap">
          <span className="search-icon">🔍</span>
          <input
            type="text"
            className="form-input"
            placeholder="Buscar por DNI, CUIL o Nombre..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
        <select className="form-input" style={{ width: 180 }} value={filterEstado || ''} onChange={e => setFilterEstado(e.target.value || undefined)}>
          <option value="">Todos los Estados</option>
          <option value="ACTIVO">Activos</option>
          <option value="INACTIVO">Inactivos</option>
          <option value="BLOQUEADO">Bloqueados</option>
        </select>
        <select className="form-input" style={{ width: 180 }} value={filterTipo || ''} onChange={e => setFilterTipo(e.target.value || undefined)}>
          <option value="">Cualquier Tipo</option>
          {tiposCliente.map((t: any) => (
            <option key={t.id} value={t.id}>{t.nombre}</option>
          ))}
        </select>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportExcel} title="Exportar a Excel" disabled={clientes.length === 0}>
            XLSX
        </button>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportPDF} title="Generar PDF" disabled={clientes.length === 0}>
            PDF
        </button>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
          <div className="loading-overlay"><div className="spinner" /> Cargando directorio...</div>
        ) : error ? (
          <div className="empty-state">
              <span className="empty-state-icon">⚠️</span>
              <div className="empty-state-title">Error de conexión</div>
              <div className="empty-state-subtitle">No se pudo cargar la lista de clientes.</div>
          </div>
        ) : clientes.length === 0 ? (
          <div className="empty-state">
             <span className="empty-state-icon">👥</span>
             <div className="empty-state-title">Directorio Vacío</div>
             <div className="empty-state-subtitle">No se encontraron clientes con los filtros actuales.</div>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Cliente / Tipo</th>
                <th>Documento</th>
                <th>Contacto</th>
                <th>Zona</th>
                <th>Límite Cta.</th>
                <th style={{ textAlign: 'center' }}>Estado</th>
                <th style={{ textAlign: 'right' }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {clientes.map((c: any) => (
                <tr key={c.id}>
                  <td>
                      <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{c.nombreCompleto}</div>
                      <div className={`badge ${getTipoBadge(c.tipoCliente)}`} style={{ marginTop: 4 }}>
                          {c.tipoCliente}
                      </div>
                  </td>
                  <td>
                      <div style={{ fontSize: 12 }}>DNI: {c.dni}</div>
                      {c.cuil && <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>CUIL: {c.cuil}</div>}
                  </td>
                  <td>
                      <div style={{ fontSize: 12 }}>{c.telefono}</div>
                      <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>{c.localidad || 'Sin Loc.'}</div>
                  </td>
                  <td>
                      <span className="badge badge-primary">{c.zonaNombre || 'No Asignada'}</span>
                  </td>
                  <td style={{ fontWeight: 600 }}>${c.limiteCredito?.toLocaleString()}</td>
                  <td style={{ textAlign: 'center' }}>
                    <span className={`badge ${c.estado === 'ACTIVO' ? 'badge-success' : c.estado === 'INACTIVO' ? 'badge-neutral' : 'badge-danger'}`}>
                      {c.estado}
                    </span>
                    {c.tieneDeudaActiva && <div style={{ fontSize: 10, color: 'var(--color-warning)', marginTop: 4, fontWeight: 700 }}>Con Deuda</div>}
                  </td>
                  <td style={{ textAlign: 'right', whiteSpace: 'nowrap' }}>
                    <button className="btn btn-ghost btn-sm" onClick={() => handleOpenEdit(c.id)}>Editar</button>
                    {c.estado === 'ACTIVO' ? (
                      <>
                        <button className="btn btn-ghost btn-sm" style={{ color: 'var(--text-muted)', marginLeft: 8 }} title="Inactivar"
                            onClick={() => { if(confirm('¿Inactivar cliente?')) statusMutation.mutate({ id: c.id, op: 'inactivar' }) }}>⏸️</button>
                        <button className="btn btn-ghost btn-sm" style={{ color: 'var(--color-danger)', marginLeft: 8 }} title="Bloquear"
                            onClick={() => { if(confirm('¿Bloquear permanentemente al cliente?')) statusMutation.mutate({ id: c.id, op: 'bloquear' }) }}>🚫</button>
                      </>
                    ) : (
                      <button className="btn btn-ghost btn-sm" style={{ color: 'var(--color-success)', marginLeft: 8 }} title="Reactivar"
                          onClick={() => { if(confirm('¿Reactivar cliente?')) statusMutation.mutate({ id: c.id, op: 'reactivar' }) }}>🔄 Reactivar</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {isModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content modal-lg">
            <div className="modal-header">
              <div>
                  <div className="modal-title">{editClientId ? 'Expediente del Cliente' : 'Alta de Nuevo Cliente'}</div>
                  <div className="modal-subtitle">Complete todos los campos requeridos para la correcta evaluación</div>
              </div>
              <button className="modal-close" onClick={handleCloseModal}>×</button>
            </div>
            
            <div className="modal-body">
              <div className="tabs">
                  <button className={`tab-btn ${activeTab === 'personales' ? 'active' : ''}`} onClick={() => setActiveTab('personales')}>Datos Per.</button>
                  <button className={`tab-btn ${activeTab === 'ubicacion' ? 'active' : ''}`} onClick={() => setActiveTab('ubicacion')}>Ubicación</button>
                  <button className={`tab-btn ${activeTab === 'comercial' ? 'active' : ''}`} onClick={() => setActiveTab('comercial')}>Comercial</button>
                  <button className={`tab-btn ${activeTab === 'fiador' ? 'active' : ''}`} onClick={() => setActiveTab('fiador')}>Fiador</button>
                  <button className={`tab-btn ${activeTab === 'adjuntos' ? 'active' : ''}`} onClick={() => setActiveTab('adjuntos')}>Docs</button>
              </div>

              <form id="clienteForm" onSubmit={handleSubmit}>
                
                {/* TABS CONTENT */}
                <div style={{ display: activeTab === 'personales' ? 'block' : 'none' }}>
                    <div className="grid-2 mb-4">
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Nombre Completo *</label>
                            <input required className="form-input" value={formData.nombreCompleto} onChange={e => setFormData({ ...formData, nombreCompleto: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">DNI *</label>
                            <input required className="form-input" value={formData.dni} onChange={e => setFormData({ ...formData, dni: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">CUIL / CUIT</label>
                            <input className="form-input" value={formData.cuil} onChange={e => setFormData({ ...formData, cuil: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Fecha Nacimiento</label>
                            <input type="date" className="form-input" value={formData.fechaNacimiento} onChange={e => setFormData({ ...formData, fechaNacimiento: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Sector</label>
                            <select className="form-input" value={formData.sectorId} onChange={e => setFormData({ ...formData, sectorId: e.target.value })}>
                                <option value="">-- Seleccionar Sector --</option>
                                {sectores.map((s:any) => <option key={s.id} value={s.id}>{s.nombre}</option>)}
                            </select>
                        </div>
                        <div>
                            <label className="form-label">Ocupación</label>
                            <select className="form-input" value={formData.ocupacionId} onChange={e => setFormData({ ...formData, ocupacionId: e.target.value })}>
                                <option value="">-- Seleccionar Ocupación --</option>
                                {ocupaciones.map((o:any) => <option key={o.id} value={o.id}>{o.nombre}</option>)}
                            </select>
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'ubicacion' ? 'block' : 'none' }}>
                    <div className="grid-2 mb-4">
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Dirección (Calle y Nro) *</label>
                            <input required className="form-input" value={formData.direccion} onChange={e => setFormData({ ...formData, direccion: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Barrio</label>
                            <input className="form-input" value={formData.barrio} onChange={e => setFormData({ ...formData, barrio: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Localidad</label>
                            <input className="form-input" value={formData.localidad} onChange={e => setFormData({ ...formData, localidad: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Código Postal</label>
                            <input className="form-input" value={formData.codigoPostal} onChange={e => setFormData({ ...formData, codigoPostal: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Provincia</label>
                            <input className="form-input" value={formData.provincia || 'Buenos Aires'} onChange={e => setFormData({ ...formData, provincia: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Teléfono Principal *</label>
                            <input required className="form-input" value={formData.telefono} onChange={e => setFormData({ ...formData, telefono: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Tel. Alternativo</label>
                            <input className="form-input" value={formData.telefonoAlternativo} onChange={e => setFormData({ ...formData, telefonoAlternativo: e.target.value })} />
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'comercial' ? 'block' : 'none' }}>
                     <div className="grid-2 mb-4">
                        <div>
                            <label className="form-label">Zona Asignada *</label>
                            <select required className="form-input" value={formData.zonaId} onChange={e => setFormData({ ...formData, zonaId: e.target.value })}>
                                {zonas.map((z: any) => <option key={z.id} value={z.id}>{z.nombre}</option>)}
                            </select>
                        </div>
                        <div>
                            <label className="form-label">Tipo de Cliente</label>
                            <select className="form-input" value={formData.tipoClienteId} onChange={e => setFormData({ ...formData, tipoClienteId: e.target.value })}>
                                <option value="">-- Seleccionar Tipo --</option>
                                {tiposCliente.map((t: any) => <option key={t.id} value={t.id}>{t.nombre}</option>)}
                            </select>
                        </div>
                        <div>
                            <label className="form-label">Límite de Crédito ($)</label>
                            <input type="number" className="form-input" value={formData.limiteCredito} onChange={e => setFormData({ ...formData, limiteCredito: Number(e.target.value) })} />
                        </div>
                        <div>
                            <label className="form-label">Email</label>
                            <input type="email" className="form-input" value={formData.email} onChange={e => setFormData({ ...formData, email: e.target.value })} />
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'fiador' ? 'block' : 'none' }}>
                     <div className="grid-2 mb-4">
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Nombre del Fiador / Garante</label>
                            <input className="form-input" value={formData.fiadorNombre} onChange={e => setFormData({ ...formData, fiadorNombre: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">DNI del Fiador</label>
                            <input className="form-input" value={formData.fiadorDNI} onChange={e => setFormData({ ...formData, fiadorDNI: e.target.value })} />
                        </div>
                        <div>
                            <label className="form-label">Teléfono del Fiador</label>
                            <input className="form-input" value={formData.fiadorTelefono} onChange={e => setFormData({ ...formData, fiadorTelefono: e.target.value })} />
                        </div>
                        <div style={{ gridColumn: 'span 2' }}>
                            <label className="form-label">Dirección del Fiador</label>
                            <input className="form-input" value={formData.fiadorDireccion} onChange={e => setFormData({ ...formData, fiadorDireccion: e.target.value })} />
                        </div>
                    </div>
                </div>

                <div style={{ display: activeTab === 'adjuntos' ? 'block' : 'none' }}>
                    <div className="grid-2 mb-4">
                        <label className="img-upload-box" style={{ height: 180 }}>
                            {formData.fotoDNIFrente ? <img src={formData.fotoDNIFrente} alt="Frente" /> : 
                            <div className="upload-placeholder">
                                <span style={{ fontSize: 24 }}>📸</span>
                                <span>DNI - Reverso / Dorso</span>
                            </div>}
                            <input type="file" accept="image/*" style={{ display: 'none' }} onChange={e => {
                                const file = e.target.files?.[0];
                                if(file) {
                                    const r = new FileReader();
                                    r.onloadend = () => setFormData({...formData, fotoDNIFrente: r.result as string});
                                    r.readAsDataURL(file);
                                }
                            }} />
                        </label>
                        <label className="img-upload-box" style={{ height: 180 }}>
                            {formData.fotoDNIDorso ? <img src={formData.fotoDNIDorso} alt="Dorso" /> : 
                            <div className="upload-placeholder">
                                <span style={{ fontSize: 24 }}>📸</span>
                                <span>DNI - Dorso</span>
                            </div>}
                            <input type="file" accept="image/*" style={{ display: 'none' }} onChange={e => {
                                const file = e.target.files?.[0];
                                if(file) {
                                    const r = new FileReader();
                                    r.onloadend = () => setFormData({...formData, fotoDNIDorso: r.result as string});
                                    r.readAsDataURL(file);
                                }
                            }} />
                        </label>
                    </div>
                </div>
              </form>
            </div>
            
            <div className="modal-footer">
              <button type="button" className="btn btn-ghost" onClick={handleCloseModal}>Cancelar / Cerrar</button>
              <button type="submit" form="clienteForm" className="btn btn-primary" disabled={isPending}>
                {isPending ? 'Guardando expediente...' : (editClientId ? 'Guardar Cambios' : 'Registrar Expediente')}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
