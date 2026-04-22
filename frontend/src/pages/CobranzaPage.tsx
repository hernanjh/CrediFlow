import { useState, useEffect } from 'react'
import { useQuery } from '@tanstack/react-query'
import { creditosApi } from '../api/client'
import { db } from '../store/db'
import { useAuthStore } from '../store/authStore'

export default function CobranzaPage() {
  const { usuario } = useAuthStore()
  const isVendedor = usuario?.rol === 'VENDEDOR'
  const [hoja, setHoja] = useState<any[]>([])
  const [isOffline, setIsOffline] = useState(!navigator.onLine)
  const [loading, setLoading] = useState(false)

  // Tracker de red dinámico
  useEffect(() => {
    const handleNet = () => setIsOffline(!navigator.onLine)
    window.addEventListener('online', handleNet)
    window.addEventListener('offline', handleNet)
    return () => { window.removeEventListener('online', handleNet); window.removeEventListener('offline', handleNet) }
  }, [])

  // Cargar Hoja de Ruta (Offline-first approach)
  useEffect(() => {
    const fetchHoja = async () => {
      const cacheId = `ruta-${new Date().toISOString().split('T')[0]}-${usuario?.id}`
      setLoading(true)
      try {
        if (!isOffline) {
          const res = await creditosApi.getHojaRuta(usuario?.id || '')
          setHoja(res.data)
          // Guardar cache para uso offline
          await db.hojaRutaCache.put({ id: cacheId, fechaGuardado: new Date(), datos: res.data })
        } else {
          // Intentar leer de cache
          const cache = await db.hojaRutaCache.get(cacheId)
          if (cache) setHoja(cache.datos)
        }
      } catch (err) {
        console.error("Error obteniendo hoja de ruta:", err)
      } finally {
        setLoading(false)
      }
    }
    if (usuario?.id) fetchHoja()
  }, [usuario?.id, isOffline])

  const registrarCobro = (cuotaId: string, monto: number) => {
    if (!confirm(`¿Confirmar cobro de $${monto}?`)) return;
    
    // Capturar GPS
    setLoading(true)
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        async (position) => {
          await ejecutarCobro(cuotaId, monto, position.coords.latitude, position.coords.longitude);
        },
        async (error) => {
          console.warn("GPS falló o denegado, registrando sin GPS", error);
          await ejecutarCobro(cuotaId, monto);
        },
        { enableHighAccuracy: true, timeout: 5000 }
      );
    } else {
      ejecutarCobro(cuotaId, monto);
    }
  }

  const ejecutarCobro = async (cuotaId: string, monto: number, lat?: number, lng?: number) => {
    try {
      await creditosApi.registrarPago(cuotaId, { monto, latitud: lat, longitud: lng, observaciones: 'Cobro en campo' });
      alert(isOffline ? 'Pago encolado exitosamente (Offline)' : 'Pago registrado exitosamente');
      // Actualizar UI provisoriamente
      setHoja(prev => prev.map(c => c.cuotaId === cuotaId ? { ...c, estado: 'PAGADA' } : c))
    } catch (e) {
      alert("Error al intentar registrar el cobro");
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 'var(--space-6)' }}>
        <div>
          <h1 style={{ fontSize: 20, fontWeight: 700 }}>Hoja de Ruta Diaria</h1>
          <p style={{ fontSize: 13, color: 'var(--text-muted)', marginTop: 4 }}>
            {!isVendedor ? 'Usa una cuenta de vendedor para ver datos de ruta (Muestra todo si eres admin)' : 'Cobranza domiciliaria del día'}
          </p>
        </div>
      </div>

      {isOffline && (
        <div style={{ marginBottom: 16, background: 'rgba(255, 107, 107, 0.1)', color: '#ff6b6b', padding: '12px 16px', borderRadius: 8, fontSize: 14 }}>
          ⚠️ <strong>Estás sin conexión.</strong> Los cobros se guardarán temporalmente en este dispositivo y se sincronizarán cuando vuelva el internet.
        </div>
      )}

      <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
        {loading ? (
          <div className="loading-overlay"><span className="spinner" /> Cargando...</div>
        ) : hoja.length === 0 ? (
          <div style={{ padding: 40, textAlign: 'center', color: 'var(--text-muted)' }}>
            🤷‍♂️ No hay cuotas asignadas para hoy o no pudimos sincronizarlas offline.
          </div>
        ) : (
          <table className="cierre-table" style={{ width: '100%' }}>
            <thead style={{ background: 'var(--color-surface-2)' }}>
              <tr>
                <th>Cliente</th>
                <th>Dirección</th>
                <th>Nº Cuota</th>
                <th>Monto a Cobrar</th>
                <th style={{ textAlign: 'center' }}>Vencimiento</th>
                <th style={{ textAlign: 'right' }}>Acción</th>
              </tr>
            </thead>
            <tbody>
              {hoja.map((h, i) => (
                <tr key={i}>
                  <td style={{ fontWeight: 600 }}>{h.cliente}</td>
                  <td>{h.direccion}</td>
                  <td>{h.numeroCuota} / {h.totalCuotas}</td>
                  <td style={{ fontWeight: 'bold' }}>${(h.monto || 0).toFixed(2)}</td>
                  <td style={{ textAlign: 'center' }}>{new Date(h.fechaVencimiento).toLocaleDateString()}</td>
                  <td style={{ textAlign: 'right' }}>
                    {h.estado === 'PAGADA' ? (
                      <span className="badge badge-success">Pagado ✔</span>
                    ) : (
                      <button 
                         className="btn btn-primary" 
                         style={{ padding: '6px 12px', fontSize: 12 }}
                         onClick={() => registrarCobro(h.cuotaId, h.monto)}
                      >
                         Registrar Cobro
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
