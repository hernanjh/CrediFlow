namespace CrediFlow.Domain.Enums;

public enum EstadoCredito
{
    ACTIVO,
    AL_DIA,
    EN_MORA,
    EN_MORA_GRAVE,
    INCOBRABLE,
    CANCELADO
}

public enum EstadoCuota
{
    PENDIENTE,
    AL_DIA,
    EN_MORA,
    PAGADA,
    PAGADA_PARCIAL
}

public enum FrecuenciaCuota
{
    DIARIA = 1,
    SEMANAL = 7,
    QUINCENAL = 15,
    MENSUAL = 30
}

public enum EstadoCliente
{
    ACTIVO,
    INACTIVO,
    BLOQUEADO,
    INCOBRABLE
}



public enum EstadoCierreCaja
{
    PENDIENTE,
    OK,
    CON_DIFERENCIA,
    FALTANTE
}

public enum TipoMovimientoStock
{
    COMPRA,
    VENTA,
    AJUSTE_POSITIVO,
    AJUSTE_NEGATIVO,
    DEVOLUCION
}

public enum TipoUsuario
{
    SUPER_ADMIN,
    ADMIN,
    VENDEDOR
}

public enum EstadoPromesaPago
{
    PENDIENTE,
    CUMPLIDA,
    INCUMPLIDA
}

public enum TipoAlerta
{
    MORA_GRAVE,
    CIERRE_CAJA_FALTANTE,
    LIQUIDEZ_BAJA,
    PRODUCTO_STOCK_BAJO,
    PROMESA_PAGO_VENCIDA
}

/// <summary>
/// GLOBAL = un % sobre el precio base de todos los productos;
/// POR_CATEGORIA = % diferente por categoría;
/// POR_PRODUCTO = precio fijo o % específico por producto;
/// MIX = global + overrides por producto
/// </summary>
public enum TipoListaPrecio
{
    GLOBAL,
    POR_CATEGORIA,
    POR_PRODUCTO,
    MIX
}
