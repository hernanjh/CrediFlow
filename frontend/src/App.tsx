import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from './store/authStore'
import AppLayout from './components/layout/AppLayout'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import ClientesPage from './pages/ClientesPage'
import CreditosPage from './pages/CreditosPage'
import CobranzaPage from './pages/CobranzaPage'
import InventarioPage from './pages/InventarioPage'
import ReportesPage from './pages/ReportesPage'
import ProveedoresPage from './pages/ProveedoresPage'
import ListaPreciosPage from './pages/ListaPreciosPage'
import VentasPage from './pages/VentasPage'
import ZonasPage from './pages/ZonasPage'
import UsuariosPage from './pages/UsuariosPage'
import ConfiguracionPage from './pages/ConfiguracionPage'
import GestionPreciosPage from './pages/GestionPreciosPage'
import ParametricaPage from './pages/ParametricaPage'
import './index.css'
import './api/sync'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutos
      retry: 1,
    },
  },
})

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuthStore()
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={
            <PrivateRoute>
              <AppLayout />
            </PrivateRoute>
          }>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="clientes" element={<ClientesPage />} />
            <Route path="creditos" element={<CreditosPage />} />
            <Route path="cobranza" element={<CobranzaPage />} />
            <Route path="inventario" element={<InventarioPage />} />
            <Route path="ventas" element={<VentasPage />} />
            <Route path="proveedores" element={<ProveedoresPage />} />
            <Route path="zonas" element={<ZonasPage />} />
            <Route path="tarifario" element={<GestionPreciosPage />} />
            <Route path="usuarios" element={<UsuariosPage />} />
            <Route path="configuracion" element={<ConfiguracionPage />} />
            
            {/* Parametrización */}
            <Route path="param/sectores" element={<ParametricaPage title="Sectores / Ocupaciones" endpoint="sectores" />} />
            <Route path="param/tipos-cliente" element={<ParametricaPage title="Tipos de Cliente" endpoint="tipos-cliente" />} />
            <Route path="param/formas-cobro" element={<ParametricaPage title="Formas de Cobro" endpoint="formas-cobro" />} />
            <Route path="param/categorias" element={<ParametricaPage title="Categorías de Productos" endpoint="categorias" />} />
            <Route path="param/cond-iva" element={<ParametricaPage title="Condiciones IVA" endpoint="condiciones-iva" />} />
            <Route path="param/cond-pago" element={<ParametricaPage title="Condiciones Pago" endpoint="condiciones-pago" hasPlazo={true} />} />
            <Route path="param/unidades" element={<ParametricaPage title="Unidades de Medida" endpoint="unidades-medida" hasSymbol={true} />} />
            
            <Route path="reportes" element={<ReportesPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
