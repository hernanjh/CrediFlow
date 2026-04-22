import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { inventarioApi, configuracionApi } from '../api/client'
import { toast } from 'react-hot-toast'
import AutocompleteInput from '../components/ui/AutocompleteInput'

export default function GestionPreciosPage() {
  const queryClient = useQueryClient()
  const [selectedLista, setSelectedLista] = useState<any>(null)
  const [isListModalOpen, setIsListModalOpen] = useState(false)
  const [isItemModalOpen, setIsItemModalOpen] = useState(false)
  
  const [listFormData, setListFormData] = useState({ nombre: '', porcentajeAjuste: 0, tipo: 'GLOBAL' })
  const [itemFormData, setItemFormData] = useState({ productoId: '', label: '', precioFijo: '', porcentajeOverride: '' })

  const { data: listas = [], isLoading } = useQuery({
    queryKey: ['listas-precios'],
    queryFn: () => configuracionApi.getListasPrecios().then(r => r.data)
  })

  const { data: productos = [] } = useQuery({
    queryKey: ['productos-precios'],
    queryFn: () => inventarioApi.getProductos({ soloActivos: true }).then(r => r.data)
  })

  const upsertMutation = useMutation({
    mutationFn: (data: any) => {
        if (selectedLista?.id) return configuracionApi.updateLista(selectedLista.id, { ...data, id: selectedLista.id })
        return configuracionApi.createLista(data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listas-precios'] })
      setIsListModalOpen(false)
      toast.success('Configuración de lista guardada')
    }
  })

  const addItemMutation = useMutation({
    mutationFn: (data: any) => configuracionApi.addListItem(selectedLista.id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listas-precios'] })
      setIsItemModalOpen(false)
      setItemFormData({ productoId: '', label: '', precioFijo: '', porcentajeOverride: '' })
      toast.success('Excepción de precio aplicada')
    }
  })

  const removeItemMutation = useMutation({
    mutationFn: (id: string) => configuracionApi.removeListItem(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['listas-precios'] })
      toast.success('Excepción eliminada')
    }
  })

  const handleEditList = (lista: any) => {
    setSelectedLista(lista)
    setListFormData({ nombre: lista.nombre, porcentajeAjuste: lista.porcentajeAjuste, tipo: lista.tipo || 'GLOBAL' })
    setIsListModalOpen(true)
  }

  const handleAddItem = (id: string, opt: any) => {
    setItemFormData({ ...itemFormData, productoId: id, label: opt.label })
    setIsItemModalOpen(true)
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Gestión de Tarifarios</h1>
          <p className="page-subtitle">Configure márgenes de rentabilidad y precios diferenciales por lista</p>
        </div>
        <button onClick={() => { setSelectedLista(null); setListFormData({ nombre: '', porcentajeAjuste: 0, tipo: 'GLOBAL' }); setIsListModalOpen(true); }} className="btn btn-primary">
          + Nueva Lista de Precios
        </button>
      </div>

      <div className="grid-2" style={{ gridTemplateColumns: '380px 1fr', gap: '32px', alignItems: 'start' }}>
        
        {/* PANEL IZQUIERDO: LISTAS */}
        <div className="flex-column gap-4">
          <h3 className="text-sm font-bold uppercase text-muted mb-2">Listas de Precios Activas</h3>
          {isLoading ? (
            <div className="loading-overlay"><span className="spinner" /> Cargando tarifarios...</div>
          ) : listas.map((l: any) => (
            <div 
              key={l.id} 
              className={`card hover-shadow ${selectedLista?.id === l.id ? 'border-primary' : ''}`} 
              onClick={() => setSelectedLista(l)} 
              style={{ cursor: 'pointer', transition: 'all 0.2s' }}
            >
              <div className="flex-between">
                <div>
                  <div className="font-bold text-primary">{l.nombre}</div>
                  <div className="text-xs text-muted mt-1">Estrategia: <span className="font-bold">{l.tipo}</span></div>
                </div>
                <div className="text-right">
                  <div className={`text-xl font-black ${l.porcentajeAjuste >= 0 ? 'text-primary' : 'text-success'}`}>
                    {l.porcentajeAjuste > 0 ? `+${l.porcentajeAjuste}%` : `${l.porcentajeAjuste}%`}
                  </div>
                  <button className="btn-icon btn-sm mt-2" onClick={(e) => { e.stopPropagation(); handleEditList(l); }}>⚙️</button>
                </div>
              </div>
            </div>
          ))}
          {listas.length === 0 && <p className="text-sm italic text-muted text-center py-8">No hay listas configuradas.</p>}
        </div>

        {/* PANEL DERECHO: DETALLE Y EXCEPCIONES */}
        <div className="flex-column gap-6">
          {selectedLista ? (
            <div className="card">
              <div className="flex-between mb-6">
                <div>
                  <h3 className="font-bold">Excepciones y Precios Fijos</h3>
                  <p className="text-xs text-muted mt-1">Defina precios específicos para productos en la lista "{selectedLista.nombre}"</p>
                </div>
                <div style={{ width: '320px' }}>
                  <AutocompleteInput 
                    placeholder="🔍 Buscar producto para excepción..."
                    onChange={handleAddItem}
                    fetchOptions={async (q) => {
                      return productos.filter((p: any) => p.nombre.toLowerCase().includes(q.toLowerCase()))
                        .map((p: any) => ({ id: p.id, label: p.nombre, subLabel: `Precio Base: $${p.precioVenta}` }));
                    }}
                  />
                </div>
              </div>
              
              <div className="data-table-wrap">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Producto</th>
                      <th>Tipo de Excepción</th>
                      <th className="text-right">Valor Aplicado</th>
                      <th />
                    </tr>
                  </thead>
                  <tbody>
                    {selectedLista.items?.map((item: any) => (
                      <tr key={item.id}>
                        <td className="font-bold">{item.productoNombre}</td>
                        <td>
                          {item.precioFijo ? (
                            <span className="badge badge-info">Precio Fijo</span>
                          ) : (
                            <span className="badge badge-warning">Ajuste Manual</span>
                          )}
                        </td>
                        <td className="text-right font-bold text-primary">
                          {item.precioFijo ? `$${item.precioFijo.toLocaleString()}` : `${item.porcentajeOverride}%`}
                        </td>
                        <td className="text-right">
                          <button className="btn-icon text-danger" onClick={() => removeItemMutation.mutate(item.id)}>&times;</button>
                        </td>
                      </tr>
                    ))}
                    {(!selectedLista.items || selectedLista.items.length === 0) && (
                      <tr>
                        <td colSpan={4} className="text-center py-12 text-muted italic">
                          No hay excepciones configuradas para esta lista.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          ) : (
            <div className="empty-state card py-24">
              <span className="empty-state-icon" style={{ fontSize: '4rem' }}>🏷️</span>
              <div className="empty-state-title">Seleccione una Lista</div>
              <p className="empty-state-subtitle">Elija un tarifario de la izquierda para gestionar sus excepciones de precio.</p>
            </div>
          )}
        </div>
      </div>

      {/* MODAL: EDICIÓN DE LISTA */}
      {isListModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content modal-sm">
            <div className="modal-header">
              <h3 className="modal-title">{selectedLista ? 'Editar' : 'Nueva'} Lista de Precios</h3>
              <button className="modal-close" onClick={() => setIsListModalOpen(false)}>&times;</button>
            </div>
            <div className="modal-body">
              <form onSubmit={e => { e.preventDefault(); upsertMutation.mutate(listFormData); }} className="flex-column gap-5">
                <div className="form-group">
                  <label className="form-label">Nombre Identificador</label>
                  <input required className="form-input" value={listFormData.nombre} onChange={e => setListFormData({ ...listFormData, nombre: e.target.value })} placeholder="Ej: Lista Minorista Especial" />
                </div>
                <div className="grid-2 gap-4">
                  <div className="form-group">
                    <label className="form-label">Ajuste Global (%)</label>
                    <input required type="number" step="0.1" className="form-input" value={listFormData.porcentajeAjuste} onChange={e => setListFormData({ ...listFormData, porcentajeAjuste: Number(e.target.value) })} />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Estrategia</label>
                    <select className="form-input" value={listFormData.tipo} onChange={e => setListFormData({ ...listFormData, tipo: e.target.value })}>
                      <option value="GLOBAL">Global</option>
                      <option value="POR_CATEGORIA">Categoría</option>
                      <option value="POR_PRODUCTO">Producto</option>
                      <option value="MIX">Mixta</option>
                    </select>
                  </div>
                </div>
                <div className="modal-footer mt-4" style={{ margin: '0 -24px -24px -24px' }}>
                  <button type="button" className="btn btn-ghost" onClick={() => setIsListModalOpen(false)}>Cancelar</button>
                  <button type="submit" className="btn btn-primary" disabled={upsertMutation.isPending}>Guardar Tarifario</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* MODAL: AGREGAR ITEM (EXCEPCIÓN) */}
      {isItemModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content modal-sm">
            <div className="modal-header">
              <h3 className="modal-title">Configurar Excepción</h3>
              <button className="modal-close" onClick={() => setIsItemModalOpen(false)}>&times;</button>
            </div>
            <div className="modal-body">
              <p className="text-sm text-muted mb-6">Para <b>{itemFormData.label}</b>, elija un precio fijo o un porcentaje de ajuste individual.</p>
              <form onSubmit={e => { 
                e.preventDefault(); 
                addItemMutation.mutate({
                  listaPrecioId: selectedLista.id,
                  productoId: itemFormData.productoId,
                  precioFijo: itemFormData.precioFijo ? Number(itemFormData.precioFijo) : null,
                  porcentajeOverride: itemFormData.porcentajeOverride ? Number(itemFormData.porcentajeOverride) : null
                });
              }} className="flex-column gap-5">
                <div className="form-group">
                  <label className="form-label">Precio Fijo ($)</label>
                  <input 
                    type="number" 
                    className="form-input" 
                    value={itemFormData.precioFijo} 
                    onChange={e => setItemFormData({ ...itemFormData, precioFijo: e.target.value, porcentajeOverride: '' })} 
                    placeholder="Ej: 1500"
                    disabled={!!itemFormData.porcentajeOverride}
                  />
                </div>
                <div className="text-center text-xs font-bold text-muted">— O BIEN —</div>
                <div className="form-group">
                  <label className="form-label">Porcentaje de Ajuste (%)</label>
                  <input 
                    type="number" 
                    className="form-input" 
                    value={itemFormData.porcentajeOverride} 
                    onChange={e => setItemFormData({ ...itemFormData, porcentajeOverride: e.target.value, precioFijo: '' })} 
                    placeholder="Ej: 25"
                    disabled={!!itemFormData.precioFijo}
                  />
                </div>
                <div className="modal-footer mt-4" style={{ margin: '0 -24px -24px -24px' }}>
                  <button type="button" className="btn btn-ghost" onClick={() => setIsItemModalOpen(false)}>Cancelar</button>
                  <button type="submit" className="btn btn-primary" disabled={addItemMutation.isPending}>Aplicar Excepción</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

