import {
  RadialBarChart, RadialBar, PolarAngleAxis,
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell,
} from 'recharts'
import { useAuthStore } from '../store/authStore'

// ─── HELPERS ────────────────────────────────────────────────────────────────
const formatMoney = (v: number) =>
  new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 0 }).format(v)

const formatPct = (v: number) => `${v.toFixed(1)}%`

// ─── MOCK DATA (mientras el backend no esté corriendo) ───────────────────────
const MOCK_KPI = {
  capitalActivo: 847000, cobradoHoy: 42300, metaCobradoHoy: 58000,
  clientesActivos: 312, indiceMora: 8.4, cuotasVencidas: 143, montoVencido: 71000
}

const MOCK_EVOLUCION = [
  { mes: 'Ene', nuevasVentas: 120000, capitalRecuperado: 95000 },
  { mes: 'Feb', nuevasVentas: 145000, capitalRecuperado: 115000 },
  { mes: 'Mar', nuevasVentas: 132000, capitalRecuperado: 128000 },
  { mes: 'Abr', nuevasVentas: 165000, capitalRecuperado: 140000 },
  { mes: 'May', nuevasVentas: 158000, capitalRecuperado: 152000 },
  { mes: 'Jun', nuevasVentas: 180000, capitalRecuperado: 168000 },
]

const MOCK_CARTERA = { alDia: 190, moraLeve: 56, moraGrave: 37, incobrables: 29, total: 312 }

const MOCK_RANKING = [
  { nombre: 'González', eficienciaPct: 94 },
  { nombre: 'Martínez', eficienciaPct: 87 },
  { nombre: 'Rodríguez', eficienciaPct: 76 },
  { nombre: 'López', eficienciaPct: 68 },
  { nombre: 'Fernández', eficienciaPct: 51 },
]

const MOCK_AGING = {
  tramo1_7: 11200, tramo8_30: 18500, tramo31_90: 16800, tramoPlusN90: 4700,
  cant1_7: 38, cant8_30: 42, cant31_90: 35, cantPlus90: 28, total: 51200
}

const MOCK_FLUJO = [
  { semana: 1, montoProyectado: 52100, montoRecuperado: 31200 },
  { semana: 2, montoProyectado: 43600, montoRecuperado: 0 },
  { semana: 3, montoProyectado: 33900, montoRecuperado: 0 },
  { semana: 4, montoProyectado: 47700, montoRecuperado: 0 },
]

const MOCK_MAPA = [
  { zonaNombre: 'Norte', indicesMora: 23, nivel: 'critico' },
  { zonaNombre: 'Sur', indicesMora: 9, nivel: 'bajo' },
  { zonaNombre: 'Este', indicesMora: 14, nivel: 'medio' },
  { zonaNombre: 'Oeste', indicesMora: 31, nivel: 'critico' },
  { zonaNombre: 'Centro', indicesMora: 5, nivel: 'bajo' },
  { zonaNombre: 'Ricoletavia', indicesMora: 18, nivel: 'medio' },
  { zonaNombre: 'Belgrano', indicesMora: 27, nivel: 'alto' },
  { zonaNombre: 'Flores', indicesMora: 11, nivel: 'bajo' },
  { zonaNombre: 'Lomas', indicesMora: 35, nivel: 'critico' },
  { zonaNombre: 'Quilmes', indicesMora: 9, nivel: 'bajo' },
  { zonaNombre: 'Tigre', indicesMora: 22, nivel: 'medio' },
  { zonaNombre: 'Morón', indicesMora: 16, nivel: 'medio' },
]

const MOCK_ALERTAS = [
  { id: '1', tipo: 'MORA_GRAVE', mensaje: '5 clientes superaron 90 días sin pago — evaluar pase a pérdida', leida: false },
  { id: '2', tipo: 'MORA_GRAVE', mensaje: 'Zona Norte — mora 23%, por encima del umbral crítico', leida: false },
  { id: '3', tipo: 'PRODUCTO_STOCK_BAJO', mensaje: "Producto 'Colchón XL' — stock crítico: 2 unidades", leida: false },
  { id: '4', tipo: 'PROMESA_PAGO_VENCIDA', mensaje: '12 promesas de pago vencen mañana', leida: false },
]

const MOCK_CIERRE = [
  { vendedor: 'González', declarado: 12400, sistema: 12400, diferencia: 0 },
  { vendedor: 'Martínez', declarado: 9800, sistema: 10100, diferencia: -300 },
  { vendedor: 'Rodríguez', declarado: 8200, sistema: 8200, diferencia: 0 },
  { vendedor: 'López', declarado: 7100, sistema: 7350, diferencia: -250 },
]

// ─── COMPONENT ───────────────────────────────────────────────────────────────
export default function DashboardPage() {
  const { usuario } = useAuthStore()
  const isAdmin = usuario?.rol !== 'VENDEDOR'

  // En un entorno real, se usaría:
  // const { data: kpi } = useQuery({ queryKey: ['kpi'], queryFn: () => dashboardApi.getKpi().then(r => r.data) })
  // Por ahora usamos mock data
  const kpi = MOCK_KPI
  const evolucion = MOCK_EVOLUCION
  const cartera = MOCK_CARTERA
  const ranking = MOCK_RANKING
  const aging = MOCK_AGING
  const flujo = MOCK_FLUJO
  const mapa = MOCK_MAPA
  const alertas = MOCK_ALERTAS
  const cierre = MOCK_CIERRE

  const pctCobranza = Math.round((kpi.cobradoHoy / kpi.metaCobradoHoy) * 100)

  const carteraData = [
    { name: 'Al día', value: cartera.alDia, color: '#22c55e' },
    { name: 'Mora leve', value: cartera.moraLeve, color: '#f59e0b' },
    { name: 'Mora grave', value: cartera.moraGrave, color: '#ef4444' },
    { name: 'Incobrables', value: cartera.incobrables, color: '#6b7280' },
  ]

  const totalAging = aging.tramo1_7 + aging.tramo8_30 + aging.tramo31_90 + aging.tramoPlusN90
  const maxFlujo = Math.max(...flujo.map(f => f.montoProyectado))

  const getAlertaStyle = (tipo: string) => {
    if (tipo === 'MORA_GRAVE') return 'alerta-danger'
    if (tipo === 'PRODUCTO_STOCK_BAJO') return 'alerta-warning'
    return 'alerta-info'
  }

  const getAlertaIcon = (tipo: string) => {
    if (tipo === 'MORA_GRAVE') return '🔴'
    if (tipo === 'PRODUCTO_STOCK_BAJO') return '🟡'
    return '🔵'
  }

  return (
    <div>
      {/* ─── KPI CARDS ─────────────────────────────────────────────── */}
      <div className="kpi-grid">
        {[
          { label: 'Capital Activo', value: formatMoney(kpi.capitalActivo), delta: '+12% vs. mes anterior', deltaUp: true, accent: '#6c63ff' },
          { label: 'Cobrado Hoy', value: formatMoney(kpi.cobradoHoy), delta: `Meta: ${formatMoney(kpi.metaCobradoHoy)}`, deltaUp: true, accent: '#22c55e' },
          { label: 'Clientes Activos', value: kpi.clientesActivos.toString(), delta: '26 en mora grave', deltaUp: false, accent: '#3b82f6' },
          { label: 'Índice de Mora', value: formatPct(kpi.indiceMora), delta: '-1.7% vs. semana anterior', deltaUp: true, accent: '#ef4444' },
          { label: 'Cuotas Vencidas', value: kpi.cuotasVencidas.toString(), delta: formatMoney(kpi.montoVencido) + ' pendiente', deltaUp: false, accent: '#f59e0b' },
        ].map(k => (
          <div key={k.label} className="kpi-card" style={{ '--accent-color': k.accent } as any}>
            <div className="kpi-label">{k.label}</div>
            <div className="kpi-value">{k.value}</div>
            <div className={`kpi-delta ${k.deltaUp ? 'delta-up' : 'delta-down'}`}>
              {k.deltaUp ? '↑' : '↓'} {k.delta}
            </div>
          </div>
        ))}
      </div>

      {/* ─── ROW 1: TERMÓMETRO + CARTERA ──────────────────────────── */}
      <div className="dashboard-grid" style={{ marginBottom: 'var(--space-4)' }}>
        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Termómetro de Cobranza Diaria</div>
              <div className="chart-subtitle">Meta del día vs. cobrado real</div>
            </div>
            <button className="btn btn-ghost btn-sm">Desglose por vendedor ↗</button>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <ResponsiveContainer width="100%" height={200}>
              <RadialBarChart
                cx="50%" cy="50%"
                innerRadius="60%" outerRadius="90%"
                barSize={20}
                data={[{ name: 'Meta', value: 100, fill: 'var(--color-surface-3)' }, { name: 'Cobrado', value: pctCobranza, fill: pctCobranza >= 80 ? '#22c55e' : pctCobranza >= 50 ? '#f59e0b' : '#ef4444' }]}
                startAngle={180} endAngle={0}
              >
                <PolarAngleAxis type="number" domain={[0, 100]} tick={false} />
                <RadialBar dataKey="value" cornerRadius={10} />
              </RadialBarChart>
            </ResponsiveContainer>

            <div className="gauge-center" style={{ marginTop: -60 }}>
              <div className="gauge-value">{pctCobranza}%</div>
              <div className="gauge-label">{formatMoney(kpi.cobradoHoy)} de {formatMoney(kpi.metaCobradoHoy)} meta</div>
            </div>

            <div className="gauge-legend" style={{ marginTop: 24 }}>
              {[
                { color: '#ef4444', label: 'Bajo (0–50%)' },
                { color: '#f59e0b', label: 'Medio (50–80%)' },
                { color: '#22c55e', label: 'Alto (80–100%)' },
              ].map(l => (
                <div key={l.label} className="legend-item">
                  <div className="legend-dot" style={{ background: l.color }} />
                  {l.label}
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Distribución de Cartera</div>
              <div className="chart-subtitle">Estado de {cartera.total} clientes activos</div>
            </div>
            <button className="btn btn-ghost btn-sm">Ver clientes en mora ↗</button>
          </div>

          <div style={{ display: 'flex', alignItems: 'center', gap: 24 }}>
            <ResponsiveContainer width={180} height={180}>
              <PieChart>
                <Pie
                  data={carteraData}
                  cx="50%" cy="50%"
                  innerRadius={55} outerRadius={80}
                  paddingAngle={3}
                  dataKey="value"
                >
                  {carteraData.map((entry, i) => (
                    <Cell key={i} fill={entry.color} />
                  ))}
                </Pie>
                <Tooltip
                  formatter={(val: unknown) => [`${val} clientes`, '']}
                  contentStyle={{ background: 'var(--color-surface-2)', border: '1px solid var(--color-border)', borderRadius: 8, fontSize: 12 }}
                />
              </PieChart>
            </ResponsiveContainer>

            <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: 8 }}>
              {carteraData.map(c => (
                <div key={c.name} style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                  <div style={{ width: 10, height: 10, borderRadius: '50%', background: c.color, flexShrink: 0 }} />
                  <span style={{ fontSize: 12, color: 'var(--text-secondary)', flex: 1 }}>{c.name}</span>
                  <span style={{ fontSize: 12, fontWeight: 700, color: c.color }}>
                    {Math.round(c.value / cartera.total * 100)}%
                  </span>
                  <span style={{ fontSize: 11, color: 'var(--text-muted)', width: 30, textAlign: 'right' }}>
                    {c.value}
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* ─── EVOLUCIÓN MENSUAL ──────────────────────────────────────── */}
      <div className="chart-container col-12" style={{ marginBottom: 'var(--space-4)' }}>
        <div className="chart-header">
          <div>
            <div className="chart-title">Evolución Mensual — Nuevas Ventas vs. Cobros Recuperados</div>
          </div>
        </div>
        <ResponsiveContainer width="100%" height={220}>
          <LineChart data={evolucion}>
            <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" />
            <XAxis dataKey="mes" tick={{ fill: 'var(--text-muted)', fontSize: 11 }} axisLine={false} tickLine={false} />
            <YAxis tickFormatter={v => `$${(v/1000).toFixed(0)}K`} tick={{ fill: 'var(--text-muted)', fontSize: 11 }} axisLine={false} tickLine={false} />
            <Tooltip
              formatter={(val: unknown) => [formatMoney(val as number)]}
              contentStyle={{ background: 'var(--color-surface-2)', border: '1px solid var(--color-border)', borderRadius: 8, fontSize: 12 }}
            />
            <Legend iconType="circle" iconSize={8} wrapperStyle={{ fontSize: 12 }} />
            <Line type="monotone" dataKey="nuevasVentas" name="Nuevas ventas/créditos" stroke="#6c63ff" strokeWidth={2.5} dot={{ fill: '#6c63ff', r: 4 }} activeDot={{ r: 6 }} />
            <Line type="monotone" dataKey="capitalRecuperado" name="Capital recuperado" stroke="#22c55e" strokeWidth={2.5} dot={{ fill: '#22c55e', r: 4 }} activeDot={{ r: 6 }} />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* ─── ROW 3: RANKING + AGING ────────────────────────────────── */}
      <div className="dashboard-grid" style={{ marginBottom: 'var(--space-4)' }}>
        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Ranking de Cobradores — Eficiencia %</div>
            </div>
            <button className="btn btn-ghost btn-sm">Auditar cobrador ↗</button>
          </div>
          {ranking.map((v, i) => (
            <div key={v.nombre} className="ranking-item">
              <span className="ranking-position">#{i + 1}</span>
              <span className="ranking-name">{v.nombre}</span>
              <div className="ranking-bar-container">
                <div className="ranking-bar" style={{ width: `${v.eficienciaPct}%` }} />
              </div>
              <span className="ranking-pct">{v.eficienciaPct}%</span>
            </div>
          ))}
        </div>

        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Aging de Deuda Vencida</div>
              <div className="chart-subtitle">Total vencido: {formatMoney(totalAging)}</div>
            </div>
            <button className="btn btn-ghost btn-sm">Exportar aging ↗</button>
          </div>
          {[
            { label: '1–7 días', amount: aging.tramo1_7, cant: aging.cant1_7, color: '#22c55e' },
            { label: '8–30 días', amount: aging.tramo8_30, cant: aging.cant8_30, color: '#f59e0b' },
            { label: '31–90 días', amount: aging.tramo31_90, cant: aging.cant31_90, color: '#ef4444' },
            { label: '+90 días', amount: aging.tramoPlusN90, cant: aging.cantPlus90, color: '#7f1d1d' },
          ].map(t => (
            <div key={t.label} className="aging-row">
              <span className="aging-tramo">{t.label}</span>
              <div className="aging-bar-container">
                <div className="aging-bar" style={{ width: `${(t.amount / totalAging) * 100}%`, background: t.color }} />
              </div>
              <span className="aging-amount">{formatMoney(t.amount)}</span>
              <span className="aging-pct" style={{ color: t.color }}>
                {Math.round(t.amount / totalAging * 100)}%
              </span>
            </div>
          ))}
        </div>
      </div>

      {/* ─── ROW 4: FLUJO + MAPA ────────────────────────────────────── */}
      <div className="dashboard-grid" style={{ marginBottom: 'var(--space-4)' }}>
        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Flujo de Fondos Proyectado — Próximos 30 Días</div>
            </div>
          </div>
          {flujo.map((s, i) => (
            <div key={s.semana} className="flujo-item">
              <span className="flujo-semana">
                {i === 0 ? 'Semana 1 (hoy)' : `Semana ${s.semana}`}
              </span>
              <div className="flujo-bar-container">
                <div className="flujo-bar" style={{ width: `${(s.montoProyectado / maxFlujo) * 100}%` }} />
              </div>
              <span className="flujo-amount">{formatMoney(s.montoProyectado)}</span>
            </div>
          ))}
          <div className="alerta-item alerta-info" style={{ marginTop: 12, fontSize: 11 }}>
            <span>💡</span>
            Capital proyectado: {formatMoney(flujo.reduce((s, f) => s + f.montoProyectado, 0))} — Se puede otorgar hasta {formatMoney(flujo[0].montoProyectado * 0.47)} en nuevos créditos manteniendo reserva mínima del 47%
          </div>
          <button className="btn btn-ghost btn-sm btn-full" style={{ marginTop: 12 }}>
            Simular nuevo crédito ↗
          </button>
        </div>

        <div className="chart-container col-6">
          <div className="chart-header">
            <div>
              <div className="chart-title">Mapa de Calor de Mora — Zones</div>
            </div>
            <button className="btn btn-ghost btn-sm">Ver por zona ↗</button>
          </div>
          <div className="mapa-grid">
            {mapa.map(z => (
              <div key={z.zonaNombre} className={`mapa-zona heat-${z.nivel}`}>
                <span className="mapa-zona-name">{z.zonaNombre}</span>
                <span className="mapa-zona-pct">{z.indicesMora}%</span>
              </div>
            ))}
          </div>
          <div className="gauge-legend" style={{ marginTop: 12 }}>
            {[
              { color: 'rgba(34,197,94,0.5)', label: 'Bajo' },
              { color: 'rgba(245,158,11,0.5)', label: 'Medio' },
              { color: 'rgba(239,68,68,0.5)', label: 'Alto' },
              { color: 'rgba(220,38,38,0.8)', label: 'Crítico' },
            ].map(l => (
              <div key={l.label} className="legend-item">
                <div className="legend-dot" style={{ background: l.color }} />
                {l.label}
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ─── ROW 5: CIERRE CIEGO + ALERTAS ─────────────────────────── */}
      {isAdmin && (
        <div className="dashboard-grid">
          <div className="chart-container col-6">
            <div className="chart-header">
              <div>
                <div className="chart-title">Cierre Ciego de Caja — Hoy</div>
              </div>
              <button className="btn btn-ghost btn-sm">Auditar faltante ↗</button>
            </div>
            <table className="cierre-table">
              <thead>
                <tr>
                  <th>Vendedor</th>
                  <th>Declarado</th>
                  <th>Sistema</th>
                  <th>Diferencia</th>
                </tr>
              </thead>
              <tbody>
                {cierre.map(c => (
                  <tr key={c.vendedor}>
                    <td style={{ color: 'var(--text-primary)', fontWeight: 500 }}>{c.vendedor}</td>
                    <td>{formatMoney(c.declarado)}</td>
                    <td>{formatMoney(c.sistema)}</td>
                    <td className={c.diferencia === 0 ? 'diff-positive' : 'diff-negative'}>
                      {c.diferencia === 0 ? '✓ OK' : formatMoney(c.diferencia)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {cierre.some(c => c.diferencia < 0) && (
              <div className="alerta-item alerta-danger" style={{ marginTop: 12, fontSize: 11 }}>
                <span>⚠️</span>
                Faltante total del día: {formatMoney(cierre.reduce((s, c) => s + Math.min(0, c.diferencia), 0))} — Requiere revisión
              </div>
            )}
          </div>

          <div className="chart-container col-6">
            <div className="chart-header">
              <div>
                <div className="chart-title">Alertas Operativas</div>
              </div>
              <button className="btn btn-ghost btn-sm">Generar plan del día ↗</button>
            </div>
            <div className="alerta-list">
              {alertas.map(a => (
                <div key={a.id} className={`alerta-item ${getAlertaStyle(a.tipo)}`}>
                  <span className="alerta-icon">{getAlertaIcon(a.tipo)}</span>
                  <span>{a.mensaje}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
