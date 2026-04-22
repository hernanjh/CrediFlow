import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ventasApi, inventarioApi, reportesApi } from '../api/client'
import { exportToExcel, exportToPDF } from '../utils/exportUtils'
import { toast } from 'react-hot-toast'

type ReportType = 'VENTAS' | 'STOCK' | 'DEUDORES';

export default function ReportesPage() {
  const [selectedReport, setSelectedReport] = useState<ReportType>('VENTAS')
  
  // Queries for data
  const { data: ventas = [], isLoading: loadingVentas } = useQuery({ 
    queryKey: ['reporte-ventas'], 
    queryFn: () => ventasApi.getRecientes().then(r => r.data) 
  })
  
  const { data: productos = [], isLoading: loadingStock } = useQuery({ 
    queryKey: ['reporte-stock'], 
    queryFn: () => inventarioApi.getProductos().then(r => r.data) 
  })

  const { data: deudores = [], isLoading: loadingDeudores } = useQuery({
    queryKey: ['reporte-deudores'],
    queryFn: () => reportesApi.getDeudores().then(r => r.data)
  })

  const handleExportExcel = () => {
    try {
        let data: any[] = []
        let filename = 'reporte'
        
        if (selectedReport === 'VENTAS') {
            data = ventas.map((v: any) => ({ Fecha: new Date(v.fecha).toLocaleDateString(), Cliente: v.clienteNombre || 'Consumidor Final', Total: v.total, Pago: v.formaCobro || 'Efectivo' }))
            filename = 'Ventas_CrediFlow'
        } else if (selectedReport === 'STOCK') {
            data = productos.map((p: any) => ({ Producto: p.nombre, SKU: p.codigo, Stock: p.stockActual, Precio: p.precioVenta }))
            filename = 'Inventario_CrediFlow'
        } else {
            data = deudores
            filename = 'Deudores_CrediFlow'
        }
        
        exportToExcel(data, filename)
        toast.success('Archivo Excel generado')
    } catch (e) {
        toast.error('Error al exportar Excel')
    }
  }

  const handleExportPDF = () => {
    try {
        if (selectedReport === 'VENTAS') {
            const headers = ['Fecha', 'Cliente', 'Cobro', 'Monto']
            const body = ventas.map((v: any) => [
                new Date(v.fecha).toLocaleDateString(), 
                v.clienteNombre || 'Consumidor Final', 
                v.formaCobro || 'Efectivo', 
                `$${v.total.toLocaleString()}`
            ])
            exportToPDF(headers, body, 'Reporte Histórico de Ventas', 'Ventas', 'portrait')
        } else if (selectedReport === 'STOCK') {
            const headers = ['Producto', 'Código', 'Stock', 'Precio']
            const body = productos.map((p: any) => [p.nombre, p.codigo || '-', p.stockActual, `$${p.precioVenta.toLocaleString()}`])
            exportToPDF(headers, body, 'Reporte de Inventario Actual', 'Stock', 'portrait')
        } else {
            const headers = ['Cliente', 'Zona', 'Deuda', 'Mora']
            const body = deudores.map(d => [d.cliente, d.zona, `$${d.deuda.toLocaleString()}`, d.mora])
            exportToPDF(headers, body, 'Reporte de Cartera y Morosidad', 'Deudores', 'portrait')
        }
        toast.success('Reporte PDF descargado')
    } catch (e) {
        toast.error('Error al generar PDF')
    }
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Centro de Reportes</h1>
          <p className="page-subtitle">Generación y descarga de informes transaccionales en tiempo real</p>
        </div>
      </div>

      <div className="grid-2" style={{ gridTemplateColumns: 'minmax(240px, 340px) 1fr' }}>
        
        {/* CATALOGO */}
        <div className="flex-column gap-3">
          <h3 className="text-xs font-bold uppercase text-muted mb-2">Informes Disponibles</h3>
          <button 
            className={`card hover-shadow ${selectedReport === 'VENTAS' ? 'border-primary' : ''}`}
            onClick={() => setSelectedReport('VENTAS')}
            style={{ textAlign: 'left', background: selectedReport === 'VENTAS' ? 'var(--color-primary-ultra)' : 'var(--color-surface)' }}
          >
            <div className="font-bold text-primary">📊 Ventas Realizadas</div>
            <p className="text-xs text-muted mt-1">Listado de transacciones al contado por fecha.</p>
          </button>

          <button 
            className={`card hover-shadow ${selectedReport === 'STOCK' ? 'border-primary' : ''}`}
            onClick={() => setSelectedReport('STOCK')}
            style={{ textAlign: 'left', background: selectedReport === 'STOCK' ? 'var(--color-primary-ultra)' : 'var(--color-surface)' }}
          >
            <div className="font-bold text-primary">📦 Existencias de Stock</div>
            <p className="text-xs text-muted mt-1">Saldos de mercadería y alertas de reposición.</p>
          </button>

          <button 
            className={`card hover-shadow ${selectedReport === 'DEUDORES' ? 'border-primary' : ''}`}
            onClick={() => setSelectedReport('DEUDORES')}
            style={{ textAlign: 'left', background: selectedReport === 'DEUDORES' ? 'var(--color-primary-ultra)' : 'var(--color-surface)' }}
          >
            <div className="font-bold text-primary">⏳ Cartera de Deudores</div>
            <p className="text-xs text-muted mt-1">Aging de deuda y estados de morosidad.</p>
          </button>
        </div>

        {/* VISTA PREVIA */}
        <div className="card">
          <div className="flex-between mb-8 pb-4 border-b">
            <div>
              <h3 className="font-bold">{selectedReport.charAt(0) + selectedReport.slice(1).toLowerCase()} - Preview</h3>
              <p className="text-xs text-muted mt-1">Datos procesados según últimos registros del sistema.</p>
            </div>
            <div className="flex-gap-2">
              <button className="btn btn-ghost" onClick={handleExportExcel}>Excel (XLSX)</button>
              <button className="btn btn-primary" onClick={handleExportPDF}>Descargar PDF</button>
            </div>
          </div>

          <div className="data-table-wrap">
            {selectedReport === 'VENTAS' && (
              <table className="data-table">
                <thead><tr><th>Fecha</th><th>Cliente</th><th className="text-right">Monto</th></tr></thead>
                <tbody>
                  {ventas.slice(0, 10).map((v: any) => (
                    <tr key={v.id}>
                      <td>{new Date(v.fecha).toLocaleDateString()}</td>
                      <td className="font-bold">{v.clienteNombre || 'Consumidor Final'}</td>
                      <td className="text-right font-bold text-primary">${v.total.toLocaleString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            {selectedReport === 'STOCK' && (
              <table className="data-table">
                <thead><tr><th>Producto</th><th>Disp.</th><th className="text-right">Costo/Venta</th></tr></thead>
                <tbody>
                  {productos.slice(0, 10).map((p: any) => (
                    <tr key={p.id}>
                      <td className="font-bold">{p.nombre}</td>
                      <td><span className={`badge ${p.stockActual < 10 ? 'badge-danger' : 'badge-neutral'}`}>{p.stockActual}</span></td>
                      <td className="text-right font-bold text-primary">${p.precioVenta.toLocaleString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            {selectedReport === 'DEUDORES' && (
              <table className="data-table">
                <thead><tr><th>Cliente</th><th>Mora</th><th className="text-right">Saldo</th></tr></thead>
                <tbody>
                  {deudores.map((d, i) => (
                    <tr key={i}>
                      <td className="font-bold">{d.cliente}</td>
                      <td><span className="badge badge-warning">{d.mora}</span></td>
                      <td className="text-right font-bold text-primary">${d.deuda.toLocaleString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
            {((selectedReport === 'VENTAS' && ventas.length === 0) ||
              (selectedReport === 'STOCK' && productos.length === 0) ||
              (selectedReport === 'DEUDORES' && deudores.length === 0)) && (
                <div className="text-center py-20 text-muted">No hay datos suficientes para generar este reporte.</div>
            )}
          </div>
        </div>

      </div>
    </div>
  )
}
