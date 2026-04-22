import { useState, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { configuracionApi, parametricasApi } from '../api/client'
import { toast } from 'react-hot-toast'

export default function ConfiguracionPage() {
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState('global')

  // -- Config Global Fetch
  const { data: config, isLoading: isConfigLoading } = useQuery({
    queryKey: ['configuracion'],
    queryFn: () => configuracionApi.get().then(r => r.data)
  })

  // -- Parametricas Fetch
  const { data: ocupaciones = [] } = useQuery({ queryKey: ['ocup'], queryFn: () => parametricasApi.getOcupaciones(false).then(r => r.data) })
  const { data: condicionesIva = [] } = useQuery({ queryKey: ['iva'], queryFn: () => parametricasApi.getCondicionesIva(false).then(r => r.data) })
  const { data: condicionesPago = [] } = useQuery({ queryKey: ['pago'], queryFn: () => parametricasApi.getCondicionesPago(false).then(r => r.data) })

  // -- States
  const [formData, setFormData] = useState<any>({
    nombreEmpresa: '', cuit: '', razonSocial: '', webSite: '',
    emailContacto: '', tasaInteresDefecto: 20,
    tasaMoraDefecto: 0.5, monedaSimbolo: '$', logoBase64: ''
  })
  
  const [paramInput, setParamInput] = useState('')
  const [paramInput2, setParamInput2] = useState<number>(0) // for DiasPlazo

  useEffect(() => {
    if (config) setFormData(config)
  }, [config])

  const mutation = useMutation({
    mutationFn: (data: any) => configuracionApi.update(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['configuracion'] })
      toast.success('Configuración actualizada')
    }
  })

  // Parametricas Mutations Handlers
  const handleCreateParam = (type: string) => {
      if(!paramInput) return;
      if(type === 'ocupacion') parametricasApi.createOcupacion({nombre: paramInput}).then(()=> {toast.success('Ocupación creada'); queryClient.invalidateQueries({queryKey:['ocup']})});
      if(type === 'iva') parametricasApi.createCondicionIva({nombre: paramInput}).then(()=> {toast.success('Condición creada'); queryClient.invalidateQueries({queryKey:['iva']})});
      if(type === 'pago') parametricasApi.createCondicionPago({nombre: paramInput, diasPlazo: paramInput2}).then(()=> {toast.success('Condición creada'); queryClient.invalidateQueries({queryKey:['pago']})});
      setParamInput('');
      setParamInput2(0);
  }

  const handleDeleteParam = (type: string, id: string) => {
      if(!confirm('¿Desactivar esta opción?')) return;
      if(type === 'ocupacion') parametricasApi.deleteOcupacion(id).then(()=>queryClient.invalidateQueries({queryKey:['ocup']}));
      if(type === 'iva') parametricasApi.deleteCondicionIva(id).then(()=>queryClient.invalidateQueries({queryKey:['iva']}));
      if(type === 'pago') parametricasApi.deleteCondicionPago(id).then(()=>queryClient.invalidateQueries({queryKey:['pago']}));
  }
  const handleReactivateParam = (type: string, id: string) => {
      if(type === 'ocupacion') parametricasApi.reactivarOcupacion(id).then(()=>queryClient.invalidateQueries({queryKey:['ocup']}));
      if(type === 'iva') parametricasApi.reactivarCondicionIva(id).then(()=>queryClient.invalidateQueries({queryKey:['iva']}));
      if(type === 'pago') parametricasApi.reactivarCondicionPago(id).then(()=>queryClient.invalidateQueries({queryKey:['pago']}));
  }

  const handleLogoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => setFormData({ ...formData, logoBase64: reader.result as string })
      reader.readAsDataURL(file)
    }
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Configuración del Sistema</h1>
          <p className="page-subtitle">Ajustes globales y ABM de parámetros.</p>
        </div>
      </div>

      <div className="tabs mb-6">
          <button className={`tab-btn ${activeTab === 'global' ? 'active' : ''}`} onClick={() => setActiveTab('global')}>⚙️ Ajustes Globales</button>
          <button className={`tab-btn ${activeTab === 'identidad' ? 'active' : ''}`} onClick={() => setActiveTab('identidad')}>🖼️ Identidad Visual</button>
          <button className={`tab-btn ${activeTab === 'parametros' ? 'active' : ''}`} onClick={() => setActiveTab('parametros')}>📚 Diccionarios de Datos (ABM)</button>
      </div>

      <div className="surface-card">
          {activeTab === 'global' && (
              <form onSubmit={e => { e.preventDefault(); mutation.mutate(formData); }} className="form-grid">
                  <h3 className="mb-4" style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Información Legal y Comercial</h3>
                  <div className="grid-2 mb-6">
                      <div>
                          <label className="form-label">Nombre Comercial</label>
                          <input className="form-input" value={formData.nombreEmpresa} onChange={e => setFormData({...formData, nombreEmpresa: e.target.value})} />
                      </div>
                      <div>
                          <label className="form-label">CUIT / ID Fiscal</label>
                          <input className="form-input" value={formData.cuit} onChange={e => setFormData({...formData, cuit: e.target.value})} />
                      </div>
                      <div style={{ gridColumn: 'span 2' }}>
                          <label className="form-label">Razón Social Legal</label>
                          <input className="form-input" value={formData.razonSocial} onChange={e => setFormData({...formData, razonSocial: e.target.value})} />
                      </div>
                      <div>
                          <label className="form-label">Sitio Web</label>
                          <input className="form-input" value={formData.webSite} onChange={e => setFormData({...formData, webSite: e.target.value})} />
                      </div>
                      <div>
                          <label className="form-label">Email de Contacto Corporativo</label>
                          <input type="email" className="form-input" value={formData.emailContacto} onChange={e => setFormData({...formData, emailContacto: e.target.value})} />
                      </div>
                  </div>

                  <h3 className="mb-4" style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Variables Financieras Default</h3>
                  <div className="grid-3 mb-6 bg-surface-2 p-4 rounded-8 border border-border">
                      <div>
                          <label className="form-label">Tasa Interés TNA Base (%)</label>
                          <input type="number" step="0.01" className="form-input" value={formData.tasaInteresDefecto} onChange={e => setFormData({...formData, tasaInteresDefecto: parseFloat(e.target.value)})} />
                      </div>
                      <div>
                          <label className="form-label">Tasa Mora Diaria Punitoria (%)</label>
                          <input type="number" step="0.01" className="form-input" value={formData.tasaMoraDefecto} onChange={e => setFormData({...formData, tasaMoraDefecto: parseFloat(e.target.value)})} />
                      </div>
                      <div>
                          <label className="form-label">Símbolo Monetario</label>
                          <input type="text" className="form-input" value={formData.monedaSimbolo} onChange={e => setFormData({...formData, monedaSimbolo: e.target.value})} />
                      </div>
                  </div>
                  <div className="flex-between">
                      <span className="text-muted text-sm">Estos valores se usan por defecto si no hay lista de precio.</span>
                      <button type="submit" className="btn btn-primary" disabled={mutation.isPending}>Guardar Ajustes</button>
                  </div>
              </form>
          )}

          {activeTab === 'identidad' && (
              <div>
                  <h3 className="mb-4" style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Isologotipo / Marca Comercial</h3>
                  <div className="flex-column" style={{ alignItems: 'flex-start', gap: 15 }}>
                      <div className="img-upload-box" style={{ width: 250, height: 250, background: 'var(--color-surface-2)', border: '2px dashed var(--color-border)' }}>
                          {formData.logoBase64 ? <img src={formData.logoBase64} alt="logo" style={{ objectFit: 'contain' }} /> : <span style={{ color: 'var(--text-muted)' }}>Sin Logo Cargado</span>}
                          <input type="file" style={{ display: 'none' }} accept="image/*" onChange={handleLogoChange} />
                      </div>
                      <div className="flex-center" style={{ gap: 10 }}>
                          <label className="btn btn-secondary cursor-pointer">
                              Examinar e Insertar Logo
                              <input type="file" style={{ display: 'none' }} accept="image/*" onChange={handleLogoChange} />
                          </label>
                          <button className="btn btn-primary" onClick={() => mutation.mutate(formData)} disabled={mutation.isPending}>Subir e Impactar Documentos</button>
                      </div>
                      <span className="text-muted text-sm mt-2">Sugerido: PNG Transparente 500x500px. Se usará en encabezados de reportes y facturas.</span>
                  </div>
              </div>
          )}

          {activeTab === 'parametros' && (
              <div>
                  <p className="text-muted text-sm mb-6">Administración de Diccionarios y opciones para listas desplegables del sistema. Los campos inactivados no aparecerán para futuras selecciones, pero mantendrán integridad referencial histórica.</p>
                  
                  <div className="grid-3">
                      {/* Ocupaciones */}
                      <div className="bg-surface p-4 rounded-8 border border-border shadow-sm">
                          <h4 style={{ fontWeight: 600, marginBottom: 15, borderBottom: '1px solid var(--color-surface-2)', paddingBottom: 10 }}>Rubros / Ocupaciones</h4>
                          <div className="flex-between mb-4">
                              <input className="form-input flex-1" placeholder="Ej: Empleado Público" value={paramInput} onChange={e => setParamInput(e.target.value)} />
                              <button className="btn btn-primary btn-sm ml-2" onClick={() => handleCreateParam('ocupacion')}>Agregar</button>
                          </div>
                          <ul style={{ listStyle: 'none', padding: 0, margin: 0, maxHeight: 300, overflowY: 'auto' }}>
                              {ocupaciones.map((o:any) => (
                                  <li key={o.id} className="flex-between py-2 border-b border-border" style={{ opacity: o.activa ? 1 : 0.5 }}>
                                      <span className="text-sm">{o.nombre}</span>
                                      {o.activa 
                                          ? <button className="text-danger bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleDeleteParam('ocupacion', o.id)}>Baja</button>
                                          : <button className="text-success bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleReactivateParam('ocupacion', o.id)}>Alta</button>}
                                  </li>
                              ))}
                          </ul>
                      </div>

                      {/* Condicion IVA */}
                      <div className="bg-surface p-4 rounded-8 border border-border shadow-sm">
                          <h4 style={{ fontWeight: 600, marginBottom: 15, borderBottom: '1px solid var(--color-surface-2)', paddingBottom: 10 }}>Condiciones de I.V.A.</h4>
                          <div className="flex-between mb-4">
                              <input className="form-input flex-1" placeholder="Nueva condición..." value={paramInput} onChange={e => setParamInput(e.target.value)} />
                              <button className="btn btn-primary btn-sm ml-2" onClick={() => handleCreateParam('iva')}>Agregar</button>
                          </div>
                          <ul style={{ listStyle: 'none', padding: 0, margin: 0, maxHeight: 300, overflowY: 'auto' }}>
                              {condicionesIva.map((o:any) => (
                                  <li key={o.id} className="flex-between py-2 border-b border-border" style={{ opacity: o.activa ? 1 : 0.5 }}>
                                      <span className="text-sm">{o.nombre}</span>
                                      {o.activa 
                                          ? <button className="text-danger bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleDeleteParam('iva', o.id)}>Baja</button>
                                          : <button className="text-success bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleReactivateParam('iva', o.id)}>Alta</button>}
                                  </li>
                              ))}
                          </ul>
                      </div>

                      {/* Condicion Pago */}
                      <div className="bg-surface p-4 rounded-8 border border-border shadow-sm">
                          <h4 style={{ fontWeight: 600, marginBottom: 15, borderBottom: '1px solid var(--color-surface-2)', paddingBottom: 10 }}>Condiciones de Pago</h4>
                          <div className="flex-between mb-4" style={{ gap: 8 }}>
                              <input className="form-input flex-1" placeholder="Nombre" value={paramInput} onChange={e => setParamInput(e.target.value)} />
                              <input type="number" className="form-input w-20" placeholder="Días" value={paramInput2} onChange={e => setParamInput2(Number(e.target.value))} title="Días Plazo" />
                              <button className="btn btn-primary btn-sm" onClick={() => handleCreateParam('pago')}>+</button>
                          </div>
                          <ul style={{ listStyle: 'none', padding: 0, margin: 0, maxHeight: 300, overflowY: 'auto' }}>
                              {condicionesPago.map((o:any) => (
                                  <li key={o.id} className="flex-between py-2 border-b border-border" style={{ opacity: o.activa ? 1 : 0.5 }}>
                                      <div>
                                          <div className="text-sm">{o.nombre}</div>
                                          <div className="text-xs text-muted">{o.diasPlazo} días de plazo</div>
                                      </div>
                                      {o.activa 
                                          ? <button className="text-danger bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleDeleteParam('pago', o.id)}>Baja</button>
                                          : <button className="text-success bg-transparent border-none cursor-pointer text-xs font-bold" onClick={() => handleReactivateParam('pago', o.id)}>Alta</button>}
                                  </li>
                              ))}
                          </ul>
                      </div>
                  </div>
              </div>
          )}
      </div>
    </div>
  )
}
