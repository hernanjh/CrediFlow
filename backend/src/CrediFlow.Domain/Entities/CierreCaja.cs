using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Events;

namespace CrediFlow.Domain.Entities;

/// <summary>
/// Cierre de Caja CIEGO: el vendedor declara lo que tiene, sin ver el total del sistema.
/// </summary>
public class CierreCaja : BaseEntity
{
    public Guid VendedorId { get; private set; }
    public Usuario? Vendedor { get; private set; }
    public DateTime Fecha { get; private set; }
    public decimal MontoDeclarado { get; private set; }
    public decimal MontoSistema { get; private set; }
    public decimal Diferencia => MontoDeclarado - MontoSistema;
    public EstadoCierreCaja Estado { get; private set; }
    public decimal UmbralDiferencia { get; private set; } = 50m;
    public string? Observaciones { get; private set; }

    protected CierreCaja() { }

    /// <summary>
    /// Paso 1: Vendedor declara su monto (sin saber el total del sistema)
    /// </summary>
    public static CierreCaja CrearDeclaracion(Guid vendedorId, decimal montoDeclarado, decimal umbral = 50m)
    {
        return new CierreCaja
        {
            VendedorId = vendedorId,
            Fecha = DateTime.UtcNow.Date,
            MontoDeclarado = montoDeclarado,
            MontoSistema = 0,
            Estado = EstadoCierreCaja.PENDIENTE,
            UmbralDiferencia = umbral
        };
    }

    /// <summary>
    /// Paso 2: Sistema calcula el monto real y compara (solo SuperAdmin/Admin puede ver)
    /// </summary>
    public void CerrarConMontoSistema(decimal montoSistema, string? observaciones = null)
    {
        MontoSistema = montoSistema;
        Observaciones = observaciones;

        var diferencia = Math.Abs(Diferencia);
        if (diferencia <= UmbralDiferencia)
        {
            Estado = EstadoCierreCaja.OK;
        }
        else if (MontoDeclarado < MontoSistema)
        {
            Estado = EstadoCierreCaja.FALTANTE;
            AddDomainEvent(new CierreCajaConDiferenciaEvent(Id, VendedorId, Fecha, Diferencia, MontoSistema));
        }
        else
        {
            Estado = EstadoCierreCaja.CON_DIFERENCIA;
            AddDomainEvent(new CierreCajaConDiferenciaEvent(Id, VendedorId, Fecha, Diferencia, MontoSistema));
        }
    }
}
