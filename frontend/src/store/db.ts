import Dexie, { Table } from 'dexie';

// Interfaces para nuestras tablas Offline
export interface HojaRutaCache {
    id: string; // Puede ser el vendedorId o la fecha (ej: 'ruta-2025-10-15')
    fechaGuardado: Date;
    datos: any; // El response JSON con la hoja de ruta
}

export interface PagoOfflineQueue {
    id?: number; // Auto-increment para Local
    cuotaId: string;
    monto: number;
    latitud?: number;
    longitud?: number;
    observaciones?: string;
    fechaCaptura: Date;
    estado: 'PENDIENTE' | 'RECHAZADO' | 'EN_PROCESO';
}

export class CrediFlowDatabase extends Dexie {
    hojaRutaCache!: Table<HojaRutaCache, string>;
    pagosOfflineQueue!: Table<PagoOfflineQueue, number>;

    constructor() {
        super('CrediFlowDB');
        // Esquema de la DB local
        this.version(1).stores({
            hojaRutaCache: 'id', // Primary key
            pagosOfflineQueue: '++id, cuotaId, estado' // Auto-increment pk, indices adicionales
        });
    }
}

export const db = new CrediFlowDatabase();
