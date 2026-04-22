import axios from 'axios'
import { useAuthStore } from '../store/authStore'
import { db } from '../store/db'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const api = axios.create({
  baseURL: `${API_URL}/api`,
  headers: { 'Content-Type': 'application/json' },
})

// Request interceptor: agrega JWT
api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// Response interceptor: auto-refresh token
api.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true
      try {
        const { refreshToken, updateToken, logout } = useAuthStore.getState()
        const res = await axios.post(`${API_URL}/api/auth/refresh`, { refreshToken })
        updateToken(res.data.accessToken, res.data.refreshToken)
        original.headers.Authorization = `Bearer ${res.data.accessToken}`
        return api(original)
      } catch {
        useAuthStore.getState().logout()
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

// ─── API SERVICES ─────────────────────────────────────────────────────────

export const authApi = {
  login: (email: string, password: string) =>
    api.post('/auth/login', { email, password }),
  me: () => api.get('/auth/me'),
}

export const dashboardApi = {
  getKpi: () => api.get('/dashboard/kpi'),
  getTermometro: () => api.get('/dashboard/cobranza-termometro'),
  getEvolucionMensual: (meses = 6) => api.get(`/dashboard/evolucion-mensual?meses=${meses}`),
  getDistribucionCartera: () => api.get('/dashboard/distribucion-cartera'),
  getRankingVendedores: () => api.get('/dashboard/ranking-vendedores'),
  getAgingDeuda: () => api.get('/dashboard/aging-deuda'),
  getFlujoCaja: () => api.get('/dashboard/flujo-proyectado'),
  getMapaMora: () => api.get('/dashboard/mapa-mora'),
  getAlertas: () => api.get('/dashboard/alertas'),
}

export const clientesApi = {
  getAll: (params?: { search?: string; estado?: string; zonaId?: string; tipoCliente?: string }) =>
    api.get('/clientes', { params }),
  getById: (id: string) => api.get(`/clientes/${id}`),
  create: (data: any) => api.post('/clientes', data),
  update: (id: string, data: any) => api.put(`/clientes/${id}`, data),
  reactivar: (id: string) => api.post(`/clientes/${id}/reactivar`),
  inactivar: (id: string) => api.post(`/clientes/${id}/inactivar`),
  bloquear: (id: string) => api.post(`/clientes/${id}/bloquear`)
}

export const creditosApi = {
  getAll: (params?: any) => api.get('/creditos', { params }), // assuming this endpoint exists/will be added
  getById: (id: string) => api.get(`/creditos/${id}`),
  getByCliente: (clienteId: string) => api.get(`/creditos/cliente/${clienteId}`),
  create: (data: any) => api.post('/creditos', data),
  registrarPago: async (cuotaId: string, data: any) => {
    if (!navigator.onLine) {
      await db.pagosOfflineQueue.add({
        cuotaId,
        monto: data.monto,
        latitud: data.latitud,
        longitud: data.longitud,
        observaciones: data.observaciones,
        fechaCaptura: new Date(),
        estado: 'PENDIENTE'
      });
      return { data: { success: true, offline: true } };
    }
    return api.post(`/creditos/${cuotaId}/pagar`, data);
  },
  getHojaRuta: (vendedorId: string, fecha?: string) =>
    api.get('/creditos/hoja-ruta', { params: { vendedorId, fecha } }),

  syncOffline: (pagos: any[]) => api.post('/creditos/sync-offline', { pagos }),
}

export const cajaApi = {
  declarar: (montoDeclarado: number) => api.post('/caja/declarar', { montoDeclarado }),
  cerrar: (vendedorId: string) => api.post(`/caja/${vendedorId}/cerrar`),
  getHoy: () => api.get('/caja/hoy'),
}

export const inventarioApi = {
  getProductos: (params?: { search?: string; soloActivos?: boolean }) => api.get('/inventario/productos', { params }),
  getProductoById: (id: string) => api.get(`/inventario/productos/${id}`),
  createProducto: (data: any) => api.post('/inventario/productos', data),
  updateProducto: (id: string, data: any) => api.put(`/inventario/productos/${id}`, data),
  deleteProducto: (id: string) => api.delete(`/inventario/productos/${id}`),
  cargarFactura: (data: { proveedorId: string, numeroFactura: string, items: any[] }) => 
    api.post('/inventario/compras/factura', data),
  reactivarProducto: (id: string) => api.post(`/inventario/productos/${id}/reactivar`)
}

export const proveedoresApi = {
  getAll: (params?: any) => api.get('/proveedores', { params }),
  create: (data: any) => api.post('/proveedores', data),
  update: (id: string, data: any) => api.put(`/proveedores/${id}`, data),
  delete: (id: string) => api.delete(`/proveedores/${id}`),
  reactivar: (id: string) => api.post(`/proveedores/${id}/reactivar`)
}

export const categoriasApi = {
  getAll: (soloActivas: boolean = true) => api.get('/categorias', { params: { soloActivas } }),
  create: (data: any) => api.post('/categorias', data)
}

export const zonasApi = {
  getAll: (soloActivas: boolean = true) => api.get('/zonas', { params: { soloActivas } }),
  getById: (id: string) => api.get(`/zonas/${id}`),
  create: (data: any) => api.post('/zonas', data),
  update: (id: string, data: any) => api.put(`/zonas/${id}`, data),
  delete: (id: string) => api.delete(`/zonas/${id}`),
  reactivar: (id: string) => api.post(`/zonas/${id}/reactivar`)
}

export const configuracionApi = {
  get: () => api.get('/configuracion'),
  update: (data: any) => api.put('/configuracion', data),
  getListasPrecios: () => api.get('/configuracion/listas-precios'),
  createLista: (data: any) => api.post('/configuracion/listas-precios', data),
  updateLista: (id: string, data: any) => api.put(`/configuracion/listas-precios/${id}`, data),
  reactivarLista: (id: string) => api.post(`/configuracion/listas-precios/${id}/reactivar`),
  addListItem: (listaId: string, data: any) => api.post(`/configuracion/listas-precios/${listaId}/items`, data),
  removeListItem: (itemId: string) => api.delete(`/configuracion/listas-precios/items/${itemId}`)
}

export const seguridadApi = {
  getUsuarios: () => api.get('/seguridad/usuarios'),
  updateUsuario: (id: string, data: any) => api.put(`/seguridad/usuarios/${id}`, data),
  reactivarUsuario: (id: string) => api.post(`/seguridad/usuarios/${id}/reactivar`),
  getPerfiles: () => api.get('/seguridad/perfiles'),
  getPermisos: () => api.get('/seguridad/permisos'),
  createPerfil: (data: any) => api.post('/seguridad/perfiles', data),
  reactivarPerfil: (id: string) => api.post(`/seguridad/perfiles/${id}/reactivar`)
}

export const ventasApi = {
  registrarContado: (data: any) => api.post('/ventas/contado', data),
  getRecientes: () => api.get('/ventas/recientes')
}
export const parametricasApi = {
  getOcupaciones: (soloActivas = true) => api.get('/parametricas/ocupaciones', { params: { soloActivas } }),
  createOcupacion: (data: any) => api.post('/parametricas/ocupaciones', data),
  updateOcupacion: (id: string, data: any) => api.put(`/parametricas/ocupaciones/${id}`, data),
  deleteOcupacion: (id: string) => api.delete(`/parametricas/ocupaciones/${id}`),
  reactivarOcupacion: (id: string) => api.post(`/parametricas/ocupaciones/${id}/reactivar`),
  getCondicionesIva: (soloActivas = true) => api.get('/parametricas/condiciones-iva', { params: { soloActivas } }),
  createCondicionIva: (data: any) => api.post('/parametricas/condiciones-iva', data),
  updateCondicionIva: (id: string, data: any) => api.put(`/parametricas/condiciones-iva/${id}`, data),
  deleteCondicionIva: (id: string) => api.delete(`/parametricas/condiciones-iva/${id}`),
  reactivarCondicionIva: (id: string) => api.post(`/parametricas/condiciones-iva/${id}/reactivar`),
  getCondicionesPago: (soloActivas = true) => api.get('/parametricas/condiciones-pago', { params: { soloActivas } }),
  createCondicionPago: (data: any) => api.post('/parametricas/condiciones-pago', data),
  updateCondicionPago: (id: string, data: any) => api.put(`/parametricas/condiciones-pago/${id}`, data),
  deleteCondicionPago: (id: string) => api.delete(`/parametricas/condiciones-pago/${id}`),
  reactivarCondicionPago: (id: string) => api.post(`/parametricas/condiciones-pago/${id}/reactivar`),
  getSectores: (soloActivas = true) => api.get('/parametricas/sectores', { params: { soloActivas } }),
  getTiposCliente: (soloActivas = true) => api.get('/parametricas/tipos-cliente', { params: { soloActivas } }),
  getFormasCobro: (soloActivas = true) => api.get('/parametricas/formas-cobro', { params: { soloActivas } }),
  getUnidadesMedida: (soloActivas = true) => api.get('/parametricas/unidades-medida', { params: { soloActivas } }),
};

// Extending authApi with register
export const extendAuthInfo = { register: (data: any) => api.post('/auth/register', data) };
