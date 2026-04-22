import { useQuery } from '@tanstack/react-query'
import { inventarioApi } from '../api/client'
function ListaPreciosPage() {
  const { data: productos, isLoading } = useQuery({
    queryKey: ['productos'],
    queryFn: () => inventarioApi.getProductos().then(r => r.data)
  })

  // Agrupar productos por categoría
  const productosPorCategoria = productos?.reduce((acc: any, prod: any) => {
    const cat = prod.categoriaNombre || 'Sin Categoría'
    if (!acc[cat]) acc[cat] = []
    acc[cat].push(prod)
    return acc
  }, {})

  return (
    <div className="page-content">
      <div className="bg-slate-800 p-6 rounded-xl shadow-lg border border-slate-700 min-h-screen">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-semibold text-white">Tarifario Actual</h2>
          <button onClick={() => window.print()} className="bg-slate-700 hover:bg-slate-600 px-4 py-2 rounded-lg text-slate-300 print:hidden transition-colors">
            🖨 Imprimir
          </button>
        </div>

        {isLoading ? (
          <div className="text-slate-400 text-center py-8">Cargando precios...</div>
        ) : (
          <div className="space-y-8">
            {Object.keys(productosPorCategoria || {}).map(categoria => (
              <div key={categoria} className="break-inside-avoid">
                <h3 className="text-lg font-medium text-indigo-400 border-b border-indigo-500/30 pb-2 mb-4">
                  {categoria}
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {productosPorCategoria[categoria].map((prod: any) => (
                    <div key={prod.id} className="bg-slate-900 border border-slate-700 p-4 rounded-lg flex justify-between items-center gap-4">
                      <div className="flex-1">
                        <h4 className="font-medium text-white">{prod.nombre}</h4>
                        <p className="text-sm text-slate-400 truncate">{prod.descripcion || 'Sin descripción'}</p>
                      </div>
                      <div className="text-right">
                        <div className="text-lg font-bold text-emerald-400">
                          ${prod.precioVenta.toLocaleString('es-AR', { minimumFractionDigits: 2 })}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ))}
            {(!productos || productos.length === 0) && (
              <div className="text-center text-slate-500 py-12">No hay productos disponibles.</div>
            )}
          </div>
        )}
      </div>
    </div>
  )
}

export default ListaPreciosPage
