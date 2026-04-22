import { db } from '../store/db'
import { creditosApi } from './client'

export async function syncOfflinePayments() {
  if (!navigator.onLine) return; // Si no hay internet, salir

  try {
    // 1. Obtener todos los pagos pendientes de la cola
    const pagosPendientes = await db.pagosOfflineQueue
      .where('estado').equals('PENDIENTE')
      .toArray();

    if (pagosPendientes.length === 0) return; // Nada que sincronizar

    // 2. Marcar como "EN_PROCESO" para evitar doble envío
    await db.pagosOfflineQueue.bulkPut(
      pagosPendientes.map(p => ({ ...p, estado: 'EN_PROCESO' }))
    );

    // 3. Formatear para el backend (API espera { pagos: [...] })
    const payload = pagosPendientes.map(p => ({
      cuotaId: p.cuotaId,
      monto: p.monto,
      latitud: p.latitud,
      longitud: p.longitud,
      observaciones: p.observaciones,
      esOffline: true,
      deviceId: 'PWA-Client' // Podría ser el fingerprint
    }));

    // 4. Enviar al backend masivamente
    await creditosApi.syncOffline(payload);

    // 5. Eliminar (o marcar cerrados) de la cola
    const ids = pagosPendientes.map(p => p.id!);
    await db.pagosOfflineQueue.bulkDelete(ids);

    console.log(`[Sync] ${pagosPendientes.length} pagos sincronizados con éxito.`);
  } catch (error) {
    console.error('[Sync] Error sincronizando pagos offline:', error);
    // Revertir a PENDIENTE para intentar luego
    const pagosPendientes = await db.pagosOfflineQueue
      .where('estado').equals('EN_PROCESO')
      .toArray();
    await db.pagosOfflineQueue.bulkPut(
      pagosPendientes.map(p => ({ ...p, estado: 'PENDIENTE' }))
    );
  }
}

// Iniciar listener de reconexión
window.addEventListener('online', syncOfflinePayments);
