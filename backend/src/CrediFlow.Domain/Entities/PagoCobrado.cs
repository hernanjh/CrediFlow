using CrediFlow.Domain.Common;

namespace CrediFlow.Domain.Entities;

public class PagoCobrado : BaseEntity
{
    public Guid CuotaId { get; private set; }
    public Cuota? Cuota { get; private set; }
    public Guid VendedorId { get; private set; }
    public Usuario? Vendedor { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaPago { get; private set; }
    public double? Latitud { get; private set; }
    public double? Longitud { get; private set; }
    public string? Observaciones { get; private set; }
    public bool EsOffline { get; private set; }
    public string? DeviceId { get; private set; } // para reconciliación offline

    protected PagoCobrado() { }

    public static PagoCobrado Crear(Guid cuotaId, Guid vendedorId, decimal monto,
        double? latitud = null, double? longitud = null, string? observaciones = null,
        bool esOffline = false, string? deviceId = null)
    {
        if (monto <= 0) throw new ArgumentException("El monto del pago debe ser mayor a cero.");
        return new PagoCobrado
        {
            CuotaId = cuotaId,
            VendedorId = vendedorId,
            Monto = monto,
            FechaPago = DateTime.UtcNow,
            Latitud = latitud,
            Longitud = longitud,
            Observaciones = observaciones,
            EsOffline = esOffline,
            DeviceId = deviceId
        };
    }
}
