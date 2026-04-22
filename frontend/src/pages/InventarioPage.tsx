import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { inventarioApi, categoriasApi, proveedoresApi } from '../api/client'
import { toast } from 'react-hot-toast'

export default function InventarioPage() {
  const [tab, setTab] = useState<'PRODUCTOS' | 'FACTURA'>('PRODUCTOS')

  return (
    <div className="page-content">
      <div className="page-header">
        <div>
          <h1 className="page-title">Módulo de Inventario</h1>
          <p className="page-subtitle">Gestiona productos, precios y facturación a proveedores</p>
        </div>
      </div>

      <div className="tabs mb-6">
        <button 
          className={`tab-btn ${tab === 'PRODUCTOS' ? 'active' : ''}`}
          onClick={() => setTab('PRODUCTOS')}
        >
          📦 Catálogo y Stock
        </button>
        <button 
          className={`tab-btn ${tab === 'FACTURA' ? 'active' : ''}`}
          onClick={() => setTab('FACTURA')}
        >
          🧾 Cargar Factura (Ingreso)
        </button>
      </div>

      {tab === 'PRODUCTOS' && <CatalogoTab />}
      {tab === 'FACTURA' && <FacturaTab onComplete={() => setTab('PRODUCTOS')} />}

    </div>
  )
}

function CatalogoTab() {
  const queryClient = useQueryClient()
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [filter, setFilter] = useState('')
  const [showInactive, setShowInactive] = useState(false)
  
  const initialForm = {
    nombre: '', descripcion: '', costoUnitario: 0, precioVenta: 0, precioVentaMayorista: 0, 
    stockInicial: 0, stockMinimo: 5, stockMaximo: 0, unidadMedidaId: '', 
    codigoInterno: '', sku: '', categoriaId: '', proveedorId: '', foto: '',
    permiteVentaSinStock: false
  }
  const [formData, setFormData] = useState<any>(initialForm)

  const { data: productos = [], isLoading } = useQuery({
    queryKey: ['productos', showInactive, filter],
    queryFn: () => inventarioApi.getProductos({ search: filter, soloActivos: !showInactive }).then(r => r.data)
  })

  const { data: categorias = [] } = useQuery({ 
    queryKey: ['categorias'], 
    queryFn: () => categoriasApi.getAll().then(r => r.data) 
  })
  
  const { data: proveedores = [] } = useQuery({ 
    queryKey: ['proveedores_admin'], 
    queryFn: () => proveedoresApi.getAll({ soloActivos: true }).then(r => r.data) 
  })

  const { data: unidades = [] } = useQuery({
    queryKey: ['unidadesMedida'],
    queryFn: () => parametricasApi.getUnidadesMedida(true).then(r => r.data)
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => inventarioApi.createProducto(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      setIsModalOpen(false)
      toast.success('Producto creado exitosamente')
    },
    onError: () => toast.error('Error al crear producto')
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string, data: any }) => inventarioApi.updateProducto(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      setIsModalOpen(false)
      toast.success('Producto actualizado exitosamente')
    },
    onError: () => toast.error('Error al actualizar producto')
  })

  const statusMutation = useMutation({
    mutationFn: ({ id, action }: { id: string, action: 'delete' | 'reactivar' }) => 
        action === 'delete' ? inventarioApi.deleteProducto(id) : inventarioApi.reactivarProducto(id),
    onSuccess: (_, vars) => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success(vars.action === 'delete' ? 'Producto inactivado' : 'Producto reactivado')
    }
  })

  const handleEdit = async (id: string) => {
      try {
          const res = await inventarioApi.getProductoById(id)
          const data = res.data
          setFormData({
             nombre: data.nombre,
             descripcion: data.descripcion || '',
             costoUnitario: data.costoUnitario,
             precioVenta: data.precioVenta,
             precioVentaMayorista: data.precioVentaMayorista || 0,
             stockInicial: data.stockActual,
             stockMinimo: data.stockMinimo,
             stockMaximo: data.stockMaximo || 0,
             unidadMedidaId: data.unidadMedidaId || '',
             codigoInterno: data.codigoInterno || '',
             sku: data.sku || '',
             categoriaId: data.categoriaId || '',
             proveedorId: data.proveedorId || '',
             foto: data.foto || '',
             permiteVentaSinStock: data.permiteVentaSinStock || false
          });
          setEditId(id)
          setIsModalOpen(true)
      } catch (err) {
          toast.error('No se pudo cargar el producto')
      }
  }

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      const reader = new FileReader()
      reader.onloadend = () => setFormData({ ...formData, foto: reader.result as string })
      reader.readAsDataURL(file)
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault();
      const payload = {
          ...formData,
          precioVentaMayorista: formData.precioVentaMayorista || null,
          stockMaximo: formData.stockMaximo || null
      }
      if (editId) {
          updateMutation.mutate({ id: editId, data: payload })
      } else {
          createMutation.mutate(payload)
      }
  }

  return (
    <>
      <div className="filter-bar">
        <div className="search-input-wrap">
          <span className="search-icon">🔍</span>
          <input 
            type="text" 
            placeholder="Buscar por Nombre, SKU o Código..." 
            value={filter} 
            onChange={e => setFilter(e.target.value)}
            className="form-input" 
          />
        </div>
        <label className="flex items-center gap-2 cursor-pointer text-sm" style={{ color: 'var(--text-muted)' }}>
          <input type="checkbox" checked={showInactive} onChange={e => setShowInactive(e.target.checked)} className="form-input" style={{ width: 18, height: 18 }} />
          Ver Productos Inactivos
        </label>
        <div style={{ flex: 1 }}></div>
        <button className="btn btn-primary" onClick={() => { setEditId(null); setFormData(initialForm); setIsModalOpen(true); }}>
            + Nuevo Producto
        </button>
      </div>

      <div className="data-table-wrap">
        {isLoading ? (
          <div className="loading-overlay"><span className="spinner" /> Cargando catálogo...</div>
        ) : productos.length === 0 ? (
          <div className="empty-state">
              <span className="empty-state-icon">📦</span>
              <div className="empty-state-title">Catálogo Vacío</div>
              <div className="empty-state-subtitle">No se encontraron productos en el inventario.</div>
          </div>
        ) : (
          <table className="cierre-table">
            <thead>
              <tr>
                <th>Producto / Código</th>
                <th>Clasificación</th>
                <th>Precios</th>
                <th style={{ textAlign: 'center' }}>Stock Actual</th>
                <th style={{ textAlign: 'right' }}>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {productos.map((p: any) => (
                <tr key={p.id} style={{ opacity: p.activo ? 1 : 0.6 }}>
                  <td>
                    <div style={{ display: 'flex', gap: 12, alignItems: 'center' }}>
                      <div style={{ width: 40, height: 40, backgroundColor: 'var(--color-surface-2)', borderRadius: 6, display: 'flex', alignItems: 'center', justifyContent: 'center', border: '1px solid var(--color-border)', overflow: 'hidden' }}>
                        {p.foto ? <img src={p.foto} style={{ width: '100%', height: '100%', objectFit: 'cover' }} alt="prod" /> : <span style={{ fontSize: 20 }}>📦</span>}
                      </div>
                      <div>
                        <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
                            {p.nombre}
                            {!p.activo && <span className="badge badge-neutral" style={{ marginLeft: 6 }}>INACTIVO</span>}
                        </div>
                        <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>SKU: {p.sku || '---'} | Cód: {p.codigoInterno || '---'}</div>
                      </div>
                    </div>
                  </td>
                  <td>
                      <span className="badge badge-secondary">{p.categoriaNombre || 'Sin Categoría'}</span>
                      <div style={{ fontSize: 11, color: 'var(--text-muted)', marginTop: 4 }}>
                          {p.proveedorNombre ? `Prov: ${p.proveedorNombre}` : ''}
                      </div>
                  </td>
                  <td>
                    <div style={{ fontSize: 13, fontWeight: 500, color: 'var(--color-primary)' }}>Venta: ${p.precioVenta.toLocaleString()}</div>
                    {p.precioVentaMayorista > 0 && <div style={{ fontSize: 11, color: 'var(--text-muted)' }}>Mayorista: ${p.precioVentaMayorista.toLocaleString()}</div>}
                    <div style={{ fontSize: 11, color: 'var(--text-secondary)', marginTop: 2 }}>Costo: ${p.costoUnitario.toLocaleString()}</div>
                  </td>
                  <td style={{ textAlign: 'center' }}>
                    <div className="flex-center flex-column">
                        <span className={`badge ${p.bajoStock ? 'badge-danger' : 'badge-success'}`} style={{ fontSize: 14, padding: '4px 10px' }}>
                          {p.stockActual} {p.unidadMedida?.simbolo || p.unidadMedida?.nombre?.slice(0,3) || 'un'}
                        </span>
                        <div style={{ fontSize: 10, color: 'var(--text-muted)', marginTop: 4 }}>Mín: {p.stockMinimo}</div>
                    </div>
                  </td>
                  <td style={{ textAlign: 'right' }}>
                    <button className="btn btn-ghost btn-sm" onClick={() => handleEdit(p.id)}>Editar</button>
                    {p.activo ? (
                      <button onClick={() => { if(confirm('¿Inactivar producto del catálogo?')) statusMutation.mutate({id: p.id, action: 'delete'}) }} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-danger)' }}>Inactivar</button>
                    ) : (
                      <button onClick={() => statusMutation.mutate({id: p.id, action: 'reactivar'})} className="btn btn-ghost btn-sm" style={{ color: 'var(--color-success)' }}>Reactivar</button>
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
                    <div className="modal-title">{editId ? 'Editar Producto' : 'Nuevo Producto'}</div>
                    <div className="modal-subtitle">Complete la ficha técnica del producto.</div>
                </div>
                <button className="modal-close" onClick={() => setIsModalOpen(false)}>×</button>
            </div>
            
            <div className="modal-body">
              <form id="prodForm" onSubmit={handleSubmit} className="form-grid">
                
                <div className="grid-2 mb-4">
                  <div style={{ gridColumn: 'span 2' }}>
                    <label className="form-label">Nombre del Producto *</label>
                    <input required className="form-input" value={formData.nombre} onChange={e => setFormData({ ...formData, nombre: e.target.value })} />
                  </div>
                  
                  <div>
                    <label className="form-label">Código Interno</label>
                    <input className="form-input" value={formData.codigoInterno} onChange={e => setFormData({ ...formData, codigoInterno: e.target.value })} />
                  </div>
                  <div>
                    <label className="form-label">SKU / Cód. Barras</label>
                    <input className="form-input" value={formData.sku} onChange={e => setFormData({ ...formData, sku: e.target.value })} />
                  </div>
                  
                  <div>
                    <label className="form-label">Categoría</label>
                    <select className="form-input" value={formData.categoriaId} onChange={e => setFormData({...formData, categoriaId: e.target.value})}>
                      <option value="">-- Sin Categoría --</option>
                      {categorias.map((c: any) => <option key={c.id} value={c.id}>{c.nombre}</option>)}
                    </select>
                  </div>
                  <div>
                    <label className="form-label">Proveedor Habitual</label>
                    <select className="form-input" value={formData.proveedorId} onChange={e => setFormData({...formData, proveedorId: e.target.value})}>
                      <option value="">-- Sin Proveedor --</option>
                      {proveedores.map((p: any) => <option key={p.id} value={p.id}>{p.razonSocial}</option>)}
                    </select>
                  </div>
                </div>

                <div className="grid-3 mb-4" style={{ background: 'var(--color-surface-2)', padding: 15, borderRadius: 8 }}>
                   <div>
                     <label className="form-label">Precio Costo ($) *</label>
                     <input required type="number" className="form-input" value={formData.costoUnitario} onChange={e => setFormData({ ...formData, costoUnitario: Number(e.target.value) })} />
                   </div>
                   <div>
                     <label className="form-label">P. Venta Público ($) *</label>
                     <input required type="number" className="form-input" value={formData.precioVenta} onChange={e => setFormData({ ...formData, precioVenta: Number(e.target.value) })} />
                   </div>
                   <div>
                     <label className="form-label">P. Venta Mayorista ($)</label>
                     <input type="number" className="form-input" value={formData.precioVentaMayorista} onChange={e => setFormData({ ...formData, precioVentaMayorista: Number(e.target.value) })} />
                   </div>
                </div>

                <div className="grid-3 mb-4">
                    {!editId && (
                        <div>
                        <label className="form-label">Stock Inicial *</label>
                        <input required type="number" className="form-input" value={formData.stockInicial} onChange={e => setFormData({ ...formData, stockInicial: Number(e.target.value) })} />
                        </div>
                    )}
                   <div>
                     <label className="form-label">Stock Mínimo *</label>
                     <input required type="number" className="form-input" value={formData.stockMinimo} onChange={e => setFormData({ ...formData, stockMinimo: Number(e.target.value) })} />
                   </div>
                   <div>
                     <label className="form-label">Stock Máximo</label>
                     <input type="number" className="form-input" value={formData.stockMaximo} onChange={e => setFormData({ ...formData, stockMaximo: Number(e.target.value) })} />
                   </div>
                   <div>
                       <label className="form-label">Unidad de Medida</label>
                       <select className="form-input" value={formData.unidadMedidaId} onChange={e => setFormData({...formData, unidadMedidaId: e.target.value})}>
                           <option value="">-- Seleccionar --</option>
                           {unidades.map((u: any) => (
                             <option key={u.id} value={u.id}>{u.nombre} ({u.simbolo})</option>
                           ))}
                       </select>
                   </div>
                </div>

                <div className="mb-4">
                  <label className="flex items-center gap-2 cursor-pointer text-sm" style={{ color: 'var(--text-primary)' }}>
                    <input type="checkbox" checked={formData.permiteVentaSinStock} onChange={e => setFormData({...formData, permiteVentaSinStock: e.target.checked})} className="form-input" style={{ width: 18, height: 18 }} />
                    Permitir facturación sin stock disponible (Venta en negativo)
                  </label>
                </div>

                <div className="mb-4">
                  <label className="form-label">Imagen Referencial</label>
                  <div style={{ display: 'flex', gap: 15, alignItems: 'center' }}>
                     <label className="img-upload-box" style={{ width: 100, height: 100, margin: 0 }}>
                       {formData.foto ? <img src={formData.foto} alt="prod" /> : <div className="upload-placeholder" style={{ fontSize: 24 }}>📸</div>}
                       <input type="file" accept="image/*" onChange={handleFileChange} style={{ display: 'none' }} />
                     </label>
                     <span style={{ fontSize: 12, color: 'var(--text-muted)' }}>Click en el recuadro para seleccionar imagen.<br/>Recomendado: 400x400px en formato JPG/PNG.</span>
                  </div>
                </div>

              </form>
            </div>
            <div className="modal-footer">
                <button type="button" className="btn btn-ghost" onClick={() => setIsModalOpen(false)}>Cancelar</button>
                <button type="submit" form="prodForm" className="btn btn-primary" disabled={createMutation.isPending || updateMutation.isPending}>
                    {editId ? 'Guardar Cambios' : 'Registrar Producto'}
                </button>
            </div>
          </div>
        </div>
      )}
    </>
  )
}

function FacturaTab({ onComplete }: { onComplete: () => void }) {
  const queryClient = useQueryClient()
  const [numero, setNumero] = useState('')
  const [proveedorId, setProveedorId] = useState('')
  const [items, setItems] = useState<any[]>([])

  const [selProdId, setSelProdId] = useState('')
  const [selCantidad, setSelCantidad] = useState(1)
  const [selCosto, setSelCosto] = useState(0)
  const [actualizarCostos, setActualizarCostos] = useState(false)

  const { data: productos = [] } = useQuery({ queryKey: ['productos'], queryFn: () => inventarioApi.getProductos({ soloActivos: true }).then(r => r.data) })
  const { data: proveedores = [] } = useQuery({ queryKey: ['proveedores'], queryFn: () => proveedoresApi.getAll({ soloActivos: true }).then(r => r.data) })

  const handleAgregarItem = () => {
    if (!selProdId || selCantidad <= 0) return;
    const prod = productos.find((p: any) => p.id === selProdId)
    // Check if already in list to update quantity instead
    const existingIndex = items.findIndex(i => i.productoId === selProdId)
    if (existingIndex >= 0) {
        const newItems = [...items];
        newItems[existingIndex].cantidad += selCantidad;
        newItems[existingIndex].costoUnitario = selCosto; // Update to latest cost input
        setItems(newItems)
    } else {
        setItems([...items, { productoId: selProdId, nombre: prod?.nombre, cantidad: selCantidad, costoUnitario: selCosto }])
    }
    setSelProdId('')
    setSelCantidad(1)
  }

  const submitMutation = useMutation({
    mutationFn: () => inventarioApi.cargarFactura({ numeroFactura: numero, proveedorId, items }), 
    // TODO: support actualizarCostos in API call if backend supports it. The backend DOES support it in the refactored code.
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success('Factura registrada y stock actualizado')
      onComplete()
    },
    onError: () => {
        toast.error('Error al procesar la factura')
    }
  })

  // We need to inject actualizarCostos in the mutation if supported. Our backend handler has bool ActualizarCostos.
  const handleFinalizar = () => {
      inventarioApi.cargarFactura({ numeroFactura: numero, proveedorId, items, actualizarCostos } as any).then(() => {
         queryClient.invalidateQueries({ queryKey: ['productos'] })
         toast.success('Factura registrada y stock actualizado')
         onComplete()
      }).catch(() => toast.error('Error al procesar la factura'));
  }

  const subtotalFactura = items.reduce((acc, it) => acc + (it.cantidad * it.costoUnitario), 0);

  return (
    <div className="surface-card" style={{ maxWidth: 900 }}>
      <h3 style={{ marginTop: 0, marginBottom: 'var(--space-6)', color: 'var(--text-primary)' }}>Registrar Ingreso de Mercadería</h3>
      
      <div className="grid-2 mb-6">
        <div>
          <label className="form-label">Número de Comprobante / Factura *</label>
          <input required className="form-input" placeholder="Ej: 0001-00004561" value={numero} onChange={e => setNumero(e.target.value)} />
        </div>
        <div>
          <label className="form-label">Proveedor Emisor *</label>
          <select required className="form-input" value={proveedorId} onChange={e => setProveedorId(e.target.value)}>
             <option value="">-- Seleccionar Proveedor --</option>
             {proveedores.map((p: any) => <option key={p.id} value={p.id}>{p.razonSocial} (CUIT: {p.cuit})</option>)}
          </select>
        </div>
      </div>

      <div style={{ background: 'var(--color-surface-2)', padding: 'var(--space-4)', borderRadius: 8, marginBottom: 'var(--space-6)', border: '1px solid var(--color-border)' }}>
        <h4 style={{ margin: '0 0 var(--space-4) 0', fontSize: 13, color: 'var(--color-primary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Agregar Artículos</h4>
        <div style={{ display: 'flex', gap: 10, alignItems: 'flex-end' }}>
          <div style={{ flex: 3 }}>
            <label className="form-label">Producto del Catálogo</label>
            <select className="form-input" value={selProdId} onChange={e => {
              setSelProdId(e.target.value);
              const p = productos.find((x: any) => x.id === e.target.value);
              if (p) setSelCosto(p.costoUnitario);
            }}>
              <option value="">-- Buscar Producto --</option>
              {productos.map((p: any) => <option key={p.id} value={p.id}>{p.nombre} (Stock Act: {p.stockActual})</option>)}
            </select>
          </div>
          <div style={{ flex: 1 }}>
            <label className="form-label">Cantidad A Ingresar</label>
            <input type="number" min="1" className="form-input" value={selCantidad} onChange={e => setSelCantidad(Number(e.target.value))} />
          </div>
          <div style={{ flex: 1 }}>
            <label className="form-label">Costo Unit. ($)</label>
            <input type="number" step="0.01" min="0" className="form-input" value={selCosto} onChange={e => setSelCosto(Number(e.target.value))} />
          </div>
          <div>
            <button type="button" className="btn btn-secondary" onClick={handleAgregarItem} disabled={!selProdId || selCantidad <= 0}>Añadir</button>
          </div>
        </div>
      </div>

      <table className="cierre-table" style={{ width: '100%', marginBottom: 'var(--space-6)' }}>
        <thead>
          <tr>
            <th>Producto</th>
            <th style={{ textAlign: 'center' }}>Cantidad</th>
            <th style={{ textAlign: 'right' }}>Costo Unit.</th>
            <th style={{ textAlign: 'right' }}>Subtotal</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items.map((it, i) => (
            <tr key={it.productoId}>
              <td style={{ fontWeight: 500, color: 'var(--text-primary)' }}>{it.nombre}</td>
              <td style={{ textAlign: 'center' }}>{it.cantidad}</td>
              <td style={{ textAlign: 'right' }}>${it.costoUnitario.toLocaleString()}</td>
              <td style={{ textAlign: 'right', fontWeight: 600 }}>${(it.cantidad * it.costoUnitario).toLocaleString()}</td>
              <td style={{ textAlign: 'center' }}>
                <button type="button" className="btn btn-ghost btn-sm" style={{ color: 'var(--color-danger)', fontSize: 16 }} onClick={() => setItems(items.filter((_, idx) => idx !== i))}>×</button>
              </td>
            </tr>
          ))}
          {items.length === 0 && <tr><td colSpan={5} style={{ textAlign: 'center', padding: 40, color: 'var(--text-muted)' }}>Sin artículos. Seleccione e incorpore ítems de la factura.</td></tr>}
          
          {items.length > 0 && (
             <tr style={{ background: 'var(--color-surface-2)', borderTop: '2px solid var(--color-border)' }}>
                <td colSpan={3} style={{ textAlign: 'right', fontWeight: 700, fontSize: 15, padding: 'var(--space-3)' }}>TOTAL FACTURADO:</td>
                <td style={{ textAlign: 'right', fontWeight: 700, fontSize: 16, color: 'var(--color-primary)', padding: 'var(--space-3)' }}>${subtotalFactura.toLocaleString()}</td>
                <td></td>
             </tr>
          )}
        </tbody>
      </table>

      <div className="flex-between">
          <label className="flex items-center gap-2 cursor-pointer text-sm" style={{ color: 'var(--color-warning)', fontWeight: 500 }}>
             <input type="checkbox" checked={actualizarCostos} onChange={e => setActualizarCostos(e.target.checked)} className="form-input" style={{ width: 18, height: 18 }} />
             ⚠️ Actualizar costos unitarios en el catálogo base
          </label>
          <button className="btn btn-primary" style={{ padding: '0 var(--space-8)' }} disabled={items.length === 0 || !numero || !proveedorId} onClick={handleFinalizar}>
             Registrar Factura
          </button>
      </div>
    </div>
  )
}
