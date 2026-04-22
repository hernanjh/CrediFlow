import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { creditosApi, clientesApi, seguridadApi } from '../api/client'
import { useAuthStore } from '../store/authStore'
import AutocompleteInput from '../components/ui/AutocompleteInput'
import { toast } from 'react-hot-toast'

export default function CreditosPage() {
  const queryClient = useQueryClient()
  const { usuario } = useAuthStore()
  const [clienteIdFiltro, setClienteIdFiltro] = useState('')
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [formData, setFormData] = useState({
    clienteId: '',
    capital: 10000,
    tasa: 30,
    cuotas: 6,
    frecuencia: 7, // SEMANAL
    vendedorId: usuario?.id || ''
  })

  // Como endpoint general, supongamos que si clienteId está vacío devuelve todos los créditos
  const { data, isLoading, error } = useQuery({
    queryKey: ['creditos', clienteIdFiltro],
    queryFn: () => creditosApi.getByCliente(clienteIdFiltro).then(r => r.data),
    enabled: clienteIdFiltro.length > 30, // Solo consultar si hay un Guid válido aprox.
  })

  const { data: vendedores = [] } = useQuery({
    queryKey: ['vendedores'],
    queryFn: () => seguridadApi.getUsuarios().then(r => r.data.filter((u: any) => u.rol === 'VENDEDOR' || u.rol === 'ADMIN'))
  })

  const createMutation = useMutation({
    mutationFn: (newCredito: any) => creditosApi.create({
      clienteId: newCredito.clienteId,
      vendedorId: newCredito.vendedorId || usuario?.id, // Asignamos al vendedor seleccionado or logged user
      capital: Number(newCredito.capital),
      tasa: Number(newCredito.tasa),
      tasaMoratoria: Number(newCredito.tasa) * 1.5,
      comisionRecargo: 0.1, // default
      cuotas: Number(newCredito.cuotas),
      frecuenciaDias: Number(newCredito.frecuencia),
      fechaPrimerVencimiento: new Date(Date.now() + 86400000).toISOString() // default mañana
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['creditos'] })
      setIsModalOpen(false)
      setFormData({ clienteId: '', capital: 10000, tasa: 30, cuotas: 6, frecuencia: 7, vendedorId: usuario?.id || '' })
      toast.success('Crédito otorgado exitosamente')
    },
    onError: () => toast.error('Error al otorgar crédito')
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!formData.clienteId) return toast.error('Seleccione un cliente de la lista')
    createMutation.mutate(formData)
  }

  // Búsqueda para Autocomplete
  const searchClientes = async (q: string) => {
    if (!q) return [];
    try {
      const res = await clientesApi.getAll({ search: q })
      return res.data.map((c: any) => ({
        id: c.id,
        label: c.nombreCompleto,
        subLabel: `DNI: ${c.dni} | ${c.estado}`
      }))
    } catch (e) {
      return []
    }
  }

  const creditos = data || []

  const handleExportExcel = () => {
      const exportData = creditos.map((c: any) => ({
          'ID Crédito': c.id,
          'Capital': `$${c.capitalOtorgado || c.capital}`, // Adjusting field names if necessary
          'Tasa Interés': `${c.tasaInteres || c.tasa}%`,
          'Cuotas': c.cantidadCuotas || c.cuotasTotales,
          'Frecuencia': c.frecuencia,
          'Estado': c.estado
      }));
      import('../utils/exportUtils').then(u => u.exportToExcel(exportData, `Creditos_Cliente_${clienteIdFiltro || 'Todos'}`));
  }

  const handleExportPDF = () => {
      const headers = ['ID Crédito', 'Capital', 'Tasa', 'Cuotas', 'Frecuencia', 'Estado'];
      const exportData = creditos.map((c: any) => [
          c.id.substring(0, 8),
          `$${c.capitalOtorgado || c.capital}`,
          `${c.tasaInteres || c.tasa}%`,
          c.cantidadCuotas || c.cuotasTotales,
          c.frecuencia,
          c.estado
      ]);
      import('../utils/exportUtils').then(u => u.exportToPDF(headers, exportData, `Extracto de Créditos`, `Creditos_${clienteIdFiltro}`));
  }

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Gestión de Créditos</h1>
          <p className="page-subtitle">Visualización de historial crediticio e ingreso de nuevos préstamos.</p>
        </div>
        <button id="btn-nuevo-credito" className="btn btn-primary" onClick={() => setIsModalOpen(true)}>
          + Otorgar Crédito
        </button>
      </div>

      <div className="filter-bar">
        <div style={{ flex: 1, maxWidth: 500 }}>
             <AutocompleteInput 
                placeholder="🔍 Buscar cliente para ver sus créditos y cuotas..."
                fetchOptions={searchClientes}
                onChange={(id) => setClienteIdFiltro(id)}
             />
        </div>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportExcel} disabled={creditos.length === 0}>
            XLSX
        </button>
        <button className="btn btn-secondary btn-sm ml-2" onClick={handleExportPDF} disabled={creditos.length === 0}>
            PDF
        </button>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
           <div className="loading-overlay"><span className="spinner" /> Cargando créditos...</div>
        ) : error ? (
           <div className="empty-state">
              <span className="empty-state-icon text-danger">⚠️</span>
              <div className="empty-state-title">Error al consultar datos</div>
              <div className="empty-state-subtitle">Compruebe la conectividad o la existencia del cliente.</div>
           </div>
        ) : (
          <table className="cierre-table">
            <thead>
              <tr>
                <th>ID Rastreable</th>
                <th>Capital ($)</th>
                <th>Tasa TNA</th>
                <th>Plan Cuotas</th>
                <th>Frecuencia Pago</th>
                <th style={{ textAlign: 'center' }}>Estado Global</th>
                <th style={{ textAlign: 'right' }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {!clienteIdFiltro ? (
                <tr>
                  <td colSpan={7} style={{ textAlign: 'center', padding: 60, color: 'var(--text-muted)' }}>
                    <div style={{ fontSize: 40, marginBottom: 15 }}>🔍</div>
                    Utiliza la barra superior para buscar un cliente y visualizar su estado crediticio.
                  </td>
                </tr>
              ) : creditos.length === 0 ? (
                <tr>
                  <td colSpan={7} style={{ textAlign: 'center', padding: 60 }}>
                     <div style={{ fontSize: 40, marginBottom: 15 }}>📂</div>
                     Ningún crédito registrado para este cliente.
                  </td>
                </tr>
              ) : (
                creditos.map((c: any) => (
                  <tr key={c.id}>
                    <td style={{ fontWeight: 600, color: 'var(--text-primary)', fontFamily: 'monospace' }}>{c.id.substring(0, 8)}...</td>
                    <td style={{ fontWeight: '500' }}>${c.capitalOtorgado || c.capital}</td>
                    <td>{c.tasaInteres || c.tasa}%</td>
                    <td>{c.cantidadCuotas || c.cuotasTotales} Cuotas</td>
                    <td>
                        {c.frecuencia === 1 && 'Diaria'}
                        {c.frecuencia === 7 && 'Semanal'}
                        {c.frecuencia === 15 && 'Quincenal'}
                        {c.frecuencia === 30 && 'Mensual'}
                        {typeof c.frecuencia === 'string' && c.frecuencia}
                    </td>
                    <td style={{ textAlign: 'center' }}>
                      <span className={`badge ${c.estado === 'ACTIVO' ? 'badge-success' : 'badge-danger'}`}>
                        {c.estado}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }}>
                        <button className="btn btn-secondary btn-sm">Ver Cuotas 👀</button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        )}
      </div>

      {isModalOpen && (
        <div className="modal-backdrop">
          <div className="surface-card" style={{ width: 500, padding: 30 }}>
            <h2 style={{ marginTop: 0, marginBottom: 20 }}>Otorgar Nuevo Préstamo / Crédito</h2>
            <form onSubmit={handleSubmit} className="flex-column" style={{ gap: 15 }}>
              <div>
                <label className="form-label">Beneficiario / Cliente</label>
                <AutocompleteInput 
                  placeholder="Ej: Juan Pérez"
                  fetchOptions={searchClientes}
                  onChange={(id) => setFormData({ ...formData, clienteId: id })}
                  required
                />
              </div>
              <div className="form-group">
                <label className="form-label">Vendedor Responsable</label>
                <select required className="form-input" value={formData.vendedorId} onChange={e => setFormData({ ...formData, vendedorId: e.target.value })}>
                   <option value="">-- Seleccionar Vendedor --</option>
                   {vendedores.map((v: any) => (
                      <option key={v.id} value={v.id}>{v.nombreCompleto}</option>
                   ))}
                </select>
              </div>
              <div className="grid-2">
                <div>
                  <label className="form-label">Capital Origen ($)</label>
                  <input required type="number" className="form-input" value={formData.capital} onChange={e => setFormData({ ...formData, capital: Number(e.target.value) })} />
                </div>
                <div>
                  <label className="form-label">Tasa Anual Base (%)</label>
                  <input required type="number" className="form-input" value={formData.tasa} onChange={e => setFormData({ ...formData, tasa: Number(e.target.value) })} />
                </div>
              </div>
              <div className="grid-2">
                <div>
                  <label className="form-label">Cantidad de Cuotas</label>
                  <input required type="number" className="form-input" value={formData.cuotas} onChange={e => setFormData({ ...formData, cuotas: Number(e.target.value) })} />
                </div>
                <div>
                  <label className="form-label">Frecuencia de Pago</label>
                  <select className="form-input" value={formData.frecuencia} onChange={e => setFormData({ ...formData, frecuencia: Number(e.target.value) })}>
                    <option value={1}>Diaria</option>
                    <option value={7}>Semanal</option>
                    <option value={15}>Quincenal</option>
                    <option value={30}>Mensual</option>
                  </select>
                </div>
              </div>
              
              <div className="flex-between" style={{ marginTop: 20, paddingTop: 20, borderTop: '1px solid var(--color-border)' }}>
                <span className="text-muted text-sm">El primer vencimiento se ajustará auto.</span>
                <div style={{ display: 'flex', gap: 10 }}>
                    <button type="button" className="btn btn-secondary" onClick={() => setIsModalOpen(false)}>Cancelar</button>
                    <button type="submit" className="btn btn-primary" disabled={createMutation.isPending || !formData.clienteId}>
                      {createMutation.isPending ? 'Procesando...' : 'Otorgar Dinero 💸'}
                    </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
