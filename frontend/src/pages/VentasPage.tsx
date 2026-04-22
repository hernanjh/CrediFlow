import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { inventarioApi, ventasApi, clientesApi, parametricasApi } from '../api/client'
import { toast } from 'react-hot-toast'
import AutocompleteInput from '../components/ui/AutocompleteInput'
import { useAuthStore } from '../store/authStore'

export default function VentasPage() {
  const [activeTab, setActiveTab] = useState<'HISTORIAL' | 'POS'>('HISTORIAL')

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Módulo de Facturación</h1>
          <p className="page-subtitle">Emisión de comprobantes y gestión de ventas al contado</p>
        </div>
        <div className="flex-gap-2">
           <button 
            className={`btn ${activeTab === 'HISTORIAL' ? 'btn-primary' : 'btn-ghost'}`}
            onClick={() => setActiveTab('HISTORIAL')}
          >
            📋 Ver Historial
          </button>
          <button 
            className={`btn ${activeTab === 'POS' ? 'btn-primary' : 'btn-ghost'}`}
            onClick={() => setActiveTab('POS')}
          >
            🛒 Nueva Venta (POS)
          </button>
        </div>
      </div>

      <div className="mt-6">
        {activeTab === 'HISTORIAL' ? <ListadoVentasTab /> : <PointOfSaleTab onComplete={() => setActiveTab('HISTORIAL')} />}
      </div>
    </div>
  )
}

function ListadoVentasTab() {
  const { data: ventas = [], isLoading } = useQuery({
    queryKey: ['ventas-recientes'],
    queryFn: () => ventasApi.getRecientes().then(r => r.data)
  })

  return (
    <div className="data-table-wrap">
      {isLoading ? (
        <div className="loading-overlay"><span className="spinner" /> Cargando historial...</div>
      ) : (
        <table className="data-table">
          <thead>
            <tr>
              <th>ID Transacción</th>
              <th>Fecha y Hora</th>
              <th>Cliente / Razón Social</th>
              <th>Forma Cobro</th>
              <th className="text-right">Monto Total</th>
              <th className="text-right">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {ventas.map((v: any) => (
              <tr key={v.id}>
                <td className="font-mono text-xs text-muted">{v.id.substring(0,8)}</td>
                <td>{new Date(v.fecha).toLocaleString()}</td>
                <td className="font-bold">{v.clienteNombre || 'Consumidor Final'}</td>
                <td>
                  <span className="badge badge-neutral">{v.formaCobro || 'Efectivo'}</span>
                </td>
                <td className="text-right font-bold text-primary">${v.total.toLocaleString()}</td>
                <td className="text-right">
                  <div className="flex-gap-2 justify-end">
                    <button className="btn btn-ghost btn-sm" title="Imprimir Comprobante">🖨️</button>
                    <button className="btn btn-ghost btn-sm">Ver Detalle</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}

function PointOfSaleTab({ onComplete }: { onComplete: () => void }) {
  const queryClient = useQueryClient()
  const { usuario } = useAuthStore()
  const [cart, setCart] = useState<any[]>([])
  const [selectedCliente, setSelectedCliente] = useState<any>(null)
  const [formaCobroId, setFormaCobroId] = useState('')

  const { data: productos = [] } = useQuery({ 
    queryKey: ['productos-activos'], 
    queryFn: () => inventarioApi.getProductos({ soloActivos: true }).then(r => r.data) 
  })

  const { data: formasCobro = [] } = useQuery({
    queryKey: ['formas-cobro-ventas'],
    queryFn: () => parametricasApi.getFormasCobro(true).then(r => r.data)
  })

  const mutation = useMutation({
    mutationFn: (data: any) => ventasApi.registrarContado(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ventas-recientes'] })
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success('Comprobante emitido correctamente')
      onComplete()
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Error al procesar la venta')
  })

  const addToCart = (p: any) => {
    const existing = cart.find(i => i.id === p.id)
    if (existing) {
      setCart(cart.map(i => i.id === p.id ? { ...i, cantidad: i.cantidad + 1 } : i))
    } else {
      setCart([...cart, { ...p, cantidad: 1 }])
    }
  }

  const handleFinalizar = () => {
    if (cart.length === 0) return
    mutation.mutate({
      vendedorId: usuario?.id,
      clienteId: selectedCliente?.id,
      formaCobroId: formaCobroId || null,
      items: cart.map(i => ({
        productoId: i.id,
        cantidad: i.cantidad,
        precioUnitario: i.precioVenta
      }))
    })
  }

  const total = cart.reduce((sum, i) => sum + (i.precioVenta * i.cantidad), 0)

  return (
    <div className="grid-2" style={{ gridTemplateColumns: '1fr 420px', gap: '24px' }}>
      
      {/* SECTOR DE CARGA */}
      <div className="flex-column gap-6">
        <div className="card">
          <div className="flex-between mb-4">
            <h3 className="font-bold">Items de Factura</h3>
            <div style={{ width: '300px' }}>
              <AutocompleteInput 
                placeholder="🔍 Escanear o buscar producto..."
                onChange={(id, opt) => {
                  if (opt) {
                    const p = productos.find((x: any) => x.id === id)
                    if (p) addToCart(p)
                  }
                }}
                fetchOptions={async (q) => {
                  return productos.filter((p: any) => p.nombre.toLowerCase().includes(q.toLowerCase()) || p.codigo?.includes(q))
                    .map((p: any) => ({
                      id: p.id,
                      label: p.nombre,
                      subLabel: `Stock: ${p.stockActual} | $${p.precioVenta}`
                    }))
                }}
              />
            </div>
          </div>

          <div className="data-table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th className="text-center" style={{ width: '100px' }}>Cantidad</th>
                  <th className="text-right">Unitario</th>
                  <th className="text-right">Subtotal</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {cart.map(i => (
                  <tr key={i.id}>
                    <td>
                      <div className="font-bold">{i.nombre}</div>
                      <div className="text-xs text-muted">ID: {i.id.substring(0,8)}</div>
                    </td>
                    <td className="text-center">
                      <div className="flex-center gap-2">
                        <button className="btn-icon btn-sm" onClick={() => setCart(cart.map(c => c.id === i.id ? { ...c, cantidad: Math.max(1, c.cantidad - 1) } : c))}>-</button>
                        <span className="font-bold">{i.cantidad}</span>
                        <button className="btn-icon btn-sm" onClick={() => setCart(cart.map(c => c.id === i.id ? { ...c, cantidad: c.cantidad + 1 } : c))}>+</button>
                      </div>
                    </td>
                    <td className="text-right">${i.precioVenta.toLocaleString()}</td>
                    <td className="text-right font-bold">${(i.cantidad * i.precioVenta).toLocaleString()}</td>
                    <td className="text-right">
                      <button className="btn-icon text-danger" onClick={() => setCart(cart.filter(c => c.id !== i.id))}>&times;</button>
                    </td>
                  </tr>
                ))}
                {cart.length === 0 && (
                  <tr>
                    <td colSpan={5} className="text-center py-12 text-muted italic">
                       No hay productos en el carrito. Utilice el buscador para comenzar.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* PANEL DE CONTROL / CHECKOUT */}
      <div className="flex-column gap-6 sticky-top">
        <div className="card border-primary-glow" style={{ border: '2px solid var(--color-primary-glow)' }}>
          <h3 className="mb-6 font-bold flex-between">
            Resumen de Cobro
            <span className="badge badge-primary">Factura Contado</span>
          </h3>
          
          <div className="form-group mb-6">
            <label className="form-label">Cliente / Receptor</label>
            <AutocompleteInput 
              placeholder="Consumidor Final"
              onChange={(id, opt) => setSelectedCliente(opt ? { id, label: opt.label } : null)}
              fetchOptions={async (q) => {
                const res = await clientesApi.getAll({ search: q })
                return res.data.map((c: any) => ({ id: c.id, label: c.nombreCompleto }))
              }}
            />
          </div>

          <div className="form-group mb-8">
            <label className="form-label">Método de Pago</label>
            <select className="form-input" value={formaCobroId} onChange={e => setFormaCobroId(e.target.value)}>
              <option value="">Seleccione forma de cobro...</option>
              {formasCobro.map((f: any) => (
                <option key={f.id} value={f.id}>{f.nombre}</option>
              ))}
            </select>
          </div>

          <div className="bg-surface-2 p-6 rounded-lg mb-8">
            <div className="flex-between mb-2">
              <span className="text-muted text-sm">Subtotal</span>
              <span className="font-bold">${total.toLocaleString()}</span>
            </div>
            <div className="flex-between mb-4 border-b pb-4">
              <span className="text-muted text-sm">Impuestos (Incl.)</span>
              <span className="font-bold">$0.00</span>
            </div>
            <div className="flex-between">
              <span className="text-lg font-extrabold uppercase">Total</span>
              <span className="text-3xl font-extrabold text-primary">${total.toLocaleString()}</span>
            </div>
          </div>

          <button 
            className="btn btn-primary btn-lg w-full py-5 text-lg shadow-lg"
            disabled={cart.length === 0 || mutation.isPending || !formaCobroId}
            onClick={handleFinalizar}
          >
            {mutation.isPending ? 'Procesando Transacción...' : '✅ Emitir y Registrar Venta'}
          </button>
          
          {!formaCobroId && cart.length > 0 && (
            <p className="text-xs text-center text-danger mt-3 font-bold">
              ⚠ Por favor, seleccione un método de pago.
            </p>
          )}
        </div>
      </div>
    </div>
  )
}

