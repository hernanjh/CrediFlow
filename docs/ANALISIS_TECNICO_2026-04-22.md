# Análisis técnico de CrediFlow (22-04-2026)

## Estado general
El sistema muestra una base funcional prometedora, pero actualmente tiene brechas críticas de **build**, **contrato frontend/backend**, **seguridad** y **responsividad** que explican fallas visibles y sensación de producto incompleto.

## Hallazgos críticos

1. **El frontend no compila en entorno limpio**
   - `npm install` falla por conflicto de peer dependencies entre `vite@8` y `vite-plugin-pwa@1.2.0` (que soporta hasta `vite@7`).
   - Consecuencia: onboarding técnico frágil y bloqueo de CI/CD.

2. **El API contract está desalineado entre backend y frontend**
   - Frontend espera `res.data.accessToken/refreshToken` al refrescar token, pero backend devuelve `AccessToken/RefreshToken`.
   - Frontend busca `err.response?.data?.error`, mientras backend devuelve `Error` en login.
   - Consecuencia: sesiones que pueden romperse al renovar token y mensajes de error inconsistentes.

3. **Hay llamadas frontend a endpoints inexistentes o no confirmados**
   - El cliente frontend define `creditosApi.getAll('/creditos')` con comentario explícito de que “se asume que existe”, pero el controller no expone `GET /creditos`.
   - Consecuencia: pantallas que dependen de listados de créditos pueden fallar en runtime.

4. **Seguridad incompleta en varios controladores sensibles**
   - `AuthController` está protegido con `[Authorize]`, pero otros controladores críticos (por ejemplo Ventas/Seguridad) no tienen `[Authorize]` a nivel clase.
   - Consecuencia: riesgo de exposición accidental de datos/operaciones si no hay otra capa de protección global.

## Hallazgos de UX/UI (“se ve mal”)

1. **Layout no responsive de forma integral**
   - Hay una sola media query global en CSS (`max-width: 1200px`) y no hay estrategia clara para móviles/tablets.
   - Sidebar fija + topbar fija + márgenes rígidos pueden romper layout en pantallas chicas.

2. **Uso intensivo de estilos inline y medidas rígidas en páginas clave**
   - Ejemplo: Reportes usa `gridTemplateColumns: '340px 1fr'` y varios bloques no adaptativos.
   - Consecuencia: comportamiento visual inconsistente y difícil de mantener.

3. **Duplicidad menor en CSS**
   - `width: calc(100% - 24px);` repetido en `.sidebar-item`.
   - Consecuencia: no rompe funcionalidad, pero evidencia deuda de refactor visual.

## Funcionalidades incompletas / modo prototipo

1. **Dashboard con datos mock**
   - Comentarios y estructura indican que aún no consume datos reales para varios widgets.

2. **Login con “modo demo sin backend” embebido en producción**
   - Útil para pruebas, pero necesita feature flag o separación por entorno para evitar confusión operativa.

3. **Reportes de deudores simulados**
   - Se usa arreglo local en vez de datasource real.

4. **Pendientes explícitos en código**
   - TODOs y placeholders (por ejemplo en Inventario y Application handlers) confirman funcionalidades no cerradas.

## Priorización recomendada (orden de ejecución)

1. **Estabilizar build frontend** (alinear versiones Vite/PWA y validar lockfile).
2. **Cerrar contrato API FE/BE** (normalizar casing JSON + errores estándar).
3. **Completar seguridad de endpoints** (`[Authorize]` y políticas por rol).
4. **Eliminar mocks productivos** (dashboard/reportes/login demo tras flag de entorno).
5. **Plan de responsive UI** (breakpoints + refactor layout base).
6. **Cerrar TODOs críticos de dominio/inventario**.

## Plan corto (7-10 días hábiles)

- Día 1-2: build & dependencias + pipeline.
- Día 2-3: contrato API y tests de integración FE/BE.
- Día 4: hardening de seguridad por endpoint.
- Día 5-6: reemplazo de mocks por endpoints reales.
- Día 7-8: responsive de shell (sidebar/topbar/pages maestras).
- Día 9-10: QA de regresión + checklist de release.

## Comandos ejecutados para este análisis

- `dotnet --version && dotnet build CrediFlow.sln` (falló: `dotnet` no instalado en el entorno).
- `npm run -s build` en `frontend` (falló por tipos de Vite no presentes al no poder instalar deps).
- `npm install` en `frontend` (falló por conflicto `vite` vs `vite-plugin-pwa`).
- Revisión estática de frontend/backend para confirmar desalineaciones y pendientes.
